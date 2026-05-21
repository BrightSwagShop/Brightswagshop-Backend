const { After, AfterAll, BeforeAll } = require('@cucumber/cucumber');

const results = [];
const startTime = Date.now();

BeforeAll(() => {
  if (process.env.BROWSERSTACK_USERNAME && process.env.BROWSERSTACK_ACCESS_KEY) {
    console.log('[BrowserStack] BrowserStack reporting enabled for APITests');
  } else {
    console.log('[BrowserStack] BrowserStack environment not configured; reporting will be skipped');
  }
});

After(function (scenario) {
  try {
    const name = scenario.pickle?.name || (scenario.gherkinDocument && scenario.gherkinDocument.feature && scenario.gherkinDocument.feature.name) || 'unnamed';
    const status = (scenario.result && scenario.result.status) || (scenario.result && scenario.result.statusName) || 'UNKNOWN';
    results.push({ name, status });
  } catch (e) {
    // best-effort; don't fail test run for reporting
  }
});

AfterAll(async () => {
  const total = results.length;
  const passed = results.filter(r => String(r.status).toLowerCase().includes('passed')).length;
  const failed = total - passed;
  const duration = Math.round((Date.now() - startTime) / 1000);

  const payload = {
    project: process.env.BROWSERSTACK_PROJECT_NAME || 'BrightSwagShop - JIRA',
    build: process.env.BROWSERSTACK_BUILD_NAME || `Build #${process.env.GITHUB_RUN_NUMBER || 'local'}`,
    total,
    passed,
    failed,
    duration
  };

  console.log('[BrowserStack] APITests summary:', JSON.stringify(payload, null, 2));

  // Optionally send to BrowserStack Test Management API here if desired.
});
