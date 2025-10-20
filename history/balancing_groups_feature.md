# Balancing Groups Implementation Plan

**Status:** Not Started  
**Priority:** Medium  
**Date Created:** October 21, 2025  
**Target Completion:** November 4, 2025

## Overview

Implement support for balancing groups using `(?<name1-name2>expr)` syntax. This feature enables matching of nested/balanced constructs such as parentheses, XML tags, and code blocks. Balancing groups work in conjunction with regular named groups to track nesting depth.

## Feature Scope

### Syntax

- **Basic balancing group:** `(?<name>-expr)` - Capture only if stack operations succeed
- **Balancing group with two names:** `(?<name1-name2>expr)` - Capture to `name1` and pop `name2` from stack
- **Supported use cases:**
  - Matching balanced parentheses: `\( (?:[^()] | (?<open>\() | (?<-open>\)))*+ \)`
  - Matching nested XML tags: `<tag> (?: [^<>] | (?<tag><\w+>) | (?<-tag></\w+>) )* </tag>`
  - Matching nested code blocks: `\{ (?: [^{}] | (?<brace>\{) | (?<-brace>\}) )* \}`

### Limitations & Edge Cases

- `.NET Regex` supports balancing groups through the syntax `(?<name1-name2>expr)`
- Balancing groups are NOT supported in JavaScript or Python regex engines
- The feature requires understanding of atomic groups and backtracking behavior
- Stack overflow handling is implementation-dependent

## Implementation Steps

### Step 1: Create RegexNodeBalancingGroup Class

**File:** `src/RegexBuilder/RegexNodeTypes/RegexNodeBalancingGroup.cs`

**Responsibilities:**

- Represent a balancing group node
- Store the push group name (`name1` in `(?<name1-name2>expr)`)
- Store the pop group name (`name2` in `(?<name1-name2>expr)`)
- Store inner expression
- Allow optional quantifier
- Support single-name form (`(?<name>-expr)`)
- Generate proper pattern string

**Key Properties:**

```csharp
public string PushGroupName { get; set; }    // name1
public string PopGroupName { get; set; }     // name2
public RegexNode InnerExpression { get; set; }
public bool IsSimpleBalancing { get; set; }  // true if only PushGroupName is used
```

**Methods:**

- Constructor(pushGroupName, popGroupName, innerExpression)
- Constructor(pushGroupName, innerExpression) - For simple balancing `(?<name>-expr)`
- `ToRegexPattern()` - Returns `(?<name1-name2>expr)` or `(?<name>-expr)`

### Step 2: Add Public API Methods to RegexBuilder

**File:** `src/RegexBuilder/RegexBuilder.cs`

**Methods to add:**

```csharp
/// <summary>
/// Creates a balancing group that pushes to one named stack and pops from another.
/// Used for matching nested/balanced constructs like parentheses or XML tags.
/// </summary>
/// <param name="pushGroupName">Name of group to push matched text onto</param>
/// <param name="popGroupName">Name of group to pop from stack</param>
/// <param name="innerExpression">The inner expression to match</param>
/// <returns>RegexNodeBalancingGroup instance</returns>
public static RegexNodeBalancingGroup BalancingGroup(
    string pushGroupName,
    string popGroupName,
    RegexNode innerExpression)

/// <summary>
/// Creates a balancing group with quantifier.
/// </summary>
public static RegexNodeBalancingGroup BalancingGroup(
    string pushGroupName,
    string popGroupName,
    RegexNode innerExpression,
    RegexQuantifier quantifier)

/// <summary>
/// Creates a simple balancing group that only uses one stack.
/// Syntax: (?<name>-expr)
/// </summary>
public static RegexNodeBalancingGroup SimpleBalancingGroup(
    string groupName,
    RegexNode innerExpression)

/// <summary>
/// Creates a simple balancing group with quantifier.
/// </summary>
public static RegexNodeBalancingGroup SimpleBalancingGroup(
    string groupName,
    RegexNode innerExpression,
    RegexQuantifier quantifier)
```

### Step 3: Add Validation & Helper Methods

**File:** `src/RegexBuilder/HelperClasses/RegexMetaChars.cs` (extend existing)

**Functions:**

- Validate group names (alphanumeric + underscore)
- Provide example patterns for common use cases
- Document stack operation behavior

