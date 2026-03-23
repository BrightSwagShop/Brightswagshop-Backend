const { BeforeAll, Before, AfterAll, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { ShoppingCartApiSom } = require('../../tests/som/shopping-cart-api.som');
const { createMugPayload } = require('../../tests/data/product-payloads');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

let apiContext;
let shoppingCartApi;
let seededProductId;
const createdCartIds = new Set();

const unique = Date.now();
const smokeUserId = `pw-user-${unique}`;
const deleteUserId = `${smokeUserId}-delete`;

BeforeAll(async function () {
  apiContext = await request.newContext({ baseURL: BASE_URL });
  shoppingCartApi = new ShoppingCartApiSom(apiContext);

  const response = await shoppingCartApi.createProduct(createMugPayload());
  const result = await shoppingCartApi.readResponse(response);

  assert.equal(result.status, 201);
  assert.ok(result.body?.id);

  seededProductId = result.body.id;
});

Before(function () {
  this.cartRequest = null;
  this.lastResponse = null;
  this.lastBody = null;
  this.rememberedCartId = null;
});

AfterAll(async function () {
  for (const cartId of createdCartIds) {
    await shoppingCartApi.deleteShoppingCart(cartId);
  }

  if (seededProductId) {
    await shoppingCartApi.deleteProduct(seededProductId);
  }

  await apiContext?.dispose();
});

async function storeResponse(world, response) {
  const result = await shoppingCartApi.readResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
}

async function createCart(world) {
  const response = await shoppingCartApi.createShoppingCart(world.cartRequest);
  await storeResponse(world, response);
}

Given('I prepare a shopping cart request for the smoke user with quantity {int}', function (quantity) {
  this.cartRequest = {
    userId: smokeUserId,
    items: [{ productId: seededProductId, quantity }]
  };
});

Given('I prepare a shopping cart request for the delete user with quantity {int}', function (quantity) {
  this.cartRequest = {
    userId: deleteUserId,
    items: [{ productId: seededProductId, quantity }]
  };
});

Given('I create and remember a shopping cart', async function () {
  await createCart(this);
  assert.equal(this.lastResponse.status(), 201);
  this.rememberedCartId = this.lastBody.id;
  createdCartIds.add(this.rememberedCartId);
});

When('I create the shopping cart', async function () {
  await createCart(this);
});

When('I get the shopping cart for the smoke user', async function () {
  const response = await shoppingCartApi.getShoppingCartByUserId(smokeUserId);
  await storeResponse(this, response);
});

When('I get the shopping cart for the delete user', async function () {
  const response = await shoppingCartApi.getShoppingCartByUserId(deleteUserId);
  await storeResponse(this, response);
});

When('I delete the remembered shopping cart', async function () {
  const response = await shoppingCartApi.deleteShoppingCart(this.rememberedCartId);
  await storeResponse(this, response);

  if (this.lastResponse.status() === 204) {
    createdCartIds.delete(this.rememberedCartId);
    this.rememberedCartId = null;
  }
});

Then('the shopping cart response status should be {int}', function (statusCode) {
  assert.equal(this.lastResponse.status(), statusCode);
});

Then('the created shopping cart should match the smoke user', function () {
  assert.equal(this.lastBody.userId, smokeUserId);
  assert.equal(this.lastBody.sessionId, null);
  assert.ok(typeof this.lastBody.id === 'string');
  assert.ok(typeof this.lastBody.totalPrice === 'number');
  assert.ok(Array.isArray(this.lastBody.items));
});

Then('the created shopping cart should contain one item for seeded product with quantity {int}', function (quantity) {
  assert.equal(this.lastBody.items.length, 1);
  const firstItem = this.lastBody.items[0];
  assert.equal(firstItem.productId, seededProductId);
  assert.ok(typeof firstItem.productName === 'string');
  assert.ok(typeof firstItem.unitPrice === 'number');
  assert.equal(firstItem.quantity, quantity);
});

Then('I remember the created shopping cart id', function () {
  this.rememberedCartId = this.lastBody.id;
  createdCartIds.add(this.rememberedCartId);
});

Then('the returned shopping cart should belong to the smoke user', function () {
  assert.equal(this.lastBody.userId, smokeUserId);
  assert.ok(typeof this.lastBody.id === 'string');
  assert.ok(Array.isArray(this.lastBody.items));
  assert.ok(this.lastBody.items.length > 0);
});

Then('the returned shopping cart should contain the seeded product', function () {
  assert.equal(this.lastBody.items[0].productId, seededProductId);
});
