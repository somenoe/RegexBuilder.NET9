# NuGet Publication Preparation Plan

## Date: October 21, 2025

## Overview

Prepare the RegexBuilder.NET9 project for NuGet publication with proper naming conventions and metadata following NuGet best practices.

## Goals

1. Update project metadata for NuGet publication
2. Remove old author name (YuriyGuts) from all project names and namespaces
3. Update documentation and changelog
4. Ensure consistent naming across all files and references

## NuGet Best Practices (from Microsoft Documentation)

### Package Metadata Requirements

- ✅ **PackageId**: Use unique, descriptive name (already set to `RegexBuilder.NET9`)
- ✅ **Version**: Follow SemVer (currently `1.0.1`)
- ✅ **Authors**: Current maintainer name
- ✅ **Description**: Short, clear description (up to 4000 chars)
- ✅ **Copyright**: Include copyright notice
- ✅ **PackageProjectUrl**: Link to project homepage
- ✅ **RepositoryUrl**: Link to source repository (needs update to new URL)
- ✅ **RepositoryType**: git
- ✅ **PackageTags**: Relevant keywords for discoverability
- ✅ **PackageReadmeFile**: Include README.md
- ✅ **PackageLicenseExpression**: MIT (already set)

### Additional Recommendations

- Consider adding a package icon (128x128 PNG with transparent background)
- Include release notes with each update
- Set up Source Link for debugging support

## Changes Required

### 1. Update Repository URL

**Status**: ❌ Not done
**Files to update**:

- `src/YuriyGuts.RegexBuilder/YuriyGuts.RegexBuilder.csproj`
  - Update `<PackageProjectUrl>` to `https://github.com/somenoe/RegexBuilder.NET9`
  - Update `<RepositoryUrl>` to `https://github.com/somenoe/RegexBuilder.NET9`

### 2. Rename Projects and Files

**Status**: ❌ Not done

#### Directory Renames:

- `src/YuriyGuts.RegexBuilder/` → `src/RegexBuilder/`
- `src/YuriyGuts.RegexBuilder.Tests/` → `src/RegexBuilder.Tests/`
- `src/YuriyGuts.RegexBuilder.TestApp/` → `src/RegexBuilder.TestApp/`

#### File Renames:

- `src/YuriyGuts.RegexBuilder.sln` → `src/RegexBuilder.sln`
- `src/YuriyGuts.RegexBuilder/YuriyGuts.RegexBuilder.csproj` → `src/RegexBuilder/RegexBuilder.csproj`
- `src/YuriyGuts.RegexBuilder.Tests/YuriyGuts.RegexBuilder.Tests.csproj` → `src/RegexBuilder.Tests/RegexBuilder.Tests.csproj`
- `src/YuriyGuts.RegexBuilder.TestApp/YuriyGuts.RegexBuilder.TestApp.csproj` → `src/RegexBuilder.TestApp/RegexBuilder.TestApp.csproj`
- `src/YuriyGuts.RegexBuilder.vsmdi` → (consider removing as legacy VS test file)

### 3. Update Project Properties

**Status**: ❌ Not done

#### Main Library Project (RegexBuilder.csproj):

```xml
<AssemblyName>RegexBuilder</AssemblyName>
<RootNamespace>RegexBuilder</RootNamespace>
<PackageId>RegexBuilder.NET9</PackageId>
```

#### Test Project (RegexBuilder.Tests.csproj):

```xml
<AssemblyName>RegexBuilder.Tests</AssemblyName>
<RootNamespace>RegexBuilder.Tests</RootNamespace>
```

#### Test App Project (RegexBuilder.TestApp.csproj):

```xml
<AssemblyName>RegexBuilder.TestApp</AssemblyName>
<RootNamespace>RegexBuilder.TestApp</RootNamespace>
```

### 4. Update Namespace Declarations in Code Files

**Status**: ❌ Not done
**Action**: Change all `namespace YuriyGuts.RegexBuilder` to `namespace RegexBuilder` in:

- All `.cs` files in `src/RegexBuilder/`
- All `.cs` files in `src/RegexBuilder.Tests/`
- All `.cs` files in `src/RegexBuilder.TestApp/`

### 5. Update Using Statements

**Status**: ❌ Not done
**Action**: Change all `using YuriyGuts.RegexBuilder;` to `using RegexBuilder;` in:

- Test files
- TestApp files
- Any other files with references

### 6. Update Documentation Files

**Status**: ❌ Not done

#### README.md:

- Update installation command from old NuGet package to new one
- Update references to file paths (remove YuriyGuts prefix)
- Update project reference examples
- Update assembly DLL name references

#### CHANGELOG.md:

- Add new entry for version 1.0.2 (or keep 1.0.1 if publishing now)
- Document the renaming changes
- Document NuGet publication preparation

### 7. Update Project References

**Status**: ❌ Not done
**Action**: Update all `<ProjectReference>` paths in .csproj files to point to new locations

### 8. Update Solution File

**Status**: ❌ Not done
**Action**: Update all project paths and names in the .sln file

## Implementation Order

1. **Phase 1: Planning** ✅
   - Create this plan document
   - Review all changes needed

2. **Phase 2: File and Directory Renames**
   - Rename directories first
   - Rename .csproj files
   - Rename .sln file
   - Update file paths in .sln

3. **Phase 3: Update Project Files**
   - Update .csproj metadata (AssemblyName, RootNamespace, URLs)
   - Update project references between projects
   - Update solution file project paths

4. **Phase 4: Update Source Code**
   - Update namespace declarations in all .cs files
   - Update using statements
   - Update AssemblyInfo.cs if needed

5. **Phase 5: Update Documentation**
   - Update README.md
   - Update CHANGELOG.md
   - Create this history document

6. **Phase 6: Testing**
   - Build solution
   - Run all unit tests
   - Verify package metadata with `dotnet pack`

7. **Phase 7: NuGet Publication**
   - Create NuGet package with `dotnet pack`
   - Test package locally
   - Publish to NuGet.org

## NuGet Publication Commands

```powershell
# Navigate to project directory
cd src\RegexBuilder

# Create NuGet package
dotnet pack --configuration Release

# The .nupkg file will be in bin\Release\

# Publish to NuGet (replace with actual API key)
dotnet nuget push bin\Release\RegexBuilder.NET9.1.0.1.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## Post-Publication Checklist

- [ ] Verify package appears on NuGet.org
- [ ] Test installation in a new project
- [ ] Update GitHub repository description
- [ ] Add GitHub topics/tags for discoverability
- [ ] Consider creating a GitHub release
- [ ] Update README with correct installation instructions

## Notes

- The package ID `RegexBuilder.NET9` is already well-chosen and unique
- Consider adding an icon in future versions for better visual differentiation
- Current copyright notice properly acknowledges both original and current authors
- MIT license is appropriate for open-source
- All 89 tests should pass after renaming (namespace changes)

## References

- [NuGet Package Authoring Best Practices](https://learn.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices)
- [Create a NuGet package using the dotnet CLI](https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli)
- [Semantic Versioning](https://semver.org/)
