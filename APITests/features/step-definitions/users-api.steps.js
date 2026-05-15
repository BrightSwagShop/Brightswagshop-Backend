const { BeforeAll, AfterAll, Before, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const {
  BASE_URL,
  createHeaderAuthContext,
  readApiResponse,
  createValidMugPayload,
  createCartRequest,
  signJwtToken
} = require('./api-helpers');

let anonymousContext;
let adminContext;
let seededProductId;
let jwtConfig;

function resolveJwtConfig() {
  return {
    key: process.env.Jwt__Key || process.env.JWT_KEY || process.env.JWT_KEY_VALUE || '',
    issuer: process.env.Jwt__Issuer || process.env.JWT_ISSUER || '',
    audience: process.env.Jwt__Audience || process.env.JWT_AUDIENCE || ''
  };
}

BeforeAll(async function () {
  anonymousContext = await request.newContext({ baseURL: BASE_URL });
  adminContext = await createHeaderAuthContext('admin');
  jwtConfig = resolveJwtConfig();

  const productResponse = await adminContext.post('/api/products', {
    data: createValidMugPayload({ name: `users-product-${Date.now()}` })
  });
  const productBody = await productResponse.json();
  assert.equal(productResponse.status(), 201);
  seededProductId = productBody.id;
});

AfterAll(async function () {
  if (seededProductId) {
    await adminContext.delete(`/api/products/${seededProductId}`).catch(() => null);
  }

  await anonymousContext?.dispose();
  await adminContext?.dispose();
});

Before(function () {
  this.userPayload = null;
  this.loginPayload = null;
  this.userResponse = null;
  this.userBody = null;
  this.userToken = null;
  this.userHeaders = {};
  this.registeredCredentials = null;
  this.favoriteProductId = seededProductId;
  this.createdOrderId = null;
  this.customerUserId = null;
  this.cartRequest = null;
  this.jwtAuthMode = 'valid CustomJwt user';
  this.registeredUserId = null;
  this.registeredUsername = null;
  this.registeredPassword = null;
});

async function storeResponse(world, response) {
  const result = await readApiResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
  world.lastBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
  // Also store in userResponse for backwards compatibility
  world.userResponse = result.response;
  world.userBody = result.body;
  world.userBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
}

function uniqueCredentials(prefix) {
  const unique = Date.now();
  return {
    username: `${prefix}-${unique}`,
    password: `P@ss-${unique}`
  };
}

function buildUserHeaders(token) {
  return token ? { Authorization: `Bearer ${token}` } : {};
}

function buildWrongRoleToken() {
  if (!jwtConfig.key || !jwtConfig.issuer || !jwtConfig.audience) {
    return null;
  }

  return signJwtToken({
    key: jwtConfig.key,
    issuer: jwtConfig.issuer,
    audience: jwtConfig.audience,
    subject: `wrong-role-${Date.now()}`,
    name: `wrong-role-${Date.now()}`,
    role: 'Admin'
  });
}

async function registerUser(context, payload) {
  const response = await context.post('/api/users/register', { data: payload });
  const result = await readApiResponse(response);
  assert.equal(result.response.status(), 200);
  assert.ok(result.body?.id);
  return result.body;
}

async function loginUser(context, payload) {
  const response = await context.post('/api/users/login', { data: payload });
  return readApiResponse(response);
}

Given('I prepare a unique user registration payload', function () {
  this.userPayload = uniqueCredentials('api-user');
});

Given('I register a unique public user', async function () {
  this.userPayload = uniqueCredentials('api-login');
  const body = await registerUser(anonymousContext, this.userPayload);
  this.registeredUserId = body.id;
});

Given('I prepare credentials for an unknown public user', function () {
  const unique = Date.now();
  this.userPayload = {
    username: `unknown-user-${unique}`,
    password: `Wrong-${unique}`
  };
});

Given('I have a valid user registration in MongoDB', async function () {
  this.userPayload = uniqueCredentials('mongo-user');
  const body = await registerUser(anonymousContext, this.userPayload);
  this.registeredUserId = body.id;
  this.registeredUsername = this.userPayload.username;
  this.registeredPassword = this.userPayload.password;
});

Given('I have a valid logged-in user token', async function () {
  if (!this.userPayload) {
    this.userPayload = uniqueCredentials('logged-in-user');
  }

  await registerUser(anonymousContext, this.userPayload);
  const loginResult = await loginUser(anonymousContext, this.userPayload);
  assert.equal(loginResult.response.status(), 200);
  this.userBody = loginResult.body;
  this.userToken = loginResult.body?.token;
  this.userHeaders = buildUserHeaders(this.userToken);
});

Given('I have an existing product id', function () {
  this.favoriteProductId = seededProductId;
});

// shopping cart setup for customers is implemented in shopping-cart-api.steps.js to avoid duplicate step definitions

Given('I prepare a registration payload in the {string} partition', function (partition) {
  const unique = Date.now();

  if (partition === 'valid') {
    this.userPayload = uniqueCredentials('register-valid');
    return;
  }

  if (partition === 'missing username') {
    this.userPayload = { password: `P@ss-${unique}` };
    return;
  }

  if (partition === 'missing password') {
    this.userPayload = { username: `register-${unique}` };
    return;
  }

  if (partition === 'malformed body') {
    this.userPayload = null;
    this.userMalformed = true;
    return;
  }

  throw new Error(`Unsupported registration partition: ${partition}`);
});

Given('I prepare login credentials in the {string} partition', function (partition) {
  const unique = Date.now();

  if (partition === 'valid') {
    this.loginPayload = this.userPayload || uniqueCredentials('login-valid');
    return;
  }

  if (partition === 'unknown user') {
    this.loginPayload = {
      username: `unknown-${unique}`,
      password: `P@ss-${unique}`
    };
    return;
  }

  if (partition === 'wrong password') {
    this.loginPayload = {
      username: this.userPayload?.username || `known-${unique}`,
      password: `Wrong-${unique}`
    };
    return;
  }

  if (partition === 'malformed body') {
    this.loginPayload = null;
    this.userMalformed = true;
    return;
  }

  throw new Error(`Unsupported login partition: ${partition}`);
});

Given('I call the user endpoint with the {string} authentication partition', async function (auth) {
  this.jwtAuthMode = auth;

  if (auth === 'valid CustomJwt user') {
    if (!this.userToken) {
      if (!this.userPayload) {
        this.userPayload = uniqueCredentials('auth-user');
      }
      await registerUser(anonymousContext, this.userPayload).catch(() => null);
      const loginResult = await loginUser(anonymousContext, this.userPayload);
      this.userToken = loginResult.body?.token;
    }

    this.userHeaders = buildUserHeaders(this.userToken);
    return;
  }

  if (auth === 'missing token') {
    this.userHeaders = {};
    return;
  }

  if (auth === 'valid token wrong role') {
    const token = buildWrongRoleToken();
    this.userHeaders = buildUserHeaders(token);
    return;
  }

  throw new Error(`Unsupported user auth partition: ${auth}`);
});

When('I POST {string} with the user payload', async function (path) {
  const response = await anonymousContext.post(path, {
    data: this.userPayload
  });

  await storeResponse(this, response);
});

When('I POST {string} with the same user credentials', async function (path) {
  const response = await anonymousContext.post(path, {
    data: this.userPayload
  });

  await storeResponse(this, response);
});

When('I POST {string} with the registration payload', async function (path) {
  if (this.userMalformed) {
    const response = await anonymousContext.post(path, {
      data: '{ malformed json',
      headers: { 'Content-Type': 'application/json' }
    });
    await storeResponse(this, response);
    return;
  }

  const response = await anonymousContext.post(path, {
    data: this.userPayload
  });

  await storeResponse(this, response);
});

When('I POST {string} with the login payload', async function (path) {
  if (this.userMalformed) {
    const response = await anonymousContext.post(path, {
      data: '{ malformed json',
      headers: { 'Content-Type': 'application/json' }
    });
    await storeResponse(this, response);
    return;
  }

  const response = await anonymousContext.post(path, {
    data: this.loginPayload
  });

  await storeResponse(this, response);
});

When('I POST {string} with the correct corresponding payload', async function (path) {
  if (path !== '/api/users/login') {
    throw new Error(`Unsupported path for login step: ${path}`);
  }

  const response = await anonymousContext.post(path, {
    data: this.userPayload
  });

  await storeResponse(this, response);
  this.userToken = this.userBody?.token;
  this.userHeaders = buildUserHeaders(this.userToken);
});

When('I POST {string} with the product id and user token', async function (path) {
  const response = await anonymousContext.post(path, {
    data: { productId: this.favoriteProductId },
    headers: this.userHeaders
  });

  await storeResponse(this, response);
});

When('I POST {string} with a favorite payload', async function (path) {
  const response = await anonymousContext.post(path, {
    data: { productId: this.favoriteProductId },
    headers: this.userHeaders
  });

  await storeResponse(this, response);
});

When('I POST {string} with the same product id and user token', async function (path) {
  const response = await anonymousContext.post(path, {
    data: { productId: this.favoriteProductId },
    headers: this.userHeaders
  });

  await storeResponse(this, response);
});

When('I POST {string} for that customer', async function (path) {
  const resolvedPath = path.replace('{userId}', this.customerUserId);
  const response = await anonymousContext.post(resolvedPath);
  await storeResponse(this, response);
  if (this.lastResponse.status() === 201 && this.lastBody?.id) {
    this.createdOrderId = this.lastBody.id;
  }
});

When('I POST {string} for the created order', async function (path) {
  const resolvedPath = path.replace('{orderId}', this.createdOrderId);
  const response = await anonymousContext.post(resolvedPath);
  await storeResponse(this, response);
});

Then('the users response status should be {int}', function (statusCode) {
  assert.equal(this.userResponse.status(), statusCode);
});

Then('the users response should contain a user id', function () {
  assert.ok(this.userBody && this.userBody.id);
});

Then('the users response username should match the request', function () {
  const responseUsername = this.userBody?.username ?? this.userBody?.user?.username;
  assert.equal(responseUsername, this.userPayload.username);
});

Then('the response should contain a JWT token', function () {
  assert.ok(this.userBody?.token);
});

Then('the user should now contain the product in favorites', function () {
  const favorites = this.userBody?.favorites || this.userBody?.Favorites || [];
  assert.ok(favorites.includes(this.favoriteProductId));
});

Then('the user should no longer contain the product in favorites', function () {
  const favorites = this.userBody?.favorites || this.userBody?.Favorites || [];
  assert.ok(!favorites.includes(this.favoriteProductId));
});

Then('I remember the created order id', function () {
  assert.ok(this.createdOrderId || this.userBody?.id);
  this.createdOrderId = this.createdOrderId || this.userBody.id;
});

Then('the response should contain a payment session reference', function () {
  assert.ok(this.userBody?.sessionId || this.userBody?.SessionId || this.userBody?.sessionUrl || this.userBody?.SessionUrl);
});
