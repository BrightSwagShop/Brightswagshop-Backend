#!/usr/bin/env node

const { spawnSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const CWD = process.cwd();
const REPORT_FILE = path.join(CWD, 'test-results', 'cucumber.json');
const ALLURE_REPORT_DIR = path.join(CWD, 'allure-report');
const HAS_TESTRAIL_CREDS = Boolean(process.env.TESTRAIL_BASE_URL && process.env.TESTRAIL_USERNAME && process.env.TESTRAIL_API_KEY);

function run(cmd, args, opts = {}) {
  console.log('> ' + [cmd].concat(args || []).join(' '));
  const res = spawnSync(cmd, args || [], { stdio: 'inherit', shell: process.platform === 'win32', ...opts });
  return res.status === null ? (res.error ? 1 : 0) : res.status;
}

function ensureDir(dir) {
  if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
}

async function main() {
  try {
    ensureDir(path.join(CWD, 'test-results'));

    // Clean previous Allure data
    run('npm', ['run', 'allure:clean']);

    // Run cucumber with testrail profile
    const cucumberStatus = run('npx', ['cucumber-js', '--profile', 'testrail']);

    // Always generate Allure report so it's available even if tests fail
    const genStatus = run('npm', ['run', 'report:allure:generate']);
    if (genStatus !== 0) {
      console.error('[runner] Allure generation returned', genStatus);
    }

    // Upload to TestRail only when credentials are available.
    if (HAS_TESTRAIL_CREDS) {
      if (fs.existsSync(REPORT_FILE)) {
        console.log('[runner] Found Cucumber JSON report:', REPORT_FILE);
        const uploadStatus = run('npm', ['run', 'upload:testrail']);
        if (uploadStatus !== 0) {
          console.error('[runner] upload:testrail failed with code', uploadStatus);
        }
      } else {
        console.warn('[runner] Cucumber JSON report not found; skipping TestRail upload');
      }
    } else {
      console.log('[runner] TestRail credentials are not configured; skipping upload step');
    }

    if (fs.existsSync(ALLURE_REPORT_DIR)) {
      console.log('[runner] Allure report available at:', ALLURE_REPORT_DIR);
    }

    // Exit with cucumber's status so CI fails when tests fail
    process.exit(cucumberStatus || 0);
  } catch (err) {
    console.error('[runner] Unexpected error:', err && err.stack || err);
    process.exit(1);
  }
}

main();
