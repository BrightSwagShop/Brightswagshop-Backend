const { Before, After, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { ProductsApiSom, createMugPayload } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

Before(async function () {
  this.apiContext = await request.newContext({ baseURL: BASE_URL });
  this.productsApi = new ProductsApiSom(this.apiContext);
});

After(async function () {
  if (this.createdProductId) {
    await this.productsApi.deleteProduct(this.createdProductId);
  }

  await this.apiContext?.dispose();
});

async function storeResponse(world, response) {
  const result = await world.productsApi.readResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
}

Given('I have a valid mug payload', function () {
  this.payload = createMugPayload();
});

When('I GET {string}', async function (path) {
  if (path === '/api/products') {
    const response = await this.productsApi.getAllProducts();
    await storeResponse(this, response);
    return;
  }

  if (path.startsWith('/api/products/')) {
    const productId = path.replace('/api/products/', '');
    const response = await this.productsApi.getProductById(productId);
    await storeResponse(this, response);
    return;
  }

  throw new Error(`Unsupported path for GET step: ${path}`);
});

When('I POST {string} with the payload', async function (path) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for POST step: ${path}`);
  }

  const response = await this.productsApi.createProduct(this.payload);
  await storeResponse(this, response);
});

When('I GET the created product by id', async function () {
  const response = await this.productsApi.getProductById(this.createdProductId);
  await storeResponse(this, response);
});

When('I DELETE the created product by id', async function () {
  const response = await this.productsApi.deleteProduct(this.createdProductId);
  await storeResponse(this, response);
  if (this.lastResponse.status() === 204) {
    this.createdProductId = null;
  }
});

Then('the response status should be {int}', function (statusCode) {
  assert.equal(this.lastResponse.status(), statusCode);
});

Then('the response should be an array', function () {
  assert.ok(Array.isArray(this.lastBody));
});

Then('I store the created product id', function () {
  assert.ok(this.lastBody && this.lastBody.id);
  this.createdProductId = this.lastBody.id;
});
