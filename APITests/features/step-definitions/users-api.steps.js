const { Before, After, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

Before(async function () {
  this.userApiContext = await request.newContext({ baseURL: BASE_URL });
  this.userPayload = null;
  this.userResponse = null;
  this.userBody = null;
});

After(async function () {
  await this.userApiContext?.dispose();
});

Given('I prepare a unique user registration payload', function () {
  const unique = Date.now();
  this.userPayload = {
    username: `api-user-${unique}`,
    password: `P@ss-${unique}`
  };
});

Given('I register a unique public user', async function () {
  const unique = Date.now();
  this.userPayload = {
    username: `api-login-${unique}`,
    password: `P@ss-${unique}`
  };

  const response = await this.userApiContext.post('/api/users/register', {
    data: this.userPayload
  });

  const body = await response.json();
  assert.equal(response.status(), 200);
  assert.ok(body?.id);
});

Given('I prepare credentials for an unknown public user', function () {
  const unique = Date.now();
  this.userPayload = {
    username: `unknown-user-${unique}`,
    password: `Wrong-${unique}`
  };
});

When('I POST {string} with the user payload', async function (path) {
  const response = await this.userApiContext.post(path, {
    data: this.userPayload
  });

  this.userResponse = response;

  const contentType = response.headers()['content-type'] || '';
  if (contentType.includes('application/json')) {
    this.userBody = await response.json();
  } else {
    this.userBody = null;
  }
});

When('I POST {string} with the same user credentials', async function (path) {
  const response = await this.userApiContext.post(path, {
    data: this.userPayload
  });

  this.userResponse = response;
  this.userBody = await response.json();
});

Then('the users response status should be {int}', function (statusCode) {
  assert.equal(this.userResponse.status(), statusCode);
});

Then('the users response should contain a user id', function () {
  assert.ok(this.userBody && this.userBody.id);
});

Then('the users response username should match the request', function () {
  assert.equal(this.userBody.username, this.userPayload.username);
});
