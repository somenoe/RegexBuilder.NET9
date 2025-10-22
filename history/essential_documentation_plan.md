# Essential Documentation Plan - 3-Page Strategy

## Date: October 22, 2025

## Overview

This document outlines the comprehensive plan for creating 3 essential documentation pages for RegexBuilder.NET9 that provide maximum value to users while keeping maintenance overhead minimal.

## Documentation Strategy

### Core Philosophy

- **Quality over Quantity**: Focus on 3 well-crafted pages instead of many incomplete pages
- **Progressive Disclosure**: Start simple, gradually reveal complexity
- **Copy-Paste Friendly**: Provide ready-to-use code examples
- **Self-Documenting**: Each example explains what it does and why

### Target Audience

1. **Beginners**: Developers new to RegexBuilder who need quick wins
2. **Practitioners**: Developers looking for production-ready patterns
3. **Advanced Users**: Developers who need comprehensive API reference

## The 3 Essential Pages

### 1. Getting Started (`getting-started.md`)

**Purpose**: Get developers productive in 5 minutes

**Target Audience**: Beginners, Quick Learners

**Content Structure**:

#### 1.1 Installation

- NuGet package installation (CLI and PackageReference)
- Version requirements (.NET 9+)
- Quick verification step

#### 1.2 Your First Pattern (Fluent API)

- Simple example using `PatternBuilder` (modern, recommended approach)
- Email validation pattern
- Complete working code with explanation
- Expected output

#### 1.3 Your First Pattern (Classic API)

- Same example using classic `RegexBuilder` API
- Shows alternative approach for comparison
- Explains when to use each style

#### 1.4 Basic Concepts

- What RegexBuilder does differently
- Pattern composition philosophy
- How it compiles to standard .NET Regex

#### 1.5 Next Steps

- Links to Common Patterns Library
- Links to API Guide
- Link to GitHub examples

**Code Examples**:

- Email validation (fluent)
- Email validation (classic)
- Phone number pattern
- URL validation

**Success Metrics**:

- User can install package ✓
- User can create first working regex in < 5 minutes ✓
- User understands basic pattern composition ✓

---

### 2. Common Patterns Library (`common-patterns.md`)

**Purpose**: Provide copy-paste ready solutions for 80% of use cases

**Target Audience**: All users looking for quick solutions

**Content Structure**:

#### 2.1 Introduction

- How to use this library
- Customization guidelines
- Performance considerations

#### 2.2 Email Addresses

- Pattern code (fluent and classic)
- What it matches (with examples)
- What it doesn't match (edge cases)
- Customization options
- Real-world usage example

#### 2.3 URLs and Web Addresses

- HTTP/HTTPS URLs
- URLs with query parameters
- URLs with fragments
- Domain-only patterns
- Complete example with protocol validation

#### 2.4 Phone Numbers

- US phone formats
- International formats
- With/without country code
- Various separator styles (dash, dot, space)

#### 2.5 Postal Codes

- US ZIP codes (5-digit and ZIP+4)
- Canadian postal codes
- UK postcodes
- Generic patterns

#### 2.6 Credit Cards

- Visa pattern
- Mastercard pattern
- American Express pattern
- Generic credit card number validator
- **Security note**: Never log or store card numbers

#### 2.7 IP Addresses

- IPv4 pattern
- IPv6 pattern
- CIDR notation

#### 2.8 Date and Time

- ISO 8601 dates (YYYY-MM-DD)
- US dates (MM/DD/YYYY)
- European dates (DD/MM/YYYY)
- Time patterns (HH:MM, HH:MM:SS)

#### 2.9 Identifiers and Codes

- UUID/GUID patterns
- Hexadecimal strings
- Base64 strings
- Social Security Numbers (with privacy note)

#### 2.10 File Paths and Names

- Windows file paths
- Unix file paths
- File extensions
- URL-safe filenames

#### 2.11 Quick Reference Table

| Pattern  | Usage                      | Example Match              |
| -------- | -------------------------- | -------------------------- |
| Email    | `CommonPatterns.Email()`   | `user@example.com`         |
| URL      | `CommonPatterns.Url()`     | `https://example.com/path` |
| US Phone | `CommonPatterns.UsPhone()` | `555-123-4567`             |
| ...      | ...                        | ...                        |

