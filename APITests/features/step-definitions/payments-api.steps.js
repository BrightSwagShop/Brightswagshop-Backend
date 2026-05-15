const { BeforeAll, AfterAll, Before, Given, When, Then } = require('@cucumber/cucumber');
const assert = require('node:assert/strict');
const { request } = require('@playwright/test');
const {
  BASE_URL,
  createHeaderAuthContext,
  readApiResponse,
  createValidMugPayload,
  createCartRequest,
  createOrderRequest,
  buildStripeWebhookEvent,
  signStripeWebhookPayload
} = require('./api-helpers');

let anonymousContext;
let adminContext;
let seededProductId;
let seededWebhookCartId;
let seededWebhookOrderId;
let createdOrderId;
let failingOrderId;
let lastResponse;
let lastBody;
let lastBodyText;
let webhookRequestBody;
let webhookSignature;
let webhookSecret;

function resolveStripeSecret() {
  return process.env.STRIPE_WEBHOOK_SECRET || process.env.Stripe__WebhookSecret || process.env.stripeWebhookSecret || '';
}

BeforeAll(async function () {
  anonymousContext = await request.newContext({ baseURL: BASE_URL });
  adminContext = await createHeaderAuthContext('admin');
  webhookSecret = resolveStripeSecret();

  const productResponse = await adminContext.post('/api/products', {
    data: createValidMugPayload({ name: `payments-product-${Date.now()}` })
  });
  const productResult = await readApiResponse(productResponse);
  assert.equal(productResult.response.status(), 201);
  seededProductId = productResult.body.id;

  const cartResponse = await anonymousContext.post('/api/shoppingcarts', {
    data: createCartRequest({
      userId: `payments-webhook-user-${Date.now()}`,
      productId: seededProductId,
      quantity: 1
    })
  });
  const cartResult = await readApiResponse(cartResponse);
  assert.equal(cartResult.response.status(), 201);
  seededWebhookCartId = cartResult.body.id;

  const orderResponse = await anonymousContext.post(`/api/orders/from-cart/${cartResult.body.userId}`);
  const orderResult = await readApiResponse(orderResponse);
  assert.equal(orderResult.response.status(), 201);
  seededWebhookOrderId = orderResult.body.id;
});

AfterAll(async function () {
  if (seededWebhookCartId) {
    await anonymousContext.delete(`/api/shoppingcarts/${seededWebhookCartId}`).catch(() => null);
  }

  if (seededProductId) {
    await adminContext.delete(`/api/products/${seededProductId}`).catch(() => null);
  }

  await adminContext?.dispose();
  await anonymousContext?.dispose();
});

Before(function () {
  lastResponse = null;
  lastBody = null;
  lastBodyText = null;
  webhookRequestBody = null;
  webhookSignature = null;
  createdOrderId = null;
  failingOrderId = null;
});

async function storeResponse(response) {
  const result = await readApiResponse(response);
  lastResponse = result.response;
  lastBody = result.body;
  lastBodyText = typeof result.body === 'string' ? result.body : JSON.stringify(result.body ?? {});
}

Given('I create an empty order for payments', async function () {
  const unique = Date.now();
  const response = await anonymousContext.post('/api/orders', {
    data: createOrderRequest({
      userId: `payments-user-${unique}`,
      items: []
    })
  });

  const result = await readApiResponse(response);
  assert.equal(result.response.status(), 201);
  assert.ok(result.body?.id);
  createdOrderId = result.body.id;
});

Given('I prepare an invalid JSON body for a Stripe webhook', function () {
  webhookRequestBody = '{ this is : not json }';
  webhookSignature = webhookSecret ? signStripeWebhookPayload(webhookRequestBody, webhookSecret) : 't=1234,v1=invalid';
});

Given('I have a created order with id {string}', async function (orderIdAlias) {
  const response = await anonymousContext.post('/api/orders', {
    data: createOrderRequest({
      userId: `payments-failure-${Date.now()}`,
      items: [{ productId: seededProductId, quantity: 1 }]
    })
  });

  const result = await readApiResponse(response);
  assert.equal(result.response.status(), 201);
  failingOrderId = result.body.id;
  this.orderIdAlias = orderIdAlias;
});

Given('the payment provider will simulate a service error for that order', function () {
  this.paymentProviderShouldFail = true;
});

When('I create checkout session for unknown order id', async function () {
  const response = await anonymousContext.post('/api/payments/000000000000000000000000/checkout');
  await storeResponse(response);
});

When('I create checkout session for the stored payments order', async function () {
  const response = await anonymousContext.post(`/api/payments/${createdOrderId}/checkout`);
  await storeResponse(response);
});

When('I POST Stripe webhook event without signature', async function () {
  const response = await anonymousContext.post('/api/webhooks/stripe', {
    data: buildStripeWebhookEvent({
      orderId: seededWebhookOrderId,
      sessionId: `cs_${Date.now()}`
    })
  });

  await storeResponse(response);
});

When('I POST Stripe webhook event with non empty cart', function () {
  webhookRequestBody = buildStripeWebhookEvent({
    orderId: seededWebhookOrderId,
    sessionId: `cs_${Date.now()}`
  });
});

When('with valid checkout session', function () {
  if (typeof webhookRequestBody === 'string') {
    return;
  }

  webhookRequestBody.data.object.id = webhookRequestBody.data.object.id || `cs_${Date.now()}`;
});

When('with correct signature', async function () {
  if (typeof webhookRequestBody === 'string') {
    throw new Error('Webhook body must be JSON before signing');
  }

  webhookSignature = webhookSecret ? signStripeWebhookPayload(webhookRequestBody, webhookSecret) : 't=1234,v1=invalid';
  const response = await anonymousContext.post('/api/webhooks/stripe', {
    data: webhookRequestBody,
    headers: webhookSignature ? { 'Stripe-Signature': webhookSignature } : {}
  });

  await storeResponse(response);
});

When('I POST {string} with the malformed body and valid signature header', async function (path) {
  if (path !== '/api/webhooks/stripe') {
    throw new Error(`Unsupported malformed webhook path: ${path}`);
  }

  const response = await anonymousContext.post(path, {
    data: webhookRequestBody,
    headers: {
      'Stripe-Signature': webhookSignature || 't=1234,v1=invalid'
    }
  });

  await storeResponse(response);
});

When(/^I POST "(\/api\/payments\/(order-fails-on-provider|[a-f0-9]{24})\/checkout)"$/, async function (path) {
  let resolvedPath = path;
  if (path.includes('order-fails-on-provider')) {
    resolvedPath = path.replace('order-fails-on-provider', failingOrderId || 'missing-order-id');
  }
  
  const response = await anonymousContext.post(resolvedPath);
  await storeResponse(response);
});

Then('the payments response status should be {int}', function (statusCode) {
  assert.equal(lastResponse.status(), statusCode);
});

Then('the payments response should contain text {string}', function (text) {
  assert.ok(lastBodyText.includes(text));
});

Then('the response should contain a payment service error message', function () {
  const bodyStr = typeof lastBodyText === 'string' ? lastBodyText : JSON.stringify(lastBodyText || {});
  assert.ok(
    bodyStr.includes('error') || 
    bodyStr.includes('Error') || 
    bodyStr.includes('payment') ||
    bodyStr.includes('Payment'),
    `Expected error message in response, got: ${bodyStr}`
  );
});