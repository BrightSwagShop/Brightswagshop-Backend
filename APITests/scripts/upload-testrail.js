#!/usr/bin/env node

const { spawnSync } = require('child_process');
const fs = require('fs');

const BASE_URL = process.env.TESTRAIL_BASE_URL;
const USERNAME = process.env.TESTRAIL_USERNAME;
const API_KEY = process.env.TESTRAIL_API_KEY;
const PROJECT_NAME = process.env.TESTRAIL_PROJECT_NAME || 'BrightSwagShop';
const SUITE_NAME = process.env.TESTRAIL_SUITE_NAME || 'API Tests';
const RUN_TITLE = process.env.TESTRAIL_RUN_TITLE || `API Tests - Build #${process.env.GITHUB_RUN_NUMBER || 'local'}`;
const RUN_DESCRIPTION = process.env.TESTRAIL_RUN_DESCRIPTION
  || `Triggered by: ${process.env.GITHUB_ACTOR || 'local'} | Run: ${process.env.GITHUB_SERVER_URL || 'local'}/${process.env.GITHUB_REPOSITORY || 'local'}/actions/runs/${process.env.GITHUB_RUN_ID || 'local'}`;
const REPORT_FILE = process.argv[2] || 'test-results/cucumber.json';

function fail(message) {
  console.error(`[TestRail] ${message}`);
  process.exit(1);
}

if (!BASE_URL || !USERNAME || !API_KEY) {
  fail('Missing TESTRAIL_BASE_URL, TESTRAIL_USERNAME, or TESTRAIL_API_KEY.');
}

if (!fs.existsSync(REPORT_FILE)) {
  console.error(`[TestRail] Report file not found: ${REPORT_FILE}`);
  console.error('[TestRail] Current working directory:', process.cwd());
  try {
    const files = fs.readdirSync(process.cwd());
    console.error('[TestRail] Files in CWD:', files.join(', '));
  } catch (e) {
    console.error('[TestRail] Failed to list cwd files:', e.message);
  }
  // Try to show a recursive listing for easier debugging
  try {
    const walk = (dir, depth = 0) => {
      if (depth > 4) return [];
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      let out = [];
      for (const ent of entries) {
        const p = `${dir}/${ent.name}`;
        out.push(p);
        if (ent.isDirectory()) out = out.concat(walk(p, depth + 1));
      }
      return out;
    };
    const listing = walk(process.cwd()).slice(0, 200);
    console.error('[TestRail] Recursive listing (truncated):');
    listing.forEach(l => console.error(' -', l));
  } catch (e) {
    console.error('[TestRail] Failed to produce recursive listing:', e.message);
  }
  fail(`Report file not found: ${REPORT_FILE}`);
}

function baseUrlWithApiPath(url) {
  return `${String(url).replace(/\/$/, '')}/index.php?/api/v2`;
}

async function apiGet(endpoint) {
  const auth = Buffer.from(`${USERNAME}:${API_KEY}`).toString('base64');
  const response = await fetch(`${baseUrlWithApiPath(BASE_URL)}${endpoint}`, {
    headers: {
      Authorization: `Basic ${auth}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    throw new Error(`TestRail API GET ${endpoint} failed with HTTP ${response.status}`);
  }

  return response.json();
}

function resolveIdentifier(entity) {
  return entity?.id ?? entity?.project_id ?? entity?.suite_id ?? entity?.run_id ?? entity?.identifier;
}

async function resolveProjectAndSuiteIds() {
  const projects = await apiGet('/get_projects');
  const projectList = Array.isArray(projects) ? projects : projects.projects || projects.data || [];
  const project = projectList.find((entry) => String(entry.name).trim() === PROJECT_NAME.trim());

  if (!project) {
    throw new Error(`Could not find TestRail project named '${PROJECT_NAME}'`);
  }

  const projectId = resolveIdentifier(project);
  if (!projectId) {
    throw new Error(`Could not resolve project id for '${PROJECT_NAME}'`);
  }

  const suites = await apiGet(`/get_suites/${projectId}`);
  const suiteList = Array.isArray(suites) ? suites : suites.suites || suites.data || [];

  let suite = suiteList.find((entry) => String(entry.name).trim() === SUITE_NAME.trim());

  if (!suite && suiteList.length === 1) {
    suite = suiteList[0];
  }

  if (!suite) {
    const availableSuites = suiteList.map((entry) => entry.name).join(', ');
    throw new Error(
      `Could not find TestRail suite named '${SUITE_NAME}' for project '${PROJECT_NAME}'. Available suites: ${availableSuites || 'none'}`
    );
  }

  const suiteId = resolveIdentifier(suite);
  if (!suiteId) {
    throw new Error(`Could not resolve suite id for '${SUITE_NAME}'`);
  }

  return { projectId, suiteId };
}

async function main() {
  const { projectId, suiteId: resolvedSuiteId } = await resolveProjectAndSuiteIds();
  const suiteId = process.env.TESTRAIL_SUITE_ID || resolvedSuiteId;

  const args = [
    '-y',
    '-h', BASE_URL,
    '--project-id', String(projectId),
    '--suite-id', String(suiteId),
    '-u', USERNAME,
    '-k', API_KEY,
    'parse_cucumber',
    '-f', REPORT_FILE,
    '--title', RUN_TITLE,
    '--run-description', RUN_DESCRIPTION,
  ];

  if (process.env.TESTRAIL_CLOSE_RUN === 'true') {
    args.push('--close-run');
  }

  console.log(`[TestRail] Resolved project id ${projectId} and suite id ${suiteId} for '${PROJECT_NAME}' / '${SUITE_NAME}'`);

  const result = spawnSync('trcli', args, {
    stdio: 'inherit',
    shell: process.platform === 'win32'
  });

  if (result.error) {
    console.error('[TestRail] trcli execution failed to start. Attempting to locate trcli...');
    try {
      const probe = spawnSync(process.platform === 'win32' ? 'where' : 'which', ['trcli'], { shell: process.platform === 'win32' });
      if (probe.error) {
        console.error('[TestRail] Could not run probe command for trcli:', probe.error.message);
      } else {
        console.error('[TestRail] trcli probe stdout:', probe.stdout && probe.stdout.toString().trim());
        console.error('[TestRail] trcli probe stderr:', probe.stderr && probe.stderr.toString().trim());
      }
    } catch (e) {
      console.error('[TestRail] Error while probing for trcli:', e.message);
    }
    fail(`Failed to start trcli: ${result.error.message}`);
  }

  if (result.status !== 0) {
    process.exit(result.status || 1);
  }
}

main().catch((error) => {
  fail(error?.message || String(error));
});