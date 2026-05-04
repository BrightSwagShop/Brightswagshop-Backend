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

function pickList(response, ...keys) {
  if (Array.isArray(response)) {
    return response;
  }
  for (const key of keys) {
    const value = response && response[key];
    if (Array.isArray(value)) {
      return value;
    }
  }
  return [];
}

function pickObject(response, ...keys) {
  for (const key of keys) {
    const value = response && response[key];
    if (value && typeof value === 'object' && !Array.isArray(value)) {
      return value;
    }
  }
  return response;
}

function pickIdentifier(entity) {
  if (!entity) {
    return undefined;
  }
  return entity.identifier
    || entity.id
    || entity.test_case_id
    || entity.testCaseId
    || (entity.urls && entity.urls.self && entity.urls.self.split('/').pop());
}

function toBrowserStackStatus(status) {
  const normalized = String(status || '').toLowerCase();
  if (normalized.includes('pass')) return 'passed';
  if (normalized.includes('skip')) return 'blocked';
  return 'failed';
}

async function getOrCreateFolder(projectId, folderName) {
  const foldersResponse = await makeRequest('GET', `/projects/${projectId}/folders`);
  const folders = pickList(foldersResponse, 'folders', 'data');
  let folder = folders.find((f) => f.name === folderName);

  if (!folder) {
    const createFolderResponse = await makeRequest('POST', `/projects/${projectId}/folders`, {
      folder: { name: folderName }
    });
    folder = pickObject(createFolderResponse, 'folder', 'data');
  }

  const folderId = folder.id || folder.folder_id;
  if (!folderId) {
    throw new Error(`Could not resolve folder id for '${folderName}'`);
  }
  return folderId;
}

async function ensureTestCases(projectId, folderId, testCaseNames) {
  // Fetch all existing test cases in the project with pagination
  const existingCasesResponse = await makeRequest('GET', `/projects/${projectId}/test-cases?per_page=1000`);
  const existingCases = pickList(existingCasesResponse, 'test_cases', 'data');
  const byName = new Map(existingCases.map((tc) => [tc.name, pickIdentifier(tc)]));

  for (const name of testCaseNames) {
    if (byName.has(name)) {
      console.log(`[BrowserStack] Reusing existing test case: ${name}`);
      continue;
    }

    const detail = (ensureTestCases.details && ensureTestCases.details.get && ensureTestCases.details.get(name)) || null;
    const payload = { name };
    if (detail && detail.description) payload.description = detail.description;
    if (detail && detail.steps) payload.steps = detail.steps;

    console.log(`[BrowserStack] Creating new test case: ${name}`);
    const createdResponse = await makeRequest('POST', `/projects/${projectId}/folders/${folderId}/test-cases`, {
      test_case: payload
    });

    const created = pickObject(createdResponse, 'test_case', 'data');
    const createdCase = created.test_case || created;
    const caseId = pickIdentifier(createdCase);
    if (!caseId) {
      throw new Error(`Created test case '${name}' but no identifier was returned`);
    }
    byName.set(name, caseId);
  }

  return byName;
}

async function addTestCasesToTestRun(projectId, testRunId, testCaseIds) {
  // Add test cases to the test run so they appear in the UI
  for (const testCaseId of testCaseIds) {
    try {
      await makeRequest('POST', `/projects/${projectId}/test-runs/${testRunId}/test-cases`, {
        test_case_id: testCaseId
      });
    } catch (e) {
      console.warn(`[BrowserStack] Warning: Could not add test case ${testCaseId} to test run:`, e.message);
    }
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
    const projectsList = pickList(projectsResponse, 'projects', 'data');
    
    let project = projectsList.find(p => p.name === PROJECT_NAME);

    if (!project) {
      console.log('[BrowserStack] Creating project...');
      const createResponse = await makeRequest('POST', '/projects', { project: { name: PROJECT_NAME } });
      project = pickObject(createResponse, 'project', 'data');
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

    const testRun = testRunResponse.test_run || testRunResponse.testRun || testRunResponse.data || testRunResponse;
    const testRunId = testRun.identifier
      || testRun.id
      || testRun.test_run_id
      || testRun.run_id
      || (testRun.urls && testRun.urls.self && testRun.urls.self.split('/').pop());

    if (!testRunId) {
      throw new Error(`Could not resolve test run identifier from response: ${JSON.stringify(testRunResponse)}`);
    }

    console.log(`[BrowserStack] Test run created: ${testRunId}`);

    // Parse Cucumber JSON report
    const rawJson = fs.readFileSync(REPORT_FILE, 'utf-8');
    const suites = JSON.parse(rawJson);

    const testCases = [];
    const testCaseDetails = new Map();

    for (const suite of suites) {
      if (suite.elements) {
        for (const element of suite.elements) {
          const name = `${suite.name} - ${element.name}`;
          const steps = element.steps || [];
          
          // Build step details and overall status/duration
          let status = 'PASSED';
          let duration = 0;
          const stepDetails = [];

          for (const step of steps) {
            if (step.result) {
              duration += step.result.duration || 0;
              const sStatus = (step.result.status || '').toLowerCase();
              const mapped = sStatus === 'passed' ? 'passed' : (sStatus === 'skipped' ? 'skipped' : 'failed');
              stepDetails.push({ name: step.keyword ? `${step.keyword} ${step.name}` : step.name, action: step.name, status: mapped });
              if (['failed', 'skipped', 'undefined', 'pending'].includes(sStatus)) {
                status = sStatus === 'skipped' ? 'SKIPPED' : 'FAILED';
              }
            } else {
              stepDetails.push({ name: step.name, action: step.name, status: 'passed' });
            }
          }

          testCaseDetails.set(name, { description: '', steps: stepDetails });

          testCases.push({
            name,
            status: status.toUpperCase(),
            duration: Math.round(duration / 1e6), // Convert nanoseconds to milliseconds
          });
        }
      }
    }

    console.log(`[BrowserStack] Found ${testCases.length} test cases`);

    const folderId = await getOrCreateFolder(projectId, 'API Tests');
    ensureTestCases.details = testCaseDetails;
    const testCaseIdByName = await ensureTestCases(
      projectId,
      folderId,
      [...new Set(testCases.map((tc) => tc.name))]
    );

    // Add all test cases to the test run before uploading results
    const uniqueTestCaseIds = [...new Set(testCaseIdByName.values())];
    console.log(`[BrowserStack] Adding ${uniqueTestCaseIds.length} test cases to test run ${testRunId}`);
    await addTestCasesToTestRun(projectId, testRunId, uniqueTestCaseIds);

    // Upload results for each test case
    for (const testCase of testCases) {
      try {
        const testCaseId = testCaseIdByName.get(testCase.name);
        if (!testCaseId) {
          console.warn(`[BrowserStack] Warning: Missing test case id for ${testCase.name}`);
          continue;
        }

        console.log(`[BrowserStack] Uploading result for: ${testCase.name}`);
        const detail = testCaseDetails.get(testCase.name) || null;
        const resultPayload = {
          test_case_id: testCaseId,
          status: toBrowserStackStatus(testCase.status),
          duration: testCase.duration,
        };
        if (detail && detail.steps && detail.steps.length) {
          resultPayload.steps = detail.steps.map((s, idx) => ({ index: idx + 1, name: s.name, action: s.action, status: s.status }));
        }

        await makeRequest('POST', `/projects/${projectId}/test-runs/${testRunId}/results`, {
          result: resultPayload
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
