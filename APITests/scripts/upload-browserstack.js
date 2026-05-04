#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

const USERNAME = process.env.BROWSERSTACK_USERNAME;
const ACCESS_KEY = process.env.BROWSERSTACK_ACCESS_KEY;
const PROJECT_NAME = process.env.BROWSERSTACK_PROJECT_NAME || 'BrightSwagShop - JIRA';
const BUILD_NAME = process.env.BROWSERSTACK_BUILD_NAME || `Build #${process.env.GITHUB_RUN_NUMBER || 'local'}`;
const REPORT_FILE = process.argv[2] || 'test-results/cucumber.json';
const API_URL = 'https://test-management.browserstack.com/api/v2';

if (!USERNAME || !ACCESS_KEY) {
  console.error('[BrowserStack] Missing BROWSERSTACK_USERNAME or BROWSERSTACK_ACCESS_KEY');
  process.exit(1);
}

const auth = Buffer.from(`${USERNAME}:${ACCESS_KEY}`).toString('base64');
const headers = {
  'Authorization': `Basic ${auth}`,
  'Content-Type': 'application/json',
};

async function makeRequest(method, endpoint, body = null) {
  const url = `${API_URL}${endpoint}`;
  const options = { method, headers };
  if (body) options.body = JSON.stringify(body);

  try {
    console.log(`[BrowserStack] ${method} ${endpoint}`);
    const response = await fetch(url, options);
    const responseText = await response.text();
    
    if (!response.ok) {
      console.error(`[BrowserStack] HTTP ${response.status} - ${responseText}`);
      throw new Error(`HTTP ${response.status}: ${responseText}`);
    }
    
    const jsonResponse = JSON.parse(responseText);
    console.log(`[BrowserStack] Response:`, JSON.stringify(jsonResponse).substring(0, 200));
    return jsonResponse;
  } catch (error) {
    console.error(`[BrowserStack] Request failed:`, error.message);
    throw error;
  }
}

async function uploadResults() {
  try {
    if (!fs.existsSync(REPORT_FILE)) {
      console.error(`[BrowserStack] Report file not found: ${REPORT_FILE}`);
      process.exit(1);
    }

    // Get or create project
    console.log(`[BrowserStack] Getting or creating project: ${PROJECT_NAME}`);
    const projectsResponse = await makeRequest('GET', '/projects');
    
    // Handle different response formats (array or object with projects property)
    const projectsList = Array.isArray(projectsResponse) ? projectsResponse : 
                        (projectsResponse.projects || projectsResponse.data || []);
    
    let project = projectsList.find(p => p.name === PROJECT_NAME);

    if (!project) {
      console.log('[BrowserStack] Creating project...');
      const createResponse = await makeRequest('POST', '/projects', { name: PROJECT_NAME });
      project = createResponse.project || createResponse.data || createResponse;
    }

    const projectId = project.id || project.identifier;
    console.log(`[BrowserStack] Using project: ${projectId}`);

    // Create test run
    console.log(`[BrowserStack] Creating test run: ${BUILD_NAME}`);
    const testRunResponse = await makeRequest('POST', `/projects/${projectId}/test-runs`, {
      test_run: {
        name: BUILD_NAME,
        description: `Automated API test run for ${BUILD_NAME}`,
      }
    });

    const testRun = testRunResponse.testRun || testRunResponse.data || testRunResponse;
    const testRunId = testRun.id || testRun.identifier;
    console.log(`[BrowserStack] Test run created: ${testRunId}`);

    // Parse Cucumber JSON report
    const rawJson = fs.readFileSync(REPORT_FILE, 'utf-8');
    const suites = JSON.parse(rawJson);

    const testCases = [];

    for (const suite of suites) {
      if (suite.elements) {
        for (const element of suite.elements) {
          const name = `${suite.name} - ${element.name}`;
          const steps = element.steps || [];
          
          // Status is determined by step results
          let status = 'PASSED';
          let duration = 0;

          for (const step of steps) {
            if (step.result) {
              duration += step.result.duration || 0;
              if (['failed', 'skipped', 'undefined', 'pending'].includes(step.result.status)) {
                status = step.result.status === 'skipped' ? 'SKIPPED' : 'FAILED';
                break;
              }
            }
          }

          testCases.push({
            name,
            status: status.toUpperCase(),
            duration: Math.round(duration / 1e6), // Convert nanoseconds to milliseconds
          });
        }
      }
    }

    console.log(`[BrowserStack] Found ${testCases.length} test cases`);

    // Upload results for each test case
    for (const testCase of testCases) {
      try {
        console.log(`[BrowserStack] Uploading result for: ${testCase.name}`);
        await makeRequest('POST', `/projects/${projectId}/test-runs/${testRunId}/results`, {
          result: {
            name: testCase.name,
            status: testCase.status,
            duration: testCase.duration,
          }
        });
      } catch (e) {
        console.warn(`[BrowserStack] Warning: Could not upload result for ${testCase.name}:`, e.message);
      }
    }

    console.log(`[BrowserStack] Upload complete. Test run: ${testRunId}`);
  } catch (error) {
    console.error('[BrowserStack] Upload failed:', error.message);
    process.exit(1);
  }
}

uploadResults();
