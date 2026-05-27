const { Before, After, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { UsersApiSom, createUserPayload } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

Before(async function () {
  this.userApiContext = await request.newContext({ baseURL: BASE_URL });
  this.usersApi = new UsersApiSom(this.userApiContext);
  this.userPayload = null;
  this.userResponse = null;
  this.userBody = null;
});

After(async function () {
  await this.userApiContext?.dispose();
});

Given('I prepare a unique user registration payload', function () {
  this.userPayload = createUserPayload();
});

Given('I register a unique public user', async function () {
  this.userPayload = createUserPayload();
  const response = await this.usersApi.register(this.userPayload);
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
  let response;
  if (path === '/api/users/register') {
    response = await this.usersApi.register(this.userPayload);
  } else if (path === '/api/users/login') {
    response = await this.usersApi.login(this.userPayload);
  } else {
    response = await this.userApiContext.post(path, { data: this.userPayload });
  }

  this.userResponse = response;
  const contentType = response.headers()['content-type'] || '';
  this.userBody = contentType.includes('application/json') ? await response.json() : null;
});

When('I POST {string} with the same user credentials', async function (path) {
  let response;
  if (path === '/api/users/register') {
    response = await this.usersApi.register(this.userPayload);
  } else if (path === '/api/users/login') {
    response = await this.usersApi.login(this.userPayload);
  } else {
    response = await this.userApiContext.post(path, { data: this.userPayload });
  }

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
  const responseUsername = this.userBody?.username ?? this.userBody?.user?.username;
  assert.equal(responseUsername, this.userPayload.username);
});
