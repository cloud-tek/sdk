# Changelog

All notable changes to `CloudTek.Sdk` are documented here.

## 10.0.0

First stable `10.0.0` release (drops the `-beta` pre-release suffix).

### Changed — strict analyzer enforcement (BREAKING for consumers on upgrade)

This release closes a "green locally / red in CI" gap where analyzer findings behaved
differently on developer machines vs CI. See
[ADR 0001](docs/adr/0001-strict-treat-warnings-as-errors.md) and
[issue #4](https://github.com/cloudtek/sdk/issues/4).

- **`TreatWarningsAsErrors` is now always on**, locally and in CI alike (previously gated on
  `CI`/`TF_BUILD` via `ContinuousIntegrationBuild`). The
  `CloudTekDotNetSdkEnableTreatWarningsAsErrors=false` per-project opt-out is retained.
- **Style-rule severities are now deterministic.** In a `.globalconfig` the `value:severity`
  shorthand does not set severity; explicit `dotnet_diagnostic.<ID>.severity` lines were added
  for every shorthand-only style rule (IDE0007=warning, IDE0019=error, the `this.`-qualification
  block, predefined-type, expression-bodied members, and more). These rules now fire identically
  on every OS and environment.

> **Expect a one-time cleanup wave.** Projects upgrading to `10.0.0` will surface previously
> hidden style violations (IDE0007, IDE0019, …) and warnings that were tolerated locally are now
> build errors. This is intended: after upgrading, "green locally" means "green in CI". Consumers
> that carried local `.editorconfig` workarounds for these rules can delete them.

### Deferred

- `csharp_prefer_braces` (IDE0011) is intentionally left without an explicit severity (its
  shorthand is self-conflicting; pinning it would cause an unrelated brace-enforcement wave).
- A handful of options without a stable/applicable `IDE` diagnostic id remain on the shorthand.

### Internal

- Added the `Net10Lib.StyleError` SDK test fixture, which asserts a shorthand-only style rule
  (IDE0019) now fails the build — a regression guard for the determinism fix.
