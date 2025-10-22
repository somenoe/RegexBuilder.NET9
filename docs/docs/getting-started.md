# Getting Started

Welcome to RegexBuilder.NET9! This guide will help you create your first human-readable regular expressions in just 5 minutes.

## Why RegexBuilder?

Traditional regex patterns like `(?:https?://)?(?:[a-z0-9.-]+)(?::[0-9]+)?` are powerful but cryptic. RegexBuilder lets you write the same pattern as readable, maintainable C# code:

```csharp
var pattern = RegexBuilder.Pattern()
    .Optional(o => o.Literal("http").Optional(s => s.Literal("s")).Literal("://"))
    .CharacterSet("a-z0-9.-", 1, null)
    .Optional(o => o.Literal(":").Digits(1, null))
    .Build();
```

## Installation

Install RegexBuilder.NET9 from NuGet:

### Using .NET CLI

```bash
dotnet add package RegexBuilder.NET9 --version 1.1.1
```

### Using PackageReference in .csproj

```xml
<PackageReference Include="RegexBuilder.NET9" Version="1.1.1" />
```

### Requirements

- .NET 9.0 or later

## Your First Pattern (Fluent API)

Let's build a simple email validator using the modern **Fluent API** (recommended for most use cases):

```csharp
using RegexBuilder;

// Build an email validation pattern
var emailPattern = RegexBuilder.Pattern()
    .Start()                                    // ^ anchor
    .CharacterSet("a-zA-Z0-9._%+-", 1, null)   // local part: one or more allowed chars
    .Literal("@")                               // @ symbol
    .CharacterSet("a-zA-Z0-9.-", 1, null)      // domain: one or more allowed chars
    .Literal(".")                               // literal dot
    .Letters(2, 6)                              // TLD: 2-6 letters
    .End()                                      // $ anchor
    .Build();

// Convert to a compiled Regex
var emailRegex = RegexBuilder.Build(emailPattern);

// Test it
Console.WriteLine(emailRegex.IsMatch("user@example.com"));    // True
Console.WriteLine(emailRegex.IsMatch("invalid.email"));       // False
Console.WriteLine(emailRegex.IsMatch("test@domain.co.uk"));   // False (needs adjustment for multiple dots)
```

### How It Works

1. **`RegexBuilder.Pattern()`** - Creates a new fluent pattern builder
2. **`.Start()`** - Adds the `^` anchor (start of line)
3. **`.CharacterSet(...)`** - Creates a character class `[...]`
4. **`.Literal(...)`** - Adds literal text (auto-escaped)
5. **`.Letters(min, max)`** - Adds `[a-zA-Z]` with quantifiers
6. **`.End()`** - Adds the `$` anchor (end of line)
7. **`.Build()`** - Converts to a `RegexNode`
8. **`RegexBuilder.Build(node)`** - Compiles to a .NET `Regex` object

### Generated Pattern

The code above generates this regex pattern:

```
^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$
```

Much more readable in code form!

## Your First Pattern (Classic API)

The same email pattern using the **Classic API** (more verbose but offers advanced features):

```csharp
using RegexBuilder;

var emailRegex = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.CharacterSet("a-zA-Z0-9._%+-", RegexQuantifier.OneOrMore),
    RegexBuilder.Literal("@"),
    RegexBuilder.CharacterSet("a-zA-Z0-9.-", RegexQuantifier.OneOrMore),
    RegexBuilder.Literal("."),
    RegexBuilder.CharacterSet("a-zA-Z", new RegexQuantifier(2, 6)),
    RegexBuilder.LineEnd()
);

// Test it
Console.WriteLine(emailRegex.IsMatch("user@example.com"));    // True
Console.WriteLine(emailRegex.IsMatch("invalid.email"));       // False
```

### When to Use Each API

