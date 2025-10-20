# Common Patterns Feature - Implementation Plan

## Date: October 21, 2025

## Overview

Adding support for commonly used regex patterns (email addresses, URLs) to the RegexBuilder library to make it easier for developers to build patterns without memorizing complex regex syntax.

## Design Goals

1. **Ease of Use**: Provide simple, intuitive methods for common patterns
2. **Consistency**: Follow existing RegexBuilder API design patterns
3. **Flexibility**: Return RegexNode objects that can be combined with other builder methods
4. **Standards Compliance**: Use industry-standard regex patterns for email and URL validation

## Implementation Plan

### 1. New Class: CommonPatterns

Create a new static class `RegexBuilder.CommonPatterns` that provides factory methods for common patterns.

#### Methods to Implement:

- `Email()` - Returns a RegexNode for basic email validation
  - Pattern: Local part + @ + domain
  - Supports standard email format (simplified but practical)
- `Url()` - Returns a RegexNode for URL validation
  - Pattern: Protocol + domain + optional path/query/fragment
  - Supports http, https, ftp protocols

### 2. Pattern Details

#### Email Pattern

- Local part: alphanumeric, dots, hyphens, underscores
- Domain: alphanumeric with dots, ending with 2-6 letter TLD
- Example: `user.name@example.com`

#### URL Pattern

- Protocol: http://, https://, ftp:// (optional)
- Domain: standard domain format
- Path: optional path, query string, and fragment
- Example: `https://example.com/path?query=value#fragment`

### 3. Testing Strategy

Create comprehensive tests for:

- Valid email addresses (various formats)
- Invalid email addresses (missing @, invalid characters, etc.)
- Valid URLs (with/without protocol, with paths, with query strings)
- Invalid URLs (malformed domain, invalid characters)
- Integration with existing RegexBuilder methods (combining patterns)

### 4. Documentation

- Add examples to README showing common pattern usage
- Update CHANGELOG.md with new feature
- Include inline XML documentation comments

## API Usage Examples

```csharp
using RegexBuilder;

// Simple email validation
var emailRegex = new RegexBuilder()
    .AddNode(CommonPatterns.Email())
    .BuildRegex();

// URL with specific protocol
var urlRegex = new RegexBuilder()
    .AddNode(CommonPatterns.Url())
    .BuildRegex();

// Combining patterns
var contactInfoRegex = new RegexBuilder()
    .Text("Email: ")
    .AddNode(CommonPatterns.Email())
    .Text(" URL: ")
    .AddNode(CommonPatterns.Url())
    .BuildRegex();
```

## Version

This feature will be released as version 1.0.4.

## Files to Modify

1. Create: `src/RegexBuilder/CommonPatterns.cs`
2. Create: `src/RegexBuilder.Tests/CommonPatternsTests.cs`
3. Update: `CHANGELOG.md`
4. Update: `src/RegexBuilder/RegexBuilder.csproj` (version bump)
5. Update: `src/NuGetPackage/RegexBuilder.nuspec` (version bump)

## Success Criteria

- [ ] All new tests pass
- [ ] All existing tests continue to pass
- [ ] Patterns correctly validate common email and URL formats
- [ ] API is consistent with existing RegexBuilder design
- [ ] Documentation is clear and includes examples
