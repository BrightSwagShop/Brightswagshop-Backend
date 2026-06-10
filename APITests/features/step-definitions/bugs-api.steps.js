'use strict';

const { Before, After, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const {
  createMugPayload,
  createUserPayload,
  createCartPayload,
} = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

async function createAdminContext() {
  return request.newContext({
    baseURL: BASE_URL,
    extraHTTPHeaders: {
      'X-User-Id': 'bug-test-admin',
      'X-User-Role': 'Admin',
    },
  });
}

async function createAnonContext() {
  return request.newContext({ baseURL: BASE_URL });
}

async function getBugStates(adminCtx) {
  const res = await adminCtx.get('/api/debug/settings');
  return res.json();
}

async function ensureBugEnabled(adminCtx, bugKey) {
  const state = await getBugStates(adminCtx);
  if (!state[bugKey]) {
    await adminCtx.post(`/api/debug/toggle/${bugKey}`);
  }
}

async function ensureBugDisabled(adminCtx, bugKey) {
  const state = await getBugStates(adminCtx);
  if (state[bugKey]) {
    await adminCtx.post(`/api/debug/toggle/${bugKey}`);
  }
}

Before(async function () {
  this.bugAdminCtx = await createAdminContext();
  this.bugAnonCtx = await createAnonContext();
  this.enabledBugs = [];
  this.testProductId = null;
  this.testProductPrice = null;
  this.testCartId = null;
  this.testUser = null;
  this.bugLastResponse = null;
  this.bugLastBody = null;
});

After(async function () {
  // Disable every bug that was enabled during this scenario
  for (const bugKey of (this.enabledBugs || [])) {
    try {
      await ensureBugDisabled(this.bugAdminCtx, bugKey);
    } catch (_) { /* best effort */ }
  }

  // Delete created cart
  if (this.testCartId) {
    try {
      await this.bugAnonCtx.delete(`/api/shoppingcarts/${this.testCartId}`);
    } catch (_) { /* best effort */ }
  }

  // Delete created product
  if (this.testProductId) {
    try {
      await this.bugAdminCtx.delete(`/api/products/${this.testProductId}`);
    } catch (_) { /* best effort */ }
  }

  await this.bugAnonCtx?.dispose();
  await this.bugAdminCtx?.dispose();
});

// ── Given ────────────────────────────────────────────────────────────────────

Given('the {string} debug bug is enabled', async function (bugKey) {
  const state = await getBugStates(this.bugAdminCtx);
  if (!state[bugKey]) {
    // Bug was off — we're turning it on, so we own cleanup
    await this.bugAdminCtx.post(`/api/debug/toggle/${bugKey}`);
    if (!this.enabledBugs.includes(bugKey)) {
      this.enabledBugs.push(bugKey);
    }
  }
  // Bug was already on (user manually enabled it) — leave it alone after the test
});

Given('a test product is seeded for bug testing', async function () {
  const payload = createMugPayload();
  const response = await this.bugAdminCtx.post('/api/products', { data: payload });
  assert.equal(response.status(), 201, 'Expected product creation to return 201');
  const body = await response.json();
  assert.ok(body.id, 'Expected created product to have an id');
  this.testProductId = body.id;
  this.testProductPrice = payload.price; // 9.99 — use the payload value, not the (potentially bug-masked) response
});

Given('a test user is registered for bug login testing', async function () {
  const payload = createUserPayload();
  const response = await this.bugAnonCtx.post('/api/users/register', { data: payload });
  assert.equal(response.status(), 200, 'Expected user registration to return 200');
  this.testUser = payload;
});

// ── When ─────────────────────────────────────────────────────────────────────

When('I request all products via the bug test context', async function () {
  const response = await this.bugAnonCtx.get('/api/products');
  this.bugLastResponse = response;
  const ct = response.headers()['content-type'] || '';
  this.bugLastBody = ct.includes('application/json') ? await response.json() : null;
});

When('I request the seeded test product by id', async function () {
  assert.ok(this.testProductId, 'No seeded test product id — run the Given step first');
  const response = await this.bugAnonCtx.get(`/api/products/${this.testProductId}`);
  this.bugLastResponse = response;
  const ct = response.headers()['content-type'] || '';
  this.bugLastBody = ct.includes('application/json') ? await response.json() : null;
});

When('I login with the bug test user credentials', async function () {
  assert.ok(this.testUser, 'No test user registered — run the Given step first');
  const response = await this.bugAnonCtx.post('/api/users/login', { data: this.testUser });
  this.bugLastResponse = response;
});

When('I create a cart with the seeded test product', async function () {
  assert.ok(this.testProductId, 'No seeded test product id — run the Given step first');
  const payload = createCartPayload(this.testProductId, { quantity: 1 });
  const response = await this.bugAnonCtx.post('/api/shoppingcarts', { data: payload });
  assert.equal(response.status(), 201, 'Expected cart creation to return 201');
  const body = await response.json();
  this.testCartId = body.id;
  this.bugLastBody = body;
});

// ── Then ─────────────────────────────────────────────────────────────────────

Then('the bug test response status should be {int}', function (expectedStatus) {
  assert.equal(
    this.bugLastResponse.status(),
    expectedStatus,
    `Expected status ${expectedStatus}, got ${this.bugLastResponse.status()}`,
  );
});

Then('all product color image URLs in the bug test response should be null', function () {
  const products = Array.isArray(this.bugLastBody) ? this.bugLastBody : [this.bugLastBody];
  assert.ok(products.length > 0, 'Expected at least one product in the response');

  for (const product of products) {
    const kleuren = product.kleuren ?? [];
    for (const kleur of kleuren) {
      assert.equal(
        kleur.imageUrl,
        null,
        `Expected imageUrl to be null for product "${product.name}" but got: ${kleur.imageUrl}`,
      );
    }
  }
});

Then('the bug test cart total should be the product price plus 10', function () {
  assert.ok(this.bugLastBody, 'No cart response body found');
  const actual = this.bugLastBody.totalPrice;
  const expected = this.testProductPrice + 10;
  assert.ok(
    Math.abs(actual - expected) < 0.01,
    `Expected cart totalPrice ~${expected} (price ${this.testProductPrice} + 10 bug), got ${actual}`,
  );
});
