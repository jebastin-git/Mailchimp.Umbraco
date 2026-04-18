# Contributing

## Branching

Use `develop` as the integration branch and `main` as the release branch.

Create feature branches from `develop` using one of these patterns:

- `feature/<short-description>`
- `fix/<short-description>`
- `chore/<short-description>`

Examples:

- `feature/mailchimp-double-opt-in`
- `fix/workflow-validation`
- `chore/update-readme`

## Pull Request Flow

1. Create a branch from `develop`.
2. Open a pull request from your feature branch into `develop`.
3. Complete review and approval.
4. Merge into `develop`.
5. Let GitHub auto-delete the merged feature branch.
6. Open a pull request from `develop` into `main` for release-ready changes.
7. Complete review and approval.
8. Merge into `main`.

## Protected Branches

- `main` must only receive changes through pull requests.
- `develop` must only receive changes through pull requests.
- `main` and `develop` must never be deleted.
- Feature branches may be deleted automatically after merge.

## Before Opening a Pull Request

Run the relevant validation locally:

```bash
dotnet build Mailchimp.Umbraco.slnx --configuration Release
```

If package-related changes were made, also validate restore, pack, or runtime behavior as appropriate.
