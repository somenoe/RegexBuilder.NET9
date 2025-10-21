# Fluent Builder Pattern Implementation Plan

## Feature Overview

Add a fluent builder pattern to compose complex regex patterns with improved API ergonomics.

## Current State

- Each pattern is created independently using static `RegexBuilder` methods
- Manual composition required using `Concatenate()` or `Alternation()`
- Users need to manage node nesting themselves

## Proposed Solution

Create a `PatternBuilder` class that provides a fluent API for composing patterns with:

- Method chaining for sequential operations
- Logical operators (Or, And)
- Anchor shortcuts (Start, End)
- Quantifier support on builder methods
- Build() method to generate final RegexNode

## Implementation Plan

### Step 1: Create PatternBuilder Class

**File**: `src/RegexBuilder/PatternBuilder.cs`

**Responsibilities**:

- Maintain internal list of RegexNode elements
- Provide fluent API methods that return `this`
- Handle concatenation and alternation logic
- Build final RegexNode tree structure

**Key Methods**:

- `Start()` - Add start anchor (^)
- `End()` - Add end anchor ($)
- `Literal(string)` - Add literal text
- `Digits(int? min, int? max)` - Add digit pattern with quantifier
- `Letters(int? min, int? max)` - Add letter pattern with quantifier
- `Whitespace(int? min, int? max)` - Add whitespace pattern
- `CharacterSet(string, int? min, int? max)` - Add character set with quantifier
- `Group(Action<PatternBuilder>)` - Add grouped pattern
- `Or(Action<PatternBuilder>)` - Add alternation
- `Or(RegexNode)` - Add alternation with existing node
- `Optional(Action<PatternBuilder>)` - Make pattern optional
- `Build()` - Generate final RegexNode

### Step 2: Add Factory Method to RegexBuilder

**File**: `src/RegexBuilder/RegexBuilder.cs`

Add static factory method:

```csharp
public static PatternBuilder Pattern() => new PatternBuilder();
```

### Step 3: Internal State Management

**PatternBuilder Internal Design**:

- `List<RegexNode> _nodes` - Sequential nodes to concatenate
- `List<List<RegexNode>> _alternations` - Track OR branches
- `bool _startAdded`, `bool _endAdded` - Track anchors
- Helper method: `RegexNode BuildInternal()` - Construct node tree

### Step 4: Support for Common Patterns

Add convenience methods that leverage CommonPatterns:

- `Email()` - Add email pattern
- `Url()` - Add URL pattern
- `Pattern(RegexNode)` - Add custom existing pattern

### Step 5: Comprehensive Testing

**File**: `src/RegexBuilder.Tests/PatternBuilderTests.cs`

Test cases:

1. Simple sequential pattern
2. Pattern with quantifiers
3. Alternation (Or) patterns
4. Nested groups
5. Start/End anchors
6. Mixed complex patterns
7. Empty builder edge cases
8. Integration with existing RegexBuilder patterns

### Step 6: Documentation & Examples

**File**: `src/RegexBuilder.Examples/Program.cs`

Add examples:

1. Basic fluent pattern
2. Complex pattern with alternation
3. Grouped patterns
4. Mixed fluent and traditional API

**File**: `README.md`

- Add "Fluent Builder Pattern" section
- Show comparison: traditional vs fluent API
- Add code examples

## API Design Examples

### Example 1: Simple Sequential Pattern

```csharp
var pattern = RegexBuilder.Pattern()
    .Start()
    .Literal("ID-")
    .Digits(3, 5)
    .End()
    .Build();
// Generates: ^ID-\d{3,5}$
```

### Example 2: Alternation Pattern

```csharp
var pattern = RegexBuilder.Pattern()
    .Start()
    .Literal("ID-")
    .Digits(3, 5)
    .Or(builder => builder.Literal("CODE-").Letters(2, 4))
    .End()
    .Build();
// Generates: ^(?:ID-\d{3,5}|CODE-[a-zA-Z]{2,4})$
```

### Example 3: Nested Groups

```csharp
var pattern = RegexBuilder.Pattern()
    .Literal("prefix-")
    .Group(g => g
        .Digits(1, null)
        .Literal("-")
        .Letters(2, 3))
    .Build();
// Generates: prefix-(\d+-[a-zA-Z]{2,3})
```

### Example 4: Optional Patterns

```csharp
var pattern = RegexBuilder.Pattern()
    .Literal("https")
    .Optional(o => o.Literal("s"))
    .Literal("://")
    .Build();
// Generates: https?://
```

## Implementation Steps

### Phase 1: Core Builder (Steps 1-2)

- [ ] Create PatternBuilder class with basic infrastructure
- [ ] Implement Start(), End(), Literal() methods
- [ ] Implement Build() method with concatenation logic
- [ ] Add factory method to RegexBuilder
- [ ] Write basic tests

### Phase 2: Quantifiers & Character Classes (Step 3)

- [ ] Implement Digits(), Letters(), Whitespace() with quantifier support
- [ ] Implement CharacterSet() with quantifier support
- [ ] Add AnyCharacter(), Word(), etc.
- [ ] Write quantifier tests

### Phase 3: Alternation & Grouping (Step 3 continued)

- [ ] Implement Or() with callback and RegexNode overloads
- [ ] Implement Group() for capturing groups
- [ ] Implement Optional() helper
- [ ] Handle alternation logic in Build()
- [ ] Write alternation and grouping tests

### Phase 4: Integration & Polish (Steps 4-6)

- [ ] Add CommonPatterns shortcuts
- [ ] Add Pattern() method for custom nodes
- [ ] Complete comprehensive test suite
- [ ] Add examples to Program.cs
- [ ] Update README.md with fluent API section
- [ ] Update CHANGELOG.md

## Technical Considerations

### Alternation Logic

When `Or()` is called:

1. Store current nodes as one branch
2. Start new branch for Or content
3. In Build(), wrap all branches in Alternation node
4. Wrap in non-capturing group to avoid precedence issues

### Quantifier Handling

Methods like `Digits(min, max)` should:

1. Create base pattern (e.g., `\d`)
2. Wrap in RegexQuantifier if min/max provided
3. Add to internal nodes list

### Anchor Handling

- `Start()` and `End()` should set flags
- During Build(), add anchors at appropriate positions
- Validate that anchors aren't duplicated

## Success Criteria

- [ ] Fluent API works for all example use cases
- [ ] Generated regex patterns match expected output
- [ ] All tests pass (100% coverage)
- [ ] Documentation updated with examples
- [ ] Code follows existing style and conventions
- [ ] No breaking changes to existing API

## Estimated Complexity

- **Core Implementation**: ~200-300 lines
- **Tests**: ~300-400 lines
- **Documentation**: ~50-100 lines
- **Total**: ~600-800 lines

## Potential Edge Cases

1. Empty builder (Build() with no nodes)
2. Multiple Or() calls in sequence
3. Or() after alternation already started
4. Nested Group() with Or() inside
5. Quantifiers on groups vs individual patterns
6. Start()/End() called multiple times

## Follow-up Enhancements (Future)

- Add `Repeat(pattern, min, max)` for explicit repetition
- Add `OneOf(params RegexNode[])` for character alternation
- Add `Lazy()` modifier for non-greedy quantifiers
- Add `Named(string name, Action<PatternBuilder>)` for named groups
- Add `Condition()` for conditional patterns