**Code Template for Each Pattern**:

````markdown
### X.X Pattern Name

**What it matches**: Brief description

**Fluent API**:

```csharp
var pattern = RegexBuilder.Pattern()
    .Start()
    .Email()  // or other pattern
    .End()
    .Build();

var regex = RegexBuilder.Build(pattern);
```
````

**Classic API**:

```csharp
var regex = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    CommonPatterns.Email(),
    RegexBuilder.LineEnd()
);
```

**Examples**:

- ✓ Matches: `example1`, `example2`
- ✗ Doesn't match: `badexample1`, `badexample2`

**Real-World Usage**:

```csharp
// Complete validation function
public bool ValidateEmail(string input)
{
    var emailRegex = RegexBuilder.Build(
        RegexBuilder.LineStart(),
        CommonPatterns.Email(),
        RegexBuilder.LineEnd()
    );
    return emailRegex.IsMatch(input);
}
```

**Customization**:

- How to make pattern more strict
- How to make pattern more lenient
- Common variations

```

```

**Success Metrics**:

- 10+ production-ready patterns ✓
- Every pattern has working example ✓
- Every pattern explains edge cases ✓
- Users can copy-paste and use immediately ✓

---

### 3. API Guide (`api-guide.md`)

**Purpose**: Comprehensive reference for all RegexBuilder capabilities

**Target Audience**: All users, especially those building complex patterns

**Content Structure**:

#### 3.1 Introduction

- What this guide covers
- How to read the API documentation
- Navigation tips

#### 3.2 Core Architecture

- `RegexBuilder` - Static factory class
- `PatternBuilder` - Fluent builder class
- `RegexNode` - Base pattern node class
- `RegexQuantifier` - Quantifier factory
- `RegexMetaChars` - Character class constants
- `CommonPatterns` - Pre-built patterns
- `SubstitutionBuilder` - Replacement patterns

#### 3.3 Building Patterns

##### 3.3.1 Fluent API (PatternBuilder)

**Entry Point**:

```csharp
var builder = RegexBuilder.Pattern();
```

**Pattern Methods**:

- `Literal(string)` - Literal text
- `Digits(min?, max?)` - Digit patterns
- `Letters(min?, max?)` - Letter patterns
- `Whitespace(min?, max?)` - Whitespace
- `WordCharacter(min?, max?)` - Word characters
- `AnyCharacter(min?, max?)` - Any character
- `CharacterSet(charset, min?, max?)` - Custom character set

**Anchor Methods**:

- `Start()` - Line/string start
- `End()` - Line/string end

**Grouping Methods**:

- `Group(action)` - Capturing group
- `NonCapturingGroup(action)` - Non-capturing group

**Alternation Methods**:

- `Or(action)` - Alternative pattern
- `Or(node)` - Alternative with RegexNode

**Utility Methods**:

- `Optional(action)` - Optional pattern
- `Email()` - Email pattern
- `Url()` - URL pattern
- `Pattern(node)` - Custom node

**Building**:

- `Build()` - Returns RegexNode
- Then use `RegexBuilder.Build(node)` for Regex object

##### 3.3.2 Classic API (RegexBuilder)

**Entry Point**:

```csharp
var regex = RegexBuilder.Build(components...);
```

**Literal and Escaping**:

- `Literal(string)` - Escaped literal text
- `NonEscapedLiteral(string)` - Raw regex pattern
- `MetaCharacter(char, quantifier?)` - Character classes

**Character Classes**:

- `Digit(quantifier?)` - `\d`
- `NonDigit(quantifier?)` - `\D`
- `Whitespace(quantifier?)` - `\s`
- `NonWhitespace(quantifier?)` - `\S`
- `WordCharacter(quantifier?)` - `\w`
- `NonWordCharacter(quantifier?)` - `\W`
- `CharacterSet(chars, quantifier?)` - `[...]`
- `NegativeCharacterSet(chars, quantifier?)` - `[^...]`
- `CharacterRange(from, to, quantifier?)` - `[a-z]`

**Anchors**:

