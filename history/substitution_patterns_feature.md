# Substitution/Replacement Patterns Feature - Implementation Plan

## Date: October 21, 2025

## Overview

Adding support for substitution/replacement patterns to the RegexBuilder library. Currently, RegexBuilder only supports pattern matching (the "find" part of find-and-replace), but not the replacement patterns used in `Regex.Replace()` operations. This feature will complete the library's coverage of .NET regex capabilities.

## Background

According to the [MSDN documentation](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference#substitutions), substitution patterns are regular expression language elements that are supported in replacement patterns used with methods like `Regex.Replace()`.

## Design Goals

1. **Completeness**: Provide full support for all .NET substitution constructs
2. **Type Safety**: Use strongly-typed API instead of raw strings
3. **Consistency**: Follow existing RegexBuilder API design patterns
4. **Composability**: Allow building complex replacement patterns by combining simple ones
5. **Readability**: Make replacement patterns as readable as match patterns

## Substitution Constructs to Support

Based on MSDN documentation, the following substitution constructs need to be supported:

| Construct | Description                                          | Example                |
| --------- | ---------------------------------------------------- | ---------------------- |
| `$number` | Substitutes the substring matched by group number    | `$1`, `$2`             |
| `${name}` | Substitutes the substring matched by the named group | `${word1}`, `${word2}` |
| `$$`      | Substitutes a literal "$"                            | `$$`                   |
| `$&`      | Substitutes a copy of the whole match                | `$&`                   |
| `$\``     | Substitutes all text before the match                | ``$` ``                |
| `$'`      | Substitutes all text after the match                 | `$'`                   |
| `$+`      | Substitutes the last group that was captured         | `$+`                   |
| `$_`      | Substitutes the entire input string                  | `$_`                   |

## Implementation Plan

### 1. New Classes

#### SubstitutionNode (Abstract Base Class)

```csharp
public abstract class SubstitutionNode
{
    public abstract string ToSubstitutionPattern();
}
```

Base class for all substitution pattern nodes, similar to how `RegexNode` works for match patterns.

#### SubstitutionLiteral

```csharp
public class SubstitutionLiteral : SubstitutionNode
{
    public string Text { get; }
    public override string ToSubstitutionPattern()
}
```

For literal text in replacement patterns. Should escape `$` characters as `$$`.

#### SubstitutionGroupReference

```csharp
public class SubstitutionGroupReference : SubstitutionNode
{
    public int? GroupNumber { get; }
    public string GroupName { get; }
    public override string ToSubstitutionPattern()
}
```

For `$number` and `${name}` patterns.

#### SubstitutionSpecialReference

```csharp
public class SubstitutionSpecialReference : SubstitutionNode
{
    public SubstitutionType Type { get; }
    public override string ToSubstitutionPattern()
}

public enum SubstitutionType
{
    WholeMatch,      // $&
    BeforeMatch,     // $`
    AfterMatch,      // $'
    LastGroup,       // $+
    EntireInput      // $_
}
```

For special substitution patterns.

#### SubstitutionConcatenation

```csharp
public class SubstitutionConcatenation : SubstitutionNode
{
    public IEnumerable<SubstitutionNode> ChildNodes { get; }
    public override string ToSubstitutionPattern()
}
```

For combining multiple substitution nodes into a single pattern.

### 2. Factory Class: SubstitutionBuilder

Create a new static class `SubstitutionBuilder` with factory methods:

```csharp
public static class SubstitutionBuilder
{
    // Literal text
    public static SubstitutionLiteral Literal(string text);

    // Group references
    public static SubstitutionGroupReference Group(int groupNumber);
    public static SubstitutionGroupReference Group(string groupName);

    // Special references
    public static SubstitutionSpecialReference WholeMatch();
    public static SubstitutionSpecialReference BeforeMatch();
    public static SubstitutionSpecialReference AfterMatch();
    public static SubstitutionSpecialReference LastCapturedGroup();
    public static SubstitutionSpecialReference EntireInput();
    public static SubstitutionSpecialReference LiteralDollar();

    // Concatenation
    public static SubstitutionConcatenation Concatenate(params SubstitutionNode[] nodes);

    // Build method
    public static string Build(params SubstitutionNode[] nodes);
}
```

### 3. Integration Points

The library should work seamlessly with existing `Regex.Replace()` operations:

```csharp
// Example usage
var pattern = RegexBuilder.Build(
    RegexBuilder.Group("word1", RegexBuilder.Word()),
    RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace),
    RegexBuilder.Group("word2", RegexBuilder.Word())
);

var replacement = SubstitutionBuilder.Build(
    SubstitutionBuilder.Group("word2"),
    SubstitutionBuilder.Literal(" "),
    SubstitutionBuilder.Group("word1")
);

string result = pattern.Replace("hello world", replacement);
// result = "world hello"
```

### 4. Testing Strategy

#### Unit Tests for SubstitutionNode Classes

- Test each substitution node type renders correct pattern
- Test escaping in literal text ($ becomes $$)
- Test concatenation of multiple nodes
- Test edge cases (empty strings, special characters)

#### Integration Tests with Regex.Replace()

Test all substitution types with real regex replacements:

1. **Numbered group substitution** (`$number`)
   - Simple substitution: `$1`, `$2`
   - Multiple groups
   - Out of order groups

2. **Named group substitution** (`${name}`)
   - Simple named groups
   - Complex names
   - Multiple named groups

3. **Special substitutions**
   - Whole match (`$&`)
   - Before match (`` $` ``)
   - After match (`$'`)
   - Last captured group (`$+`)
   - Entire input (`$_`)
   - Literal dollar (`$$`)

