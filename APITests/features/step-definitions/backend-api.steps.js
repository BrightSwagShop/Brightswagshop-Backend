const { Before, After, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { BackendApiSom } = require('../../tests/som/backend-api.som');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

Before(async function () {
  this.apiContext = await request.newContext({ baseURL: BASE_URL });
  this.backendApi = new BackendApiSom(this.apiContext);
});

After(async function () {
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
