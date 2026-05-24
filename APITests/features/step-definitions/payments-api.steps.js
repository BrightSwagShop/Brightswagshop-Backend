const { BeforeAll, AfterAll, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const { PaymentsApiSom, OrderApiSom, createOrderPayload } = require('@brightswagshop/testing-framework');

const BASE_URL = process.env.API_BASE_URL || 'http://127.0.0.1:5076';

let apiContext;
let paymentsApi;
let orderApi;
let createdOrderId;
let lastResponse;
let lastBodyText;

BeforeAll(async function () {
  apiContext = await request.newContext({ baseURL: BASE_URL });
  paymentsApi = new PaymentsApiSom(apiContext);
  orderApi = new OrderApiSom(apiContext);
});

AfterAll(async function () {
  await apiContext?.dispose();
});

async function storeResponse(response) {
  lastResponse = response;
  lastBodyText = await response.text();
}

Given('I create an empty order for payments', async function () {
  const payload = createOrderPayload();
  const response = await orderApi.createOrder(payload);
  const body = await response.json();
  assert.equal(response.status(), 201);
  assert.ok(body?.id);
  createdOrderId = body.id;
});

When('I create checkout session for unknown order id', async function () {
  const response = await paymentsApi.createCheckoutSession('000000000000000000000000');
  await storeResponse(response);
});

When('I create checkout session for the stored payments order', async function () {
  const response = await paymentsApi.createCheckoutSession(createdOrderId);
  await storeResponse(response);
});

When('I POST Stripe webhook event without signature', async function () {
  const response = await paymentsApi.postStripeWebhook({
    id: 'evt_test',
    type: 'checkout.session.completed'
  });
  await storeResponse(response);
});

Then('the payments response status should be {int}', function (statusCode) {
  assert.equal(lastResponse.status(), statusCode);
});

Then('the payments response should contain text {string}', function (text) {
  assert.ok(lastBodyText.includes(text));
});
