const { BeforeAll, AfterAll, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { ShoppingCartApiSom, createMugPayload } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';
const VALID_DISCOUNT_CODE = 'SPRING20';

let apiContext;
let authApiContext;
let shoppingCartApi;
let seededProductId;
let seededDiscountId;
let authCreatedDiscountId;
let cartId;
let lastResponse;
let lastBody;

async function createApiContext(role = 'anonymous') {
  const headers = {};

  if (role !== 'anonymous') {
    headers['X-User-Id'] = `test-${role}-user`;
  }

  if (role === 'admin') {
    headers['X-User-Role'] = 'Admin';
  } else if (role === 'user') {
    headers['X-User-Role'] = 'User';
  }

  return request.newContext({
    baseURL: BASE_URL,
    extraHTTPHeaders: headers
  });
}

BeforeAll(async function () {
  apiContext = await request.newContext({ baseURL: BASE_URL });
  shoppingCartApi = new ShoppingCartApiSom(apiContext);
  authApiContext = await createApiContext('admin');

  // Create a product to use in all tests via admin context (POST /api/products is protected)
  const productResponse = await authApiContext.post('/api/products', {
    data: createMugPayload()
  });
  const productBody = await productResponse.json();
  assert.equal(productResponse.status(), 201);
  assert.ok(productBody?.id);
  seededProductId = productBody.id;

  const now = new Date();
  const discountResponse = await authApiContext.post('/api/discounts', {
    data: {
      name: 'Spring 20',
      description: 'Discount for API tests',
      percentage: 20,
      code: VALID_DISCOUNT_CODE,
      startsAt: new Date(now.getTime() - 60 * 60 * 1000).toISOString(),
      endsAt: new Date(now.getTime() + 24 * 60 * 60 * 1000).toISOString(),
      isActive: true
    }
  });
  const discountBody = await discountResponse.json();
  assert.equal(discountResponse.status(), 201);
  assert.ok(discountBody?.id);
  seededDiscountId = discountBody.id;
});

AfterAll(async function () {
  if (authCreatedDiscountId) {
    await apiContext.delete(`/api/discounts/${authCreatedDiscountId}`);
  }
  if (cartId) {
    await shoppingCartApi.deleteShoppingCart(cartId);
  }
  if (seededDiscountId) {
    await apiContext.delete(`/api/discounts/${seededDiscountId}`);
  }
  if (seededProductId) {
    await authApiContext.delete(`/api/products/${seededProductId}`);
  }
  await authApiContext?.dispose();
  await apiContext?.dispose();
});

Given('I am authenticated as a regular user', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('user');
});

Given('I am authenticated as an admin user', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('admin');
});

Given('I am not authenticated', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('anonymous');
});

Given('a shopping cart exists with user {string} and product {string}', async function (userId, productName) {
  // Use the seeded product
  const cartPayload = {
    userId,
    sessionId: 'sess1',
    items: [{ productId: seededProductId, quantity: 1 }]
  };
  const response = await shoppingCartApi.createShoppingCart(cartPayload);
  const result = await shoppingCartApi.readResponse(response);
  assert.equal(result.status, 201);
  cartId = result.body.id;
});

When('I apply the discount code {string} to the cart', async function (code) {
  const response = await apiContext.post(`/api/shoppingcarts/${cartId}/apply-discount`, {
    data: { code }
  });
  lastResponse = response;
  lastBody = await response.json();
});

When('I apply the discount code {string} to the cart again', async function (code) {
  const response = await apiContext.post(`/api/shoppingcarts/${cartId}/apply-discount`, {
    data: { code }
  });
  lastResponse = response;
  lastBody = await response.json();
});

When('I create a discount with code {string}', async function (code) {
  const now = new Date();
  const response = await authApiContext.post('/api/discounts', {
    data: {
      name: `Test ${code}`,
      description: 'Authorization test discount',
      percentage: 10,
      code,
      startsAt: new Date(now.getTime() - 60 * 60 * 1000).toISOString(),
      endsAt: new Date(now.getTime() + 24 * 60 * 60 * 1000).toISOString(),
      isActive: true
    }
  });

  lastResponse = response;
  lastBody = await response.json().catch(() => null);

  if (response.status() === 201 && lastBody?.id) {
    authCreatedDiscountId = lastBody.id;
  }
});

Then('the cart total should reflect the discount', async function () {
  assert.equal(lastResponse.status(), 200);
  assert.ok(lastBody.totalPrice < 100);
});

Then('I should receive a 409 Conflict error', async function () {
  assert.equal(lastResponse.status(), 409);
});

Then('I should receive a 404 Not Found error', async function () {
  assert.equal(lastResponse.status(), 404);
});

Then('I should receive a 403 Forbidden error', function () {
  assert.equal(lastResponse.status(), 403);
});

Then('I should receive a 201 Created response', function () {
  assert.equal(lastResponse.status(), 201);
  assert.ok(lastBody?.id);
});
