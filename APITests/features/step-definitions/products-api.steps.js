const { BeforeAll, AfterAll, Before, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const {
  BASE_URL,
  createHeaderAuthContext,
  readApiResponse,
  createValidMugPayload,
  createMultipartFile,
  createOrderRequest
} = require('./api-helpers');

let adminContext;
let userContext;
let anonymousContext;
let seededProductId;
let seededProductPrice = 19.95;

BeforeAll(async function () {
  adminContext = await createHeaderAuthContext('admin');
  userContext = await createHeaderAuthContext('user');
  anonymousContext = await createHeaderAuthContext('anonymous');

  const response = await adminContext.post('/api/products', {
    data: createValidMugPayload({ name: `seeded-product-${Date.now()}`, imageUrl: 'https://example.invalid/seeded-product.jpg' })
  });

  const body = await response.json();
  assert.equal(response.status(), 201);
  seededProductId = body.id;
});

AfterAll(async function () {
  if (seededProductId) {
    await adminContext.delete(`/api/products/${seededProductId}`).catch(() => null);
  }

  await adminContext?.dispose();
  await userContext?.dispose();
  await anonymousContext?.dispose();
});

Before(function () {
  this.adminContext = adminContext;
  this.userContext = userContext;
  this.anonymousContext = anonymousContext;
  this.activeContext = adminContext;
  this.lastResponse = null;
  this.lastBody = null;
  this.lastBodyText = null;
  this.productPayload = null;
  this.productIdList = null;
  this.uploadFile = null;
  this.lookupContext = null;
  this.createdProductId = null;
  this.checkoutOrderId = null;
  this.checkoutOrderMode = null;
  this.debugContext = null;
  this.debugPath = '/api/debug/claims';
});

async function storeResponse(world, response) {
  const result = await readApiResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
  world.lastBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
}

function resolveProductId(productId) {
  if (productId === 'existing-product-id') {
    return seededProductId;
  }

  return productId;
}

function createValidProductPayload() {
  return createValidMugPayload({
    name: `products-admin-${Date.now()}`,
    imageUrl: 'https://example.invalid/product.jpg'
  });
}

function createInvalidEnumProductPayload() {
  const payload = createValidProductPayload();
  payload.productType = 'INVALID_TYPE';
  payload.$type = 'Mok';
  return payload;
}

Given('I have a valid mug payload', function () {
  this.productPayload = createValidProductPayload();
});

Given('I am authenticated as a regular user for products API', function () {
  this.activeContext = userContext;
});

Given('I am authenticated as an admin user for products API', function () {
  this.activeContext = adminContext;
});

Given('I am not authenticated for products API', function () {
  this.activeContext = anonymousContext;
});

Given('I am authenticated as admin', function () {
  this.activeContext = adminContext;
});

Given(/^I prepare a product id list with (\d+) item\(s\)$/, function (count) {
  this.productIdList = parseInt(count) > 0 ? [seededProductId] : [];
});

// upload-file and prepare-order steps are provided by shopping-cart-api.steps.js

Given('I call a protected endpoint with X-User-Id value {string}', async function (userIdHeader) {
  const headers = {};

  if (userIdHeader === 'missing') {
    this.debugContext = await request.newContext({ baseURL: BASE_URL });
    return;
  }

  if (userIdHeader === 'whitespace') {
    headers['X-User-Id'] = '   ';
  } else {
    headers['X-User-Id'] = userIdHeader;
  }

  this.debugContext = await request.newContext({ baseURL: BASE_URL, extraHTTPHeaders: headers });
});

When('I GET {string}', async function (path) {
  if (path === '/api/products') {
    const response = await this.activeContext.get(path);
    await storeResponse(this, response);
    return;
  }

  if (path.startsWith('/api/products/type/')) {
    const slug = path.replace('/api/products/type/', '');
    const response = await this.activeContext.get(`/api/products/type/${slug}`);
    await storeResponse(this, response);
    return;
  }

  if (path.startsWith('/api/products/')) {
    const productId = resolveProductId(path.replace('/api/products/', ''));
    const response = await this.activeContext.get(`/api/products/${productId}`);
    await storeResponse(this, response);
    return;
  }

  if (path === '/api/categories' || path === '/api/producttypes') {
    const response = await this.activeContext.get(path);
    await storeResponse(this, response);
    return;
  }

  if (path === '/api/debug/claims') {
    const response = await (this.debugContext || this.activeContext).get(path);
    await storeResponse(this, response);
    return;
  }

  throw new Error(`Unsupported path for GET step: ${path}`);
});

When('I POST {string} with the payload', async function (path) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for POST step: ${path}`);
  }

  const response = await this.activeContext.post(path, {
    data: this.productPayload
  });

  await storeResponse(this, response);
});

When('I POST {string} as a regular user with the payload', async function (path) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for POST step: ${path}`);
  }

  const response = await userContext.post(path, {
    data: this.productPayload
  });

  await storeResponse(this, response);
});

