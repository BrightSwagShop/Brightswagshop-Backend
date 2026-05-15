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
  createCartRequest
} = require('./api-helpers');

const VALID_DISCOUNT_CODE = 'SPRING20';

let adminContext;
let userContext;
let anonymousContext;
let seededProductId;
let seededDiscountId;
const createdDiscountIds = new Set();
let cartId;
let cartBody;

BeforeAll(async function () {
  adminContext = await createHeaderAuthContext('admin');
  userContext = await createHeaderAuthContext('user');
  anonymousContext = await request.newContext({ baseURL: BASE_URL });

  const productResponse = await adminContext.post('/api/products', {
    data: createValidMugPayload({ name: `discount-product-${Date.now()}` })
  });
  const productResult = await readApiResponse(productResponse);
  assert.equal(productResult.response.status(), 201);
  seededProductId = productResult.body.id;

  const discountResponse = await adminContext.post('/api/discounts', {
    data: createValidDiscountPayload({ code: VALID_DISCOUNT_CODE, name: 'Spring 20', percentage: 20 })
  });
  const discountResult = await readApiResponse(discountResponse);
  assert.equal(discountResult.response.status(), 201);
  seededDiscountId = discountResult.body.id;
});

AfterAll(async function () {
  for (const discountId of createdDiscountIds) {
    await adminContext.delete(`/api/discounts/${discountId}`).catch(() => null);
  }

  if (seededDiscountId) {
    await adminContext.delete(`/api/discounts/${seededDiscountId}`).catch(() => null);
  }

  if (cartId) {
    await anonymousContext.delete(`/api/shoppingcarts/${cartId}`).catch(() => null);
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
  this.discountPayload = null;
  this.updatedDiscountPayload = null;
  this.lastResponse = null;
  this.lastBody = null;
  this.lastBodyText = null;
  this.cartUserId = null;
  this.cartProductId = seededProductId;
  this.currentDiscountCode = VALID_DISCOUNT_CODE;
  this.discountCartId = null;
});

async function storeResponse(world, response) {
  const result = await readApiResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
  world.lastBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
}

async function createCartForUser(userId) {
  const response = await anonymousContext.post('/api/shoppingcarts', {
    data: createCartRequest({
      userId,
      productId: seededProductId,
      quantity: 1
    })
  });

  const result = await readApiResponse(response);
  assert.equal(result.response.status(), 201);
  cartId = result.body.id;
  cartBody = result.body;
}

Given('I am authenticated as a regular user for discount operations', function () {
  this.activeContext = userContext;
});

Given('I am authenticated as an admin user for discount operations', function () {
  this.activeContext = adminContext;
});

Given('I am not authenticated for discount operations', function () {
  this.activeContext = anonymousContext;
});

Given('a shopping cart exists with user {string} and product {string}', async function (userId, productId) {
  this.cartUserId = userId;
  this.productId = productId;
  await createCartForUser(userId);
  this.discountCartId = cartId;
});

Given('a shopping cart exists with at least one item', async function () {
  this.cartUserId = `discount-user-${Date.now()}`;
  await createCartForUser(this.cartUserId);
  this.discountCartId = cartId;
});

Given('a valid discount code exists', async function () {
  // Seeded in BeforeAll; this step just makes the scenario intent explicit.
  this.currentDiscountCode = VALID_DISCOUNT_CODE;
});

Given('no cart exists with id {string}', function (id) {
  this.discountCartId = id;
});

When('I apply the discount code {string} to the cart', async function (code) {
  const response = await anonymousContext.post(`/api/shoppingcarts/${this.discountCartId}/apply-discount`, {
    data: { code }
  });

  await storeResponse(this, response);
  this.cartBody = this.lastBody;
});

When('I apply the discount code {string} to the cart again', async function (code) {
  const response = await anonymousContext.post(`/api/shoppingcarts/${this.discountCartId}/apply-discount`, {
    data: { code }
  });

  await storeResponse(this, response);
  this.cartBody = this.lastBody;
});

When('I POST {string} with the discount code', async function (path) {
  const resolvedPath = path.replace('{cartId}', this.discountCartId);
  const response = await anonymousContext.post(resolvedPath, {
    data: { code: this.currentDiscountCode }
  });

  await storeResponse(this, response);
});

When('I POST {string} with the same discount code again', async function (path) {
  const resolvedPath = path.replace('{cartId}', this.discountCartId);
  const response = await anonymousContext.post(resolvedPath, {
    data: { code: this.currentDiscountCode }
  });

  await storeResponse(this, response);
});

When('I POST {string} with code {string}', async function (path, code) {
  const response = await anonymousContext.post(path, {
    data: { code }
  });

  await storeResponse(this, response);
});

When('I create a discount with code {string}', async function (code) {
  const payload = createValidDiscountPayload({ code, name: `Test ${code}`, percentage: 10 });
  const response = await this.activeContext.post('/api/discounts', {
    data: payload
  });

  await storeResponse(this, response);

  if (this.lastResponse.status() === 201 && this.lastBody?.id) {
    createdDiscountIds.add(this.lastBody.id);
  }
});

When('I POST {string} with a discount where endDate < startDate', async function (path) {
  if (path !== '/api/discounts') {
    throw new Error(`Unsupported invalid-discount path: ${path}`);
  }

  const response = await adminContext.post(path, {
    data: createInvalidDateDiscountPayload()
  });

  await storeResponse(this, response);
});

Then('the cart total should reflect the discount', function () {
  assert.equal(this.lastResponse.status(), 200);
  assert.ok(Number(this.lastBody.totalPrice) < Number(this.lastBody.subTotal));
});

Then('the cart totals should reflect the discount', function () {
  assert.equal(this.lastResponse.status(), 200);
  assert.ok(Number(this.lastBody.totalPrice) < Number(this.lastBody.subTotal));
});

Then('the cart should keep the first discount state', function () {
  assert.equal(this.lastResponse.status(), 409);
});

Then('I should receive a 409 Conflict error', function () {
  assert.equal(this.lastResponse.status(), 409);
});

Then('I should receive a 404 Not Found error', function () {
  assert.equal(this.lastResponse.status(), 404);
});

Then('I should receive a 403 Forbidden error', function () {
  assert.equal(this.lastResponse.status(), 403);
});

Then('I should receive a 201 Created response', function () {
  assert.equal(this.lastResponse.status(), 201);
  assert.ok(this.lastBody?.id);
});