| Fluent API                 | Classic API                           |
| -------------------------- | ------------------------------------- |
| ✓ Simpler, more intuitive  | ✓ Full access to advanced features    |
| ✓ Chainable methods        | ✓ Backreferences and group references |
| ✓ Best for common patterns | ✓ Lookaround assertions               |
| ✓ Less verbose             | ✓ Conditional matching                |
|                            | ✓ Unicode categories                  |
|                            | ✓ Inline option grouping              |

**Recommendation**: Start with the Fluent API. Switch to Classic API when you need advanced features.

## Common Patterns Library

Don't want to build patterns from scratch? Use the **CommonPatterns** library for instant productivity:

```csharp
using RegexBuilder;

// Email validation (pre-built)
var emailRegex = RegexBuilder.Build(CommonPatterns.Email());

// URL validation (pre-built)
var urlRegex = RegexBuilder.Build(CommonPatterns.Url());

// Test them
Console.WriteLine(emailRegex.IsMatch("user@example.com"));           // True
Console.WriteLine(urlRegex.IsMatch("https://github.com/example"));   // True
```

See the [Common Patterns Library](common-patterns.md) for a complete list of pre-built patterns.

## More Examples

### Phone Number Pattern

```csharp
// Matches: 555-123-4567, +1-555-123-4567, 5551234567
var phonePattern = RegexBuilder.Pattern()
    .Optional(o => o.Literal("+1").Optional(sep => sep.CharacterSet("- ")))
    .Digits(3, 3)                          // Area code
    .Optional(o => o.CharacterSet("- "))
    .Digits(3, 3)                          // Prefix
    .Optional(o => o.CharacterSet("- "))
    .Digits(4, 4)                          // Line number
    .Build();

var phoneRegex = RegexBuilder.Build(phonePattern);
Console.WriteLine(phoneRegex.IsMatch("555-123-4567"));      // True
Console.WriteLine(phoneRegex.IsMatch("+1-555-123-4567"));   // True
Console.WriteLine(phoneRegex.IsMatch("5551234567"));        // True
```

### ID Pattern with Alternation

```csharp
// Matches: ID-123 or CODE-AB
var idPattern = RegexBuilder.Pattern()
    .Start()
    .Literal("ID-")
    .Digits(3, 5)
    .Or(o => o.Literal("CODE-").Letters(2, 4))
    .End()
    .Build();

var idRegex = RegexBuilder.Build(idPattern);
Console.WriteLine(idRegex.IsMatch("ID-123"));       // True
Console.WriteLine(idRegex.IsMatch("CODE-AB"));      // True
Console.WriteLine(idRegex.IsMatch("INVALID"));      // False
```

### Capturing Groups

```csharp
// Extract parts of a version string
var versionPattern = RegexBuilder.Build(
    RegexBuilder.Group("major", RegexBuilder.Digit(RegexQuantifier.OneOrMore)),
    RegexBuilder.Literal("."),
    RegexBuilder.Group("minor", RegexBuilder.Digit(RegexQuantifier.OneOrMore)),
    RegexBuilder.Literal("."),
    RegexBuilder.Group("patch", RegexBuilder.Digit(RegexQuantifier.OneOrMore))
);

var match = versionPattern.Match("1.2.3");
if (match.Success)
{
    Console.WriteLine($"Major: {match.Groups["major"].Value}");    // Major: 1
    Console.WriteLine($"Minor: {match.Groups["minor"].Value}");    // Minor: 2
    Console.WriteLine($"Patch: {match.Groups["patch"].Value}");    // Patch: 3
}
```

## Basic Concepts

### Pattern Composition

RegexBuilder treats patterns as composable building blocks. You can:

1. **Build small patterns** and combine them
2. **Reuse patterns** across multiple regex expressions
3. **Test patterns independently** before combining

```csharp
// Build reusable components
var digitPart = RegexBuilder.Pattern().Digits(3).Build();
var separator = RegexBuilder.Literal("-");

// Combine them
var ssn = RegexBuilder.Build(
    digitPart,
    separator,
    RegexBuilder.Pattern().Digits(2).Build(),
    separator,
    RegexBuilder.Pattern().Digits(4).Build()
);
// Matches: 123-45-6789
```