When('I POST {string} with the payload that has productType {string}', async function (path, productType) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for invalid product payload: ${path}`);
  }

  const payload = createInvalidEnumProductPayload();
  payload.productType = productType;

  const response = await adminContext.post(path, {
    data: payload
  });

  await storeResponse(this, response);
});

When('I POST {string} with the id list', async function (path) {
  if (path !== '/api/products/by-ids') {
    throw new Error(`Unsupported path for product id list: ${path}`);
  }

  const response = await this.activeContext.post(path, {
    data: this.productIdList
  });

  await storeResponse(this, response);
});

// admin image upload and cart retrieval are implemented in other step files to avoid duplicates

// Accept both 'a' and 'the' article in the invalid productType payload step used by features
When(/^I POST "(\/api\/products)" with (?:a|the) payload that has productType "([^"]+)"$/, async function (path, productType) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for invalid product payload: ${path}`);
  }

  const payload = createInvalidEnumProductPayload();
  payload.productType = productType;

  const response = await adminContext.post(path, { data: payload });
  await storeResponse(this, response);
});

When(/^I POST "(\/api\/payments\/(empty-order-id|single-item-order-id)\/checkout)"$/, async function (path) {
  const resolvedPath = path
    .replace('<orderId>', this.checkoutOrderId || 'missing-order-id')
    .replace('empty-order-id', this.checkoutOrderId)
    .replace('single-item-order-id', this.checkoutOrderId);

  const response = await this.activeContext.post(resolvedPath);
  await storeResponse(this, response);
});

Then('the response status should be {int}', function (statusCode) {
  assert.equal(this.lastResponse.status(), statusCode);
});

Then('the response should be an array of products', function () {
  assert.ok(Array.isArray(this.lastBody));
});

Then('every returned category should contain a numeric id and a string name', function () {
  assert.ok(Array.isArray(this.lastBody));
  for (const category of this.lastBody) {
    assert.equal(typeof category.id, 'number');
    assert.equal(typeof category.name, 'string');
  }
});

Then('every returned product type should contain a string name and a string slug', function () {
  assert.ok(Array.isArray(this.lastBody));
  for (const productType of this.lastBody) {
    assert.equal(typeof productType.name, 'string');
    assert.equal(typeof productType.slug, 'string');
  }
});

Then('the response should describe the invalid enum/value', function () {
  assert.ok(this.lastBodyText.includes('INVALID_TYPE') || this.lastBodyText.toLowerCase().includes('producttype'));
});

Then(/^the response should describe the invalid enum\/value$/, function () {
  assert.ok(this.lastBodyText.includes('INVALID_TYPE') || this.lastBodyText.toLowerCase().includes('producttype'));
});

Then('I store the created product id', function () {
  assert.ok(this.lastBody && this.lastBody.id);
  this.createdProductId = this.lastBody.id;
});

