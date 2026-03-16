const { test, expect } = require('@playwright/test');
const { qase } = require('playwright-qase-reporter/dist/playwright');

function createMugPayload() {
  const unique = Date.now();

  return {
    $type: 'Mok',
    name: `Playwright Mug ${unique}`,
    description: 'Simple API test product',
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

test.describe('Products API', () => {
  test(qase(52, '[Products API - Smoke] GET /api/products returns 200 and array'), async ({ request }) => {
    const response = await request.get('/api/products');

    expect(response.ok()).toBeTruthy();
    const body = await response.json();
    expect(Array.isArray(body)).toBeTruthy();
  });

  test(qase(53, '[Products API - Smoke] GET /api/products/:id returns 404 for unknown id'), async ({ request }) => {
    const response = await request.get('/api/products/000000000000000000000000');

    expect(response.status()).toBe(404);
  });

  test(qase(54, '[Products API - Smoke] POST + GET + DELETE product flow'), async ({ request }) => {
    const createResponse = await request.post('/api/products', {
      data: createMugPayload()
    });

    expect(createResponse.status()).toBe(201);
    const created = await createResponse.json();
    expect(created.id).toBeTruthy();

    const getResponse = await request.get(`/api/products/${created.id}`);
    expect(getResponse.status()).toBe(200);

    const deleteResponse = await request.delete(`/api/products/${created.id}`);
    expect(deleteResponse.status()).toBe(204);
  });
});
