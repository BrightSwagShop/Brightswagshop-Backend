const { Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

function createMugPayload() {
  const unique = Date.now();

  return {
    $type: 'Mok',
    name: `Cucumber Mug ${unique}`,
    description: 'Cucumber API test product',
    price: 9.99,
    category: 'Drinkartikelen',
    productType: 'Mok',
    isActive: true,
    kleuren: [
      {
        kleur: 'Zwart',
        imageUrl: 'https://example.com/mug-black.png',
        stock: 10,
        sku: `MUG-${unique}`
      }
    ]
  };
}

async function request(method, path, body) {
  const response = await fetch(`${BASE_URL}${path}`, {
    method,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: body ? JSON.stringify(body) : undefined
  });

  let jsonBody = null;
  const contentType = response.headers.get('content-type') || '';
  if (contentType.includes('application/json')) {
    jsonBody = await response.json();
  }

  return { response, jsonBody };
}

Given('I have a valid mug payload', function () {
  this.payload = createMugPayload();
});

When('I GET {string}', async function (path) {
  const result = await request('GET', path);
  this.lastResponse = result.response;
  this.lastBody = result.jsonBody;
});

When('I POST {string} with the payload', async function (path) {
  const result = await request('POST', path, this.payload);
  this.lastResponse = result.response;
  this.lastBody = result.jsonBody;
});

When('I GET the created product by id', async function () {
  const result = await request('GET', `/api/products/${this.createdProductId}`);
  this.lastResponse = result.response;
  this.lastBody = result.jsonBody;
});

When('I DELETE the created product by id', async function () {
  const result = await request('DELETE', `/api/products/${this.createdProductId}`);
  this.lastResponse = result.response;
  this.lastBody = result.jsonBody;
});

Then('the response status should be {int}', function (statusCode) {
  assert.equal(this.lastResponse.status, statusCode);
});

Then('the response should be an array', function () {
  assert.ok(Array.isArray(this.lastBody));
});

Then('I store the created product id', function () {
  assert.ok(this.lastBody && this.lastBody.id);
  this.createdProductId = this.lastBody.id;
});
