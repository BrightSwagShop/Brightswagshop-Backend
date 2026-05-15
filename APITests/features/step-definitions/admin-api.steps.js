const { BeforeAll, AfterAll, Before, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const {
  BASE_URL,
  createHeaderAuthContext,
  readApiResponse,
  createValidMugPayload,
  createValidDiscountPayload,
  createInvalidDateDiscountPayload,
  createMultipartFile,
  buildStripeWebhookEvent,
  signStripeWebhookPayload
} = require('./api-helpers');

let adminContext;
let userContext;
let anonymousContext;
let seededProductId;
let seededWebhookOrderId;
let seededWebhookCartId;

BeforeAll(async function () {
  adminContext = await createHeaderAuthContext('admin');
  userContext = await createHeaderAuthContext('user');
  anonymousContext = await createHeaderAuthContext('anonymous');

  const seedResponse = await adminContext.post('/api/products', {
    data: createValidMugPayload({ name: `seeded-admin-product-${Date.now()}` })
  });
  const seedBody = await seedResponse.json();
  assert.equal(seedResponse.status(), 201);
  seededProductId = seedBody.id;

  const webhookProductResponse = await adminContext.post('/api/products', {
    data: createValidMugPayload({ name: `seeded-webhook-product-${Date.now()}`, imageUrl: 'https://example.invalid/webhook.jpg' })
  });
  const webhookProductBody = await webhookProductResponse.json();
  assert.equal(webhookProductResponse.status(), 201);

  const cartResponse = await anonymousContext.post('/api/shoppingcarts', {
    data: {
      userId: `webhook-user-${Date.now()}`,
      items: [
        {
          productId: webhookProductBody.id,
          quantity: 1,
          selectedColor: null
        }
      ]
    }
  });
  const cartBody = await cartResponse.json();
  assert.equal(cartResponse.status(), 201);
  seededWebhookCartId = cartBody.id;

  const orderResponse = await anonymousContext.post(`/api/orders/from-cart/${cartBody.userId}`);
  const orderBody = await orderResponse.json();
  assert.equal(orderResponse.status(), 201);
  seededWebhookOrderId = orderBody.id;
});

AfterAll(async function () {
  if (seededWebhookCartId) {
    await anonymousContext.delete(`/api/shoppingcarts/${seededWebhookCartId}`).catch(() => null);
  }

  if (seededWebhookOrderId) {
    await anonymousContext.delete(`/api/orders/${seededWebhookOrderId}`).catch(() => null);
  }

  if (seededProductId) {
    await adminContext.delete(`/api/products/${seededProductId}`).catch(() => null);
  }

  await adminContext?.dispose();
  await userContext?.dispose();
  await anonymousContext?.dispose();
});

Before(function () {
  this.activeContext = adminContext;
  this.lastResponse = null;
  this.lastBody = null;
  this.lastBodyText = null;
  this.productPayload = null;
  this.discountPayload = null;
  this.updatedDiscountPayload = null;
  this.uploadFile = null;
  this.uploadedImageUrl = null;
  this.createdProductId = null;
  this.createdDiscountId = null;
  this.supportEndpoint = '/api/debug/claims';
  this.webhookRequestBody = null;
  this.webhookSignature = null;
  this.webhookPartition = null;
});

async function storeResponse(world, response) {
  const result = await readApiResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
  world.lastBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
}

function applyUploadedImageToProduct(world) {
  if (world.productPayload?.kleuren?.length && world.uploadedImageUrl) {
    world.productPayload.kleuren[0].imageUrl = world.uploadedImageUrl;
  }
}

function createUpdatedDiscountPayload(basePayload) {
  return {
    ...basePayload,
    name: `${basePayload.name}-updated`,
    description: `${basePayload.description} (updated)`,
    percentage: basePayload.percentage + 5,
    code: `${basePayload.code}-UPD`,
    isActive: !basePayload.isActive
  };
}

function resolveAuthPartition(partition) {
  if (partition === 'public user') {
    return userContext;
  }

  if (partition === 'admin user') {
    return adminContext;
  }

  if (partition === 'anonymous') {
    return anonymousContext;
  }

  if (partition === 'valid HeaderAuth user') {
    return userContext;
  }

  if (partition === 'valid HeaderAuth admin') {
    return adminContext;
  }

  if (partition === 'wrong role for admin api') {
    return userContext;
  }

  throw new Error(`Unsupported auth partition: ${partition}`);
}

Given('I am authenticated as an admin user', function () {
  this.activeContext = adminContext;
});

Given('I have a valid image file', function () {
  this.uploadFile = createMultipartFile({ fileType: 'valid image', fileName: `admin-upload-${Date.now()}.jpg` });
});

Given('I have a valid product payload', function () {
  this.productPayload = createValidMugPayload({
    name: `admin-product-${Date.now()}`,
    imageUrl: this.uploadedImageUrl || 'https://example.invalid/admin-product.jpg'
  });
});

Given('I have a valid discount payload', function () {
  this.discountPayload = createValidDiscountPayload({
    name: `admin-discount-${Date.now()}`,
    code: `ADMIN-${Date.now()}`
  });
  this.updatedDiscountPayload = createUpdatedDiscountPayload(this.discountPayload);
});

Given('I call the products endpoint with the {string} authentication partition', function (partition) {
  this.activeContext = resolveAuthPartition(partition);
});

Given('I call the image upload endpoint with the {string} authentication partition', function (partition) {
  this.activeContext = resolveAuthPartition(partition);
});

Given('I attach a file in the {string} partition', function (fileType) {
  this.uploadFile = createMultipartFile({ fileType, fileName: `admin-upload-${Date.now()}.jpg` });
});

Given('I call the discounts endpoint with the {string} authentication partition', function (partition) {
  this.activeContext = resolveAuthPartition(partition);
});

Given('I call the protected support endpoint with the {string} authentication partition', function (partition) {
  this.activeContext = resolveAuthPartition(partition);

  if (partition === 'wrong role for admin api') {
    this.supportEndpoint = '/api/admins/admin-only';
    return;
  }

  this.supportEndpoint = '/api/debug/claims';
});

Given('I prepare a Stripe webhook request in the {string} partition', function (partition) {
  this.webhookPartition = partition;
  const secret = process.env.STRIPE_WEBHOOK_SECRET || process.env.Stripe__WebhookSecret || process.env.stripeWebhookSecret || '';

  if (partition === 'missing webhook secret') {
    this.webhookRequestBody = buildStripeWebhookEvent({
      orderId: seededWebhookOrderId,
      sessionId: `cs_${Date.now()}`
    });
    this.webhookSignature = null;
    this.webhookSecretOverride = '';
    return;
  }

  this.webhookRequestBody = buildStripeWebhookEvent({
    orderId: seededWebhookOrderId,
    sessionId: `cs_${Date.now()}`
  });

  if (partition === 'missing signature') {
    this.webhookSignature = null;
    this.webhookSecretOverride = secret;
    return;
  }

  if (partition === 'invalid signature') {
    this.webhookSignature = 't=1234,v1=invalid';
    this.webhookSecretOverride = secret;
    return;
  }

  if (partition === 'valid event') {
    this.webhookSignature = signStripeWebhookPayload(this.webhookRequestBody, secret);
    this.webhookSecretOverride = secret;
    return;
  }

  throw new Error(`Unsupported Stripe webhook partition: ${partition}`);
});

Given('I am authenticated via HeaderAuth with role {string}', async function (role) {
  await this.headerAuthContext?.dispose?.();
  this.headerAuthContext = await request.newContext({
    baseURL: BASE_URL,
    extraHTTPHeaders: {
      'X-User-Id': `test-${Date.now()}`,
      'X-User-Role': role
    }
  });
  this.activeContext = this.headerAuthContext;
});

When('I POST {string} with the image file as admin', async function (path) {
  if (path !== '/api/images/upload') {
    throw new Error(`Unsupported path for admin image upload: ${path}`);
  }

  const file = this.uploadFile || createMultipartFile({ fileType: 'valid image' });
  const response = await this.activeContext.post(path, {
    multipart: {
      file
    }
  });

  await storeResponse(this, response);
});

When(/^I POST "(\/api\/images\/upload)"$/, async function (path) {
  const file = this.uploadFile || createMultipartFile({ fileType: 'valid image' });
  const response = await this.activeContext.post(path, {
    multipart: {
      file
    }
  });

  await storeResponse(this, response);
});

When('I POST {string} as admin', async function (path) {
  if (path !== '/api/images/upload') {
    throw new Error(`Unsupported path for admin image upload alias: ${path}`);
  }

  const file = this.uploadFile || createMultipartFile({ fileType: 'valid image' });
  const response = await this.activeContext.post(path, {
    multipart: { file }
  });

  await storeResponse(this, response);
});

When('I POST {string} with the product payload as admin', async function (path) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for admin product create: ${path}`);
  }

  applyUploadedImageToProduct(this);
  const response = await this.activeContext.post(path, {
    data: this.productPayload
  });

  await storeResponse(this, response);

  if (this.lastResponse.status() === 201 && this.lastBody?.id) {
    this.createdProductId = this.lastBody.id;
  }
});

When('I DELETE the created product by id as admin', async function () {
  const response = await adminContext.delete(`/api/products/${this.createdProductId}`);
  await storeResponse(this, response);
});

When('I POST {string} with the discount payload as admin', async function (path) {
  if (path !== '/api/discounts') {
    throw new Error(`Unsupported path for admin discount create: ${path}`);
  }

  const response = await this.activeContext.post(path, {
    data: this.discountPayload
  });

  await storeResponse(this, response);

  if (this.lastResponse.status() === 201 && this.lastBody?.id) {
    this.createdDiscountId = this.lastBody.id;
  }
});

When('I PUT {string} with an updated discount payload', async function (path) {
  const resolvedPath = path.replace('{discountId}', this.createdDiscountId);

  if (!resolvedPath.startsWith('/api/discounts/')) {
    throw new Error(`Unsupported path for discount update: ${path}`);
  }

  const response = await this.activeContext.put(resolvedPath, {
    data: this.updatedDiscountPayload || createUpdatedDiscountPayload(this.discountPayload)
  });

  await storeResponse(this, response);
});

When('I POST {string} with a valid discount payload', async function (path) {
  if (path !== '/api/discounts') {
    throw new Error(`Unsupported path for discount creation: ${path}`);
  }

  const payload = createValidDiscountPayload();
  const response = await this.activeContext.post(path, {
    data: payload
  });

  await storeResponse(this, response);

  if (this.lastResponse.status() === 201 && this.lastBody?.id) {
    this.createdDiscountId = this.lastBody.id;
  }
});

When('I POST {string} with a valid product payload', async function (path) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for valid product payload: ${path}`);
  }

  this.productPayload = this.productPayload || createValidMugPayload({ name: `role-product-${Date.now()}` });
  applyUploadedImageToProduct(this);

  const response = await this.activeContext.post(path, {
    data: this.productPayload
  });

  await storeResponse(this, response);
});

