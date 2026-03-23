# API Tests (Cucumber + Playwright API client)

Simple automated API tests for `FakeWebShop.Api`.

## What is covered

- `GET /api/products` returns `200`
- `GET /api/products/{id}` returns `404` for unknown id
- Basic create/get/delete flow via `POST` + `GET` + `DELETE`
- `GET /api/categories` returns non-empty `id/name` data
- `GET /api/producttypes` returns non-empty `name/slug` data
- `POST /api/images/upload` without file returns `400`
- Shopping cart create/get/delete smoke flow

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

`npm test` runs Cucumber (`cucumber-js --profile default`).

Feature files:
- `features/products-api.feature`
- `features/backend-api.feature`
- `features/shopping-cart-api.feature`

Step definitions:
- `features/step-definitions/products-api.steps.js`
- `features/step-definitions/backend-api.steps.js`
- `features/step-definitions/shopping-cart-api.steps.js`

## Qase TestOps

Cucumber reporter is enabled via `cucumberjs-qase-reporter`.

All scenarios intended for TestOps sync are tagged with `@qase`.
The `qase` Cucumber profile runs only `@qase` scenarios.

Set these env vars before running tests with Qase upload:

```powershell
$env:QASE_MODE = "testops"
$env:QASE_TESTOPS_API_TOKEN = "<your_qase_api_token>"
$env:QASE_TESTOPS_PROJECT = "<your_project_code>"
$env:QASE_REPORT = "1"
```

Run with uploads:

```powershell
npm run test:cucumber:qase
```

Alias command:

```powershell
npm run test:qase
```

The `test:qase` command validates required env vars first and fails fast if missing:

- `QASE_TESTOPS_API_TOKEN` (or `QASE_API_TOKEN`)
- `QASE_TESTOPS_PROJECT` (or `QASE_PROJECT_CODE`)

Auto-creation behavior:

- Scenarios with `@QaseID=<id>` are linked to existing Qase test cases.
- Scenarios with `@qase` but without `@QaseID=...` are auto-created in Qase from feature/scenario names.

For GitHub Actions, set repository secrets:

- `QASE_TESTOPS_API_TOKEN`
- `QASE_TESTOPS_PROJECT`