### Step 4: Write Comprehensive Unit Tests

**File:** `src/RegexBuilder.Tests/RegexNodeRenderingTests.cs` (extend existing)

**Test cases:**

#### Basic Balancing Groups

- `(?<name1-name2>expr)` basic syntax rendering
- `(?<name>-expr)` simple balancing syntax rendering
- Pattern combinations with inner expressions

#### Practical Use Cases

- **Balanced parentheses:**

  ```
  \( (?:[^()] | (?<paren>\() | (?<-paren>\)))*+ \)
  ```

  Should match: `(text)`, `(nested (text))`, `(multiple (levels (deep)))`

- **XML tags:**

  ```
  <\w+> (?:[^<>] | (?<tag><\w+>) | (?<-tag></\w+>))* </\w+>
  ```

  Should match: `<tag>content</tag>`, nested tags, etc.

- **Code blocks:**
  ```
  \{ (?:[^{}] | (?<brace>\{) | (?<-brace>\}))*+ \}
  ```

#### Integration Tests

- Combining balancing groups with other regex constructs
- Alternation with balancing groups
- Quantifiers on balancing groups

#### Edge Cases

- Empty group names (should throw)
- Null inner expression (should throw)
- Same push/pop group name
- Complex nesting scenarios

### Step 5: Integration & Documentation

**File Updates:**

1. **CHANGELOG.md**

   ```markdown
   - Added support for balancing groups `(?<name1-name2>expr)` and `(?<name>-expr)`
     for matching nested/balanced constructs
   - New API methods: `BalancingGroup()` and `SimpleBalancingGroup()`
   ```

2. **ROADMAP.md**
   - Mark "Balancing Groups" as completed [x]

3. **README.md** (if needed)
   - Add example usage of balancing groups
   - Show practical use cases with balanced parentheses/XML

## Testing Strategy

### Unit Tests Coverage Goals

- **Syntax rendering:** 100% coverage of all `ToRegexPattern()` paths
- **Validation:** Test null/empty input handling
- **Integration:** Combine with Concatenation, Alternation, Quantifiers

### Functional Tests

- Test actual regex matching using built patterns
- Verify balanced constructs work as expected
- Test with real-world scenarios (XML, JSON brackets, code)

### Regression Tests

- Ensure no existing functionality is broken
- Run full test suite after implementation

## Implementation Order

1. ✅ Create `RegexNodeBalancingGroup.cs` class
2. ✅ Add methods to `RegexBuilder.cs`
3. ✅ Add validation helpers to `RegexMetaChars.cs` if needed
4. ✅ Write comprehensive tests
5. ✅ Update `CHANGELOG.md` and `ROADMAP.md`
6. ✅ Run full test suite and verify all pass
7. ✅ Code formatting with `make format`

## References

- [MS .NET Balancing Groups](https://learn.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#balancing_group_definition)
- [Stack overflow example](https://stackoverflow.com/questions/1732348/regex-match-open-tags-except-xhtml-self-contained-tags/1732454#1732454)
- [Practical balanced regex patterns](https://www.regular-expressions.info/balancing.html)

## Success Criteria

- [ ] `RegexNodeBalancingGroup` class created and compiles
- [ ] All `BalancingGroup()` and `SimpleBalancingGroup()` methods implemented in `RegexBuilder`
- [ ] All unit tests pass (targeting 20+ test cases)
- [ ] Practical examples work (balanced parentheses, XML tags, code blocks)
- [ ] `CHANGELOG.md` and `ROADMAP.md` updated
- [ ] Code formatted with Prettier and dotnet format
- [ ] Feature branch merged to dev

## Estimated Effort

- Code implementation: 2-3 hours
- Unit testing: 2-3 hours
- Integration testing: 1 hour
- Documentation: 1 hour
- **Total: 6-8 hours**

## Known Risks & Mitigations

| Risk                                      | Mitigation                                                 |
| ----------------------------------------- | ---------------------------------------------------------- |
| Complex regex patterns may be error-prone | Write comprehensive unit tests covering edge cases         |
| Stack behavior edge cases                 | Reference MS documentation, add clear comments             |
| Performance with deep nesting             | Document nesting depth limitations                         |
| API design confusion with two names       | Provide clear method names and documentation with examples |
