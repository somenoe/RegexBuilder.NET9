---
mode: agent
---

# Update Package Version SOP

This prompt provides step-by-step instructions for updating the RegexBuilder.NET9 package version across all necessary files.

## When to Use This Prompt

Use this prompt when:

- Preparing a new release
- Incrementing the package version after adding features or fixing bugs
- Ensuring version consistency across all package-related files

## Prerequisites

- All changes for the new version should be documented in `CHANGELOG.md`
- All tests should be passing
- Code changes should be committed

## Files That Need Version Updates

The following files must be updated with the new version number:

1. `src/RegexBuilder/RegexBuilder.csproj` - Main project file with package metadata
2. `src/NuGetPackage/RegexBuilder.nuspec` - NuGet package specification
3. `src/RegexBuilder/Properties/AssemblyInfo.cs` - Assembly version attributes
4. `CHANGELOG.md` - Already updated with new version section
5. `README.md` - Update installation instructions if needed

## Step-by-Step Instructions

### Step 1: Determine the New Version Number

Follow [Semantic Versioning](https://semver.org/):

- **Major version (X.0.0)**: Breaking changes, incompatible API changes
- **Minor version (0.X.0)**: New features, backwards-compatible
- **Patch version (0.0.X)**: Bug fixes, backwards-compatible

Example: If current version is `1.0.4` and you added a new feature → `1.0.5`

### Step 2: Search for Current Version

Use the `#file_search` tool to find all occurrences of the current version:

**Using file_search (recommended for AI agents):**

```
#file_search with:
  - query: "1\.0\.4"
  - isRegexp: true
```

**Or using command line:**

```bash
# Search for the current version number in all files
grep -r "1\.0\.4" .
```

This will help identify all files containing the version number that need to be updated.

### Step 3: Update RegexBuilder.csproj

**File:** `src/RegexBuilder/RegexBuilder.csproj`

**Change:**

```xml
<Version>1.0.4</Version>
```

**To:**

```xml
<Version>1.0.5</Version>
```

**Location:** Inside the `<PropertyGroup>` section under "Package Metadata" comment

### Step 4: Update RegexBuilder.nuspec

**File:** `src/NuGetPackage/RegexBuilder.nuspec`

**Changes Required:**

1. Update version number:

   ```xml
   <version>1.0.4</version>
   ```

   To:

   ```xml
   <version>1.0.5</version>
   ```

2. Update release notes to describe the new version:
   ```xml
   <releaseNotes>Version 1.0.5: Brief description of changes.
     See CHANGELOG.md for details.</releaseNotes>
   ```

**Note:** Keep release notes concise. Full details should be in `CHANGELOG.md`.

### Step 5: Update AssemblyInfo.cs

**File:** `src/RegexBuilder/Properties/AssemblyInfo.cs`

**Change:**

```csharp
[assembly: AssemblyVersion("1.0.4.0")]
[assembly: AssemblyFileVersion("1.0.4.0")]
```

**To:**

```csharp
[assembly: AssemblyVersion("1.0.5.0")]
[assembly: AssemblyFileVersion("1.0.5.0")]
```

**Note:** Assembly version uses four parts (Major.Minor.Patch.Build). Keep the fourth part as `0` for releases.

### Step 6: Update README.md (If Needed)

**File:** `README.md`

If the installation instructions show a specific version, update them:

```bash
dotnet add package RegexBuilder.NET9 --version 1.0.4
```

To:

```bash
dotnet add package RegexBuilder.NET9 --version 1.0.5
```

Also update PackageReference examples:

```xml
<PackageReference Include="RegexBuilder.NET9" Version="1.0.4" />
```

To:

```xml
<PackageReference Include="RegexBuilder.NET9" Version="1.0.5" />
```

**Note:** Some projects prefer to omit version numbers in README to always show "latest". This is a style choice.

### Step 7: Verify the Changes

Run the following commands to ensure everything builds correctly:

```bash
# Build the project
dotnet build src/RegexBuilder/RegexBuilder.csproj --configuration Release

# Run all tests
dotnet test

# Create the NuGet package
dotnet pack src/RegexBuilder/RegexBuilder.csproj --configuration Release --output artifacts
```

### Step 8: Verify Package Creation

Check that the package was created with the correct version:

```bash
# List packages in artifacts directory
ls artifacts/*.nupkg
```

Expected output: `RegexBuilder.NET9.1.0.5.nupkg`

### Step 9: Review Changed Files

Use the `#get_changed_files` tool to see what files were modified:

```
#get_changed_files to verify all version-related files were updated
```

This helps ensure you didn't miss any files and that only the intended files were changed.

## Checklist

Before completing the version update:

- [ ] All version numbers updated consistently
- [ ] `CHANGELOG.md` has entry for new version with date
- [ ] Release notes in `.nuspec` file are accurate
- [ ] Build succeeds without errors
- [ ] All tests pass (171/171 tests)
- [ ] NuGet package created successfully
- [ ] Package filename matches expected version
- [ ] Verified changes using `#get_changed_files` tool

## Common Mistakes to Avoid

1. **Inconsistent versions**: Ensure all three files have the same version number
2. **Forgetting AssemblyVersion**: This file uses 4-part version (add `.0` at the end)
3. **Outdated release notes**: Always update the release notes in `.nuspec`
4. **Not running tests**: Always verify tests pass before publishing
5. **Wrong version format in AssemblyInfo**: Use `"X.Y.Z.0"` not `"X.Y.Z"`

## Troubleshooting

### Build Fails After Version Update

- Check for syntax errors in XML files (`.csproj`, `.nuspec`)
- Ensure no extra spaces or special characters in version strings
- Verify all XML tags are properly closed

### Package Not Created

- Check that output directory exists or can be created
- Ensure you're running the pack command from the correct directory
- Check for build errors in the output

### Version Mismatch Warnings

- Search for all occurrences of the old version: `grep -r "1\.0\.4" .`
- Update any missed references
- Some files (like history docs) may intentionally reference old versions

## Related Documentation

- See `SOP/nuget_package_publishing.md` for publishing instructions
- See `CHANGELOG.md` for version history and changelog format
- See [Semantic Versioning](https://semver.org/) for versioning guidelines

## Example: Complete Version Update

**Using AI tools (recommended for AI agents):**

```
# 1. Search for current version using file_search tool
Use #file_search with:
  - query: "1\.0\.4"
  - isRegexp: true

# 2. Read and update each file using read_file and replace_string_in_file tools
Files to update:
  - src/RegexBuilder/RegexBuilder.csproj
  - src/NuGetPackage/RegexBuilder.nuspec
  - src/RegexBuilder/Properties/AssemblyInfo.cs

# 3. Build and test using run_in_terminal tool
dotnet build --configuration Release
dotnet test

# 4. Create package
dotnet pack src/RegexBuilder/RegexBuilder.csproj --configuration Release --output artifacts

# 5. Verify package creation
ls artifacts/*.nupkg

# 6. Review changed files
Use #get_changed_files to verify all updates were made correctly
```

**Using command line (manual process):**

```bash
# 1. Search for current version
grep -r "1\.0\.4" src/

# 2. Update all three files (use your editor)
# - src/RegexBuilder/RegexBuilder.csproj
# - src/NuGetPackage/RegexBuilder.nuspec
# - src/RegexBuilder/Properties/AssemblyInfo.cs

# 3. Build and test
dotnet build --configuration Release
dotnet test

# 4. Create package
dotnet pack src/RegexBuilder/RegexBuilder.csproj --configuration Release --output artifacts

# 5. Verify
ls artifacts/*.nupkg

# 6. Review changes
git status
git diff
```

## Notes

- This SOP focuses on version updates only. For publishing to NuGet, see `SOP/nuget_package_publishing.md`
- Version numbers should follow semantic versioning principles
- Always update CHANGELOG.md before updating package version
- The package version is part of the release process, not development
