# Named Group Apostrophe Syntax Feature

**Date:** October 21, 2025  
**Status:** ✅ COMPLETED  
**Priority:** Low  
**Feature:** `(?'name'expr)` - VBScript/legacy pattern compatibility

## Overview

This feature adds support for the VBScript and legacy-style apostrophe-based named group syntax `(?'name'expr)` in addition to the standard angle-bracket syntax `(?<name>expr)`. Both syntaxes are valid in .NET regex and produce identical behavior - this is purely a syntax choice to accommodate legacy patterns.

## Current State

- ✅ Angle-bracket named groups `(?<name>expr)` are fully supported
- ❌ Apostrophe-based named groups `(?'name'expr)` are **NOT** supported
- The `RegexNodeGroup` class currently only renders with angle-bracket syntax in `ToRegexPattern()`

## Implementation Plan

### Phase 1: Design Decisions

1. **Syntax selection method**: Add an optional parameter to track which syntax style to use
   - Option A: Add `bool useApostropheSyntax` property to `RegexNodeGroup`
   - Option B: Create a new separate `RegexNodeGroupApostrophe` class
   - **Decision**: Option A (simpler, less code duplication)

2. **API Design**: Add new overloads to `RegexBuilder` class
   - `Group(string name, RegexNode expr, bool useApostropheSyntax)` - with apostrophe option
   - `GroupApostrophe(string name, RegexNode expr)` - explicit convenience method
   - `GroupApostrophe(string name, RegexNode expr, RegexQuantifier quantifier)` - with quantifier

### Phase 2: Code Changes

#### 2.1 Update `RegexNodeGroup.cs`

- Add `bool UseApostropheSyntax { get; set; }` property (default: false)
- Update `ToRegexPattern()` method to check this property and render accordingly:
  ```csharp
  if (UseApostropheSyntax)
  {
      result = string.Format(CultureInfo.InvariantCulture, "(?'{0}'{1})", Name, InnerExpression.ToRegexPattern());
  }
  else
  {
      result = string.Format(CultureInfo.InvariantCulture, "(?<{0}>{1})", Name, InnerExpression.ToRegexPattern());
  }
  ```
- Consider updating constructor to accept an optional parameter

#### 2.2 Update `RegexBuilder.cs`

Add new public methods (maintaining backwards compatibility):

```csharp
/// <summary>
/// Generates a named capturing group using apostrophe syntax for VBScript compatibility.
/// Syntax: (?'name'expr)
/// </summary>
public static RegexNodeGroup GroupApostrophe(string groupName, RegexNode matchExpression)
{
    var group = new RegexNodeGroup(matchExpression, groupName);
    group.UseApostropheSyntax = true;
    return group;
}

public static RegexNodeGroup GroupApostrophe(string groupName, RegexNode matchExpression, RegexQuantifier quantifier)
{
    var group = new RegexNodeGroup(matchExpression, groupName) { Quantifier = quantifier };
    group.UseApostropheSyntax = true;
    return group;
}

// Optional: Overload existing Group method with flag
public static RegexNodeGroup Group(string groupName, RegexNode matchExpression, bool useApostropheSyntax)
{
    var group = new RegexNodeGroup(matchExpression, groupName);
    group.UseApostropheSyntax = useApostropheSyntax;
    return group;
}

public static RegexNodeGroup Group(string groupName, RegexNode matchExpression, bool useApostropheSyntax, RegexQuantifier quantifier)
{
    var group = new RegexNodeGroup(matchExpression, groupName) { Quantifier = quantifier };
    group.UseApostropheSyntax = useApostropheSyntax;
    return group;
}
```

### Phase 3: Testing

#### 3.1 Unit Tests (in `RegexBuilder.Tests/RegexBuilderTests.cs`)

1. **Basic apostrophe syntax rendering**
   - `(?'name'expr)` renders correctly
   - Verify apostrophe syntax is used in output

2. **Backwards compatibility**
   - Existing angle-bracket syntax still works
   - Default behavior unchanged

3. **With quantifiers**
   - `(?'name'expr)*`, `(?'name'expr)+`, etc.

4. **Integration tests**
   - Test apostrophe syntax with actual Regex matching
   - Verify both syntaxes work identically

#### 3.2 Test Cases to Add

