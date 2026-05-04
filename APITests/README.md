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

## Allure reporting

All Cucumber runs also write Allure results to `allure-results/`.

Generate an HTML report:

```powershell
npm run report:allure:generate
```

Open the generated report locally:

```powershell
npm run report:allure:open
```

Feature files:
- `features/products-api.feature`
- `features/backend-api.feature`
- `features/shopping-cart-api.feature`

Step definitions:
- `features/step-definitions/products-api.steps.js`
- `features/step-definitions/backend-api.steps.js`
- `features/step-definitions/shopping-cart-api.steps.js`

## BrowserStack reporting

BrowserStack reporting is enabled through the `browserstack` Cucumber profile.

Set these env vars before running tests with BrowserStack reporting.

- For CI: configure `BROWSERSTACK_USERNAME` and `BROWSERSTACK_ACCESS_KEY` as GitHub repository secrets (do not commit secrets to source).
- For local runs only: set them in your local environment and do not check them into version control. Example (PowerShell):

```powershell
$env:BROWSERSTACK_USERNAME = "your_username"; $env:BROWSERSTACK_ACCESS_KEY = "your_access_key"; npm run test:browserstack
```

Run with BrowserStack reporting:

```powershell
npm run test:browserstack
```

The `test:browserstack` command validates required env vars first and fails fast if missing:

- `BROWSERSTACK_USERNAME`
- `BROWSERSTACK_ACCESS_KEY`

For GitHub Actions, set repository secrets:

- `BROWSERSTACK_USERNAME`
- `BROWSERSTACK_ACCESS_KEY`

Optional email notifications (workflow):

- `SENDGRID_API_KEY`

If `SENDGRID_API_KEY` is configured, CI sends an email with a link to the uploaded `backend-allure-report` artifact.
