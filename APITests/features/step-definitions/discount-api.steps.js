const { Given, When, Then } = require('@cucumber/cucumber');
const { expect } = require('@playwright/test');
const request = require('supertest');
const apiUrl = process.env.API_URL || 'http://localhost:5000/api';

let cartId;
let lastResponse;

Given('a shopping cart exists with user {string} and product {string}', async function (userId, productName) {
  // Create product (assume product API exists)
  const productRes = await request(apiUrl)
    .post('/products')
    .send({ name: productName, price: 100 });
  const productId = productRes.body.id;

  // Create cart
  const cartRes = await request(apiUrl)
    .post('/shoppingcarts')
    .send({ userId, sessionId: 'sess1', items: [{ productId, quantity: 1 }] });
  cartId = cartRes.body.id;
});

When('I apply the discount code {string} to the cart', async function (code) {
  lastResponse = await request(apiUrl)
    .post(`/shoppingcarts/${cartId}/apply-discount`)
    .send({ code });
});

Then('the cart total should reflect the discount', async function () {
  expect(lastResponse.status).toBe(200);
  expect(lastResponse.body.totalPrice).toBeLessThan(100);
});

Then('I should receive a 409 Conflict error', async function () {
  expect(lastResponse.status).toBe(409);
});

Then('I should receive a 404 Not Found error', async function () {
  expect(lastResponse.status).toBe(404);
});
