# How to Create a Git Commit Message in Conventional Commits Format

Task: Generate a commit message in Conventional Commits format and execute the exact git CLI commands to stage and commit the changes, adapted for the working OS (default: Windows 11).

Workflow Steps:

1. Call the get_changed_files tool (provide repositoryPath if not in the active repo) to retrieve changed files and incorporate a concise summary of the changed paths into the commit body.
2. Generate the commit message and use the #create_file tool to create temp_git_commit_message.txt with the commit message as content.
3. Execute the git commands for staging, committing using the message file, and running the removal script `remove_temp_git_commit_message` using the run_in_terminal tool.
4. Ensure commands are syntactically correct for the target OS.

Commit Message Requirements:

- Header: <type>(<scope>): <short description>
  - Types: feat, fix, docs, style, refactor, perf, test, chore
- Blank line after header
- Body: Bullet-point list of key changes (use "- " for each item)

Example Commit Message:

```
feat(api): add shrimp data endpoint

- implemented GET /shrimp route
- validated input and added tests
```

Command Guidance:

```bash
git add .
git commit -F temp_git_commit_message.txt
remove_temp_git_commit_message # my custom cli command to remove the temp file
```

Important Notes:

- Execute each command one at a time, not concatenated with `;`
- On Windows (PowerShell): Use `remove_temp_git_commit_message` (custom CLI command), NOT PowerShell's `Remove-Item` or `rm` or `del`
- Always use the custom removal script as specified, regardless of the shell

Output Structure:

1. Summarize changed files (obtained via get_changed_files tool) and include in commit body.
2. Generate the full commit message as a string.
3. Use the #create_file tool to create temp_git_commit_message.txt with the commit message content, execute git add ., git commit -F temp_git_commit_message.txt, then run the removal script `remove_temp_git_commit_message` using the run_in_terminal tool.

Goal: Produce clear, conventional commit messages that describe changes and rationale, by executing the commands directly.
