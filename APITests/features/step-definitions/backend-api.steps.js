const { Before, After, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { BackendApiSom } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

let authApiContext;
let authBackendApi;

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
  this.apiContext = await request.newContext({ baseURL: BASE_URL });
  this.backendApi = new BackendApiSom(this.apiContext);
});

After(async function () {
  await authApiContext?.dispose();
  await this.apiContext?.dispose();
});

async function storeResponse(world, response) {
  const result = await world.backendApi.readResponse(response);
  world.lastResponse = result.response;
  world.lastBody = result.body;
}

When('I GET backend categories', async function () {
  const response = await this.backendApi.getCategories();
  await storeResponse(this, response);
});

When('I GET backend product types', async function () {
  const response = await this.backendApi.getProductTypes();
  await storeResponse(this, response);
});

When('I POST backend image upload without file', async function () {
  const response = await this.backendApi.uploadImageWithoutFile();
  await storeResponse(this, response);
});

When('I POST backend image upload as a regular user without file', async function () {
  const response = await authApiContext.post('/api/images/upload');
  await storeResponse(this, response);
});

Given('I am authenticated as a regular user', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('user');
  authBackendApi = new BackendApiSom(authApiContext);
});

Given('I am authenticated as an admin user', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('admin');
  authBackendApi = new BackendApiSom(authApiContext);
});

Given('I am not authenticated', async function () {
  await authApiContext?.dispose();
  authApiContext = await createApiContext('anonymous');
  authBackendApi = new BackendApiSom(authApiContext);
});

Then('the backend response status should be {int}', function (statusCode) {
  assert.equal(this.lastResponse.status(), statusCode);
});

Then('the backend response should be a non-empty array', function () {
  assert.ok(Array.isArray(this.lastBody));
  assert.ok(this.lastBody.length > 0);
});

Then('the first backend item should contain number id and string name', function () {
  const first = this.lastBody[0];
  assert.equal(typeof first.id, 'number');
  assert.equal(typeof first.name, 'string');
});

Then('the first backend item should contain string name and string slug', function () {
  const first = this.lastBody[0];
  assert.equal(typeof first.name, 'string');
  assert.equal(typeof first.slug, 'string');
});
