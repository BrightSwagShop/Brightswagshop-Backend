// @ts-check
const { defineConfig } = require('@playwright/test');

if (!process.env.QASE_REPORT) {
  process.env.QASE_REPORT = '1';
}

const qaseApiToken = process.env.QASE_TESTOPS_API_TOKEN || process.env.QASE_API_TOKEN;
const qaseProjectCode = process.env.QASE_TESTOPS_PROJECT || process.env.QASE_PROJECT_CODE;

module.exports = defineConfig({
  testDir: './tests',
  fullyParallel: false,
  retries: 0,
  reporter: [
    ['html'],
    ['list'],
    [
      'playwright-qase-reporter',
      {
        apiToken: qaseApiToken,
        projectCode: qaseProjectCode,
        logging: false,
        runComplete: true,
        uploadAttachments: true,
        basePath: 'https://api.qase.io/v1'
      },
    ],
  ],
  use: {
    baseURL: process.env.API_BASE_URL || 'http://localhost:5076',
    extraHTTPHeaders: {
      Accept: 'application/json'
    }
  }
});
