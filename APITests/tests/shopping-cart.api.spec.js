const { test, expect } = require('@playwright/test');
const { ShoppingCartApiSom } = require('./som/shopping-cart-api.som');
const { createMugPayload } = require('./data/product-payloads');

const BASE_URL = process.env.API_BASE_URL || 'http://localhost:5076';

test.describe('Shopping Cart API - Smoke', () => {
  let apiContext;
  let shoppingCartApi;
  let seededProductId;
  let createdCartIds = [];
  const unique = Date.now();
  const userId = `pw-user-${unique}`;
  const sessionId = `pw-session-${unique}`;

  test.beforeAll(async ({ playwright }) => {
    apiContext = await playwright.request.newContext({ baseURL: BASE_URL });
    shoppingCartApi = new ShoppingCartApiSom(apiContext);

    const response = await shoppingCartApi.createProduct(createMugPayload());
    const result = await shoppingCartApi.readResponse(response);

    expect(result.status).toBe(201);
    expect(result.body?.id).toBeTruthy();

    seededProductId = result.body.id;
  });

  test.afterAll(async () => {
    if (shoppingCartApi) {
      for (const cartId of createdCartIds) {
        await shoppingCartApi.deleteShoppingCart(cartId);
      }
    }

    if (seededProductId && shoppingCartApi) {
      await shoppingCartApi.deleteProduct(seededProductId);
    }

    await apiContext?.dispose();
  });

  test('[Shopping Cart API - Smoke] POST /api/shoppingcarts creates cart', async ({ request }) => {
    const shoppingCartApi = new ShoppingCartApiSom(request);
    const response = await shoppingCartApi.createShoppingCart({
      userId,
      items: [
        {
          productId: seededProductId,
          quantity: 2
        }
      ]
    });
    const result = await shoppingCartApi.readResponse(response);

    expect(result.status).toBe(201);
    expect(result.body).toEqual(expect.objectContaining({
      id: expect.any(String),
      userId,
      sessionId: null,
      totalPrice: expect.any(Number),
      items: expect.any(Array)
    }));
    expect(result.body.items.length).toBe(1);
    expect(result.body.items[0]).toEqual(expect.objectContaining({
      productId: seededProductId,
      productName: expect.any(String),
      unitPrice: expect.any(Number),
      quantity: 2
    }));

    createdCartIds.push(result.body.id);
  });

  test('[Shopping Cart API - Smoke] GET /api/shoppingcarts/user/:userId returns created cart', async ({ request }) => {
    const shoppingCartApi = new ShoppingCartApiSom(request);
    const response = await shoppingCartApi.getShoppingCartByUserId(userId);
    const result = await shoppingCartApi.readResponse(response);

    expect(result.status).toBe(200);
    expect(result.body).toEqual(expect.objectContaining({
      id: expect.any(String),
      userId,
      items: expect.any(Array)
    }));
    expect(result.body.items.length).toBeGreaterThan(0);
    expect(result.body.items[0].productId).toBe(seededProductId);
  });

  test('[Shopping Cart API - Smoke] DELETE /api/shoppingcarts/:id deletes cart', async ({ request }) => {
    const shoppingCartApi = new ShoppingCartApiSom(request);

    const createResponse = await shoppingCartApi.createShoppingCart({
      userId: `${userId}-delete`,
      items: [
        {
          productId: seededProductId,
          quantity: 1
        }
      ]
    });
    const createResult = await shoppingCartApi.readResponse(createResponse);

    expect(createResult.status).toBe(201);
    const cartId = createResult.body.id;

    const deleteResponse = await shoppingCartApi.deleteShoppingCart(cartId);
    const deleteResult = await shoppingCartApi.readResponse(deleteResponse);

    expect(deleteResult.status).toBe(204);

    const getAfterDeleteResponse = await shoppingCartApi.getShoppingCartByUserId(`${userId}-delete`);
    const getAfterDeleteResult = await shoppingCartApi.readResponse(getAfterDeleteResponse);

    expect(getAfterDeleteResult.status).toBe(404);
  });
});