```csharp
[TestMethod]
public void TestApostropheNamedGroup()
{
    RegexNode node = RegexBuilder.GroupApostrophe("name", RegexBuilder.Literal("abc"));
    Assert.AreEqual("(?'name'abc)", node.ToRegexPattern());
}

[TestMethod]
public void TestApostropheNamedGroupWithQuantifier()
{
    RegexNode node = RegexBuilder.GroupApostrophe("name", RegexBuilder.Literal("abc"), RegexQuantifier.OneOrMore);
    Assert.AreEqual("(?'name'abc)+", node.ToRegexPattern());
}

[TestMethod]
public void TestGroupWithApostropheFlagTrue()
{
    RegexNode node = RegexBuilder.Group("name", RegexBuilder.Literal("abc"), true);
    Assert.AreEqual("(?'name'abc)", node.ToRegexPattern());
}

[TestMethod]
public void TestGroupWithApostropheFlagFalse()
{
    RegexNode node = RegexBuilder.Group("name", RegexBuilder.Literal("abc"), false);
    Assert.AreEqual("(?<name>abc)", node.ToRegexPattern());
}

[TestMethod]
public void TestApostropheGroupRegexMatching()
{
    Regex regex = RegexBuilder.Build(
        RegexBuilder.GroupApostrophe("first", RegexBuilder.Literal("hello")),
        RegexBuilder.Literal(" "),
        RegexBuilder.GroupApostrophe("second", RegexBuilder.Literal("world"))
    );
    Match match = regex.Match("hello world");
    Assert.IsTrue(match.Success);
    Assert.AreEqual("hello", match.Groups["first"].Value);
    Assert.AreEqual("world", match.Groups["second"].Value);
}
```

### Phase 4: Documentation Updates

1. **ROADMAP.md**: Update status to ✅ COMPLETED
2. **CHANGELOG.md**: Add entry describing the new feature
3. **Code comments**: Ensure clear documentation in RegexNodeGroup and RegexBuilder methods

## Acceptance Criteria

- [x] Feature planned and documented
- [x] `RegexNodeGroup.cs` updated with apostrophe syntax property
- [x] `RegexBuilder.cs` updated with new overload methods
- [x] All unit tests pass (including new tests)
- [x] Backwards compatibility maintained (existing angle-bracket syntax still works)
- [x] ROADMAP.md marked as completed
- [x] CHANGELOG.md updated
- [x] Code formatted with `make format`
- [x] All existing tests still pass (no regressions)

## Implementation Summary

### Changes Made

1. **RegexNodeGroup.cs**
   - Added `public bool UseApostropheSyntax { get; set; }` property (defaults to false for backwards compatibility)
   - Updated `ToRegexPattern()` method to render apostrophe syntax when `UseApostropheSyntax` is true
   - Renders `(?'name'expr)` when flag is true, `(?<name>expr)` when false

2. **RegexBuilder.cs**
   - Added `GroupApostrophe(string groupName, RegexNode matchExpression)` - convenience method for apostrophe syntax
   - Added `GroupApostrophe(string groupName, RegexNode matchExpression, RegexQuantifier quantifier)` - with quantifier support
   - Both methods create a `RegexNodeGroup` with `UseApostropheSyntax = true`

3. **RegexNodeRenderingTests.cs**
   - Added `TestGroupApostropheSyntaxRendering()` - Tests rendering of apostrophe syntax with various quantifiers
   - Added `TestGroupApostropheSyntaxMatching()` - Tests functional equivalence with actual regex matching
   - Added `TestGroupApostropheSyntaxBackreference()` - Tests that backreferences work with apostrophe syntax

### Test Results

- **Apostrophe syntax tests:** 3/3 passed ✅
- **Total regression tests:** 285/285 passed ✅
- **No breaking changes:** All existing functionality preserved ✅

### API Usage Examples

```csharp
// Create a named group with apostrophe syntax
RegexNode group = RegexBuilder.GroupApostrophe("name", RegexBuilder.Literal("abc"));
// Output: (?'name'abc)

// With quantifier
RegexNode groupWithQuantifier = RegexBuilder.GroupApostrophe("name", RegexBuilder.Literal("abc"), RegexQuantifier.OneOrMore);
// Output: (?'name'abc)+

// Use in a complete pattern with backreference
Regex regex = RegexBuilder.Build(
    RegexBuilder.GroupApostrophe("word", RegexBuilder.WordCharacter(RegexQuantifier.OneOrMore)),
    RegexBuilder.Literal(" "),
    RegexBuilder.GroupBackReference("word")
);
// Matches: "hello hello" ✓
// Does not match: "hello world" ✗
```

## References

- .NET Named Groups: https://learn.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#named_matched_subexpressions
- VBScript Regex: Legacy syntax using apostrophes `(?'name'expr)`
- RegexNodeGroup implementation: `src/RegexBuilder/RegexNodeTypes/RegexNodeGroup.cs`
- RegexBuilder API: `src/RegexBuilder/RegexBuilder.cs`

## Implementation Notes

- Both syntaxes produce identical regex behavior in .NET
- Default behavior remains angle-bracket syntax for backwards compatibility
- Apostrophe syntax is provided for VBScript compatibility and legacy pattern support
- Both syntaxes work seamlessly with backreferences, quantifiers, and all other regex features
- No performance impact - this is purely a rendering choice
