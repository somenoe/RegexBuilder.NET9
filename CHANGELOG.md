# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2025-10-21

### Added

- **Fluent Pattern Builder** - New `PatternBuilder` class for composing complex regex patterns with improved ergonomics
  - Fluent API with chainable methods for intuitive pattern construction
  - Entry point: `RegexBuilder.Pattern()` returns new `PatternBuilder` instance
  - Core pattern methods:
    - `Literal(string)` - Add literal text with automatic escaping
    - `Digits(min, max)` - Add digit pattern `\d` with optional quantifiers
    - `Letters(min, max)` - Add letter pattern `[a-zA-Z]` with optional quantifiers
    - `Whitespace(min, max)` - Add whitespace pattern `\s` with optional quantifiers
    - `WordCharacter(min, max)` - Add word character pattern `\w` with optional quantifiers
    - `AnyCharacter(min, max)` - Add any character pattern `.` with optional quantifiers
    - `CharacterSet(string, min, max)` - Add custom character set with optional quantifiers
  - Anchor methods:
    - `Start()` - Add start-of-line anchor `^`
    - `End()` - Add end-of-line anchor `$`
  - Grouping methods:
    - `Group(Action<PatternBuilder>)` - Add capturing group `(pattern)`
    - `NonCapturingGroup(Action<PatternBuilder>)` - Add non-capturing group `(?:pattern)`
  - Alternation methods:
    - `Or(Action<PatternBuilder>)` - Add alternation with builder callback
    - `Or(RegexNode)` - Add alternation with existing RegexNode
  - Utility methods:
    - `Optional(Action<PatternBuilder>)` - Make pattern optional with `?` quantifier
    - `Email()` - Add common email pattern
    - `Url()` - Add common URL pattern
    - `Pattern(RegexNode)` - Add custom RegexNode
  - Method chaining support for fluent composition
  - Comprehensive validation with helpful exception messages
  - Complete test suite with 45+ test cases covering:
    - Sequential patterns and concatenation
    - Quantifiers (min, max, none)
    - Character classes and custom character sets
    - Grouping and nesting
    - Alternation with multiple branches
    - Optional patterns
    - Complex real-world patterns (IDs, URLs, phone numbers)
    - Error handling and edge cases
  - Full integration with existing RegexBuilder API

## [1.1.0] - 2025-10-21

### Added

- **Named Group Apostrophe Syntax** - Support for `(?'name'expr)` for VBScript and legacy pattern compatibility
  - New `UseApostropheSyntax` property in `RegexNodeGroup` class to control output format
  - Alternative syntax `(?'name'expr)` for named capturing groups (equivalent to `(?<name>expr)`)
  - Enables compatibility with legacy regex patterns written for VBScript
  - Full quantifier support: `(?'name'expr)*`, `(?'name'expr)+`, etc.
  - New public API methods:
    - `RegexBuilder.GroupApostrophe(string groupName, RegexNode matchExpression)` - Create apostrophe syntax named group
    - `RegexBuilder.GroupApostrophe(string groupName, RegexNode matchExpression, RegexQuantifier quantifier)` - With quantifier
  - Comprehensive test suite with 3 new test cases covering:
    - Apostrophe syntax rendering with various quantifiers
    - Functional equivalence with angle-bracket syntax for group capturing
    - Backreference support with apostrophe syntax groups

- **Inline Option Grouping** - Support for `(?imnsx-imnsx:expr)` for scoped regex option application
  - New `RegexNodeInlineOptionGrouping` class for inline option grouping constructs
  - Enable/disable regex options for specific expression scopes: `(?i:expr)` for case-insensitive, `(?m:expr)` for multiline, etc.
  - Support for option negation (disabling options): `(?i-m:expr)` - enable IgnoreCase, disable Multiline
  - Supports all inline-compatible options: IgnoreCase (i), Multiline (m), Singleline (s), ExplicitCapture (n), IgnorePatternWhitespace (x)
  - Proper validation to prevent invalid options (Compiled, RightToLeft, ECMAScript, CultureInvariant)
  - New public API methods:
    - `RegexBuilder.InlineOptionGrouping(RegexOptions enabledOptions, RegexNode expression)` - Enable specific options
    - `RegexBuilder.InlineOptionGrouping(RegexOptions enabledOptions, RegexOptions disabledOptions, RegexNode expression)` - Enable and disable options
  - Comprehensive test suite with 30 test cases covering:
    - Single option enabling: `(?i:expr)`, `(?m:expr)`, `(?s:expr)`, `(?n:expr)`, `(?x:expr)`
    - Multiple options: `(?im:expr)`, `(?imsnx:expr)`
    - Option negation: `(?i-m:expr)`, `(?is-mn:expr)`
    - Complex nested expressions
    - Invalid option validation
    - Real-world matching scenarios (case-insensitive, multiline, singleline)

