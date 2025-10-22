# add-doc-examples-to-examples-project

Short plan to add code examples from docs into `RegexBuilder.Examples` so they compile and run.

Goal

- Ensure representative code examples from `docs/*.md` compile and run inside `src/RegexBuilder.Examples` as proof the snippets work.

Scope

- Add examples that are present in docs but not yet in `Program.cs`, focused on advanced features:
  - `CommonPatterns.Email()` and `CommonPatterns.Url()` usage
  - Balancing groups
  - Unicode category matching
  - Inline option grouping
  - Conditional matching
  - Backtracking suppression (atomic groups)
  - IPv4 strict validation example

Assumptions

- `src/RegexBuilder/RegexBuilder.csproj` builds and exposes the APIs used in docs.
- The examples project already references the main project (it does in the csproj).

Implementation steps

1. Update `src/RegexBuilder.Examples/Program.cs` to add new example methods and call them from `Main`.
2. Keep examples small and self-verifying (Console.WriteLine with expected results).
3. Add this history file (done).
4. Build and run `RegexBuilder.Examples` locally to verify everything compiles and the examples run.

Acceptance criteria

- `dotnet build` for the examples project succeeds.
- `dotnet run` for the examples project produces console output showing the added examples executing and expected matches.

Next steps (optional)

- Add unit tests mirroring examples in `RegexBuilder.Tests`.
- Expand coverage to every single code fence if you want exhaustive mirroring.
