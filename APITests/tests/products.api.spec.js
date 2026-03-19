const { test, expect } = require('@playwright/test');
const { qase } = require('playwright-qase-reporter/dist/playwright');
const { ProductsApiSom } = require('./som/products-api.som');
const { createMugPayload } = require('./data/product-payloads');

const BASE_URL = process.env.API_BASE_URL || 'http://localhost:5076';

test.describe('Products API', () => {
  let seededProductId;
  let apiContext;
  let productsApi;

  test.beforeAll(async ({ playwright }) => {
    apiContext = await playwright.request.newContext({ baseURL: BASE_URL });
    productsApi = new ProductsApiSom(apiContext);
    const response = await productsApi.createProduct(createMugPayload());
    const result = await productsApi.readResponse(response);

    expect(result.status).toBe(201);
    expect(result.body?.id).toBeTruthy();

    const created = result.body;
    seededProductId = created.id;
  });

  test.afterAll(async () => {
    if (seededProductId && apiContext) {
      await productsApi.deleteProduct(seededProductId);
    }
    await apiContext?.dispose();
  });

  test(qase(52, '[Products API - Smoke] GET /api/products returns 200 and array'), async ({ request }) => {
    const productsApi = new ProductsApiSom(request);
    const response = await productsApi.getAllProducts();
    const result = await productsApi.readResponse(response);

    expect(result.ok).toBeTruthy();
    expect(Array.isArray(result.body)).toBeTruthy();
  });

  test(qase(53, '[Products API - Smoke] GET /api/products/:id returns 200 for seeded product'), async ({ request }) => {
    const productsApi = new ProductsApiSom(request);
    const response = await productsApi.getProductById(seededProductId);
    const result = await productsApi.readResponse(response);

    expect(result.status).toBe(200);
    expect(result.body.id).toBe(seededProductId);
  });

  test(qase(54, '[Products API - Smoke] GET /api/products/:id returns 404 for unknown id'), async ({ request }) => {
    const productsApi = new ProductsApiSom(request);
    const response = await productsApi.getProductById('000000000000000000000000');
    const result = await productsApi.readResponse(response);

    expect(result.status).toBe(404);
  });
});
