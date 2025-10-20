# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
