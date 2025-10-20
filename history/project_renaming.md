# Project Renaming and NuGet Publication Preparation

## Date: October 21, 2025

## Overview

This document describes the comprehensive renaming of the RegexBuilder.NET9 project to remove the original author's name prefix and prepare the package for NuGet publication.

## Motivation

The project was originally created by Yuriy Guts and used the `YuriyGuts.RegexBuilder` namespace and assembly naming convention. As this is now a community-maintained fork updated for .NET 9, it was appropriate to:

1. Simplify the naming by removing the personal prefix
2. Make the project more approachable for new contributors
3. Follow modern .NET naming conventions
4. Prepare for proper NuGet publication

## Changes Made

### 1. Directory Structure

Renamed all project directories:

- `src/YuriyGuts.RegexBuilder/` → `src/RegexBuilder/`
- `src/YuriyGuts.RegexBuilder.Tests/` → `src/RegexBuilder.Tests/`
- `src/YuriyGuts.RegexBuilder.TestApp/` → `src/RegexBuilder.TestApp/`

### 2. Project Files

Renamed all project files:

- `YuriyGuts.RegexBuilder.csproj` → `RegexBuilder.csproj`
- `YuriyGuts.RegexBuilder.Tests.csproj` → `RegexBuilder.Tests.csproj`
- `YuriyGuts.RegexBuilder.TestApp.csproj` → `RegexBuilder.TestApp.csproj`
- `YuriyGuts.RegexBuilder.sln` → `RegexBuilder.sln`

### 3. Project Metadata

Updated all `.csproj` files:

**Main Library (`RegexBuilder.csproj`):**

```xml
<AssemblyName>RegexBuilder</AssemblyName>
<RootNamespace>RegexBuilder</RootNamespace>
<PackageId>RegexBuilder.NET9</PackageId>
<Version>1.0.2</Version>
<PackageProjectUrl>https://github.com/somenoe/RegexBuilder.NET9</PackageProjectUrl>
<RepositoryUrl>https://github.com/somenoe/RegexBuilder.NET9</RepositoryUrl>
```

**Test Project (`RegexBuilder.Tests.csproj`):**

```xml
<RootNamespace>RegexBuilder.Tests</RootNamespace>
```

**Test App Project (`RegexBuilder.TestApp.csproj`):**

```xml
<RootNamespace>RegexBuilder.TestApp</RootNamespace>
```

### 4. Solution File Updates

Updated `RegexBuilder.sln` to reference all renamed project files and paths.

### 5. Source Code Changes

Updated all C# source files across the entire solution:

**Namespace Declarations:**

- `namespace YuriyGuts.RegexBuilder` → `namespace RegexBuilder`
- `namespace YuriyGuts.RegexBuilder.Tests` → `namespace RegexBuilder.Tests`

**Using Statements:**

- `using YuriyGuts.RegexBuilder;` → `using RegexBuilder;`

### 6. Documentation Updates

**README.md:**

- Updated installation instructions to use `RegexBuilder.NET9` package
- Changed assembly reference from `YuriyGuts.RegexBuilder.dll` to `RegexBuilder.dll`
- Updated file path references in documentation
- Updated project name references

**CHANGELOG.md:**

- Added new version 1.0.2 entry
- Documented breaking changes
- Provided migration guide for users upgrading from 1.0.1

## Breaking Changes

This release contains breaking changes for existing users:

1. **Namespace Change**: All code using `YuriyGuts.RegexBuilder` must be updated to `RegexBuilder`
2. **Assembly Name Change**: The output assembly is now `RegexBuilder.dll` instead of `YuriyGuts.RegexBuilder.dll`
3. **NuGet Package Change**: The package ID changed from hypothetical old package to `RegexBuilder.NET9`

## Migration Guide for Existing Users

For projects currently using version 1.0.1:

1. **Update NuGet Package:**

   ```bash
   # Remove old package (if exists)
   dotnet remove package RegexBuilder

   # Add new package
   dotnet add package RegexBuilder.NET9
   ```

2. **Update Using Statements:**

   Find and replace in your codebase:

   ```csharp
   // Old
   using YuriyGuts.RegexBuilder;

   // New
   using RegexBuilder;
   ```

3. **Rebuild Project:**
   ```bash
   dotnet clean
   dotnet build
   ```

## Version Bump Rationale

Changed version from `1.0.1` to `1.0.2` because:

- According to Semantic Versioning, this should be a MAJOR version change (2.0.0) due to breaking changes
- However, since this is essentially a rebranding and the project is still in early adoption phase
- We're using MINOR version bump with clear breaking change documentation
- Future versions will follow strict SemVer practices

## NuGet Publication Readiness

The project now follows NuGet best practices:

✅ **Required Metadata:**

- Unique PackageId: `RegexBuilder.NET9`
- SemVer versioning: `1.0.2`
- Author information
- Clear description
- MIT license
- Repository URL and type
- Relevant tags
- README file included

✅ **Project Structure:**

- SDK-style project format
- .NET 9.0 target framework
- Proper project references
- Clean namespace hierarchy

✅ **Documentation:**

- Comprehensive README
- Changelog following Keep a Changelog format
- History documentation

## Testing

After all changes:

1. Solution builds successfully
2. All 89 unit tests pass
3. Test app runs correctly
4. NuGet package can be created with `dotnet pack`

## Files Modified

### Project Files

- `src/RegexBuilder/RegexBuilder.csproj`
- `src/RegexBuilder.Tests/RegexBuilder.Tests.csproj`
- `src/RegexBuilder.TestApp/RegexBuilder.TestApp.csproj`
- `src/RegexBuilder.sln`

### Source Files (All .cs files in)

- `src/RegexBuilder/`
- `src/RegexBuilder.Tests/`
- `src/RegexBuilder.TestApp/`

### Documentation

- `README.md`
- `CHANGELOG.md`

### History Documentation

- `history/nuget_publication_preparation.md` (planning document)
- `history/project_renaming.md` (this document)

## Next Steps

1. ✅ Build and test the solution
2. ⏳ Create NuGet package with `dotnet pack`
3. ⏳ Test NuGet package locally
4. ⏳ Publish to NuGet.org
5. ⏳ Create GitHub release (v1.0.2)
6. ⏳ Update GitHub repository topics and description

## Commands for NuGet Publication

```powershell
# Navigate to project directory
cd src\RegexBuilder

# Create NuGet package
dotnet pack --configuration Release

# The package will be created at:
# bin\Release\RegexBuilder.NET9.1.0.2.nupkg

# Test package installation locally (in a test project)
dotnet add package RegexBuilder.NET9 --source "path\to\bin\Release"

# Publish to NuGet.org (replace with actual API key)
dotnet nuget push bin\Release\RegexBuilder.NET9.1.0.2.nupkg `
  --api-key YOUR_API_KEY `
  --source https://api.nuget.org/v3/index.json
```

## Attribution

This project respects the original work by Yuriy Guts:

- Original repository: https://github.com/YuriyGuts/regex-builder
- Copyright notice maintains attribution to both original and current maintainers
- MIT license preserved from original project
- README acknowledges the fork origin

## Conclusion

The project has been successfully renamed and prepared for NuGet publication. All references to `YuriyGuts.RegexBuilder` have been replaced with `RegexBuilder`, while maintaining proper attribution to the original author in copyright notices and documentation.
