#!/usr/bin/env node

const { spawnSync } = require('child_process');
const fs = require('fs');

const BASE_URL = process.env.TESTRAIL_BASE_URL;
const USERNAME = process.env.TESTRAIL_USERNAME;
const API_KEY = process.env.TESTRAIL_API_KEY;
const PROJECT_NAME = process.env.TESTRAIL_PROJECT_NAME || 'BrightSwagShop';
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
  fail(`Report file not found: ${REPORT_FILE}`);
}

const args = [
  '-y',
  '-h', BASE_URL,
  '--project', PROJECT_NAME,
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

if (process.env.TESTRAIL_SUITE_ID) {
  args.push('--suite-id', process.env.TESTRAIL_SUITE_ID);
}

const result = spawnSync('trcli', args, {
  stdio: 'inherit',
  shell: process.platform === 'win32'
});

if (result.error) {
  fail(`Failed to start trcli: ${result.error.message}`);
}

if (result.status !== 0) {
  process.exit(result.status || 1);
}