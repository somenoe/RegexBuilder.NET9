# RegexBuilder.Examples

This project contains executable demonstrations of all code examples from the main README.md file.

## Purpose

This example project serves to:

1. **Verify** all README code examples compile and run correctly
2. **Demonstrate** practical usage of RegexBuilder.NET9
3. **Provide** a starting point for developers to experiment with the library

## Running the Examples

### Via Visual Studio

1. Open `src/RegexBuilder.slnx`
2. Set `RegexBuilder.Examples` as startup project (right-click → Set as Startup Project)
3. Press F5 or click "Run"

### Via Command Line

```bash
cd src/RegexBuilder.Examples
dotnet run
```

## Examples Included

### 1. Quick Start: Email Regex

Demonstrates building a simple email matching regex with named capture groups.

- Pattern: `(\w+)@(\w+)\.(\w+)`
- Shows: Basic pattern building, named groups, extracting match results

### 2. Real-World Example: href Extraction

Extracts `href` attributes from HTML anchor tags, handling both quoted and unquoted values.

- Compares: Traditional regex vs RegexBuilder approach
- Shows: Alternation, concatenation, character sets, options

### 3. Basic Pattern Building

Simple patterns demonstrating core concepts:

- Literal matching with quantifiers
- RegexOptions usage (IgnoreCase)
- Pattern testing with IsMatch

### 4. Grouping and Capturing

Demonstrates different grouping constructs:

- Named capturing groups
- Non-capturing groups with alternation
- Extracting captured values

### 5. Substitution Patterns

Shows how to use SubstitutionBuilder for text replacement:

- Swapping words using named group references
- Building replacement patterns
- Using Regex.Replace() with RegexBuilder patterns

## Expected Output

When you run the project, you'll see output similar to:

```text
=== RegexBuilder.NET9 README Examples ===

--- Quick Start: Email Regex ---
  Local Part: user
  Domain: example.com

--- Real-World Example: Extract href Attributes ---
  Testing traditional regex:
    Found: https://example.com
    Found: https://github.com
    Found: https://microsoft.com

  Testing RegexBuilder pattern:
    Found: https://example.com
    Found: https://github.com
    Found: https://microsoft.com

--- Basic Pattern Building ---
  Simple pattern matches:
    'prefix-123': True
    'prefix-42': True
    'prefix-abc': False

  Pattern with IgnoreCase:
    'hello world': True
    'HELLO WORLD': True
    'Hello World': True

--- Grouping and Capturing ---
  Capturing group 'name': Alice

  Non-capturing group alternation:
    'cat': True
    'dog': True
    'bird': False

--- Substitution Patterns ---
  Original: 'hello world'
  Swapped:  'world hello'

  More swap examples:
    'foo bar' -> 'bar foo'
    'first second' -> 'second first'

=== All Examples Completed Successfully ===
```

## Adding Your Own Examples

Feel free to modify `Program.cs` to experiment with RegexBuilder. Some ideas:

- Try different quantifiers (ExactCount, Range, Optional)
- Experiment with anchors (StartOfString, EndOfString, WordBoundary)
- Build complex patterns with multiple alternations
- Test backreferences and lookahead/lookbehind assertions

## Related Files

- Main documentation: [README.md](../../README.md)
- Test suite: [RegexBuilder.Tests/](../RegexBuilder.Tests/)