When('I DELETE {string} with the same authentication partition', async function (path) {
  const resolvedPath = path.replace('existing-product-id', seededProductId);

  if (!resolvedPath.startsWith('/api/products/')) {
    throw new Error(`Unsupported path for delete partition step: ${path}`);
  }

  const response = await this.activeContext.delete(resolvedPath);
  await storeResponse(this, response);
});

When(/^I POST "(\/api\/webhooks\/stripe)"$/, async function (path) {
  if (path !== '/api/webhooks/stripe') {
    throw new Error(`Unsupported webhook POST in admin feature: ${path}`);
  }

  const headers = {};
  if (this.webhookSignature) {
    headers['Stripe-Signature'] = this.webhookSignature;
  }

  const response = await anonymousContext.post(path, {
    data: this.webhookRequestBody,
    headers
  });

  await storeResponse(this, response);
});

When('I GET the protected endpoint', async function () {
  const response = await this.activeContext.get(this.supportEndpoint);
  await storeResponse(this, response);
});

When('I GET {string} with the HeaderAuth credentials', async function (path) {
  const response = await this.activeContext.get(path);
  await storeResponse(this, response);
});

When('I POST {string} with the correct Stripe webhook payload', async function (path) {
  if (path !== '/api/webhooks/stripe') {
    throw new Error(`Unsupported webhook path: ${path}`);
  }

  const response = await anonymousContext.post(path, {
    data: this.webhookRequestBody,
    headers: this.webhookSignature ? { 'Stripe-Signature': this.webhookSignature } : {}
  });

  await storeResponse(this, response);
});

