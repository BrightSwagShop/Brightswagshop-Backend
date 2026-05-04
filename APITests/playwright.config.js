// @ts-check
const { defineConfig } = require('@playwright/test');


module.exports = defineConfig({
  testDir: './tests',
  fullyParallel: false,
  retries: 0,
  reporter: [
    ['html'],
    ['list'],
  ],
  use: {
    baseURL: process.env.API_BASE_URL || 'http://localhost:5076',
    extraHTTPHeaders: {
      Accept: 'application/json'
    }
  }
});
