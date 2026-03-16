# API Tests (Playwright)

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

## Qase TestOps

Reporter is enabled in Playwright config using `playwright-qase-reporter`.
This reporter requires `QASE_REPORT=1`.

Set these env vars before running tests:

```powershell
$env:QASE_TESTOPS_API_TOKEN = "<your_qase_api_token>"
$env:QASE_TESTOPS_PROJECT = "<your_project_code>"
$env:QASE_REPORT = "1"
```

For GitHub Actions, set repository secrets:

- `QASE_TESTOPS_API_TOKEN`
- `QASE_TESTOPS_PROJECT`
