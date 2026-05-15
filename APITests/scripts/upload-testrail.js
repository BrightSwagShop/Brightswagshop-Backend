#!/usr/bin/env node

const fs = require('fs');

const BASE_URL = (process.env.TESTRAIL_BASE_URL || '').replace(/\/+$/, '');
const USERNAME = process.env.TESTRAIL_USERNAME;
const API_KEY = process.env.TESTRAIL_API_KEY;
const PROJECT_NAME = process.env.TESTRAIL_PROJECT_NAME || 'BrightSwagShop';
const BUILD_NAME = process.env.TESTRAIL_BUILD_NAME || `Build #${process.env.GITHUB_RUN_NUMBER || 'local'}`;
const REPORT_FILE = process.argv[2] || 'test-results/junit.xml';

if (!BASE_URL || !USERNAME || !API_KEY) {
  console.error('[TestRail] Missing TESTRAIL_BASE_URL, TESTRAIL_USERNAME, or TESTRAIL_API_KEY');
  process.exit(1);
}

const API_URL = `${BASE_URL}/index.php?/api/v2`;
const auth = Buffer.from(`${USERNAME}:${API_KEY}`).toString('base64');
const headers = {
  Authorization: `Basic ${auth}`,
  'Content-Type': 'application/json'
};

async function makeRequest(method, endpoint, body = null) {
  const url = `${API_URL}${endpoint}`;
  const options = { method, headers };
  if (body) {
    options.body = JSON.stringify(body);
  }

  console.log(`[TestRail] ${method} ${endpoint}`);
  const response = await fetch(url, options);
  const responseText = await response.text();

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${responseText}`);
  }

  if (!responseText) {
    return {};
  }

  return JSON.parse(responseText);
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
  if (!entity) return undefined;
  return entity.id || entity.identifier || entity.case_id || entity.test_case_id;
}

function normalizeName(name) {
  return String(name || '').trim().replace(/::/g, '.').replace(/\s+/g, ' ').toLowerCase();
}

function toElapsed(ms) {
  const totalSeconds = Math.max(0, Math.round(ms / 1000));
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${minutes}m ${seconds}s`;
}

function toTestRailStatus(status) {
  const normalized = String(status || '').toLowerCase();
  if (normalized.includes('pass')) return 1;
  if (normalized.includes('skip')) return 2;
  return 5;
}

async function getProject(projectName) {
  const projectsResponse = await makeRequest('GET', '/get_projects');
  const projects = pickList(projectsResponse, 'projects', 'data');
  const project = projects.find((p) => p.name === projectName);
  if (!project) {
    throw new Error(`Could not find TestRail project '${projectName}'`);
  }
  return project;
}

async function getSuite(projectId) {
  const suitesResponse = await makeRequest('GET', `/get_suites/${projectId}`);
  const suites = pickList(suitesResponse, 'suites', 'data');

  if (!suites.length) {
    return null;
  }

  const preferredName = process.env.TESTRAIL_SUITE_NAME;
  if (preferredName) {
    const preferred = suites.find((suite) => suite.name === preferredName);
    if (preferred) {
      return preferred;
    }
  }

  return suites[0];
}

async function getOrCreateSection(projectId, suiteId, sectionName) {
  const sectionsEndpoint = suiteId
    ? `/get_sections/${projectId}&suite_id=${suiteId}`
    : `/get_sections/${projectId}`;
  const sectionsResponse = await makeRequest('GET', sectionsEndpoint);
  const sections = pickList(sectionsResponse, 'sections', 'data');
  let section = sections.find((item) => item.name === sectionName);

  if (!section) {
    const createBody = { name: sectionName };
    if (suiteId) {
      createBody.suite_id = suiteId;
    }

    const createdResponse = await makeRequest('POST', `/add_section/${projectId}`, createBody);
    section = pickObject(createdResponse, 'section', 'data');
  }

  const sectionId = pickIdentifier(section);
  if (!sectionId) {
    throw new Error(`Could not resolve section id for '${sectionName}'`);
  }

  return sectionId;
}

async function ensureTestCases(projectId, suiteId, sectionByName, testCaseNames) {
  const casesEndpoint = suiteId
    ? `/get_cases/${projectId}&suite_id=${suiteId}`
    : `/get_cases/${projectId}`;
  const existingCasesResponse = await makeRequest('GET', casesEndpoint);
  const existingCases = pickList(existingCasesResponse, 'cases', 'data');
  const byName = new Map();

  for (const testCase of existingCases) {
    const id = pickIdentifier(testCase);
    if (!id) continue;
    byName.set(testCase.title || testCase.name, id);
    byName.set(normalizeName(testCase.title || testCase.name), id);
  }

  for (const name of testCaseNames) {
    const normalized = normalizeName(name);
    if (byName.has(name) || byName.has(normalized)) {
      const usedId = byName.get(name) || byName.get(normalized);
      console.log(`[TestRail] Reusing existing test case: ${name} -> ${usedId}`);
      byName.set(name, usedId);
      byName.set(normalized, usedId);
      continue;
    }

    const sectionName = name.split('::')[0] || 'APITests';
    const sectionId = sectionByName.get(sectionName) || sectionByName.get('APITests');
    if (!sectionId) {
      throw new Error(`Could not resolve section for test case '${name}'`);
    }

    console.log(`[TestRail] Creating new test case: ${name}`);
    const createdResponse = await makeRequest('POST', `/add_case/${sectionId}`, {
      title: name
    });

    const createdCase = pickObject(createdResponse, 'case', 'data');
    const caseId = pickIdentifier(createdCase);
    if (!caseId) {
      throw new Error(`Created test case '${name}' but no identifier was returned`);
    }

    byName.set(name, caseId);
    byName.set(normalized, caseId);
  }

  return byName;
}

