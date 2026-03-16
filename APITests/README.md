# API Tests (Playwright + Cucumber)

Simple automated API tests for `FakeWebShop.Api`.

## What is covered

- `GET /api/products` returns `200`
- `GET /api/products/{id}` returns `404` for unknown id
- Basic create/get/delete flow via `POST` + `GET` + `DELETE`

## Run

From this folder:

```bash
npm install
npm test
```

By default tests use `http://localhost:5076`.

Optional custom base URL (PowerShell):

```powershell
$env:API_BASE_URL = "http://localhost:5076"; npm test
```

## Cucumber

Run Cucumber API tests:

```powershell
npm run test:cucumber
```

Feature file: `features/products-api.feature`
Step definitions: `features/step-definitions/products-api.steps.js`

## Qase TestOps

Playwright reporter is enabled via `playwright-qase-reporter`.
Cucumber reporter is enabled via `cucumberjs-qase-reporter`.

Set these env vars before running tests with Qase upload:

```powershell
$env:QASE_MODE = "testops"
$env:QASE_TESTOPS_API_TOKEN = "<your_qase_api_token>"
$env:QASE_TESTOPS_PROJECT = "<your_project_code>"
$env:QASE_REPORT = "1"
```

Run with uploads:

```powershell
npm test
npm run test:cucumber:qase
```

For GitHub Actions, set repository secrets:

- `QASE_TESTOPS_API_TOKEN`
- `QASE_TESTOPS_PROJECT`
