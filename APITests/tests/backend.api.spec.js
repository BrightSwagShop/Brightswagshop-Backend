const { test, expect } = require('@playwright/test');
const { BackendApiSom } = require('./som/backend-api.som');

test.describe('Backend API - Smoke', () => {
  test('GET /api/categories returns 200 and list with id/name', async ({ request }) => {
    const backendApi = new BackendApiSom(request);
    const response = await backendApi.getCategories();
    const result = await backendApi.readResponse(response);

    expect(result.status).toBe(200);
    expect(Array.isArray(result.body)).toBeTruthy();
    expect(result.body.length).toBeGreaterThan(0);
    expect(result.body[0]).toEqual(expect.objectContaining({
      id: expect.any(Number),
      name: expect.any(String)
    }));
  });

  test('GET /api/producttypes returns 200 and list with name/slug', async ({ request }) => {
    const backendApi = new BackendApiSom(request);
    const response = await backendApi.getProductTypes();
    const result = await backendApi.readResponse(response);

    expect(result.status).toBe(200);
    expect(Array.isArray(result.body)).toBeTruthy();
    expect(result.body.length).toBeGreaterThan(0);
    expect(result.body[0]).toEqual(expect.objectContaining({
      name: expect.any(String),
      slug: expect.any(String)
    }));
  });

  test('POST /api/images/upload without file returns 400', async ({ request }) => {
    const backendApi = new BackendApiSom(request);
    const response = await backendApi.uploadImageWithoutFile();
    const result = await backendApi.readResponse(response);

    expect(result.status).toBe(400);
  });
});