- **Convenience Shortcut Methods** - Intuitive methods for common regex patterns
  - Character class shortcuts: `Digit()`, `NonDigit()`, `Whitespace()`, `NonWhitespace()`, `WordCharacter()`, `NonWordCharacter()`
  - Anchor shortcuts: `LineStart()`, `LineEnd()`, `StringStart()`, `StringEnd()`, `StringEndAbsolute()`, `WordBoundary()`, `NonWordBoundary()`, `MatchPointAnchor()`
  - Escape character shortcuts: `BellCharacter()`, `FormFeed()`, `VerticalTab()`, `EscapeCharacter()`, `OctalCharacter(int octalValue)`
  - All methods support optional quantifiers for flexible pattern matching
  - Comprehensive test suite with 28 new test cases covering:
    - Individual shortcut method validation
    - Quantifier application and rendering
    - Integration with other regex constructs
    - Real-world pattern matching scenarios

- **Balancing Groups** - Support for `(?<name1-name2>expr)` and `(?<name>-expr)` constructs
  - New `RegexNodeBalancingGroup` class for balancing group constructs
  - Support for two-name balancing groups: `(?<name1-name2>expr)` - push to name1, pop from name2
  - Support for single-name balancing groups: `(?<name>-expr)` - push to name only
  - Essential for matching nested/balanced constructs (parentheses, XML tags, code blocks)
  - Full quantifier support for balancing group patterns
  - New public API methods:
    - `RegexBuilder.BalancingGroup(string pushName, string popName, RegexNode expr)` - Create two-name balancing group
    - `RegexBuilder.BalancingGroup(string pushName, string popName, RegexNode expr, RegexQuantifier quantifier)` - With quantifier
    - `RegexBuilder.SimpleBalancingGroup(string name, RegexNode expr)` - Create single-name balancing group
    - `RegexBuilder.SimpleBalancingGroup(string name, RegexNode expr, RegexQuantifier quantifier)` - With quantifier
  - Comprehensive test suite with practical examples:
    - Balanced parentheses matching
    - Nested XML tag matching
    - Code block nesting patterns
    - Edge case validation
    - Integration tests with other regex constructs

### Convenience Shortcut Methods Examples

- `RegexBuilder.Digit()` → `\d`
- `RegexBuilder.Whitespace(RegexQuantifier.OneOrMore)` → `\s+`
- `RegexBuilder.StringStart()` → `\A`
- `RegexBuilder.OctalCharacter(32)` → `\040` (space character)

### Balancing Groups Examples

```csharp
// Match balanced parentheses
var balancedParens = RegexBuilder.Build(
    RegexBuilder.Literal("("),
    RegexBuilder.BalancingGroup("paren", "paren",
        RegexBuilder.NonEscapedLiteral(@"[^()]"),
        RegexQuantifier.ZeroOrMore),
    RegexBuilder.Literal(")")
);

// Matches: "()", "(text)", "((nested))", "(level (deep (nesting)))"

// Match balanced XML-like tags
var xmlTags = RegexBuilder.Build(
    RegexBuilder.BalancingGroup("tag", "open", RegexBuilder.NonEscapedLiteral(@"<\w+>")),
    RegexBuilder.NegativeCharacterSet("<>", null, RegexQuantifier.ZeroOrMore),
    RegexBuilder.BalancingGroup("tag", "tag", RegexBuilder.NonEscapedLiteral(@"</\w+>"))
);

// Matches: "<div></div>", "<div><span>content</span></div>"
```

