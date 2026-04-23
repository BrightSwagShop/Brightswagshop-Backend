const { Before, After, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { ProductsApiSom, createMugPayload } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

let authApiContext;
let authProductsApi;

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

Before(async function () {
  this.apiContext = await createApiContext('admin');
  this.productsApi = new ProductsApiSom(this.apiContext);
});

After(async function () {
  await authApiContext?.dispose();
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

Given('I am authenticated as a regular user for products API', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('user');
  authProductsApi = new ProductsApiSom(authApiContext);
});

Given('I am authenticated as an admin user for products API', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('admin');
  authProductsApi = new ProductsApiSom(authApiContext);
});

Given('I am not authenticated for products API', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('anonymous');
  authProductsApi = new ProductsApiSom(authApiContext);
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

When('I POST {string} as a regular user with the payload', async function (path) {
  if (path !== '/api/products') {
    throw new Error(`Unsupported path for POST step: ${path}`);
  }

  const response = await authApiContext.post(path, {
    data: this.payload
  });

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

When('I DELETE {string} as a regular user', async function (path) {
  if (!path.startsWith('/api/products/')) {
    throw new Error(`Unsupported path for DELETE step: ${path}`);
  }

  const response = await authApiContext.delete(path);
  await storeResponse(this, response);
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
