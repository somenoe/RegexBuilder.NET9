# Unicode Category Matching Implementation Plan

**Status:** In Progress  
**Priority:** High  
**Date Created:** October 21, 2025

## Overview

Implement support for Unicode category matching using `\p{name}` and `\P{name}` syntax to enable matching of international text patterns.

## Feature Scope

- `\p{L}` - Letters (all Unicode letters)
- `\p{N}` - Numbers (all Unicode numbers)
- `\p{P}` - Punctuation
- `\p{S}` - Symbols
- `\p{Z}` - Separators
- `\p{C}` - Control characters
- `\p{IsCyrillic}` - Unicode blocks (e.g., Cyrillic, Latin, Arabic, etc.)
- `\P{name}` - Negated Unicode categories (inverse of `\p{name}`)

## Implementation Steps

### 1. Create New RegexNode Type
- **File:** `src/RegexBuilder/RegexNodeTypes/RegexNodeUnicodeCategory.cs`
- **Responsibilities:**
  - Represent Unicode category escape sequences
  - Store category name (e.g., "L", "N", "IsCyrillic")
  - Store negation flag (for `\P{}`)
  - Generate proper `\p{}` or `\P{}` patterns
  - Allow quantifiers (optional)

### 2. Add Public API Methods to RegexBuilder
- **File:** `src/RegexBuilder/RegexBuilder.cs`
- **Methods to add:**
  - `UniqueCategory(string categoryName)` - Creates a `\p{name}` node
  - `UniqueCategory(string categoryName, bool negated)` - Creates `\P{name}` if negated=true
  - `UnicodeCategory(string categoryName, RegexQuantifier quantifier)` - With quantifier
  - `UnicodeCategory(string categoryName, bool negated, RegexQuantifier quantifier)` - Full version

### 3. Add Validation & Helper Methods
- **File:** `src/RegexBuilder/HelperClasses/RegexMetaChars.cs` (or new file)
- **Functions:**
  - Validate Unicode category names (whitelist approach)
  - Map common names to standard Unicode categories
  - Generate documentation of supported categories

### 4. Write Comprehensive Unit Tests
- **File:** `src/RegexBuilder.Tests/RegexNodeRenderingTests.cs` or new test file
- **Test cases:**
  - Basic categories: `\p{L}`, `\p{N}`, etc.
  - Negated categories: `\P{L}`, `\P{N}`, etc.
  - Unicode blocks: `\p{IsCyrillic}`, `\p{IsLatin}`, etc.
  - With quantifiers: `\p{L}+`, `\p{N}{2,5}`, etc.
  - Integration tests: Matching international text

### 5. Update Documentation
- **Files:**
  - `CHANGELOG.md` - Add feature entry
  - `ROADMAP.md` - Mark as completed
  - `README.md` - Add usage examples

## Technical Details

### RegexNodeUnicodeCategory Class Structure

```csharp
public class RegexNodeUnicodeCategory : RegexNode
{
    public string CategoryName { get; set; }
    public bool IsNegated { get; set; }
    
    public override string ToRegexPattern()
    {
        // Returns "\p{name}" or "\P{name}"
    }
}
```

### Supported Unicode Categories (Standard .NET)

**General Categories:**
- `L` - Letter
- `Lu` - Uppercase Letter
- `Ll` - Lowercase Letter
- `Lt` - Titlecase Letter
- `Lm` - Modifier Letter
- `Lo` - Other Letter
- `N` - Number
- `Nd` - Decimal Number
- `Nl` - Letter Number
- `No` - Other Number
- `P` - Punctuation
- `Pc` - Connector Punctuation
- `Pd` - Dash Punctuation
- `Ps` - Open Punctuation
- `Pe` - Close Punctuation
- `Pi` - Initial Quote Punctuation
- `Pf` - Final Quote Punctuation
- `Po` - Other Punctuation
- `M` - Mark
- `Mn` - Nonspacing Mark
- `Mc` - Spacing Mark
- `Me` - Enclosing Mark
- `Z` - Separator
- `Zs` - Space Separator
- `Zl` - Line Separator
- `Zp` - Paragraph Separator
- `S` - Symbol
- `Sm` - Math Symbol
- `Sc` - Currency Symbol
- `Sk` - Modifier Symbol
- `So` - Other Symbol
- `C` - Other
- `Cc` - Control
- `Cf` - Format
- `Cs` - Surrogate
- `Co` - Private Use
- `Cn` - Unassigned

**Named Blocks (Examples):**
- `IsBasicLatin`
- `IsLatin1Supplement`
- `IsLatinExtended-A`
- `IsGreek`
- `IsCyrillic`
- `IsArmenian`
- `IsHebrew`
- `IsArabic`
- `IsDevanagari`
- `IsBengali`
- `IsGurmukhi`
- `IsGujarati`
- `IsOriya`
- `IsTamil`
- `IsTelugu`
- `IsKannada`
- `IsMalayalam`
- `IsThai`
- `IsLao`
- `IsTibetan`
- `IsMyanmar`
- `IsGeorgian`
- `IsHangul`
- `IsEthiopic`
- `IsCherokee`
- `IsKhmer`
- `IsMongolian`
- `IsJamo`
- `IsHiragana`
- `IsKatakana`
- `IsBopomofo`
- `IsHanCompatibilityJamo`
- `IsKanbun`
- `IsEnclosedCharacters`
- `IsCJKUnifiedIdeographs`
- `IsPrivateUseArea`
- `IsCJKCompatibilityIdeographs`
- `IsAlphabeticPresentationForms`
- `IsArabicPresentationFormsA`
- `IsVariationSelectors`
- `IsArabicPresentationFormsB`
- `IsCJKCompatibilityIdeographsSupplement`
- `IsTags`

## Acceptance Criteria

- [ ] `RegexNodeUnicodeCategory` class created and tested
- [ ] Public API methods added to `RegexBuilder`
- [ ] Basic category matching works (`\p{L}`, `\p{N}`, etc.)
- [ ] Negated categories work (`\P{L}`, `\P{N}`, etc.)
- [ ] Unicode blocks work (`\p{IsCyrillic}`, etc.)
- [ ] Quantifiers properly applied
- [ ] Unit tests pass (100% coverage)
- [ ] Integration tests validate real-world usage
- [ ] Documentation updated
- [ ] Feature marked as completed in ROADMAP.md

## Related Issues

- None currently

## References

- [MS .NET Unicode Categories](https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#SupportsUnicodeCategories)
- [Unicode Standard](https://unicode.org/)
