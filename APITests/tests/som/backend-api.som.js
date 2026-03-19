class BackendApiSom {
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

  getCategories() {
    return this.request.get('/api/categories');
  }

  getProductTypes() {
    return this.request.get('/api/producttypes');
  }

  uploadImageWithoutFile() {
    return this.request.post('/api/images/upload');
  }
}

module.exports = {
  BackendApiSom
};