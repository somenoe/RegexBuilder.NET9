# RegexBuilder.NET9

C# library for building .NET regular expressions with human-readable code.

> **Note:** This is a fork of the original [regex-builder](https://github.com/YuriyGuts/regex-builder) by Yuriy Guts, updated to support .NET 9.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Real-World Example](#real-world-example)
- [Documentation](#documentation)
- [When to Use RegexBuilder](#when-to-use-regexbuilder)
- [Supported Features](#supported-features)
- [API Guide](#api-guide)
- [Advanced Usage](#advanced-usage)
- [Testing](#testing)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Overview

Just another day at the office—you write a .NET Regex like a boss, and suddenly realize that you need to declare a non-capturing group.
Is it `(?:pattern)`? Or `(?=pattern)` (positive lookahead)? Or `(?<=pattern)` (positive lookbehind)?

> "Aaargh! Where's that Regex cheat sheet?!"

**The Solution:** Inspired by [Expression Trees](http://msdn.microsoft.com/en-us/library/bb397951.aspx) in .NET, RegexBuilder provides a more verbose but **infinitely more human-readable** way of declaring regular expressions using fluent, chainable C# code instead of cryptic regex patterns.

## When to Use RegexBuilder

- **Complex expressions** that might be frequently changed
- **Team environments** where regex readability matters for maintainability
- **Building patterns programmatically** with conditional logic
- When **clarity is worth the modest performance cost** of constructing the Regex object

## Supported Features

The following elements are supported:

- [Quantifiers](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#quantifiers)
- [Character escapes](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#character_escapes)
- [Character classes](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#character_classes)
- [Anchors (atomic zero-width assertions)](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#atomic_zerowidth_assertions)
- [Grouping constructs](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#grouping_constructs)
- [Backreference constructs](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#backreference_constructs)
- [Alternation constructs](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#alternation_constructs)
- [Inline options and comments](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#miscellaneous_constructs)
- [Substitution patterns](http://msdn.microsoft.com/en-us/library/az24scfc.aspx#substitutions) - for use with `Regex.Replace()`

## Installation

Install RegexBuilder.NET9 from NuGet using one of the following methods:

**Via .NET CLI:**

```bash
dotnet add package RegexBuilder.NET9 --version 1.1.1
```

**Via PackageReference in .csproj:**

```xml
<PackageReference Include="RegexBuilder.NET9" Version="1.1.1" />
```

You can also view the package page on NuGet: [RegexBuilder.NET9 on NuGet](https://www.nuget.org/packages/RegexBuilder.NET9/)

## Quick Start

Here's a simple example to get you started. Let's build a regex to match email addresses:

```csharp
var emailRegex = RegexBuilder.Build(
    RegexBuilder.Group(
        "localPart",
        RegexBuilder.CharacterSet(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.Literal("@"),
    RegexBuilder.Group(
        "domain",
        RegexBuilder.Concatenate(
            RegexBuilder.CharacterSet(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore),
            RegexBuilder.Literal("."),
            RegexBuilder.CharacterSet(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)
        )
    )
);

var match = emailRegex.Match("user@example.com");
if (match.Success)
{
    Console.WriteLine(match.Groups["localPart"].Value); // user
    Console.WriteLine(match.Groups["domain"].Value);    // example.com
}
```

### Fluent API Quick Start (recommended)

Use the Fluent `PatternBuilder` for concise, chainable pattern creation. The following builds the same email validator:

```csharp
var emailPattern = RegexBuilder.Pattern()
    .Start()
    .CharacterSet("a-zA-Z0-9._%+-", 1, null)
    .Literal("@")
    .CharacterSet("a-zA-Z0-9.-", 1, null)
    .Literal(".")
    .Letters(2, 6)
    .End()
    .Build();

var emailRegex2 = RegexBuilder.Build(emailPattern);
Console.WriteLine(emailRegex2.IsMatch("user@example.com")); // True
```

## Real-World Example

Let's build a regex to capture `href` attributes from HTML hyperlinks. Compare the traditional approach:

**Traditional regex (hard to read):**

```csharp
Regex hrefRegex = new Regex(
    "href\\s*=\\s*(?:[\"'](?<Target>[^\"']*)[\"']|(?<Target>\\S+))",
    RegexOptions.IgnoreCase
);
```

**With RegexBuilder (self-documenting):**

```csharp
const string quotationMark = "\"";
Regex hrefRegex = RegexBuilder.Build(
    RegexOptions.IgnoreCase,
    RegexBuilder.Literal("href"),
    RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrMore),
    RegexBuilder.Literal("="),
    RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrMore),
    RegexBuilder.Alternate(
        RegexBuilder.Concatenate(
            RegexBuilder.NonEscapedLiteral(quotationMark),
            RegexBuilder.Group(
                "Target",
                RegexBuilder.NegativeCharacterSet(quotationMark, RegexQuantifier.ZeroOrMore)
            ),
            RegexBuilder.NonEscapedLiteral(quotationMark)
        ),
        RegexBuilder.Group(
            "Target",
            RegexBuilder.MetaCharacter(RegexMetaChars.NonwhiteSpace, RegexQuantifier.OneOrMore)
        )
    )
);
```

More examples can be found in [CustomRegexTests.cs](/src/RegexBuilder.Tests/CustomRegexTests.cs).

## Documentation

Full documentation (including Getting Started, Common Patterns, and API Guide) is available in the `docs/` folder and on the generated site.

- Local docs: `docs/` (open `docs/index.md`)
- Generated site: [https://somenoe.github.io/RegexBuilder.NET9/](https://somenoe.github.io/RegexBuilder.NET9/)

## API Guide

### Fluent Pattern Builder

RegexBuilder now includes a **Fluent Builder Pattern** for composing complex regex patterns with improved ergonomics. The `PatternBuilder` class provides a chainable API for building patterns step-by-step.

#### Quick Example

```csharp
// Build an ID pattern that matches either "ID-123" or "CODE-AB"
var pattern = RegexBuilder.Pattern()
    .Start()                          // ^ anchor
    .Literal("ID-")
    .Digits(3, 5)                     // \d{3,5}
    .Or(o => o.Literal("CODE-").Letters(2, 4))  // | CODE-[a-zA-Z]{2,4}
    .End()                            // $ anchor
    .Build();

var regex = RegexBuilder.Build(pattern);
Console.WriteLine(regex.IsMatch("ID-123"));   // True
Console.WriteLine(regex.IsMatch("CODE-AB"));  // True
```

#### PatternBuilder Methods

**Anchors:**

- `Start()` - Add start-of-line anchor `^`
- `End()` - Add end-of-line anchor `$`

**Patterns:**

- `Literal(string)` - Literal text (auto-escaped)
- `Digits(min, max)` - Digit pattern `\d` with quantifiers
- `Letters(min, max)` - Letter pattern `[a-zA-Z]` with quantifiers
- `Whitespace(min, max)` - Whitespace pattern `\s` with quantifiers
- `WordCharacter(min, max)` - Word character pattern `\w` with quantifiers
- `AnyCharacter(min, max)` - Any character pattern `.` with quantifiers
- `CharacterSet(charset, min, max)` - Custom character set with quantifiers

**Grouping:**

- `Group(action)` - Capturing group `(pattern)`
- `NonCapturingGroup(action)` - Non-capturing group `(?:pattern)`

**Operators:**

- `Or(action)` - Alternation with another pattern via builder
- `Or(node)` - Alternation with an existing RegexNode
- `Optional(action)` - Optional pattern `pattern?`

**Common Patterns:**

- `Email()` - Email pattern
- `Url()` - URL pattern
- `Pattern(node)` - Add a custom RegexNode

#### Advanced Example

```csharp
// Phone number pattern: [+1-]555-123-4567
var phonePattern = RegexBuilder.Pattern()
    .Optional(o => o.Literal("+1"))
    .Optional(o => o.Literal("-"))
    .Group(g => g.Digits(3))       // Area code
    .Optional(o => o.Literal("-"))
    .Group(g => g.Digits(3))       // Prefix
    .Optional(o => o.Literal("-"))
    .Group(g => g.Digits(4))       // Line number
    .Build();

var regex = RegexBuilder.Build(phonePattern);
Console.WriteLine(regex.IsMatch("555-123-4567"));     // True
Console.WriteLine(regex.IsMatch("+1-555-123-4567"));  // True
Console.WriteLine(regex.IsMatch("5551234567"));       // True
```

### Core Classes

RegexBuilder has 4 main classes you'll work with:

1. **`RegexBuilder`** - Factory class for building regex patterns

   - Static methods that produce and combine different parts of a regular expression
   - Entry points: `RegexBuilder.Build(...)` and `RegexBuilder.Pattern()`

2. **`PatternBuilder`** - Fluent builder for composing patterns

   - Chainable methods for building complex patterns
   - Entry point: `RegexBuilder.Pattern()`

3. **`RegexQuantifier`** - Produces quantifiers for regex parts

   - Properties like `ZeroOrMore`, `OneOrMore`, `Optional`, `ExactCount(n)`, `Range(min, max)`, etc.

4. **`RegexMetaChars`** - Named constants for character classes
   - `WordCharacter`, `NonwordCharacter`, `WhiteSpace`, `NonwhiteSpace`, etc.
   - More readable than memorizing `\w`, `\W`, `\s`, `\S`

### Basic Pattern Building

Start by calling `RegexBuilder.Build()` and pass the components you want to combine:

```csharp
// Simple pattern
var pattern = RegexBuilder.Build(
    RegexBuilder.Literal("prefix-"),
    RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore)
);
// Matches: "prefix-123", "prefix-42", etc.

// With options
var pattern = RegexBuilder.Build(
    RegexOptions.IgnoreCase,
    RegexBuilder.Literal("hello"),
    RegexBuilder.Literal(" "),
    RegexBuilder.Literal("world")
);
```

### Grouping and Capturing

```csharp
// Capturing group
var pattern = RegexBuilder.Build(
    RegexBuilder.Group("name",
        RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)
    )
);

// Non-capturing group
var pattern = RegexBuilder.Build(
    RegexBuilder.NonCapturingGroup(
        RegexBuilder.Alternate(
            RegexBuilder.Literal("cat"),
            RegexBuilder.Literal("dog")
        )
    )
);
```

## Advanced Usage

### Substitution Patterns

Use `SubstitutionBuilder` to create replacement patterns for `Regex.Replace()`:

```csharp
// Swap two words
var pattern = RegexBuilder.Build(
    RegexBuilder.Group("word1", RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)),
    RegexBuilder.Literal(" "),
    RegexBuilder.Group("word2", RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore))
);

var replacement = SubstitutionBuilder.Build(
    SubstitutionBuilder.Group("word2"),
    SubstitutionBuilder.Literal(" "),
    SubstitutionBuilder.Group("word1")
);

string result = pattern.Replace("hello world", replacement);
// result = "world hello"
```

### Substitution Methods

The `SubstitutionBuilder` class supports all .NET substitution constructs:

- `SubstitutionBuilder.Group(int)` or `Group(string)` - Reference captured groups
- `SubstitutionBuilder.WholeMatch()` - Insert entire matched text ($&)
- `SubstitutionBuilder.BeforeMatch()` - Insert text before match ($`)
- `SubstitutionBuilder.AfterMatch()` - Insert text after match ($')
- `SubstitutionBuilder.LastCapturedGroup()` - Insert last captured group ($+)
- `SubstitutionBuilder.EntireInput()` - Insert entire input string ($\_)
- `SubstitutionBuilder.LiteralDollar()` - Insert literal $ ($$)
- `SubstitutionBuilder.Literal(string)` - Insert literal text (auto-escapes $)

See [SubstitutionBuilderTests.cs](/src/RegexBuilder.Tests/SubstitutionBuilderTests.cs) for more examples.

## Testing

RegexBuilder uses MSTest for unit testing. The `RegexBuilder.Tests` project contains all unit tests.

**Run tests from Visual Studio:**

- Open the Test Explorer (Test > Test Explorer)
- Click "Run All Tests" or run specific test classes

**Run tests from command line:**

```bash
dotnet test src/RegexBuilder.slnx
```

**Test project structure:**

- `RegexBuilder.Tests/` - All unit test files
- `RegexBuilder.Examples/` - Executable examples demonstrating all README code samples

## Development

**Format code** (requires Prettier and dotnet format):

```bash
make format
# or manually:
npx prettier --write .
dotnet format src
```

**Build the project:**

```bash
dotnet build src/RegexBuilder.slnx
```

**Pack as NuGet package:**

```bash
dotnet pack src/RegexBuilder/RegexBuilder.csproj
```

## Contributing

Contributions are welcome! Please:

1. Create a feature branch from `master`
2. Add tests for any new functionality
3. Run `make format` to format code
4. Submit a pull request with a clear description

## License

The source code is licensed under [The MIT License](http://opensource.org/licenses/MIT).
