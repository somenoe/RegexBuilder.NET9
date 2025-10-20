# .NET 9 Upgrade Plan

This document outlines the steps to upgrade the solution to .NET 9 and validate all tests pass before and after the upgrade.

## Goals

- Create a working branch for the upgrade work.
- Run baseline build/tests on current code.
- Migrate projects to SDK-style and target `net9.0`.
- Update test project to MSTest V2 and run on `dotnet test`.
- Fix any build/test issues until all tests are green.

## Current State (pre-upgrade)

- Projects target .NET Framework 4.0 (old, non-SDK csproj format).
- Tests use classic MSTest V1 (Visual Studio Test Framework 10.0) which does not run with `dotnet test`.

## Approach

1. Baseline
   - Attempt to build and run tests with current setup.
   - If classic MSTest runner is unavailable on the machine, record that baseline tests cannot be executed with the current toolchain.

2. Migration
   - Convert all `.csproj` files to SDK-style.
   - Library: target `net9.0`.
   - Test project: target `net9.0` and migrate to MSTest V2 packages with `Microsoft.NET.Test.Sdk`.
   - Test app: target `net9.0`.
   - Remove legacy build settings that are incompatible (e.g., ToolsVersion, PostBuildEvent copying to `NuGetPackage/lib/net40`).
   - Keep assembly metadata via SDK properties; remove legacy `AssemblyInfo.cs` if necessary.

3. Validation
   - Run `dotnet restore` and `dotnet build` for the solution.
   - Run `dotnet test`; fix compile and runtime issues until all tests pass.

4. Packaging (optional follow-up)
   - If needed, migrate packing to SDK (`dotnet pack`) and update nuspec/target folder to `lib/net9.0`.

## Risks & Mitigations

- Old MSTest V1 tests won’t run in `dotnet test` — migrate to MSTest V2 keeping the same attributes/namespaces.
- Behavior differences between .NET Framework 4.0 and .NET 9 in `System.Text.RegularExpressions` — validate with existing tests and adjust if needed.

## Success Criteria

- `dotnet test` passes 100% on the upgraded solution targeting `net9.0`.

## Status & How to Run

- Branch: `chore/upgrade-to-net9`
- SDKs: Verified `dotnet --version` = 9.0.306
- Build: dotnet build src\YuriyGuts.RegexBuilder.sln
- Test: dotnet test src\YuriyGuts.RegexBuilder.sln

All tests pass on .NET 9: 89 succeeded, 0 failed.
