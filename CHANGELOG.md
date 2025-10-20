# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.3] - 2025-10-21

### Added

- Created comprehensive SOP documentation for publishing NuGet packages (`SOP/nuget_package_publishing.md`)
  - CLI publishing instructions with environment variable usage for API keys
  - GitHub workflow publishing instructions
  - Troubleshooting guide and best practices
  - Security notes for handling API keys
- Updated release workflow to automatically publish to NuGet.org
  - Added publish step using `NUGET_API_KEY` secret
  - Added `--skip-duplicate` flag to prevent duplicate package errors
- Added `artifacts/` directory and `*.nupkg` files to `.gitignore`

### Changed

- Release workflow now publishes packages to NuGet.org automatically on tag push or manual dispatch
- Improved PowerShell command syntax for `dotnet nuget push` to properly handle wildcards

## [1.0.2] - 2025-10-21

### Changed

- **BREAKING**: Renamed all namespaces from `YuriyGuts.RegexBuilder` to `RegexBuilder`
- **BREAKING**: Renamed assembly from `YuriyGuts.RegexBuilder.dll` to `RegexBuilder.dll`
- Renamed all project files and directories to remove `YuriyGuts` prefix
- Updated NuGet package ID to `RegexBuilder.NET9`
- Updated repository URL to `https://github.com/somenoe/RegexBuilder.NET9`
- Simplified project structure with cleaner naming conventions

### Migration Guide

If upgrading from version 1.0.1:

1. Update your NuGet package reference from `RegexBuilder` to `RegexBuilder.NET9`
2. Update all `using YuriyGuts.RegexBuilder;` statements to `using RegexBuilder;`
3. Rebuild your project

## [1.0.1] - 2025-10-21

### Added

- Forked from [YuriyGuts/regex-builder](https://github.com/YuriyGuts/regex-builder)
- Added GitHub Actions workflows for CI/CD
- Added CHANGELOG.md following Keep a Changelog format

### Changed

- Upgraded all projects from .NET Framework 4.0 to .NET 9.0
- Migrated all `.csproj` files from legacy format to SDK-style project format
- Upgraded test framework from MSTest V1 to MSTest V2 (3.6.0)
- Added `Microsoft.NET.Test.Sdk` (17.11.1) for modern test execution
- Enabled `ImplicitUsings` in the main library project
- Tests now run via `dotnet test` command instead of requiring Visual Studio Test Runner

### Removed

- Removed legacy MSBuild targets and tooling references
- Removed deprecated PostBuildEvent for NuGet package output
- Cleaned up legacy build configurations and platform-specific settings

### Fixed

- Resolved duplicate assembly attribute errors by preserving existing `AssemblyInfo.cs` files with `GenerateAssemblyInfo=false`

### Technical Details

- All 89 existing unit tests pass successfully on .NET 9.0
- Projects now use modern SDK-style format for better maintainability
- Compatible with .NET 9 SDK (tested with 9.0.306)

## [1.0.0] - 2011

### Added

- Initial release targeting .NET Framework 4.0
- Human-readable regex builder API
- Support for character ranges, character sets, and quantifiers
- Support for groups, alternations, and look-around assertions
- Comprehensive unit test coverage