### Quantifiers

Control how many times a pattern repeats:

```csharp
// Fluent API quantifiers
.Digits(3, 3)        // Exactly 3 digits: \d{3}
.Digits(1, null)     // One or more digits: \d+
.Digits(0, null)     // Zero or more digits: \d*
.Digits(2, 5)        // 2 to 5 digits: \d{2,5}

// Classic API quantifiers
RegexQuantifier.Exactly(3)        // {3}
RegexQuantifier.OneOrMore         // +
RegexQuantifier.ZeroOrMore        // *
RegexQuantifier.Optional          // ?
RegexQuantifier.Range(2, 5)       // {2,5}
RegexQuantifier.AtLeast(2)        // {2,}
```

### Anchors

Ensure patterns match at specific positions:

```csharp
// Fluent API
.Start()             // ^ - Start of line
.End()               // $ - End of line

// Classic API
RegexBuilder.LineStart()              // ^
RegexBuilder.LineEnd()                // $
RegexBuilder.StringStart()            // \A
RegexBuilder.StringEnd()              // \Z
RegexBuilder.WordBoundary()           // \b
```

### Character Classes

Match specific types of characters:

```csharp
// Fluent API
.Digits()            // \d
.Letters()           // [a-zA-Z]
.Whitespace()        // \s
.WordCharacter()     // \w
.AnyCharacter()      // .
.CharacterSet("abc") // [abc]

// Classic API
RegexBuilder.Digit()                    // \d
RegexBuilder.Whitespace()               // \s
RegexBuilder.WordCharacter()            // \w
RegexBuilder.CharacterSet("a-z")        // [a-z]
RegexBuilder.NegativeCharacterSet("a-z") // [^a-z]
```

## Performance Tips

### Cache Compiled Regex Objects

```csharp
// ✓ Good: Cache and reuse
private static readonly Regex EmailRegex = RegexBuilder.Build(
    RegexOptions.Compiled,
    CommonPatterns.Email()
);

public bool ValidateEmail(string input) => EmailRegex.IsMatch(input);

// ✗ Avoid: Rebuilding on every call
public bool ValidateEmail(string input)
{
    var regex = RegexBuilder.Build(CommonPatterns.Email());  // Overhead!
    return regex.IsMatch(input);
}
```

### Use RegexOptions.Compiled for Hot Paths

```csharp
// For frequently-used patterns, use Compiled option
var regex = RegexBuilder.Build(
    RegexOptions.Compiled | RegexOptions.IgnoreCase,
    CommonPatterns.Email()
);
```

## Next Steps

Now that you understand the basics, explore more advanced features:

- **[Common Patterns Library](common-patterns.md)** - Pre-built patterns for email, URL, phone, and more
- **[API Guide](api-guide.md)** - Complete reference for all RegexBuilder capabilities
- **[GitHub Examples](https://github.com/somenoe/RegexBuilder.NET9/tree/master/src/RegexBuilder.Tests)** - Real-world examples and test cases

### Advanced Features to Explore

- **Lookaround Assertions** - Match patterns based on what comes before/after
- **Backreferences** - Reference previously captured groups
- **Balancing Groups** - Match nested structures (parentheses, XML tags)
- **Unicode Categories** - Match international characters
- **Substitution Patterns** - Advanced `Regex.Replace()` operations

## Getting Help

- **GitHub Issues**: [Report bugs or request features](https://github.com/somenoe/RegexBuilder.NET9/issues)
- **Example Code**: Check out [CustomRegexTests.cs](https://github.com/somenoe/RegexBuilder.NET9/blob/master/src/RegexBuilder.Tests/CustomRegexTests.cs)
- **API Documentation**: Explore the auto-generated API reference

Happy pattern building! 🎉