4. **Complex combinations**
   - Mixing literals and substitutions
   - Multiple substitution types in one replacement
   - Real-world scenarios (formatting, restructuring)

#### Test Examples

```csharp
[TestMethod]
public void NumberedGroupSubstitution_SwapsTwoWords()
{
    var pattern = RegexBuilder.Build(
        RegexBuilder.Group(RegexBuilder.Word()),
        RegexBuilder.Literal(" "),
        RegexBuilder.Group(RegexBuilder.Word())
    );

    var replacement = SubstitutionBuilder.Build(
        SubstitutionBuilder.Group(2),
        SubstitutionBuilder.Literal(" "),
        SubstitutionBuilder.Group(1)
    );

    string result = pattern.Replace("one two", replacement);
    Assert.AreEqual("two one", result);
}

[TestMethod]
public void NamedGroupSubstitution_FormatsPhoneNumber()
{
    var pattern = RegexBuilder.Build(
        RegexBuilder.Group("area", RegexBuilder.Digit(RegexQuantifier.Exactly(3))),
        RegexBuilder.Literal("-"),
        RegexBuilder.Group("prefix", RegexBuilder.Digit(RegexQuantifier.Exactly(3))),
        RegexBuilder.Literal("-"),
        RegexBuilder.Group("number", RegexBuilder.Digit(RegexQuantifier.Exactly(4)))
    );

    var replacement = SubstitutionBuilder.Build(
        SubstitutionBuilder.Literal("("),
        SubstitutionBuilder.Group("area"),
        SubstitutionBuilder.Literal(") "),
        SubstitutionBuilder.Group("prefix"),
        SubstitutionBuilder.Literal("-"),
        SubstitutionBuilder.Group("number")
    );

    string result = pattern.Replace("555-123-4567", replacement);
    Assert.AreEqual("(555) 123-4567", result);
}

[TestMethod]
public void WholeMatch_DuplicatesMatch()
{
    var pattern = RegexBuilder.Build(RegexBuilder.Word());
    var replacement = SubstitutionBuilder.Build(
        SubstitutionBuilder.WholeMatch(),
        SubstitutionBuilder.Literal("-"),
        SubstitutionBuilder.WholeMatch()
    );

    string result = pattern.Replace("hello", replacement);
    Assert.AreEqual("hello-hello", result);
}

[TestMethod]
public void LiteralDollar_InsertsMoneySymbol()
{
    var pattern = RegexBuilder.Build(
        RegexBuilder.Digit(RegexQuantifier.OneOrMore)
    );

    var replacement = SubstitutionBuilder.Build(
        SubstitutionBuilder.LiteralDollar(),
        SubstitutionBuilder.WholeMatch()
    );

    string result = pattern.Replace("The price is 100", replacement);
    Assert.AreEqual("The price is $100", result);
}
```

### 5. Documentation Requirements

1. **README.md Update**
   - Add section on substitution patterns
   - Include examples of common use cases
   - Update feature support table

2. **XML Documentation**
   - Document all public classes and methods
   - Include code examples in XML comments
   - Explain the difference between match and replacement patterns

3. **CHANGELOG.md**
   - Document as a new feature
   - List all new classes and methods
   - Include migration guide if needed

## Implementation Steps

1. **Phase 1: Core Classes** (Day 1)
   - Create `SubstitutionNode` base class
   - Implement `SubstitutionLiteral`
   - Implement `SubstitutionGroupReference`
   - Implement `SubstitutionSpecialReference`
   - Implement `SubstitutionConcatenation`

2. **Phase 2: Factory Class** (Day 1)
   - Create `SubstitutionBuilder` static class
   - Implement all factory methods
   - Implement `Build()` method

3. **Phase 3: Testing** (Day 2)
   - Write unit tests for each substitution node type
   - Write integration tests with `Regex.Replace()`
   - Test all substitution constructs
   - Test edge cases and error conditions

4. **Phase 4: Documentation** (Day 2)
   - Update README.md
   - Add XML documentation
   - Update CHANGELOG.md
   - Create usage examples

## Potential Challenges

1. **String Escaping**: Need to properly escape `$` in literal text
2. **API Consistency**: Ensure naming and patterns match existing RegexBuilder API
3. **Testing Coverage**: Need comprehensive tests for all substitution types
4. **Documentation**: Must clearly explain the difference between match patterns and replacement patterns

## Success Criteria

1. All .NET substitution constructs are supported
2. API is consistent with existing RegexBuilder patterns
3. Comprehensive test coverage (>95%)
4. Clear documentation with examples
5. No breaking changes to existing functionality

## Future Enhancements

1. **Conditional Replacements**: Support for conditional substitutions
2. **Custom Transformations**: Allow custom transformation functions
3. **Performance Optimization**: Cache commonly used patterns
4. **Validation**: Validate group references exist in the pattern