- **Unicode Category Matching** - Support for `\p{name}` and `\P{name}` escape sequences
  - New `RegexNodeUnicodeCategory` class for Unicode category escape sequences
  - Support for all .NET Unicode general categories: L, Lu, Ll, Lt, Lm, Lo, N, Nd, Nl, No, P, Pc, Pd, Ps, Pe, Pi, Pf, Po, M, Mn, Mc, Me, Z, Zs, Zl, Zp, S, Sm, Sc, Sk, So, C, Cc, Cf, Cs, Co, Cn
  - Support for Unicode named blocks: IsCyrillic, IsArabic, IsLatin1Supplement, IsGreekandCoptic, IsHebrew, and many others
  - Negated Unicode categories via `\P{name}` syntax
  - Full quantifier support for Unicode category patterns
  - New public API methods:
    - `RegexBuilder.UnicodeCategory(string categoryName)` - Create positive Unicode category match
    - `RegexBuilder.UnicodeCategory(string categoryName, RegexQuantifier quantifier)` - With quantifier
    - `RegexBuilder.NegativeUnicodeCategory(string categoryName)` - Create negated Unicode category match
    - `RegexBuilder.NegativeUnicodeCategory(string categoryName, RegexQuantifier quantifier)` - With quantifier
  - Unicode category validation helper: `RegexMetaChars.IsValidUnicodeCategory(string)`
  - Comprehensive test suite with integration tests for international text matching

### Examples

```csharp
// Match Unicode letters followed by digits
var pattern = RegexBuilder.Build(
    RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore),
    RegexBuilder.Literal(" "),
    RegexBuilder.UnicodeCategory("Nd", RegexQuantifier.Exactly(3))
);

// Matches: "Hello 123", "Привет 456", "שלום 789"
var regex = RegexBuilder.Build(pattern);
Assert.IsTrue(regex.IsMatch("Привет 123")); // Cyrillic letters
Assert.IsTrue(regex.IsMatch("مرحبا 456")); // Arabic letters

// Match non-Latin characters
var nonLatin = RegexBuilder.NegativeUnicodeCategory("IsLatin1Supplement");
var cyrillic = RegexBuilder.UnicodeCategory("IsCyrillic", RegexQuantifier.OneOrMore);
var pattern2 = RegexBuilder.Alternate(cyrillic, nonLatin);
```

## [1.0.5] - 2025-10-21

### Added

- **Substitution/Replacement Patterns Support** - Complete implementation of .NET regex substitution patterns
  - New `SubstitutionNode` base class for all substitution pattern nodes
  - New `SubstitutionBuilder` factory class for creating replacement patterns
  - Support for all .NET substitution constructs:
    - `$number` - Numbered group references via `SubstitutionBuilder.Group(int)`
    - `${name}` - Named group references via `SubstitutionBuilder.Group(string)`
    - `$$` - Literal dollar sign via `SubstitutionBuilder.LiteralDollar()`
    - `$&` - Whole match via `SubstitutionBuilder.WholeMatch()`
    - `` $` `` - Text before match via `SubstitutionBuilder.BeforeMatch()`
    - `$'` - Text after match via `SubstitutionBuilder.AfterMatch()`
    - `$+` - Last captured group via `SubstitutionBuilder.LastCapturedGroup()`
    - `$_` - Entire input string via `SubstitutionBuilder.EntireInput()`
  - Automatic escaping of dollar signs in literal text
  - Comprehensive test suite with 43 new tests covering all substitution types
  - Planning documentation in `history/substitution_patterns_feature.md`

### Changed

- RegexBuilder now provides complete coverage of .NET regular expression capabilities
- Feature support table updated: substitution/replacement patterns are now fully supported

### Examples

```csharp
// Simple group swapping
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

// Phone number formatting
var phonePattern = RegexBuilder.Build(
    RegexBuilder.Group("area", RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Exactly(3))),
    RegexBuilder.Literal("-"),
    RegexBuilder.Group("prefix", RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Exactly(3))),
    RegexBuilder.Literal("-"),
    RegexBuilder.Group("number", RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Exactly(4)))
);

var phoneReplacement = SubstitutionBuilder.Build(
    SubstitutionBuilder.Literal("("),
    SubstitutionBuilder.Group("area"),
    SubstitutionBuilder.Literal(") "),
    SubstitutionBuilder.Group("prefix"),
    SubstitutionBuilder.Literal("-"),
    SubstitutionBuilder.Group("number")
);

string formatted = phonePattern.Replace("555-123-4567", phoneReplacement);
// formatted = "(555) 123-4567"

// Adding currency symbol
var pricePattern = RegexBuilder.Build(
    RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore)
);

var priceReplacement = SubstitutionBuilder.Build(
    SubstitutionBuilder.LiteralDollar(),
    SubstitutionBuilder.WholeMatch()
);

string withCurrency = pricePattern.Replace("The price is 100", priceReplacement);
// withCurrency = "The price is $100"
```

## [1.0.4] - 2025-10-21

