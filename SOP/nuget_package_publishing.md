# SOP: Publishing NuGet Package

This document outlines the standard operating procedures for publishing the RegexBuilder NuGet package to NuGet.org.

## Prerequisites

- .NET 9.0 SDK installed
- NuGet API key from NuGet.org (<https://www.nuget.org/account/apikeys>)
- Appropriate permissions to publish packages

## Method 1: Publishing via CLI

### Step 1: Set Up Environment Variable

Set your NuGet API key as an environment variable to avoid exposing it in command history.

**Windows (PowerShell):**

```powershell
$env:NUGET_API_KEY = "your-api-key-here"
```

**Linux/macOS (Bash):**

```bash
export NUGET_API_KEY="your-api-key-here"
```

### Step 2: Build the Project

Navigate to the project root directory:

```bash
cd d:\pj\regex-builder
```

Restore dependencies:

```bash
dotnet restore src/RegexBuilder.slnx
```

Build in Release configuration:

```bash
dotnet build src/RegexBuilder.slnx --configuration Release --no-restore
```

### Step 3: Run Tests

Ensure all tests pass before publishing:

```bash
dotnet test src/RegexBuilder.slnx --configuration Release --no-build --verbosity normal
```

### Step 4: Pack the NuGet Package

Create the NuGet package:

```bash
dotnet pack src/RegexBuilder/RegexBuilder.csproj --configuration Release --no-build --output ./artifacts
```

### Step 5: Publish to NuGet.org

Publish the package using the environment variable:

**Windows (PowerShell):**

```powershell
dotnet nuget push (Get-Item ./artifacts/*.nupkg).FullName --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
```

**Linux/macOS (Bash):**

```bash
dotnet nuget push ./artifacts/*.nupkg --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
```

### Step 6: Verify Publication

1. Visit <https://www.nuget.org/packages/RegexBuilder>
2. Verify the new version appears in the package listing
3. Check that the package metadata is correct

## Method 2: Publishing via GitHub Workflow

### Step 1: Set Up GitHub Secret

1. Navigate to your GitHub repository
2. Go to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: Your NuGet.org API key
6. Click **Add secret**

### Step 2: Trigger the Release Workflow

The release workflow can be triggered in two ways:

#### Option A: Push a Git Tag

```bash
# Create and push a version tag
git tag v1.0.0
git push origin v1.0.0
```

The workflow will automatically:

- Build the project
- Run tests
- Pack the NuGet package
- Publish to NuGet.org
- Create a GitHub release

#### Option B: Manual Workflow Dispatch

1. Go to **Actions** tab in your GitHub repository
2. Select **Release** workflow
3. Click **Run workflow**
4. Enter the version number (e.g., `1.0.0`)
5. Click **Run workflow**

### Step 3: Monitor Workflow Execution

1. Navigate to the **Actions** tab
2. Click on the running workflow
3. Monitor each step's progress
4. Check for any errors in the logs

### Step 4: Verify Publication

1. Visit <https://www.nuget.org/packages/RegexBuilder>
2. Verify the new version appears
3. Check the GitHub Releases page for the created release

## Troubleshooting

### Common Issues

#### Invalid API Key

- **Error:** `401 Unauthorized`
- **Solution:** Verify your API key is valid and has push permissions

#### Package Already Exists

- **Error:** `409 Conflict`
- **Solution:** You cannot overwrite existing versions. Increment the version number

#### Missing Dependencies

- **Error:** Build or pack failures
- **Solution:** Run `dotnet restore` and ensure all dependencies are available

#### Test Failures

- **Error:** Tests fail during workflow
- **Solution:** Run tests locally, fix issues, and commit changes before re-running

### Environment Variable Not Set

If you see errors about missing API key:

- Verify the environment variable is set in your current shell session
- For GitHub Actions, verify the secret is properly configured

## Version Numbering

Follow Semantic Versioning (SemVer):

- **Major.Minor.Patch** (e.g., 1.0.0)
- **Major**: Breaking changes
- **Minor**: New features, backward compatible
- **Patch**: Bug fixes, backward compatible

## Best Practices

1. **Always run tests** before publishing
2. **Use tags for releases** to maintain version history
3. **Update CHANGELOG.md** before releasing
4. **Never commit API keys** to source control
5. **Use environment variables** for sensitive data
6. **Test locally first** before using GitHub workflow
7. **Review package metadata** in the .nuspec file before publishing
8. **Create pre-release packages** for testing using alpha/beta suffixes

## Security Notes

- NuGet API keys should be treated as passwords
- Rotate API keys periodically
- Use scoped API keys with minimal required permissions
- For GitHub Actions, always use repository secrets
- Never log or print API keys in workflows or scripts

## References

- [NuGet Package Publishing Documentation](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [GitHub Actions Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Semantic Versioning](https://semver.org/)
