# Contributing to QueryContracts

Thank you for your interest in contributing to QueryContracts!

## Getting started

1. Fork the repository.
2. Clone your fork locally.
3. Create a new branch from `main`:
   ```bash
   git checkout -b feat/your-feature-name
   ```

## Building and testing

```bash
dotnet build QueryContracts.slnx
dotnet test QueryContracts.slnx
dotnet format QueryContracts.slnx --verify-no-changes
```

All changes must pass the build, tests and format check.

## Commit messages

This project follows [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` new feature
- `fix:` bug fix
- `docs:` documentation only
- `style:` formatting, missing semi colons, etc.
- `refactor:` code change that neither fixes a bug nor adds a feature
- `test:` adding or updating tests
- `chore:` maintenance tasks

Example:

```bash
git commit -m "feat: add GreaterThan filter operator"
```

## Pull requests

- Open a pull request against `main`.
- Fill out the PR template.
- Wait for the CI checks to pass.
- Request review from a maintainer.

## Code of conduct

This project adheres to the [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## Questions?

Open a [Discussion](https://github.com/LucasFernandes0101/querycontracts/discussions) or ask in an issue.
