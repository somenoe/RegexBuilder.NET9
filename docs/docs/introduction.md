# Introduction

## Welcome to RegexBuilder.NET9

RegexBuilder.NET9 is a C# library that lets you build .NET regular expressions using human-readable, maintainable code instead of cryptic regex syntax.

## The Problem

Traditional regex patterns are powerful but notoriously difficult to read and maintain:

`^(?:https?://)?(?:[a-z0-9.-]+)(?::[0-9]+)?(?:/[^?\s]*)?(?:\?[^\s]*)?$`

Quick—can you tell what this pattern matches? How about explaining it to a colleague?

## The Solution

With RegexBuilder, write the same pattern as self-documenting C# code:

```csharp
var urlPattern = RegexBuilder.Pattern()
    .Start()
    .Optional(o => o
        .Literal("http")
        .Optional(s => s.Literal("s"))
        .Literal("://")
    )
    .CharacterSet("a-z0-9.-", 1, null)
    .Optional(o => o.Literal(":").Digits(1, null))
    .Optional(o => o.Literal("/").CharacterSet("^?\\s", 0, null))
    .Optional(o => o.Literal("?").CharacterSet("^\\s", 0, null))
    .End()
    .Build();

var urlRegex = RegexBuilder.Build(urlPattern);
```

Now it's clear: this matches a URL with optional protocol, optional port, optional path, and optional query string.

## When to Use RegexBuilder

RegexBuilder is ideal when:

- ✅ **Complexity matters**: Your regex patterns are complex enough that readability is important
- ✅ **Team maintenance**: Multiple developers need to understand and modify patterns
- ✅ **Dynamic patterns**: You're building patterns programmatically with conditional logic
- ✅ **Clarity over brevity**: You value self-documenting code

RegexBuilder may not be necessary when:

- ❌ **Simple patterns**: Trivial patterns like `\d{3}` are fine as-is
- ❌ **One-time use**: Quick throwaway scripts where you won't revisit the code
- ❌ **Performance-critical**: Extreme performance requirements where construction overhead matters (though runtime is identical)

## Key Features

### 🎯 Two APIs for Different Needs

**Fluent API (Modern)**: Chainable, intuitive, perfect for common patterns

```csharp
RegexBuilder.Pattern()
    .Start()
    .Email()
    .End()
    .Build();
```

**Classic API (Powerful)**: Full access to advanced regex features

```csharp
RegexBuilder.Build(
    RegexBuilder.PositiveLookAhead(RegexBuilder.Digit()),
    RegexBuilder.BalancingGroup("depth", "depth", expr)
);
```

### 📚 Common Patterns Library

Pre-built, production-ready patterns for instant productivity:

```csharp
var emailRegex = RegexBuilder.Build(CommonPatterns.Email());
var urlRegex = RegexBuilder.Build(CommonPatterns.Url());
```

### 🔧 Advanced Features

- **Balancing Groups**: Match nested structures (parentheses, XML tags)
- **Unicode Categories**: Match international text by character category
- **Lookaround Assertions**: Lookahead and lookbehind patterns
- **Substitution Patterns**: Advanced `Regex.Replace()` operations
- **Inline Options**: Apply options to specific sub-expressions

### ✨ Developer Experience

- **IntelliSense Support**: Full code completion and documentation
- **Compile-Time Safety**: Catch errors before runtime
- **Composable Patterns**: Build complex patterns from reusable components
- **Automatic Escaping**: No more forgetting to escape special characters

## How It Works

RegexBuilder uses the **Builder Pattern** to construct regex patterns:

1. **Build patterns** using fluent methods or static factory methods
2. **Compose patterns** by combining smaller patterns into larger ones
3. **Compile to Regex** - Generate a standard .NET `Regex` object
4. **Identical runtime performance** - No overhead after construction

```csharp
// 1. Build a pattern
var phonePattern = RegexBuilder.Pattern()
    .Digits(3)
    .Literal("-")
    .Digits(3)
    .Literal("-")
    .Digits(4)
    .Build();

// 2. Compile to Regex
var phoneRegex = RegexBuilder.Build(
    RegexOptions.Compiled,  // Optional: compile for performance
    phonePattern
);

// 3. Use like any .NET Regex
bool isValid = phoneRegex.IsMatch("555-123-4567");
```

