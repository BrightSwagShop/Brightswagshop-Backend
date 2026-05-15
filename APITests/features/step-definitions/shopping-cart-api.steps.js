const { BeforeAll, AfterAll, Before, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const {
  BASE_URL,
  createHeaderAuthContext,
  readApiResponse,
  createValidMugPayload,
  createCartRequest
} = require('./api-helpers');

let anonymousContext;
let adminContext;
let seededProductFixtures = [];
let seededProductId;
const createdCartIds = new Set();

const fixedPriceCatalog = [
  { label: '61549841', price: 12.0 },
  { label: '5595897812', price: 20.0 },
  { label: '895984955', price: 30.0 }
];

BeforeAll(async function () {
  anonymousContext = await request.newContext({ baseURL: BASE_URL });
  adminContext = await createHeaderAuthContext('admin');

  for (const fixture of fixedPriceCatalog) {
    const response = await adminContext.post('/api/products', {
      data: createValidMugPayload({
        name: `cart-fixture-${fixture.label}-${Date.now()}`,
        imageUrl: 'https://example.invalid/cart-fixture.jpg'
      })
    });

    const body = await response.json();
    assert.equal(response.status(), 201);
    seededProductFixtures.push({
      ...fixture,
      id: body.id
    });
  }

  seededProductId = seededProductFixtures[0].id;
});

AfterAll(async function () {
  for (const cartId of createdCartIds) {
    await anonymousContext.delete(`/api/shoppingcarts/${cartId}`).catch(() => null);
  }

  for (const fixture of seededProductFixtures) {
    await adminContext.delete(`/api/products/${fixture.id}`).catch(() => null);
  }

  await adminContext?.dispose();
  await anonymousContext?.dispose();
});

Before(function () {
  this.cartRequest = null;
  this.lastResponse = null;
  this.lastBody = null;
  this.rememberedCartId = null;
  this.cartUserId = null;
  this.cartProductId = null;
  this.cartProductPrice = null;
  this.boundaryUserId = null;
  this.checkoutOrderId = null;
  this.productIdList = null;
  this.uploadFile = null;
});

async function storeResponse(world, response) {
  const result = await readApiResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
  world.lastBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
}

async function createCart(world) {
  const response = await anonymousContext.post('/api/shoppingcarts', {
    data: world.cartRequest
  });

  await storeResponse(world, response);
}

function resolveCatalogFixture(productIdLabel) {
  return seededProductFixtures.find(fixture => fixture.label === productIdLabel) || seededProductFixtures[0];
}

Given('I prepare a shopping cart request for the smoke user with quantity {int}', function (quantity) {
  this.cartUserId = `pw-user-${Date.now()}`;
  this.cartRequest = createCartRequest({
    userId: this.cartUserId,
    productId: seededProductId,
    quantity
  });
});

Given('I prepare a shopping cart request for the delete user with quantity {int}', function (quantity) {
  this.cartUserId = `pw-delete-${Date.now()}`;
  this.cartRequest = createCartRequest({
    userId: this.cartUserId,
    productId: seededProductId,
    quantity
  });
});

Given('I create and remember a shopping cart', async function () {
  await createCart(this);
  assert.equal(this.lastResponse.status(), 201);
  this.rememberedCartId = this.lastBody.id;
  createdCartIds.add(this.rememberedCartId);
});

Given(/^I have (\d+) (\d+) in my cart$/, function (amount, productIdLabel) {
  const fixture = resolveCatalogFixture(productIdLabel);
  this.cartUserId = `price-user-${Date.now()}`;
  this.cartProductId = fixture.id;
  this.cartProductPrice = fixture.price;
  this.cartRequest = createCartRequest({
    userId: this.cartUserId,
    productId: fixture.id,
    quantity: amount
  });
});

Given('there is no active Discount', function () {
  this.activeDiscountCode = null;
});

Given('I have a shopping cart with at least one product', async function () {
  this.cartUserId = `delete-user-${Date.now()}`;
  this.cartRequest = createCartRequest({
    userId: this.cartUserId,
    productId: seededProductId,
    quantity: 1
  });
  await createCart(this);
  assert.equal(this.lastResponse.status(), 201);
  this.rememberedCartId = this.lastBody.id;
  createdCartIds.add(this.rememberedCartId);
});

Given('I am authenticated as user {string}', function (userId) {
  this.boundaryUserId = userId;
});

Given('a fresh shopping cart exists for a user', async function () {
  this.cartUserId = `fresh-user-${Date.now()}`;
  this.cartRequest = createCartRequest({
    userId: this.cartUserId,
    productId: seededProductId,
    quantity: 0
  });
  await createCart(this);
  assert.equal(this.lastResponse.status(), 201);
  this.rememberedCartId = this.lastBody.id;
  createdCartIds.add(this.rememberedCartId);
});

Given('I have a shopping cart with at least one item for the customer', async function () {
  this.cartUserId = `customer-${Date.now()}`;
  this.cartRequest = createCartRequest({
    userId: this.cartUserId,
    productId: seededProductId,
    quantity: 1
  });
  await createCart(this);
  assert.equal(this.lastResponse.status(), 201);
  this.rememberedCartId = this.lastBody.id;
  createdCartIds.add(this.rememberedCartId);
});

// product id list preparation is owned by products-api.steps.js

Given('I prepare an upload file in the {string} boundary state', function (fileSize) {
  this.uploadFile = fileSize === 'zero-byte'
    ? { name: 'empty.jpg', mimeType: 'image/jpeg', buffer: Buffer.alloc(0) }
    : { name: `boundary-${Date.now()}.jpg`, mimeType: 'image/jpeg', buffer: Buffer.from([0xff, 0xd8, 0xff, 0xd9]) };
});

Given('I prepare an order in the {string} boundary state', async function (itemCount) {
  this.checkoutUserId = `order-user-${Date.now()}`;
  const orderRequest = itemCount === 'zero-items'
    ? { userId: this.checkoutUserId, items: [] }
    : { userId: this.checkoutUserId, items: [{ productId: seededProductId, quantity: 1 }] };

  const response = await anonymousContext.post('/api/orders', {
    data: orderRequest
  });

  const result = await readApiResponse(response);
  assert.equal(result.response.status(), 201);
  this.checkoutOrderId = result.body.id;
  this.checkoutOrderMode = itemCount;
});

When('I create the shopping cart', async function () {
  await createCart(this);
});

When('I get the shopping cart for the smoke user', async function () {
  const response = await anonymousContext.get(`/api/shoppingcarts/user/${this.cartUserId}`);
  await storeResponse(this, response);
});

When('I get the shopping cart for the delete user', async function () {
  const response = await anonymousContext.get(`/api/shoppingcarts/user/${this.cartUserId}`);
  await storeResponse(this, response);
});

When('I delete the remembered shopping cart', async function () {
  const response = await anonymousContext.delete(`/api/shoppingcarts/${this.rememberedCartId}`);
  await storeResponse(this, response);

  if (this.lastResponse.status() === 204) {
    createdCartIds.delete(this.rememberedCartId);
    this.rememberedCartId = null;
  }
});

When('I GET my cart', async function () {
  const response = await anonymousContext.get(`/api/shoppingcarts/user/${this.cartUserId}`);
  await storeResponse(this, response);
});

When('I delete a product from my shoppingcart', async function () {
  const path = `/api/shoppingcarts/user/${this.cartUserId}/item`;
  const response = await anonymousContext.delete(path, {
    data: {
      productId: seededProductId,
      selectedColor: null,
      quantity: 1
    }
  });
  await storeResponse(this, response);
});

When('I POST {string} with a product and quantity {int}', async function (path, quantity) {
  const resolvedPath = path.replace('{userId}', this.cartUserId);
  const response = await anonymousContext.post(resolvedPath, {
    data: {
      productId: seededProductId,
      quantity,
      selectedColor: null
    }
  });
  await storeResponse(this, response);
  if (this.lastResponse.status() === 200 && this.lastBody?.id) {
    this.rememberedCartId = this.lastBody.id;
    createdCartIds.add(this.rememberedCartId);
  }
});

When('I PUT {string} with the same product and quantity {int}', async function (path, quantity) {
  const resolvedPath = path.replace('{userId}', this.cartUserId);
  const response = await anonymousContext.put(resolvedPath, {
    data: {
      productId: seededProductId,
      quantity,
      selectedColor: null
    }
  });
  await storeResponse(this, response);
});

When('I DELETE {string} with the same product', async function (path) {
  const resolvedPath = path.replace('{userId}', this.cartUserId);
  const response = await anonymousContext.delete(resolvedPath, {
    data: {
      productId: seededProductId,
      selectedColor: null,
      quantity: 1
    }
  });
  await storeResponse(this, response);
});

When('I DELETE {string}', async function (path) {
  const resolvedPath = path.replace('{cartId}', this.rememberedCartId);
  const response = await anonymousContext.delete(resolvedPath);
  await storeResponse(this, response);
  if (this.lastResponse.status() === 204) {
    createdCartIds.delete(this.rememberedCartId);
    this.rememberedCartId = null;
  }
});

// admin image upload is implemented in admin-api.steps.js to avoid duplicate matchers

When('I POST {string} with quantity {int}', async function (path, quantity) {
  const resolvedPath = path.replace('{userId}', this.boundaryUserId);
  const response = await anonymousContext.post(resolvedPath, {
    data: {
      productId: seededProductId,
      quantity,
      selectedColor: null
    }
  });
  await storeResponse(this, response);
});

Then('the shopping cart response status should be {int}', function (statusCode) {
  assert.equal(this.lastResponse.status(), statusCode);
});

Then('the created shopping cart should match the smoke user', function () {
  assert.equal(this.lastBody.userId, this.cartUserId);
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
  assert.equal(this.lastBody.userId, this.cartUserId);
  assert.ok(typeof this.lastBody.id === 'string');
  assert.ok(Array.isArray(this.lastBody.items));
  assert.ok(this.lastBody.items.length > 0);
});

Then('the returned shopping cart should contain the seeded product', function () {
  assert.equal(this.lastBody.items[0].productId, seededProductId);
});

Then(/^the cart totalPrice should be (\d+) \* (\d+(?:\.\d+)?)$/, function (amount, productPrice) {
  const priceFixture = fixedPriceCatalog.find(fixture => Number(fixture.price) === Number(productPrice));
  assert.ok(priceFixture, `Missing price fixture for ${productPrice}`);
  assert.equal(Number(this.lastBody.totalPrice), amount * priceFixture.price);
});

Then('the cart should contain exactly one item', function () {
  assert.equal(this.lastBody.items.length, 1);
});

Then('the cart item quantity should be {int}', function (quantity) {
  assert.equal(this.lastBody.items[0].quantity, quantity);
});

Then('the cart should no longer contain that product', function () {
  assert.ok(Array.isArray(this.lastBody.items));
  assert.equal(this.lastBody.items.length, 0);
});

Then('the cart should no longer exist', function () {
  assert.equal(this.lastResponse.status(), 204);
});

