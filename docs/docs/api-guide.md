# API Guide

Complete reference for all RegexBuilder.NET9 capabilities. This guide covers both the modern **Fluent API** and the **Classic API**, along with advanced features.

## Table of Contents

- [Core Architecture](#core-architecture)
- [Fluent API (PatternBuilder)](#fluent-api-patternbuilder)
- [Classic API (RegexBuilder)](#classic-api-regexbuilder)
- [Quantifiers](#quantifiers)
- [Substitution Patterns](#substitution-patterns)
- [Advanced Features](#advanced-features)
- [Syntax Mapping Reference](#syntax-mapping-reference)
- [Troubleshooting](#troubleshooting)

---

## Core Architecture

RegexBuilder consists of several key classes:

### RegexBuilder

**Purpose**: Static factory class for building regex patterns

**Main Methods**:

- `Build(params RegexNode[])` - Builds a Regex from nodes
- `Build(RegexOptions, params RegexNode[])` - Builds with options
- `Pattern()` - Returns new PatternBuilder instance
- Static factory methods for all pattern types

### PatternBuilder

**Purpose**: Fluent builder for composing patterns with chainable methods

**Entry Point**:

```csharp
var builder = RegexBuilder.Pattern();
```

**Returns**: `RegexNode` via `Build()` method

### RegexNode

**Purpose**: Base class for all regex pattern nodes

**Inheritance Hierarchy**:

- `RegexNode` (abstract base)
  - `RegexNodeLiteral`
  - `RegexNodeCharacterSet`
  - `RegexNodeGroup`
  - `RegexNodeAlternation`
  - `RegexNodeConcatenation`
  - `RegexNodeLookAround`
  - And many more...

### RegexQuantifier

**Purpose**: Factory class for creating quantifiers

**Common Properties**:

- `ZeroOrMore` → `*`
- `OneOrMore` → `+`
- `Optional` → `?`
- `Exactly(n)` → `{n}`
- `Range(min, max)` → `{min,max}`
- `AtLeast(n)` → `{n,}`

### RegexMetaChars

**Purpose**: Named constants for character classes

**Constants**:

- `WordCharacter` → `\w`
- `NonwordCharacter` → `\W`
- `Digit` → `\d`
- `Nondigit` → `\D`
- `WhiteSpace` → `\s`
- `NonwhiteSpace` → `\S`
- `AnyCharacter` → `.`

### CommonPatterns

**Purpose**: Pre-built patterns for common scenarios

**Methods**:

- `Email()` - Email validation pattern
- `Url()` - URL validation pattern

### SubstitutionBuilder

**Purpose**: Factory class for creating replacement patterns (used with `Regex.Replace()`)

**Methods**: Group references, special references, literals

---

## Fluent API (PatternBuilder)

The modern, recommended approach for building patterns.

### Getting Started

```csharp
// Create a new builder
var builder = RegexBuilder.Pattern();

// Chain methods
var pattern = builder
    .Start()
    .Literal("prefix-")
    .Digits(3, 5)
    .End()
    .Build();

// Convert to Regex
var regex = RegexBuilder.Build(pattern);
```

### Pattern Methods

#### Literal Text

**`Literal(string text)`**

Adds literal text with automatic escaping.

```csharp
.Literal("hello.world")  // → hello\.world
.Literal("price: $5")    // → price:\ \$5
```

#### Digits

**`Digits()`** - One or more digits (`\d+`)

**`Digits(int exact)`** - Exactly N digits (`\d{n}`)

**`Digits(int? min, int? max)`** - Range of digits

```csharp
.Digits()           // \d+
.Digits(3)          // \d{3}
.Digits(2, 5)       // \d{2,5}
.Digits(1, null)    // \d+ (one or more)
.Digits(0, null)    // \d* (zero or more)
```

#### Letters

**`Letters()`** - One or more letters (`[a-zA-Z]+`)

**`Letters(int exact)`** - Exactly N letters

**`Letters(int? min, int? max)`** - Range of letters

```csharp
.Letters()          // [a-zA-Z]+
.Letters(2)         // [a-zA-Z]{2}
.Letters(2, 4)      // [a-zA-Z]{2,4}
```

#### Whitespace

**`Whitespace()`** - One or more whitespace (`\s+`)

**`Whitespace(int exact)`** - Exactly N whitespace

**`Whitespace(int? min, int? max)`** - Range of whitespace

```csharp
.Whitespace()       // \s+
.Whitespace(1)      // \s{1}
.Whitespace(0, 1)   // \s?
```

#### Word Characters

**`WordCharacter()`** - One or more word characters (`\w+`)

**`WordCharacter(int exact)`** - Exactly N word characters

**`WordCharacter(int? min, int? max)`** - Range of word characters

```csharp
.WordCharacter()        // \w+
.WordCharacter(5)       // \w{5}
.WordCharacter(3, 10)   // \w{3,10}
```

#### Any Character

**`AnyCharacter()`** - One or more of any character (`.+`)

**`AnyCharacter(int exact)`** - Exactly N of any character

**`AnyCharacter(int? min, int? max)`** - Range of any character

```csharp
.AnyCharacter()         // .+
.AnyCharacter(3)        // .{3}
.AnyCharacter(0, null)  // .*
```

#### Character Sets

**`CharacterSet(string characters)`** - One or more from set

**`CharacterSet(string characters, int exact)`** - Exactly N from set

**`CharacterSet(string characters, int? min, int? max)`** - Range from set

```csharp
.CharacterSet("abc")              // [abc]+
.CharacterSet("a-z", 3)           // [a-z]{3}
.CharacterSet("0-9A-F", 2, 4)     // [0-9A-F]{2,4}
```

### Anchor Methods

#### Start

**`Start()`**

Adds start-of-line anchor (`^`).

```csharp
.Start()  // ^
```

#### End

**`End()`**

Adds end-of-line anchor (`$`).

```csharp
.End()  // $
```

### Grouping Methods

#### Capturing Group

**`Group(Action<PatternBuilder> configure)`**

Creates a capturing group.

```csharp
.Group(g => g.Digits(3))  // (\d{3})
```

#### Non-Capturing Group

**`NonCapturingGroup(Action<PatternBuilder> configure)`**

Creates a non-capturing group.

```csharp
.NonCapturingGroup(g => g.Literal("http").Optional(s => s.Literal("s")))  // (?:https?)
```

### Alternation Methods

#### Or with Builder

**`Or(Action<PatternBuilder> configure)`**

Adds an alternative pattern using a builder.

```csharp
.Literal("cat").Or(o => o.Literal("dog"))  // cat|dog
```

#### Or with Node

**`Or(RegexNode node)`**

Adds an alternative pattern using an existing RegexNode.

```csharp
var catNode = RegexBuilder.Literal("cat");
builder.Literal("dog").Or(catNode)  // dog|cat
```

### Utility Methods

#### Optional

**`Optional(Action<PatternBuilder> configure)`**

Makes a pattern optional (`?` quantifier).

```csharp
.Optional(o => o.Literal("+1"))  // (?:\+1)?
```

#### Email

**`Email()`**

Adds the pre-built email pattern.

```csharp
.Email()  // Adds CommonPatterns.Email()
```

#### URL

**`Url()`**

Adds the pre-built URL pattern.

```csharp
.Url()  // Adds CommonPatterns.Url()
```

#### Custom Pattern

**`Pattern(RegexNode node)`**

Adds a custom RegexNode to the builder.

```csharp
var customNode = RegexBuilder.Digit(RegexQuantifier.Exactly(3));
builder.Pattern(customNode)
```

### Build Method

**`Build()`**

Converts the built pattern to a `RegexNode`.

```csharp
var node = builder.Build();
var regex = RegexBuilder.Build(node);
```

### Complete Example

```csharp
// Build a phone number pattern
var phonePattern = RegexBuilder.Pattern()
    .Start()
    .Optional(o => o.Literal("+1").Whitespace(0, 1))
    .Group(g => g.Digits(3))
    .CharacterSet("- .", 0, 1)
    .Group(g => g.Digits(3))
    .CharacterSet("- .", 0, 1)
    .Group(g => g.Digits(4))
    .End()
    .Build();

var phoneRegex = RegexBuilder.Build(phonePattern);
Console.WriteLine(phoneRegex.IsMatch("555-123-4567"));  // True
```

---

## Classic API (RegexBuilder)

The traditional, more verbose API with full access to advanced features.

### Basic Building

```csharp
// Entry point
var regex = RegexBuilder.Build(components...);

// With options
var regex = RegexBuilder.Build(
    RegexOptions.IgnoreCase | RegexOptions.Compiled,
    components...
);
```

### Literal and Escaping

#### Literal

**`Literal(string text)`**

Escaped literal text.

```csharp
RegexBuilder.Literal("hello.world")  // → hello\.world
```

#### NonEscapedLiteral

**`NonEscapedLiteral(string pattern)`**

Raw regex pattern (not escaped).

```csharp
RegexBuilder.NonEscapedLiteral(@"\d+")  // → \d+ (raw)
```

> ⚠️ **Warning**: Use with caution. Incorrect patterns can break regex compilation.

#### MetaCharacter

**`MetaCharacter(char metaChar, RegexQuantifier? quantifier = null)`**

Adds a character class metacharacter.

```csharp
RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore)  // \d+
```

### Character Classes

#### Digit

**`Digit(RegexQuantifier? quantifier = null)`**

Matches digit characters (`\d`).

```csharp
RegexBuilder.Digit()                              // \d
RegexBuilder.Digit(RegexQuantifier.OneOrMore)     // \d+
RegexBuilder.Digit(RegexQuantifier.Exactly(3))    // \d{3}
```

#### NonDigit

**`NonDigit(RegexQuantifier? quantifier = null)`**

Matches non-digit characters (`\D`).

```csharp
RegexBuilder.NonDigit(RegexQuantifier.OneOrMore)  // \D+
```

#### Whitespace

**`Whitespace(RegexQuantifier? quantifier = null)`**

Matches whitespace characters (`\s`).

```csharp
RegexBuilder.Whitespace()                          // \s
RegexBuilder.Whitespace(RegexQuantifier.ZeroOrMore) // \s*
```

#### NonWhitespace

**`NonWhitespace(RegexQuantifier? quantifier = null)`**

Matches non-whitespace characters (`\S`).

```csharp
RegexBuilder.NonWhitespace(RegexQuantifier.OneOrMore)  // \S+
```

#### WordCharacter

**`WordCharacter(RegexQuantifier? quantifier = null)`**

Matches word characters (`\w`).

```csharp
RegexBuilder.WordCharacter()                          // \w
RegexBuilder.WordCharacter(RegexQuantifier.OneOrMore) // \w+
```

#### NonWordCharacter

**`NonWordCharacter(RegexQuantifier? quantifier = null)`**

Matches non-word characters (`\W`).

```csharp
RegexBuilder.NonWordCharacter(RegexQuantifier.OneOrMore)  // \W+
```

#### CharacterSet

**`CharacterSet(string characters, RegexQuantifier? quantifier = null)`**

Positive character set (`[...]`).

```csharp
RegexBuilder.CharacterSet("abc")                             // [abc]
RegexBuilder.CharacterSet("a-z", RegexQuantifier.OneOrMore)  // [a-z]+
RegexBuilder.CharacterSet("0-9A-F", RegexQuantifier.Exactly(2)) // [0-9A-F]{2}
```

#### NegativeCharacterSet

**`NegativeCharacterSet(string characters, RegexQuantifier? quantifier = null)`**

Negative character set (`[^...]`).

```csharp
RegexBuilder.NegativeCharacterSet("abc")                     // [^abc]
RegexBuilder.NegativeCharacterSet("0-9", RegexQuantifier.OneOrMore) // [^0-9]+
```

#### CharacterRange

**`CharacterRange(char fromChar, char toChar, RegexQuantifier? quantifier = null)`**

Character range (`[a-z]`).

```csharp
RegexBuilder.CharacterRange('a', 'z')                            // [a-z]
RegexBuilder.CharacterRange('A', 'Z', RegexQuantifier.OneOrMore) // [A-Z]+
```

### Anchors

#### LineStart

**`LineStart()`**

Start of line anchor (`^`).

```csharp
RegexBuilder.LineStart()  // ^
```

#### LineEnd

**`LineEnd()`**

End of line anchor (`$`).

```csharp
RegexBuilder.LineEnd()  // $
```

#### StringStart

**`StringStart()`**

Start of string anchor (`\A`).

```csharp
RegexBuilder.StringStart()  // \A
```

#### StringEnd

**`StringEnd()`**

End of string anchor (`\Z`).

```csharp
RegexBuilder.StringEnd()  // \Z
```

#### StringEndAbsolute

**`StringEndAbsolute()`**

Absolute end of string anchor (`\z`).

```csharp
RegexBuilder.StringEndAbsolute()  // \z
```

#### WordBoundary

**`WordBoundary(RegexQuantifier? quantifier = null)`**

Word boundary anchor (`\b`).

```csharp
RegexBuilder.WordBoundary()  // \b
```

#### NonWordBoundary

**`NonWordBoundary(RegexQuantifier? quantifier = null)`**

Non-word boundary anchor (`\B`).

```csharp
RegexBuilder.NonWordBoundary()  // \B
```

#### MatchPointAnchor

**`MatchPointAnchor(RegexQuantifier? quantifier = null)`**

Match point anchor (`\G`).

```csharp
RegexBuilder.MatchPointAnchor()  // \G
```

### Grouping and Capturing

#### Group (Named)

**`Group(string groupName, RegexNode matchExpression, RegexQuantifier? quantifier = null)`**

Named capturing group.

```csharp
RegexBuilder.Group("area", RegexBuilder.Digit(RegexQuantifier.Exactly(3)))  // (?<area>\d{3})
```

#### Group (Numbered)

**`Group(RegexNode matchExpression, RegexQuantifier? quantifier = null)`**

Numbered capturing group.

```csharp
RegexBuilder.Group(RegexBuilder.Digit(RegexQuantifier.Exactly(3)))  // (\d{3})
```

#### GroupApostrophe

**`GroupApostrophe(string groupName, RegexNode matchExpression, RegexQuantifier? quantifier = null)`**

Named capturing group with apostrophe syntax (VBScript compatible).

```csharp
RegexBuilder.GroupApostrophe("name", RegexBuilder.Literal("value"))  // (?'name'value)
```

#### NonCapturingGroup

**`NonCapturingGroup(RegexNode matchExpression, RegexQuantifier? quantifier = null)`**

Non-capturing group.

```csharp
RegexBuilder.NonCapturingGroup(
    RegexBuilder.Alternate(
        RegexBuilder.Literal("cat"),
        RegexBuilder.Literal("dog")
    )
)  // (?:cat|dog)
```

#### BalancingGroup

**`BalancingGroup(string pushName, string popName, RegexNode matchExpression, RegexQuantifier? quantifier = null)`**

Two-name balancing group for nested structures.

```csharp
RegexBuilder.BalancingGroup("depth", "depth",
    RegexBuilder.CharacterSet("^()", RegexQuantifier.ZeroOrMore)
)  // (?<depth-depth>[^()]*)
```

#### SimpleBalancingGroup

**`SimpleBalancingGroup(string name, RegexNode matchExpression, RegexQuantifier? quantifier = null)`**

Single-name balancing group.

```csharp
RegexBuilder.SimpleBalancingGroup("stack", RegexBuilder.Literal("item"))  // (?<stack>-item)
```

### Backreferences

#### GroupReference (Named)

**`GroupReference(string groupName)`**

Reference a named capturing group.

```csharp
RegexBuilder.GroupReference("word")  // \k<word>
```

#### GroupReference (Numbered)

**`GroupReference(int groupNumber)`**

Reference a numbered capturing group.

```csharp
RegexBuilder.GroupReference(1)  // \1
```

### Lookaround Assertions

#### PositiveLookAhead

**`PositiveLookAhead(RegexNode matchExpression)`**

Positive lookahead (`(?=...)`).

```csharp
RegexBuilder.PositiveLookAhead(RegexBuilder.Digit())  // (?=\d)
```

#### NegativeLookAhead

**`NegativeLookAhead(RegexNode matchExpression)`**

Negative lookahead (`(?!...)`).

```csharp
RegexBuilder.NegativeLookAhead(RegexBuilder.Digit())  // (?!\d)
```

#### PositiveLookBehind

**`PositiveLookBehind(RegexNode matchExpression)`**

Positive lookbehind (`(?<=...)`).

```csharp
RegexBuilder.PositiveLookBehind(RegexBuilder.Literal("$"))  // (?<=\$)
```

#### NegativeLookBehind

**`NegativeLookBehind(RegexNode matchExpression)`**

Negative lookbehind (`(?<!...)`).

```csharp
RegexBuilder.NegativeLookBehind(RegexBuilder.Literal("$"))  // (?<!\$)
```

### Alternation

**`Alternate(params RegexNode[] options)`**

Creates alternation between multiple patterns.

```csharp
RegexBuilder.Alternate(
    RegexBuilder.Literal("cat"),
    RegexBuilder.Literal("dog"),
    RegexBuilder.Literal("bird")
)  // cat|dog|bird
```

### Concatenation

**`Concatenate(params RegexNode[] nodes)`**

Combines multiple patterns in sequence.

```csharp
RegexBuilder.Concatenate(
    RegexBuilder.Literal("prefix-"),
    RegexBuilder.Digit(RegexQuantifier.Exactly(3)),
    RegexBuilder.Literal("-suffix")
)  // prefix-\d{3}-suffix
```

### Inline Options

#### InlineOption

**`InlineOption(RegexOptions options)`**

Inline option modifier (`(?imnsx-imnsx)`).

```csharp
RegexBuilder.InlineOption(RegexOptions.IgnoreCase)  // (?i)
```

#### InlineOptionGrouping

**`InlineOptionGrouping(RegexOptions enabledOptions, RegexNode expression)`**

Inline option grouping (`(?i:expr)`).

```csharp
RegexBuilder.InlineOptionGrouping(
    RegexOptions.IgnoreCase,
    RegexBuilder.Literal("abc")
)  // (?i:abc)
```

**`InlineOptionGrouping(RegexOptions enabledOptions, RegexOptions disabledOptions, RegexNode expression)`**

Inline option grouping with enabled and disabled options (`(?i-m:expr)`).

```csharp
RegexBuilder.InlineOptionGrouping(
    RegexOptions.IgnoreCase,
    RegexOptions.Multiline,
    RegexBuilder.Literal("abc")
)  // (?i-m:abc)
```

### Comments

**`Comment(string commentText)`**

Adds an inline comment (`(?#comment)`).

```csharp
RegexBuilder.Comment("This matches digits")  // (?#This matches digits)
```

### Unicode Categories

#### UnicodeCategory

**`UnicodeCategory(string categoryName, RegexQuantifier? quantifier = null)`**

Matches Unicode category (`\p{...}`).

```csharp
RegexBuilder.UnicodeCategory("L")                              // \p{L}
RegexBuilder.UnicodeCategory("Lu", RegexQuantifier.OneOrMore)  // \p{Lu}+
RegexBuilder.UnicodeCategory("IsCyrillic")                     // \p{IsCyrillic}
```

#### NegativeUnicodeCategory

**`NegativeUnicodeCategory(string categoryName, RegexQuantifier? quantifier = null)`**

Matches negative Unicode category (`\P{...}`).

```csharp
RegexBuilder.NegativeUnicodeCategory("L")  // \P{L}
```

**Common Categories**:

- `L` - All letters
- `Lu` - Uppercase letters
- `Ll` - Lowercase letters
- `N` - All numbers
- `Nd` - Decimal digits
- `P` - All punctuation
- `IsCyrillic`, `IsArabic`, `IsGreek`, etc. - Unicode blocks

### Special Escapes

#### BellCharacter

**`BellCharacter(RegexQuantifier? quantifier = null)`**

Bell character (`\a`).

```csharp
RegexBuilder.BellCharacter()  // \a
```

#### FormFeed

**`FormFeed(RegexQuantifier? quantifier = null)`**

Form feed character (`\f`).

```csharp
RegexBuilder.FormFeed()  // \f
```

#### VerticalTab

**`VerticalTab(RegexQuantifier? quantifier = null)`**

Vertical tab character (`\v`).

```csharp
RegexBuilder.VerticalTab()  // \v
```

#### EscapeCharacter

**`EscapeCharacter(RegexQuantifier? quantifier = null)`**

Escape character (`\e`).

```csharp
RegexBuilder.EscapeCharacter()  // \e
```

#### OctalCharacter

**`OctalCharacter(int octalValue, RegexQuantifier? quantifier = null)`**

Octal character code (`\NNN`).

```csharp
RegexBuilder.OctalCharacter(40)  // \040 (space character)
```

### Conditional Matching

**`ConditionalMatch(RegexNode conditionExpression, RegexNode trueExpression, RegexNode? falseExpression = null)`**

Conditional matching (`(?(condition)yes|no)`).

```csharp
RegexBuilder.ConditionalMatch(
    RegexBuilder.GroupReference("prefix"),
    RegexBuilder.Literal("-suffix"),
    RegexBuilder.Literal("_suffix")
)  // (?(prefix)-suffix|_suffix)
```

### Backtracking Suppression

**`BacktrackingSuppression(RegexNode matchExpression)`**

Atomic grouping/backtracking suppression (`(?>...)`).

```csharp
RegexBuilder.BacktrackingSuppression(
    RegexBuilder.Digit(RegexQuantifier.OneOrMore)
)  // (?>\d+)
```

---

## Quantifiers

Control how many times a pattern should repeat.

### Properties

```csharp
RegexQuantifier.ZeroOrMore       // *
RegexQuantifier.OneOrMore        // +
RegexQuantifier.Optional         // ?
```

### Factory Methods

```csharp
RegexQuantifier.Exactly(5)       // {5}
RegexQuantifier.AtLeast(3)       // {3,}
RegexQuantifier.Range(2, 5)      // {2,5}
```

### Lazy Quantifiers

```csharp
var quantifier = RegexQuantifier.OneOrMore;
quantifier.Lazy = true;  // +?
```

### Usage Examples

```csharp
// With Classic API
RegexBuilder.Digit(RegexQuantifier.OneOrMore)           // \d+
RegexBuilder.Digit(RegexQuantifier.Exactly(3))          // \d{3}
RegexBuilder.Digit(RegexQuantifier.Range(2, 5))         // \d{2,5}

// With Fluent API
.Digits(3)          // {3}
.Digits(2, 5)       // {2,5}
.Digits(1, null)    // +
```

---

## Substitution Patterns

Build replacement patterns for use with `Regex.Replace()`.

### Entry Point

```csharp
var replacement = SubstitutionBuilder.Build(components...);
string result = regex.Replace(input, replacement);
```

### Group References

#### Named Group

**`Group(string groupName)`**

Reference a named group (`${name}`).

```csharp
SubstitutionBuilder.Group("word")  // ${word}
```

#### Numbered Group

**`Group(int groupNumber)`**

Reference a numbered group (`$1`).

```csharp
SubstitutionBuilder.Group(1)  // $1
```

### Special References

**`WholeMatch()`** - Entire match (`$&`)

```csharp
SubstitutionBuilder.WholeMatch()  // $&
```

**`BeforeMatch()`** - Text before match (`` $` ``)

```csharp
SubstitutionBuilder.BeforeMatch()  // $`
```

**`AfterMatch()`** - Text after match (`$'`)

```csharp
SubstitutionBuilder.AfterMatch()  // $'
```

**`LastCapturedGroup()`** - Last captured group (`$+`)

```csharp
SubstitutionBuilder.LastCapturedGroup()  // $+
```

**`EntireInput()`** - Entire input string (`$_`)

```csharp
SubstitutionBuilder.EntireInput()  // $_
```

**`LiteralDollar()`** - Literal dollar sign (`$$`)

```csharp
SubstitutionBuilder.LiteralDollar()  // $$
```

### Literal Text

**`Literal(string text)`**

Literal text in replacement (auto-escapes `$`).

```csharp
SubstitutionBuilder.Literal("price: $5")  // price: $$5
```

### Complete Example

```csharp
// Swap two words
var pattern = RegexBuilder.Build(
    RegexBuilder.Group("word1", RegexBuilder.WordCharacter(RegexQuantifier.OneOrMore)),
    RegexBuilder.Whitespace(),
    RegexBuilder.Group("word2", RegexBuilder.WordCharacter(RegexQuantifier.OneOrMore))
);

var replacement = SubstitutionBuilder.Build(
    SubstitutionBuilder.Group("word2"),
    SubstitutionBuilder.Literal(" "),
    SubstitutionBuilder.Group("word1")
);

string result = pattern.Replace("hello world", replacement);
// result = "world hello"
```

---

## Advanced Features

### Balancing Groups

Match nested/balanced structures like parentheses, XML tags, or code blocks.

**Syntax**: `(?<push-pop>expr)` or `(?<name>-expr)`

```csharp
// Match balanced parentheses
var balancedParens = RegexBuilder.Build(
    RegexBuilder.Literal("("),
    RegexBuilder.BalancingGroup("depth", "depth",
        RegexBuilder.NegativeCharacterSet("()", RegexQuantifier.ZeroOrMore)
    ),
    RegexBuilder.Literal(")")
);

// Matches: "()", "(text)", "((nested))"
```

**Use Cases**:

- Matching balanced parentheses
- Parsing XML/HTML tags
- Extracting code blocks
- Finding nested JSON structures

### Unicode Category Matching

Match characters by Unicode category or block.

```csharp
// Match any letter (any language)
var letters = RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore);

// Match Cyrillic text
var cyrillic = RegexBuilder.UnicodeCategory("IsCyrillic", RegexQuantifier.OneOrMore);

// Match non-Latin characters
var nonLatin = RegexBuilder.NegativeUnicodeCategory("IsBasicLatin");

// Test
var regex = RegexBuilder.Build(cyrillic);
Console.WriteLine(regex.IsMatch("Привет"));  // True (Cyrillic)
Console.WriteLine(regex.IsMatch("Hello"));   // False (Latin)
```

**Common Categories**:

- `L` - All letters
- `Lu` - Uppercase letters
- `Ll` - Lowercase letters
- `N` - Numbers
- `Nd` - Decimal digits
- `P` - Punctuation
- `S` - Symbols
- `Z` - Separators

**Common Blocks**:

- `IsBasicLatin`
- `IsCyrillic`
- `IsArabic`
- `IsGreek`
- `IsHebrew`
- `IsCJKUnifiedIdeographs`

### Inline Option Grouping

Apply regex options to specific sub-expressions only.

```csharp
// Case-insensitive match for specific part
var pattern = RegexBuilder.Build(
    RegexBuilder.Literal("ID:"),
    RegexBuilder.InlineOptionGrouping(
        RegexOptions.IgnoreCase,
        RegexBuilder.Literal("abc")
    ),
    RegexBuilder.Digit(RegexQuantifier.Exactly(3))
);

// Matches: "ID:abc123", "ID:ABC123", "ID:AbC123"
// Doesn't match: "id:abc123" (ID is case-sensitive)
```

**Available Options**:

- `IgnoreCase` (i)
- `Multiline` (m)
- `Singleline` (s)
- `ExplicitCapture` (n)
- `IgnorePatternWhitespace` (x)

### Conditional Matching

Match different patterns based on whether a condition is true.

```csharp
// Match suffix based on whether prefix group exists
var pattern = RegexBuilder.Build(
    RegexBuilder.Optional(RegexBuilder.Group("prefix", RegexBuilder.Literal("PRE"))),
    RegexBuilder.Literal("-"),
    RegexBuilder.ConditionalMatch(
        RegexBuilder.GroupReference("prefix"),
        RegexBuilder.Literal("SUFFIX"),
        RegexBuilder.Literal("suffix")
    )
);

// Matches: "PRE-SUFFIX" or "-suffix"
```

---

## Syntax Mapping Reference

Quick lookup table for traditional regex → RegexBuilder conversion.

| Traditional     | RegexBuilder (Classic)                                | RegexBuilder (Fluent)                   |
| --------------- | ----------------------------------------------------- | --------------------------------------- |
| `^`             | `LineStart()`                                         | `.Start()`                              |
| `$`             | `LineEnd()`                                           | `.End()`                                |
| `\d`            | `Digit()`                                             | `.Digits()`                             |
| `\d+`           | `Digit(RegexQuantifier.OneOrMore)`                    | `.Digits(1, null)`                      |
| `\d{3}`         | `Digit(RegexQuantifier.Exactly(3))`                   | `.Digits(3)` or `.Digits(3, 3)`         |
| `\d{2,5}`       | `Digit(RegexQuantifier.Range(2, 5))`                  | `.Digits(2, 5)`                         |
| `\w`            | `WordCharacter()`                                     | `.WordCharacter()`                      |
| `\w+`           | `WordCharacter(RegexQuantifier.OneOrMore)`            | `.WordCharacter(1, null)`               |
| `\s`            | `Whitespace()`                                        | `.Whitespace()`                         |
| `\s*`           | `Whitespace(RegexQuantifier.ZeroOrMore)`              | `.Whitespace(0, null)`                  |
| `.`             | `MetaCharacter(RegexMetaChars.AnyCharacter)`          | `.AnyCharacter()`                       |
| `[a-z]`         | `CharacterRange('a', 'z')`                            | `.CharacterSet("a-z")`                  |
| `[abc]`         | `CharacterSet("abc")`                                 | `.CharacterSet("abc")`                  |
| `[^a-z]`        | `NegativeCharacterSet("a-z")`                         | N/A (use classic)                       |
| `(expr)`        | `Group(expr)`                                         | `.Group(g => g...)`                     |
| `(?:expr)`      | `NonCapturingGroup(expr)`                             | `.NonCapturingGroup(g => g...)`         |
| `(?<name>expr)` | `Group("name", expr)`                                 | N/A (use classic)                       |
| `\1`            | `GroupReference(1)`                                   | N/A (use classic)                       |
| `\k<name>`      | `GroupReference("name")`                              | N/A (use classic)                       |
| `a\|b`          | `Alternate(Literal("a"), Literal("b"))`               | `.Literal("a").Or(o => o.Literal("b"))` |
| `(?=expr)`      | `PositiveLookAhead(expr)`                             | N/A (use classic)                       |
| `(?!expr)`      | `NegativeLookAhead(expr)`                             | N/A (use classic)                       |
| `(?<=expr)`     | `PositiveLookBehind(expr)`                            | N/A (use classic)                       |
| `(?<!expr)`     | `NegativeLookBehind(expr)`                            | N/A (use classic)                       |
| `(?i:expr)`     | `InlineOptionGrouping(RegexOptions.IgnoreCase, expr)` | N/A (use classic)                       |
| `\p{L}`         | `UnicodeCategory("L")`                                | N/A (use classic)                       |
| `\P{L}`         | `NegativeUnicodeCategory("L")`                        | N/A (use classic)                       |
| `literal\.text` | `Literal("literal.text")`                             | `.Literal("literal.text")`              |

---

## Troubleshooting

### Common Issues

#### Pattern Doesn't Match Expected Input

**Solution**:

1. Test the generated pattern on [regex101.com](https://regex101.com)
2. Check for proper escaping of special characters
3. Verify quantifiers are applied correctly
4. Add anchors (`Start()` / `End()`) if needed

```csharp
// Debug: Print the generated pattern
var pattern = builder.Build();
var regex = RegexBuilder.Build(pattern);
Console.WriteLine(regex.ToString());  // See the actual regex
```

#### Performance is Slow

**Solution**:

1. Cache compiled Regex objects
2. Use `RegexOptions.Compiled` for hot paths
3. Test with pathological inputs to avoid catastrophic backtracking

```csharp
// ✓ Good: Static, cached, compiled
private static readonly Regex EmailRegex = RegexBuilder.Build(
    RegexOptions.Compiled,
    CommonPatterns.Email()
);
```

#### Pattern is Too Verbose

**Solution**:

1. Use Fluent API for simpler patterns
2. Extract reusable components
3. Consider if hand-written regex is better for very simple cases

```csharp
// Extract reusable components
public static class MyPatterns
{
    public static RegexNode Identifier() =>
        RegexBuilder.Pattern()
            .Letters(1)
            .WordCharacter(0, null)
            .Build();
}
```

#### Can't Figure Out Which Method to Use

**Solution**:

1. Check the [Syntax Mapping Reference](#syntax-mapping-reference)
2. Browse [Common Patterns Library](common-patterns.md) for examples
3. Use IDE IntelliSense for method discovery

### Getting Help

- **GitHub Issues**: [Report bugs or request features](https://github.com/somenoe/RegexBuilder.NET9/issues)
- **Example Code**: Check [CustomRegexTests.cs](https://github.com/somenoe/RegexBuilder.NET9/blob/master/src/RegexBuilder.Tests/CustomRegexTests.cs)
- **Common Patterns**: See [Common Patterns Library](common-patterns.md)
- **Getting Started**: Review [Getting Started Guide](getting-started.md)

---

## Next Steps

- **[Getting Started](getting-started.md)** - Learn the basics
- **[Common Patterns](common-patterns.md)** - Copy-paste ready patterns
- **[GitHub Examples](https://github.com/somenoe/RegexBuilder.NET9)** - More examples

Happy pattern building! 🚀
