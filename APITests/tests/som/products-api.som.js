class ProductsApiSom {
  constructor(request) {
    this.request = request;
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
