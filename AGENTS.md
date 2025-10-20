# AGENTS.md

This file provides guidance when working with code in this repository.

## Development Workflow

1. Analyze feature and create a detailed implementation plan on `history` folder e.g. `history/feature-name.md`.
2. Create a new branch for the feature: `git checkout -b feature/your-feature-name`
3. Implement the feature and commit changes: `git commit -m "Implement feature"`
4. Review and test the feature thoroughly. Update `history/feature-name.md` if planned steps change.
5. After successful testing, update
   - `ROADMAP.md` to reflect the new feature status.
   - `CHANGELOG.md` with the new feature details.
   - (optional) `README.md` if necessary.
   - (optional) `AGENTS.md` if the feature affects agent behavior.

## Recommended Tools

- **#test_failure**: Includes test failure information in the prompt for debugging
- **#runTests**: Run unit tests in files or collect coverage reports
- **#file_search**: Search for files by glob pattern matching
- **#get_changed_files**: Get git diffs of current file changes in a git repository
- **#create_file**: Create new files in the workspace
- **#edit_files**: Edit existing files in the workspace
- **#create_directory**: Create directory structures in the workspace
- **#fetch_webpage**: Fetch content from web pages for documentation reference
- **#vscode-websearchforcopilot_webSearch**: Search the web for relevant up-to-date information
