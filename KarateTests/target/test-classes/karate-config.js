function fn() {
  var baseUrl = karate.properties['baseUrl'] ||
    java.lang.System.getenv('API_BASE_URL') ||
    'http://127.0.0.1:5076';

  return {
    baseUrl: baseUrl,
    adminHeaders: {
      'X-User-Role': 'Admin',
      'X-User-Id': 'test-admin-user'
    },
    userHeaders: {
      'X-User-Role': 'User',
      'X-User-Id': 'test-user-user'
    }
  };
}
