# Contributing to BrightSwagShop Backend

Thank you for contributing! Please follow the branching strategy below to keep the codebase organised.

---

## Branching Strategy

This project uses a **Git Flow**-inspired branching model:

```
main
 └── development
      ├── feature/<name>
      ├── feature/<name>
      └── fix/<name>
```

| Branch | Purpose |
|---|---|
| `main` | Stable, production-ready code. Only `development` (and `hotfix/*` / `release/*`) merges here. |
| `development` | Integration branch. All feature work is merged here first. |
| `feature/<name>` | New features – always branched **from** and merged back **into** `development`. |
| `fix/<name>` | Bug fixes – always branched **from** and merged back **into** `development`. |
| `hotfix/<name>` | Critical production fixes – branched from `main`, merged into **both** `main` and `development`. |
| `release/<version>` | Release preparation – branched from `development`, merged into `main` and `development`. |

---

## Setting Up the `development` Branch

If the `development` branch does not yet exist, create it from `main`:

```bash
git checkout main
git pull origin main
git checkout -b development
git push -u origin development
```

---

## Starting a New Feature

```bash
git checkout development
git pull origin development
git checkout -b feature/<your-feature-name>

# ... make your changes ...

git push -u origin feature/<your-feature-name>
# Then open a Pull Request targeting 'development'
```

---

## Merging Back to `development`

1. Open a Pull Request from `feature/<name>` → `development`.
2. Get at least one code review approval.
3. Ensure the CI pipeline passes.
4. Merge (prefer *Squash and Merge* to keep history clean).

---

## Merging `development` into `main`

When the team is ready to release:

1. Open a Pull Request from `development` → `main`.
2. Get approval and ensure CI passes.
3. Merge using *Merge Commit* to preserve the release history.

---

## CI Pipeline

The CI pipeline runs automatically on every push and pull request targeting `main` or `development`. A separate **branch strategy enforcement** workflow checks that feature and fix branches are not accidentally opened against `main`.
