#!/usr/bin/env node

const { spawnSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const CWD = process.cwd();
const REPORT_FILE = path.join(CWD, 'test-results', 'cucumber.json');

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

    // If cucumber produced the report, try uploading to TestRail
    if (fs.existsSync(REPORT_FILE)) {
      console.log('[runner] Found Cucumber JSON report:', REPORT_FILE);
      const uploadStatus = run('npm', ['run', 'upload:testrail']);
      if (uploadStatus !== 0) {
        console.error('[runner] upload:testrail failed with code', uploadStatus);
      }
    } else {
      console.warn('[runner] Cucumber JSON report not found; skipping TestRail upload');
    }

    // Always generate Allure report so it's available even if tests fail
    try {
      const genStatus = run('npm', ['run', 'report:allure:generate']);
      if (genStatus !== 0) console.error('[runner] Allure generation returned', genStatus);
    } catch (e) {
      console.error('[runner] Failed to generate Allure report:', e && e.message);
    }

    // Exit with cucumber's status so CI fails when tests fail
    process.exit(cucumberStatus || 0);
  } catch (err) {
    console.error('[runner] Unexpected error:', err && err.stack || err);
    process.exit(1);
  }
}

main();