## Project History

RegexBuilder.NET9 is a fork of the original [regex-builder](https://github.com/YuriyGuts/regex-builder) by Yuriy Guts, updated and enhanced for .NET 9 with:

- ✨ .NET 9 support
- ✨ Fluent API (PatternBuilder)
- ✨ Common Patterns Library
- ✨ Unicode category matching
- ✨ Balancing groups
- ✨ Inline option grouping
- ✨ Substitution patterns
- ✨ Enhanced convenience methods

## Quick Start

Ready to get started? Jump right in:

1. **[Getting Started](getting-started.md)** - Installation and first examples (5 minutes)
2. **[Common Patterns](common-patterns.md)** - Copy-paste ready patterns for common needs
3. **[API Guide](api-guide.md)** - Complete reference for all features

## Installation

Install via NuGet:

```bash
dotnet add package RegexBuilder.NET9 --version 1.1.1
```

Or add to your `.csproj`:

```xml
<PackageReference Include="RegexBuilder.NET9" Version="1.1.1" />
```

## Quick Example

Let's validate an email address:

```csharp
using RegexBuilder;

// Build the pattern
var emailPattern = RegexBuilder.Pattern()
    .Start()
    .CharacterSet("a-zA-Z0-9._%+-", 1, null)
    .Literal("@")
    .CharacterSet("a-zA-Z0-9.-", 1, null)
    .Literal(".")
    .Letters(2, 6)
    .End()
    .Build();

// Compile to Regex
var emailRegex = RegexBuilder.Build(emailPattern);

// Use it
Console.WriteLine(emailRegex.IsMatch("user@example.com"));  // True
Console.WriteLine(emailRegex.IsMatch("invalid.email"));     // False
```

## Philosophy

RegexBuilder follows these principles:

### Readability First

Code is read far more often than it's written. RegexBuilder optimizes for clarity:

```csharp
// Traditional: What does this do?
new Regex(@"(?<=\$)\d+\.\d{2}")

// RegexBuilder: Crystal clear
RegexBuilder.Build(
    RegexBuilder.PositiveLookBehind(RegexBuilder.Literal("$")),
    RegexBuilder.Digit(RegexQuantifier.OneOrMore),
    RegexBuilder.Literal("."),
    RegexBuilder.Digit(RegexQuantifier.Exactly(2))
)
// Matches: Dollar amount after $ sign (e.g., "5.99" in "$5.99")
```

### Composability

Build complex patterns from simple, reusable components:

```csharp
// Define reusable components
var areaCode = RegexBuilder.Pattern().Digits(3).Build();
var exchange = RegexBuilder.Pattern().Digits(3).Build();
var lineNumber = RegexBuilder.Pattern().Digits(4).Build();

// Compose them
var phonePattern = RegexBuilder.Build(
    areaCode,
    RegexBuilder.Literal("-"),
    exchange,
    RegexBuilder.Literal("-"),
    lineNumber
);
```

### Safety

Let the compiler catch errors before runtime:

```csharp
// Compile-time error: Method doesn't exist
.Dgits(3)  // Typo caught by IDE!

// Automatic escaping prevents subtle bugs
.Literal("price: $5.99")  // Correctly escapes special chars
```

## Community and Support

- **GitHub**: [somenoe/RegexBuilder.NET9](https://github.com/somenoe/RegexBuilder.NET9)
- **Issues**: [Report bugs or request features](https://github.com/somenoe/RegexBuilder.NET9/issues)
- **Examples**: [See real-world examples](https://github.com/somenoe/RegexBuilder.NET9/tree/master/src/RegexBuilder.Tests)

## License

This project is licensed under the **MIT License**. See [LICENSE](https://github.com/somenoe/RegexBuilder.NET9/blob/master/LICENSE) for details.

---

Ready to write better regex? **[Get Started →](getting-started.md)**
