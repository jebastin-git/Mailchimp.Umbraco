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

## Versioning Policy

Use standard semantic versioning for the package.

- `patch`: bug fixes, metadata tweaks, dependency patch updates, workflow or pipeline fixes
- `minor`: backward-compatible features and enhancements
- `major`: breaking changes, dropped Umbraco support, or incompatible workflow/configuration changes

Examples:

- `1.0.1` for fixes or patch dependency updates
- `1.1.0` for new workflow capabilities
- `2.0.0` for breaking changes or support-policy changes

Do not align the package version directly to the Umbraco major version. Communicate Umbraco compatibility through:

- package dependency ranges
- `README.md`
- `umbraco-marketplace.json`
- release notes

## Release Guidance

Use `develop` for integration and `main` for publishable releases.

Recommended release flow:

1. Merge feature work into `develop`.
2. Create a small version-bump branch from `develop` when preparing a release.
3. Merge the version bump into `develop`.
4. Promote `develop` to `main`.
5. Let GitHub Actions publish from `main`.

If older Umbraco support becomes too costly to maintain, drop it only in a package major release and call that out clearly in release notes.

## Before Opening a Pull Request

Run the relevant validation locally:

```bash
dotnet build Mailchimp.Umbraco.slnx --configuration Release
```

If package-related changes were made, also validate restore, pack, or runtime behavior as appropriate.