async function createTestRun(projectId, suiteId, caseIds) {
  const payload = {
    name: BUILD_NAME,
    description: `Automated API run for ${BUILD_NAME}`,
    include_all: false,
    case_ids: caseIds
  };

  if (suiteId) {
    payload.suite_id = suiteId;
  }

  const response = await makeRequest('POST', `/add_run/${projectId}`, payload);
  const testRun = pickObject(response, 'run', 'test_run', 'data');
  const runId = pickIdentifier(testRun);

  if (!runId) {
    throw new Error(`Could not resolve test run identifier from response: ${JSON.stringify(response)}`);
  }

  return runId;
}

function parseJUnitReport(xml) {
  const testCases = [];
  const testCaseDetails = new Map();
  const testCaseRegex = /<testcase[^>]+>/g;
  let match;

  while ((match = testCaseRegex.exec(xml)) !== null) {
    const tag = match[0];
    const nameMatch = tag.match(/name="([^"]*)"/);
    const classNameMatch = tag.match(/classname="([^"]*)"/);
    const timeMatch = tag.match(/time="([^"]*)"/);

    if (!nameMatch || !classNameMatch) {
      continue;
    }

    const name = nameMatch[1];
    const className = classNameMatch[1];
    const time = parseFloat(timeMatch ? timeMatch[1] : 0) || 0;
    const fullName = `${className}::${name}`;

    const startIdx = xml.indexOf(tag);
    const closingIdx = xml.indexOf('</testcase>', startIdx);
    if (closingIdx === -1) {
      continue;
    }

    const testcaseContent = xml.substring(startIdx + tag.length, closingIdx);
    const hasFailure = testcaseContent.includes('<failure');
    const failureMatch = testcaseContent.match(/<failure[^>]*>([\s\S]*?)<\/failure>/);
    const systemOutMatch = testcaseContent.match(/<system-out[^>]*>([\s\S]*?)<\/system-out>/);

    const comment = failureMatch
      ? failureMatch[1].trim()
      : systemOutMatch
        ? systemOutMatch[1].trim()
        : '';

    testCaseDetails.set(fullName, { comment });
    testCases.push({
      fullName,
      className,
      statusId: toTestRailStatus(hasFailure ? 'failed' : 'passed'),
      elapsed: toElapsed(time * 1000),
      comment
    });
  }

  return { testCases, testCaseDetails };
}

async function uploadResults() {
  try {
    if (!fs.existsSync(REPORT_FILE)) {
      throw new Error(`Report file not found: ${REPORT_FILE}`);
    }

    console.log(`[TestRail] Getting project: ${PROJECT_NAME}`);
    const project = await getProject(PROJECT_NAME);
    const projectId = project.id;

    const suite = await getSuite(projectId);
    const suiteId = suite?.id ?? null;

    const xml = fs.readFileSync(REPORT_FILE, 'utf-8');
    const { testCases } = parseJUnitReport(xml);
    console.log(`[TestRail] Found ${testCases.length} test cases`);

    const sectionByName = new Map();
    for (const sectionName of [...new Set(testCases.map((item) => item.className))]) {
      const sectionId = await getOrCreateSection(projectId, suiteId, sectionName);
      sectionByName.set(sectionName, sectionId);
    }

    if (!sectionByName.has('APITests')) {
      const fallbackSectionId = await getOrCreateSection(projectId, suiteId, 'APITests');
      sectionByName.set('APITests', fallbackSectionId);
    }

    const caseIdByName = await ensureTestCases(
      projectId,
      suiteId,
      sectionByName,
      [...new Set(testCases.map((item) => item.fullName))]
    );

    const runCaseIds = [...new Set([...caseIdByName.values()])];
    console.log(`[TestRail] Creating run with ${runCaseIds.length} cases`);
    const runId = await createTestRun(projectId, suiteId, runCaseIds);
    console.log(`[TestRail] Test run created: ${runId}`);

    const results = testCases
      .map((item) => {
        const caseId = caseIdByName.get(item.fullName);
        if (!caseId) {
          console.warn(`[TestRail] Missing case id for ${item.fullName}`);
          return null;
        }

        return {
          case_id: caseId,
          status_id: item.statusId,
          elapsed: item.elapsed,
          comment: item.comment || `${item.fullName}`
        };
      })
      .filter(Boolean);

    if (results.length) {
      console.log(`[TestRail] Uploading ${results.length} results`);
      await makeRequest('POST', `/add_results_for_cases/${runId}`, { results });
    }

    console.log(`[TestRail] Upload complete. Test run: ${runId}`);
  } catch (error) {
    console.error('[TestRail] Upload failed:', error.message);
    process.exit(1);
  }
}

uploadResults();