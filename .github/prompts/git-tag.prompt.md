---
mode: agent
description: Create and push git tags for a new release based on the version in CHANGELOG.md
---

# Git Tag Creation for New Release

Follow these steps to create and push a git tag for a new release based on the version in CHANGELOG.md.

## Prerequisites

- Ensure all changes are committed
- Ensure you're on the correct branch (typically `main` or `master`)
- Ensure the CHANGELOG.md has been updated with the new version

## Step-by-Step Instructions

### 1. Extract the Version from CHANGELOG.md

Open `CHANGELOG.md` and locate the latest version number in the format `[X.Y.Z]` at the top of the unreleased changes section.

Example: If you see `## [1.0.5] - 2025-10-21`, the version is `1.0.5`.

### 2. Verify Your Working Directory is Clean

```powershell
git status
```

Ensure there are no uncommitted changes. If there are, commit them first.

### 3. Create an Annotated Tag

Create an annotated tag with the version number prefixed with `v`:

```powershell
git tag -a v1.0.5 -m "Release version 1.0.5"
```

Replace `1.0.5` with the actual version from CHANGELOG.md.

**Note:** Use annotated tags (`-a`) rather than lightweight tags for releases, as they include metadata like the tagger name, date, and message.

### 4. Verify the Tag was Created

```powershell
git tag -l
```

Or to see the specific tag with details:

```powershell
git show v1.0.5
```

### 5. Push the Tag to Remote

Push the tag to the remote repository:

```powershell
git push origin v1.0.5
```

Or push all tags at once:

```powershell
git push origin --tags
```

### 6. Verify the Tag on Remote

Check that the tag appears on GitHub/remote repository:

```powershell
git ls-remote --tags origin
```

## Common Scenarios

### Creating a Tag for a Previous Commit

If you need to tag a specific commit (not HEAD):

```powershell
git tag -a v1.0.5 <commit-hash> -m "Release version 1.0.5"
git push origin v1.0.5
```

### Deleting a Tag (if mistakes were made)

Delete locally:

```powershell
git tag -d v1.0.5
```

Delete from remote:

```powershell
git push origin --delete v1.0.5
```

Then recreate the tag correctly.

### Moving a Tag to a Different Commit

```powershell
# Delete the old tag locally and remotely
git tag -d v1.0.5
git push origin --delete v1.0.5

# Create the new tag at the correct commit
git tag -a v1.0.5 <correct-commit-hash> -m "Release version 1.0.5"
git push origin v1.0.5
```

## Best Practices

1. **Always use annotated tags** (`-a` flag) for releases
2. **Follow semantic versioning**: `vMAJOR.MINOR.PATCH`
3. **Tag message format**: "Release version X.Y.Z" or include notable changes
4. **Tag after merging to main/master**: Don't tag feature branches
5. **Create tags on stable commits**: Ensure all tests pass before tagging
6. **Document in CHANGELOG**: Always update CHANGELOG.md before creating the tag

## Quick Reference

```powershell
# Full workflow for version 1.0.5
git status                                           # Verify clean working directory
git tag -a v1.0.5 -m "Release version 1.0.5"        # Create annotated tag
git tag -l                                           # Verify tag exists
git push origin v1.0.5                               # Push tag to remote
git ls-remote --tags origin                          # Verify on remote
```
