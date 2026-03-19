class ProductsApiSom {
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

  getAllProducts() {
    return this.request.get('/api/products');
  }

  getProductById(productId) {
    return this.request.get(`/api/products/${productId}`);
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
  ProductsApiSom
};