When('I POST {string} with the Stripe webhook payload', async function (path) {
  if (path !== '/api/webhooks/stripe') {
    throw new Error(`Unsupported webhook path: ${path}`);
  }

  const headers = {};
  if (this.webhookSignature) {
    headers['Stripe-Signature'] = this.webhookSignature;
  }

  const response = await anonymousContext.post(path, {
    data: this.webhookRequestBody,
    headers
  });

  await storeResponse(this, response);
});

Then('the response should contain an imageUrl', function () {
  assert.ok(this.lastBody?.imageUrl);
  this.uploadedImageUrl = this.lastBody.imageUrl;
});

Then('the created product should be retrievable by id', async function () {
  const response = await adminContext.get(`/api/products/${this.createdProductId}`);
  await storeResponse(this, response);
  assert.equal(this.lastResponse.status(), 200);
  assert.equal(this.lastBody.id, this.createdProductId);
});

Then('I remember the created discount id', function () {
  assert.ok(this.lastBody?.id);
  this.createdDiscountId = this.lastBody.id;
});

Then('the response should reflect the updated discount values', function () {
  assert.equal(this.lastBody.id, this.createdDiscountId);
  assert.equal(this.lastBody.name, this.updatedDiscountPayload.name);
  assert.equal(this.lastBody.description, this.updatedDiscountPayload.description);
  assert.equal(this.lastBody.percentage, this.updatedDiscountPayload.percentage);
  assert.equal(this.lastBody.code, this.updatedDiscountPayload.code);
  assert.equal(this.lastBody.isActive, this.updatedDiscountPayload.isActive);
});
