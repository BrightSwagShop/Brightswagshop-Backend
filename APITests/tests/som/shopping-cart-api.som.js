class ShoppingCartApiSom {
  constructor(request) {
    this.request = request;
  }

  async parseJsonSafely(response) {
    const contentType = response.headers()['content-type'] || '';
    if (!contentType.includes('application/json')) {
      return null;
    }

    try {
      return await response.json();
    } catch {
      return null;
    }
  }

  async readResponse(response) {
    const body = await this.parseJsonSafely(response);

    return {
      response,
      status: response.status(),
      ok: response.ok(),
      body
    };
  }

  createShoppingCart(payload) {
    return this.request.post('/api/shoppingcarts', {
      data: payload
    });
  }

  getShoppingCartByUserId(userId) {
    return this.request.get(`/api/shoppingcarts/user/${userId}`);
  }

  getShoppingCartBySessionId(sessionId) {
    return this.request.get(`/api/shoppingcarts/session/${sessionId}`);
  }

  deleteShoppingCart(cartId) {
    return this.request.delete(`/api/shoppingcarts/${cartId}`);
  }

  createProduct(payload) {
    return this.request.post('/api/products', {
      data: payload
    });
  }

  deleteProduct(productId) {
    return this.request.delete(`/api/products/${productId}`);
  }
}

module.exports = {
  ShoppingCartApiSom
};