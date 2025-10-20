# RegexBuilder.NET9 Feature Roadmap

**Last Updated:** October 21, 2025

## Core Regex Features (Already Implemented)

### Character Escapes & Classes

- [x] ASCII characters (`\xNN`)
- [x] Unicode characters (`\uNNNN`)
- [x] Character sets `[abc]`, negated `[^abc]`
- [x] Character ranges `[a-z]`, negated `[^a-z]`

### Anchors & Boundaries

- [x] String anchors (`^`, `$`, `\A`, `\Z`, `\z`)
- [x] Word boundaries (`\b`, `\B`)
- [x] Previous match point (`\G`)

### Groups & Capturing

- [x] Unnamed capturing groups `(expr)`
- [x] Named capturing groups `(?<name>expr)`
- [x] Non-capturing groups `(?:expr)`
- [ ] Apostrophe-based named groups `(?'name'expr)`
- [ ] Balancing groups `(?<name1-name2>expr)`

### Quantifiers

- [x] Greedy quantifiers (`*`, `+`, `?`, `{n}`, `{n,}`, `{n,m}`)
- [x] Lazy quantifiers (`*?`, `+?`, `??`, `{n}?`, `{n,}?`, `{n,m}?`)

### Lookarounds

- [x] Positive lookahead `(?=expr)`
- [x] Negative lookahead `(?!expr)`
- [x] Positive lookbehind `(?<=expr)`
- [x] Negative lookbehind `(?<!expr)`

### Advanced Constructs

- [x] Backreferences (numbered `\1`, named `\k<name>`)
- [x] Atomic groups / Backtracking suppression `(?>expr)`
- [x] Alternation `a|b|c`
- [x] Conditional matching `(?(condition)yes|no)`
- [x] Inline comments `(?#comment)`
- [x] Inline options `(?i)`, `(?m)`, `(?s)`, `(?x)`

---

## Missing Features to Implement

### 🔴 HIGH PRIORITY

- [x] **Unicode Category Matching** `\p{name}`, `\P{name}`
  - Enables `\p{L}` (letters), `\p{N}` (numbers), `\p{IsCyrillic}` (blocks)
  - Essential for international text processing
  - ✅ COMPLETED: Full support for Unicode categories, blocks, and negated patterns

### 🟠 MEDIUM PRIORITY

- [ ] **Substitution Pattern Audit** (Verify `SubstitutionBuilder` completeness)
  - Verify support: `$number`, `${name}`, `$$`, `$&`, `` $` ``, `$'`, `$+`, `$_`
- [ ] **Balancing Groups** `(?<name1-name2>expr)`
  - For nested/balanced pattern matching (parentheses, XML tags, code blocks)

### 🟡 LOW PRIORITY

- [ ] **Convenience Shortcut Methods**
  - Character classes: `Digit()`, `NonDigit()`, `Whitespace()`, `NonWhitespace()`, `WordCharacter()`, `NonWordCharacter()`
  - Anchors: `LineStart()`, `LineEnd()`, `StringStart()`, `StringEnd()`, `StringEndAbsolute()`, `WordBoundary()`, `NonWordBoundary()`, `MatchPointAnchor()`
  - Escapes: `BellCharacter()`, `FormFeed()`, `VerticalTab()`, `EscapeCharacter()`, `OctalCharacter()`

- [ ] **Inline Option Grouping** `(?imnsx-imnsx:expr)`
  - Dedicated node for scoped option application

- [ ] **Named Group Apostrophe Syntax** `(?'name'expr)`
  - For compatibility with VBScript and legacy patterns

---

## References

- [MS .NET Regex Quick Reference](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)
- [Character Classes](https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions)
- [Unicode Categories](https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#SupportsUnicodeCategories)
- [Balancing Groups](https://learn.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#balancing_group_definition)