- `LineStart()` - `^`
- `LineEnd()` - `$`
- `StringStart()` - `\A`
- `StringEnd()` - `\Z`
- `StringEndAbsolute()` - `\z`
- `WordBoundary(quantifier?)` - `\b`
- `NonWordBoundary(quantifier?)` - `\B`

**Grouping and Capturing**:

- `Group(name, expr, quantifier?)` - Named capturing group
- `Group(expr, quantifier?)` - Numbered capturing group
- `GroupApostrophe(name, expr, quantifier?)` - Apostrophe syntax
- `NonCapturingGroup(expr, quantifier?)` - Non-capturing group
- `BalancingGroup(push, pop, expr, quantifier?)` - Balancing group
- `SimpleBalancingGroup(name, expr, quantifier?)` - Simple balancing

**Backreferences**:

- `GroupReference(name)` - Reference named group
- `GroupReference(index)` - Reference numbered group

**Lookaround Assertions**:

- `PositiveLookAhead(expr)` - `(?=...)`
- `NegativeLookAhead(expr)` - `(?!...)`
- `PositiveLookBehind(expr)` - `(?<=...)`
- `NegativeLookBehind(expr)` - `(?<!...)`

**Alternation**:

- `Alternate(nodes...)` - `(a|b|c)`

**Concatenation**:

- `Concatenate(nodes...)` - Sequence of patterns

**Inline Options**:

- `InlineOption(options)` - `(?imnsx-imnsx)`
- `InlineOptionGrouping(enabled, disabled?, expr)` - `(?i:expr)`

**Comments**:

- `Comment(text)` - `(?#comment)`

**Unicode**:

- `UnicodeCategory(name, quantifier?)` - `\p{...}`
- `NegativeUnicodeCategory(name, quantifier?)` - `\P{...}`

**Special Escapes**:

- `BellCharacter(quantifier?)` - `\a`
- `FormFeed(quantifier?)` - `\f`
- `VerticalTab(quantifier?)` - `\v`
- `EscapeCharacter(quantifier?)` - `\e`
- `OctalCharacter(value, quantifier?)` - `\NNN`

#### 3.4 Quantifiers

**RegexQuantifier Properties**:

- `ZeroOrMore` - `*`
- `OneOrMore` - `+`
- `Optional` - `?`
- `Exactly(n)` - `{n}`
- `AtLeast(n)` - `{n,}`
- `Range(min, max)` - `{min,max}`

**Lazy Quantifiers**:

```csharp
quantifier.Lazy = true;  // Adds ? suffix
```

#### 3.5 Substitution Patterns

**SubstitutionBuilder Methods**:

- `Group(name)` - `${name}`
- `Group(index)` - `$1`
- `WholeMatch()` - `$&`
- `BeforeMatch()` - `` $` ``
- `AfterMatch()` - `$'`
- `LastCapturedGroup()` - `$+`
- `EntireInput()` - `$_`
- `LiteralDollar()` - `$$`
- `Literal(text)` - Escaped literal text

**Usage**:

```csharp
var replacement = SubstitutionBuilder.Build(
    SubstitutionBuilder.Group("word2"),
    SubstitutionBuilder.Literal(" "),
    SubstitutionBuilder.Group("word1")
);

string result = regex.Replace(input, replacement);
```

#### 3.6 Advanced Patterns

##### 3.6.1 Balancing Groups

**Purpose**: Match nested/balanced constructs (parentheses, XML tags, brackets)

**Syntax**:

- `(?<push-pop>expr)` - Push to "push", pop from "pop"
- `(?<name>-expr)` - Push to "name" only

**Example**: Balanced Parentheses

```csharp
var pattern = RegexBuilder.Build(
    RegexBuilder.Literal("("),
    RegexBuilder.BalancingGroup("depth", "depth",
        RegexBuilder.CharacterSet("^()", RegexQuantifier.ZeroOrMore)
    ),
    RegexBuilder.Literal(")")
);
// Matches: "()", "(text)", "((nested))"
```

##### 3.6.2 Conditional Matching

**Syntax**: `(?(condition)yes|no)`

**Example**:

```csharp
var pattern = RegexBuilder.ConditionalMatch(
    conditionExpression: RegexBuilder.GroupReference("prefix"),
    trueExpression: RegexBuilder.Literal("-suffix"),
    falseExpression: RegexBuilder.Literal("_suffix")
);
```

##### 3.6.3 Inline Option Grouping

**Purpose**: Apply regex options to specific sub-expressions

**Example**:

```csharp
// Case-insensitive match for specific part only
var pattern = RegexBuilder.Build(
    RegexBuilder.Literal("ID:"),
    RegexBuilder.InlineOptionGrouping(
        RegexOptions.IgnoreCase,
        RegexBuilder.Literal("abc")
    ),
    RegexBuilder.Digits(3)
);
// Matches: "ID:abc123", "ID:ABC123", "ID:AbC123"
```

##### 3.6.4 Unicode Category Matching

**Purpose**: Match characters by Unicode category

**Examples**:

```csharp
// Match any letter (any language)
var letters = RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore);

// Match Cyrillic text
var cyrillic = RegexBuilder.UnicodeCategory("IsCyrillic", RegexQuantifier.OneOrMore);

// Match non-Latin characters
var nonLatin = RegexBuilder.NegativeUnicodeCategory("IsLatin1Supplement");
```

**Common Categories**:

- `L` - All letters
- `Lu` - Uppercase letters
- `Ll` - Lowercase letters
- `N` - All numbers
- `Nd` - Decimal digits
- `P` - All punctuation
- `IsCyrillic` - Cyrillic block
- `IsArabic` - Arabic block
- See [Unicode Categories Reference](http://www.unicode.org/reports/tr44/#General_Category_Values)

#### 3.7 Pattern Composition

**Combining Patterns**:

```csharp
// Build complex patterns from simpler ones
var idPattern = RegexBuilder.Pattern()
    .Literal("ID-")
    .Digits(3)
    .Build();

var codePattern = RegexBuilder.Pattern()
    .Literal("CODE-")
    .Letters(2, 4)
    .Build();

var combined = RegexBuilder.Build(
    RegexBuilder.Alternate(idPattern, codePattern)
);
// Matches: "ID-123" or "CODE-AB"
```

**Reusable Components**:

```csharp
public class MyPatterns
{
    public static RegexNode Identifier()
    {
        return RegexBuilder.Pattern()
            .Letters(1)
            .WordCharacter(0, null)
            .Build();
    }

    public static RegexNode Version()
    {
        return RegexBuilder.Pattern()
            .Digits(1, 2)
            .Literal(".")
            .Digits(1, 2)
            .Build();
    }
}

// Use in larger patterns
var packagePattern = RegexBuilder.Build(
    MyPatterns.Identifier(),
    RegexBuilder.Literal("@"),
    MyPatterns.Version()
);
// Matches: "package@1.2", "myLib@10.5"
```

#### 3.8 Common Patterns Reference

**Quick Access**:

```csharp
using RegexBuilder;

// Email
var emailRegex = RegexBuilder.Build(CommonPatterns.Email());

// URL
var urlRegex = RegexBuilder.Build(CommonPatterns.Url());
```

See [Common Patterns Library](common-patterns.md) for complete list.

#### 3.9 Performance Considerations

**Construction Overhead**:

- RegexBuilder adds modest overhead when constructing the pattern
- Once built, performance is identical to hand-written regex
- Consider caching constructed Regex objects for repeated use

**Best Practices**:

```csharp
// ✓ Good: Cache the compiled regex
private static readonly Regex EmailRegex = RegexBuilder.Build(
    RegexOptions.Compiled,
    CommonPatterns.Email()
);

public bool ValidateEmail(string input) => EmailRegex.IsMatch(input);

// ✗ Avoid: Rebuilding on every call
public bool ValidateEmail(string input)
{
    var regex = RegexBuilder.Build(CommonPatterns.Email()); // Overhead!
    return regex.IsMatch(input);
}
```

**Compilation Options**:

- Use `RegexOptions.Compiled` for frequently used patterns
- Consider `RegexOptions.IgnoreCase` for case-insensitive matching
- Combine options: `RegexOptions.Compiled | RegexOptions.IgnoreCase`

#### 3.10 Syntax Mapping Reference

**Quick lookup: Traditional Regex → RegexBuilder**

| Traditional     | RegexBuilder (Classic)                  | RegexBuilder (Fluent)           |
| --------------- | --------------------------------------- | ------------------------------- |
| `^`             | `LineStart()`                           | `.Start()`                      |
| `$`             | `LineEnd()`                             | `.End()`                        |
| `\d`            | `Digit()`                               | `.Digits()`                     |
| `\d+`           | `Digit(RegexQuantifier.OneOrMore)`      | `.Digits(1, null)`              |
| `\d{3}`         | `Digit(RegexQuantifier.Exactly(3))`     | `.Digits(3, 3)`                 |
| `\w`            | `WordCharacter()`                       | `.WordCharacter()`              |
| `\s`            | `Whitespace()`                          | `.Whitespace()`                 |
| `[a-z]`         | `CharacterRange('a', 'z')`              | `.CharacterSet("a-z")`          |
| `[^a-z]`        | `NegativeCharacterSet("a-z")`           | N/A (use classic)               |
| `(expr)`        | `Group(expr)`                           | `.Group(g => g...)`             |
| `(?:expr)`      | `NonCapturingGroup(expr)`               | `.NonCapturingGroup(g => g...)` |
| `(?<name>expr)` | `Group("name", expr)`                   | N/A (use classic)               |
| `\1`            | `GroupReference(1)`                     | N/A (use classic)               |
| `\k<name>`      | `GroupReference("name")`                | N/A (use classic)               |
| `(a\|b)`        | `Alternate(Literal("a"), Literal("b"))` | `.Or(o => o.Literal("b"))`      |
| `(?=expr)`      | `PositiveLookAhead(expr)`               | N/A (use classic)               |
| `(?!expr)`      | `NegativeLookAhead(expr)`               | N/A (use classic)               |
| `(?<=expr)`     | `PositiveLookBehind(expr)`              | N/A (use classic)               |
| `(?<!expr)`     | `NegativeLookBehind(expr)`              | N/A (use classic)               |
| `(?i:expr)`     | `InlineOptionGrouping(...)`             | N/A (use classic)               |
| `\p{L}`         | `UnicodeCategory("L")`                  | N/A (use classic)               |
| `literal\.text` | `Literal("literal.text")`               | `.Literal("literal.text")`      |

#### 3.11 Common Patterns and Recipes

**Email with TLD validation**:

```csharp
var pattern = RegexBuilder.Pattern()
    .Start()
    .CharacterSet("a-zA-Z0-9._%+-", 1, null)
    .Literal("@")
    .CharacterSet("a-zA-Z0-9.-", 1, null)
    .Literal(".")
    .Letters(2, 6)
    .End()
    .Build();
```

**Phone number (flexible format)**:

```csharp
var pattern = RegexBuilder.Pattern()
    .Optional(o => o.Literal("+1").Optional(sep => sep.CharacterSet("- ")))
    .Group(g => g.Digits(3))
    .Optional(o => o.CharacterSet("- "))
    .Group(g => g.Digits(3))
    .Optional(o => o.CharacterSet("- "))
    .Group(g => g.Digits(4))
    .Build();
```

**Extract URL components**:

```csharp
var pattern = RegexBuilder.Build(
    RegexBuilder.Group("protocol",
        RegexBuilder.CharacterSet("a-z", RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.Literal("://"),
    RegexBuilder.Group("domain",
        RegexBuilder.CharacterSet("a-zA-Z0-9.-", RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.Group("path",
        RegexBuilder.CharacterSet("/a-zA-Z0-9._~:?#@!$&'()*+,;=%-", RegexQuantifier.ZeroOrMore)
    )
);

var match = pattern.Match("https://example.com/path");
Console.WriteLine(match.Groups["protocol"].Value);  // https
Console.WriteLine(match.Groups["domain"].Value);    // example.com
Console.WriteLine(match.Groups["path"].Value);      // /path
```

#### 3.12 Troubleshooting

**Common Issues**:

1. **Pattern doesn't match expected input**
   - Use online regex testers (regex101.com) with generated pattern
   - Check for proper escaping of special characters
   - Verify quantifiers are applied correctly

2. **Performance is slow**
   - Cache compiled Regex objects
   - Use `RegexOptions.Compiled` for hot paths
   - Avoid catastrophic backtracking (test with pathological inputs)

3. **Pattern is too verbose**
   - Use Fluent API for simpler syntax
   - Extract reusable components
   - Consider if hand-written regex is better for simple cases

4. **Can't figure out which method to use**
   - Check Syntax Mapping Reference (section 3.10)
   - Look at examples in Common Patterns Library
   - Use IDE IntelliSense for method discovery

**Getting Help**:

- GitHub Issues: [https://github.com/somenoe/RegexBuilder.NET9/issues](https://github.com/somenoe/RegexBuilder.NET9/issues)
- API Documentation: Generated from XML comments
- Example Code: [CustomRegexTests.cs](https://github.com/somenoe/RegexBuilder.NET9/blob/master/src/RegexBuilder.Tests/CustomRegexTests.cs)

#### 3.13 Migration from Original regex-builder

**Key Differences**:

- .NET 9 support
- New Fluent API (PatternBuilder)
- CommonPatterns library
- Unicode category support
- Balancing groups
- Inline option grouping
- Substitution patterns

**Migration Steps**:

1. Update package reference to `RegexBuilder.NET9`
2. No breaking changes in classic API
3. Consider adopting Fluent API for new code
4. Use CommonPatterns for standard validation

## Implementation Plan

### Phase 1: Content Creation (Days 1-2)

1. ✅ Create history/essential_documentation_plan.md
2. Write getting-started.md content
3. Write common-patterns.md content
4. Write api-guide.md content
5. Update introduction.md with project overview

### Phase 2: DocFX Integration (Day 3)

1. Update docs/docs/toc.yml with new pages
2. Update docs/toc.yml (root) to ensure proper navigation
3. Verify docfx.json configuration
4. Test local build with `docfx docs/docfx.json --serve`

### Phase 3: Validation (Day 3)

1. Build documentation locally
2. Review all navigation links
3. Test all code examples
4. Proofread for clarity and accuracy
5. Verify mobile responsiveness

### Phase 4: Finalization (Day 3)

1. Update CHANGELOG.md with documentation improvements
2. Commit with proper message format
3. Deploy to GitHub Pages
4. Verify live documentation

## Success Criteria

### Content Quality

- [ ] All code examples compile and run correctly
- [ ] Every pattern has "matches" and "doesn't match" examples
- [ ] Navigation flows logically between pages
- [ ] No broken links
- [ ] Consistent formatting and style

### User Experience

- [ ] New user can get started in < 5 minutes
- [ ] Common use cases have copy-paste ready solutions
- [ ] Advanced users can find complete API reference
- [ ] Mobile-friendly rendering
- [ ] Fast page load times

### Completeness

- [ ] getting-started.md covers installation and first examples
- [ ] common-patterns.md has 10+ production-ready patterns
- [ ] api-guide.md documents all public APIs
- [ ] introduction.md explains project value proposition
- [ ] All docfx navigation files updated

## Maintenance Strategy

### Regular Updates

- Add new patterns to common-patterns.md as requested
- Update API guide when new features are added
- Keep version numbers current

### User Feedback Integration

- Monitor GitHub issues for documentation requests
- Track which pages are most visited
- Iterate based on user confusion points

### Quality Control

- Test all examples with each major release
- Verify links quarterly
- Update screenshots if UI changes

## Version

This documentation plan will be implemented for version 1.1.1+.

## Files to Create/Modify

### New Files

1. `docs/docs/getting-started.md` (comprehensive rewrite)
2. `docs/docs/common-patterns.md` (new file)
3. `docs/docs/api-guide.md` (new file)

### Updated Files

1. `docs/docs/introduction.md` (add content)
2. `docs/docs/toc.yml` (update navigation)
3. `docs/toc.yml` (verify root navigation)
4. `CHANGELOG.md` (document additions)

### Cleanup

- Remove empty folders: `docs/docs/advanced/`, `docs/docs/concepts/`, `docs/docs/examples/`, `docs/docs/tutorials/`
- Keep folder structure simple and flat
