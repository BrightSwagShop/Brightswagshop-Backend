const { BeforeAll, AfterAll, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { ShoppingCartApiSom, createMugPayload } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';
const VALID_DISCOUNT_CODE = 'SPRING20';

let apiContext;
let shoppingCartApi;
let seededProductId;
let seededDiscountId;
let cartId;
let lastResponse;
let lastBody;

BeforeAll(async function () {
  apiContext = await request.newContext({ baseURL: BASE_URL });
  shoppingCartApi = new ShoppingCartApiSom(apiContext);
  // Create a product to use in all tests
  const response = await shoppingCartApi.createProduct(createMugPayload());
  const result = await shoppingCartApi.readResponse(response);
  assert.equal(result.status, 201);
  assert.ok(result.body?.id);
  seededProductId = result.body.id;

  const now = new Date();
  const discountResponse = await apiContext.post('/api/discounts', {
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
  if (cartId) {
    await shoppingCartApi.deleteShoppingCart(cartId);
  }
  if (seededDiscountId) {
    await apiContext.delete(`/api/discounts/${seededDiscountId}`);
  }
  if (seededProductId) {
    await shoppingCartApi.deleteProduct(seededProductId);
  }
  await apiContext?.dispose();
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
