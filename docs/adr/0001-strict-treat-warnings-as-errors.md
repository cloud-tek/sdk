# ADR 0001 — Strict `TreatWarningsAsErrors`, deterministic analyzer severities

- Status: Accepted
- Date: 2026-06-21
- Deciders: CloudTek SDK maintainers
- Related: [issue #4](https://github.com/cloudtek/sdk/issues/4)

## Context

`CloudTek.Sdk` consumers (Hive, `ai-cloudtek-brain-mcp`, siblings) repeatedly hit a
"green locally / red in CI" pattern: the same code built clean on a developer machine but
failed in CI on analyzer diagnostics (`IDE0007`, `IDE0019`, `CA1859`). This was long
misattributed to a macOS-vs-Linux toolchain divergence. A read-only investigation (toggling
only one environment variable on a single machine) showed it is **not** OS-related — it was
two SDK-side defects:

### Mechanism A — style severities silently dropped locally

`editor.globalconfig` is a **global** AnalyzerConfig (`is_global = true`). In a `.globalconfig`,
the `option = value:severity` shorthand only sets the preference *value*; the `:severity`
suffix is ignored. Severity for an `IDE`/style rule must be set with an explicit
`dotnet_diagnostic.<ID>.severity` line. Many style options (e.g. `csharp_style_var_*` → IDE0007,
`csharp_style_pattern_matching_over_as_with_null_check` → IDE0019) carried severity *only* on
the shorthand, so they emitted **no diagnostic at all** locally. CI surfaced them only because
of Mechanism B. Consumers had been hand-patching this with local `.editorconfig` files.

### Mechanism B — CI-only warn→error escalation

`CloudTek.DotNet.Sdk.Common.props` gated `TreatWarningsAsErrors=true` behind
`ContinuousIntegrationBuild` (set from `CI`/`TF_BUILD`). The same finding was a **warning**
locally and an **error** in CI (`CA1859` is purely this — a standard CA surfaced by the SDK's
`AnalysisLevel=latest-Recommended`).

## Decision

1. **Strict warnings-as-errors, always (decision B1).** `TreatWarningsAsErrors=true`
   unconditionally — local and CI alike — under the existing
   `CloudTekDotNetSdkEnableTreatWarningsAsErrors` opt-out (retained as the escape hatch). The
   `ContinuousIntegrationBuild` property is kept (it still drives deterministic builds); it is
   no longer the warnings-as-errors lever.
2. **Deterministic style severities.** Every shorthand-only style rule in `editor.globalconfig`
   gets an explicit `dotnet_diagnostic.<ID>.severity` line mirroring its intended severity, so
   the rule fires identically on every OS and in every environment.

After this change, "green locally" means "green in CI", period.

### Deliberately deferred

`csharp_prefer_braces` (IDE0011) is **not** pinned to an explicit severity. Its shorthand is
self-conflicting in the config (`when_multiline:suggestion` vs a later `true:silent`), and
enforcing it would trigger a broad brace-insertion wave unrelated to this fix. A few options
without a stable/applicable `IDE` diagnostic id (`csharp_style_prefer_readonly_span`,
`csharp_style_lambda_parameter_modifiers`, `dotnet_style_prefer_field_keyword`) are likewise
left on the shorthand. These can be revisited in a follow-up.

## Consequences

- **Blast radius:** every `CloudTek.Sdk` consumer, picked up only on a deliberate version bump
  + re-pin — no surprise breakage. The first upgrade after this change will surface a one-time
  cleanup wave of previously-hidden style violations. This is intended.
- **No measurable build-time cost** — analyzers already ran locally via
  `EnforceCodeStyleInBuild=true`; the only change is that more findings now fail the build.
- **Consumer follow-ups:** delete local `.editorconfig` workarounds (e.g.
  `ai-cloudtek-brain-mcp/.editorconfig`); correct any CLAUDE.md note claiming the SDK does not
  set `TreatWarningsAsErrors` — it does, now always-on.
- **Regression guard:** the `Net10Lib.StyleError` test fixture builds a file that violates
  IDE0019 and asserts the build fails, locking in that shorthand-only style rules now fire.

## Verification

On a single machine, the `Net10Lib.StyleError` fixture fails to build identically with and
without `CI=true` (`error IDE0019`), where previously the local build was silently green.