### Added

- New `CommonPatterns` static class providing factory methods for commonly used regex patterns
  - `CommonPatterns.Email()` - Returns a RegexNode for validating email addresses
    - Supports standard email format with local part, domain, and TLD (2-6 characters)
    - Allows alphanumeric characters, dots, hyphens, underscores, percent, and plus signs in local part
  - `CommonPatterns.Url()` - Returns a RegexNode for validating URLs
    - Supports http://, https://, and ftp:// protocols (optional)
    - Supports domains with optional ports and subdomains
    - Supports paths, query strings, and fragments
- Comprehensive test suite with 39 new tests for CommonPatterns functionality
- Planning documentation in `history/common_patterns_feature.md`

### Examples

```csharp
// Simple email validation
var emailRegex = RegexBuilder.Build(CommonPatterns.Email());
bool isValid = emailRegex.IsMatch("user@example.com"); // true

// URL validation
var urlRegex = RegexBuilder.Build(CommonPatterns.Url());
bool isValid = urlRegex.IsMatch("https://example.com/path"); // true

// Combining patterns
var contactRegex = RegexBuilder.Build(
    RegexBuilder.Literal("Email: "),
    CommonPatterns.Email(),
    RegexBuilder.Literal(", Website: "),
    CommonPatterns.Url()
);
```

## [1.0.3] - 2025-10-21

### Added

- Created comprehensive SOP documentation for publishing NuGet packages (`SOP/nuget_package_publishing.md`)
  - CLI publishing instructions with environment variable usage for API keys
  - GitHub workflow publishing instructions
  - Troubleshooting guide and best practices
  - Security notes for handling API keys
- Updated release workflow to automatically publish to NuGet.org
  - Added publish step using `NUGET_API_KEY` secret
  - Added `--skip-duplicate` flag to prevent duplicate package errors
- Added `artifacts/` directory and `*.nupkg` files to `.gitignore`

### Changed

- Release workflow now publishes packages to NuGet.org automatically on tag push or manual dispatch
- Improved PowerShell command syntax for `dotnet nuget push` to properly handle wildcards

## [1.0.2] - 2025-10-21

### Changed

- **BREAKING**: Renamed all namespaces from `YuriyGuts.RegexBuilder` to `RegexBuilder`
- **BREAKING**: Renamed assembly from `YuriyGuts.RegexBuilder.dll` to `RegexBuilder.dll`
- Renamed all project files and directories to remove `YuriyGuts` prefix
- Updated NuGet package ID to `RegexBuilder.NET9`
- Updated repository URL to `https://github.com/somenoe/RegexBuilder.NET9`
- Simplified project structure with cleaner naming conventions

### Migration Guide

If upgrading from version 1.0.1:

1. Update your NuGet package reference from `RegexBuilder` to `RegexBuilder.NET9`
2. Update all `using YuriyGuts.RegexBuilder;` statements to `using RegexBuilder;`
3. Rebuild your project

## [1.0.1] - 2025-10-21

### Added

- Forked from [YuriyGuts/regex-builder](https://github.com/YuriyGuts/regex-builder)
- Added GitHub Actions workflows for CI/CD
- Added CHANGELOG.md following Keep a Changelog format

### Changed

- Upgraded all projects from .NET Framework 4.0 to .NET 9.0
- Migrated all `.csproj` files from legacy format to SDK-style project format
- Upgraded test framework from MSTest V1 to MSTest V2 (3.6.0)
- Added `Microsoft.NET.Test.Sdk` (17.11.1) for modern test execution
- Enabled `ImplicitUsings` in the main library project
- Tests now run via `dotnet test` command instead of requiring Visual Studio Test Runner

### Removed

- Removed legacy MSBuild targets and tooling references
- Removed deprecated PostBuildEvent for NuGet package output
- Cleaned up legacy build configurations and platform-specific settings

### Fixed

- Resolved duplicate assembly attribute errors by preserving existing `AssemblyInfo.cs` files with `GenerateAssemblyInfo=false`

### Technical Details

- All 89 existing unit tests pass successfully on .NET 9.0
- Projects now use modern SDK-style format for better maintainability
- Compatible with .NET 9 SDK (tested with 9.0.306)

## [1.0.0] - 2011

### Added

- Initial release targeting .NET Framework 4.0
- Human-readable regex builder API
- Support for character ranges, character sets, and quantifiers
- Support for groups, alternations, and look-around assertions
- Comprehensive unit test coverage
