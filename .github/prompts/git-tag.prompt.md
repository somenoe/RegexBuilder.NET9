---
mode: agent
description: Create and push git tags for a new release based on the version in CHANGELOG.md
---

# Git Tag Creation for New Release

Follow these steps to create and push a git tag for a new release based on the version in CHANGELOG.md.

## Prerequisites

- Ensure all changes are committed on the `dev` branch
- Ensure the CHANGELOG.md has been updated with the new version
- Ensure all tests pass before merging to `master`

## Step-by-Step Instructions

### 1. Extract the Version from CHANGELOG.md

Open `CHANGELOG.md` and locate the latest version number in the format `[X.Y.Z]` at the top of the unreleased changes section.

Example: If you see `## [1.0.5] - 2025-10-21`, the version is `1.0.5`.

### 2. Verify Your Working Directory is Clean on `dev` Branch

```powershell
git status
```

Ensure there are no uncommitted changes on the `dev` branch. If there are, commit them first.

### 3. Switch to `master` Branch

```powershell
git checkout master
```

Switch from the `dev` branch to the `master` branch where tags and releases are created.

### 4. Pull Latest Changes from Remote

```powershell
git pull origin master
```

Ensure you have the latest changes on the `master` branch.

### 5. Merge `dev` into `master`

```powershell
git merge dev --no-ff -m "Merge dev into master for release v1.0.5"
```

Merge the release changes from `dev` branch into `master`. The `--no-ff` flag creates a merge commit, which is recommended for releases.

Replace `1.0.5` with your actual version number in the commit message.

### 6. Create an Annotated Tag

Create an annotated tag with the version number prefixed with `v` on the `master` branch:

```powershell
git tag -a v1.0.5 -m "Release version 1.0.5"
```

Replace `1.0.5` with the actual version from CHANGELOG.md.

**Note:** Use annotated tags (`-a`) rather than lightweight tags for releases, as they include metadata like the tagger name, date, and message.

### 7. Verify the Tag was Created

```powershell
git tag -l
```

Or to see the specific tag with details:

```powershell
git show v1.0.5
```

### 8. Push `master` Branch to Remote

```powershell
git push origin master
```

Push the merged `master` branch to remote before pushing tags.

### 9. Push the Tag to Remote

Push the tag to the remote repository:

```powershell
git push origin v1.0.5
```

Or push all tags at once:

```powershell
git push origin --tags
```

### 10. Verify the Tag on Remote

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
3. **Merge before tagging**: Always merge `dev` to `master` before creating release tags
4. **Tag message format**: "Release version X.Y.Z" or include notable changes
5. **Tag on master branch**: Only create tags on the `master` branch for releases
6. **Create tags on stable commits**: Ensure all tests pass before tagging
7. **Document in CHANGELOG**: Always update CHANGELOG.md before creating the tag
8. **Push branch before tags**: Always push the `master` branch before pushing tags to remote

## Quick Reference

```powershell
# Full workflow for version 1.0.5
git status                                                        # Verify clean on dev
git checkout master                                               # Switch to master
git pull origin master                                            # Get latest master
git merge dev --no-ff -m "Merge dev into master for release v1.0.5"  # Merge dev
git tag -a v1.0.5 -m "Release version 1.0.5"                    # Create annotated tag
git tag -l                                                        # Verify tag exists
git push origin master                                            # Push master branch
git push origin v1.0.5                                            # Push tag to remote
git ls-remote --tags origin                                       # Verify on remote
```
