# Common Patterns Library

This library provides ready-to-use regex patterns for the most common validation and extraction scenarios. Copy, paste, and customize as needed!

## How to Use This Library

All patterns in this library are designed to be:

- **Copy-paste ready**: Use them directly in your code
- **Production-tested**: Validated with comprehensive test suites
- **Customizable**: Easy to modify for your specific needs
- **Well-documented**: Clear examples of what matches and what doesn't

### Basic Usage

```csharp
using RegexBuilder;

// Use a pre-built pattern
var emailRegex = RegexBuilder.Build(CommonPatterns.Email());

// Validate input
bool isValid = emailRegex.IsMatch("user@example.com");

// With regex options
var regex = RegexBuilder.Build(
    RegexOptions.Compiled | RegexOptions.IgnoreCase,
    CommonPatterns.Email()
);
```

## Quick Reference

| Pattern                             | Usage                    | Example Match              |
| ----------------------------------- | ------------------------ | -------------------------- |
| [Email](#email-addresses)           | `CommonPatterns.Email()` | `user@example.com`         |
| [URL](#urls-and-web-addresses)      | `CommonPatterns.Url()`   | `https://example.com/path` |
| [US Phone](#phone-numbers)          | See examples below       | `555-123-4567`             |
| [US ZIP Code](#postal-codes)        | See examples below       | `12345` or `12345-6789`    |
| [IPv4 Address](#ip-addresses)       | See examples below       | `192.168.1.1`              |
| [Date (ISO)](#dates-and-time)       | See examples below       | `2025-10-22`               |
| [Credit Card](#credit-cards)        | See examples below       | `4111-1111-1111-1111`      |
| [UUID/GUID](#identifiers-and-codes) | See examples below       | `550e8400-e29b-41d4-a716`  |

---

## Email Addresses

### What It Matches

A basic email address with local part, @ symbol, domain, and TLD.

- Local part: alphanumeric, dots, hyphens, underscores, percent, plus
- Domain: alphanumeric and hyphens, separated by dots
- TLD: 2-6 alphabetic characters

### Pre-Built Pattern

```csharp
using RegexBuilder;

// Use the built-in pattern
var emailRegex = RegexBuilder.Build(CommonPatterns.Email());

// Test it
Console.WriteLine(emailRegex.IsMatch("user@example.com"));       // ✓ True
Console.WriteLine(emailRegex.IsMatch("test.user+tag@domain.co")); // ✓ True
Console.WriteLine(emailRegex.IsMatch("invalid.email"));          // ✗ False
Console.WriteLine(emailRegex.IsMatch("@nodomain.com"));          // ✗ False
```

### Examples

**✓ Matches:**

- `user@example.com`
- `first.last@company.co`
- `test+tag@domain.org`
- `user_name@sub-domain.com`

**✗ Doesn't Match:**

- `invalid.email` (no @ symbol)
- `@nodomain.com` (no local part)
- `user@` (no domain)
- `user @example.com` (space in email)

### Custom Email Pattern

Want more control? Build your own:

```csharp
// Stricter email validation
var strictEmail = RegexBuilder.Pattern()
    .Start()
    .CharacterSet("a-zA-Z0-9", 1, null)           // Must start with alphanumeric
    .CharacterSet("a-zA-Z0-9._%+-", 0, null)      // Followed by allowed chars
    .Literal("@")
    .CharacterSet("a-zA-Z0-9", 1, null)           // Domain starts with alphanumeric
    .CharacterSet("a-zA-Z0-9.-", 0, null)         // Followed by domain chars
    .Literal(".")
    .Letters(2, 6)                                 // TLD: 2-6 letters only
    .End()
    .Build();

var regex = RegexBuilder.Build(strictEmail);
```

---

## URLs and Web Addresses

### What It Matches

URLs with optional protocol, domain, path, query string, and fragment.

### Pre-Built Pattern

```csharp
using RegexBuilder;

// Use the built-in URL pattern
var urlRegex = RegexBuilder.Build(CommonPatterns.Url());

// Test it
Console.WriteLine(urlRegex.IsMatch("https://github.com/example"));     // ✓ True
Console.WriteLine(urlRegex.IsMatch("http://example.com/path?q=1"));    // ✓ True
Console.WriteLine(urlRegex.IsMatch("ftp://files.example.com"));        // ✓ True
Console.WriteLine(urlRegex.IsMatch("not a url"));                      // ✗ False
```

### Examples

**✓ Matches:**

- `https://example.com`
- `http://sub.domain.com/path`
- `https://example.com/path?query=value&more=data`
- `ftp://files.example.com/file.txt`
- `https://example.com:8080/path`

**✗ Doesn't Match:**

- `not a url`
- `htp://typo.com` (invalid protocol)
- `example` (no protocol or domain structure)

### Custom URL Pattern with Capturing Groups

```csharp
// Extract URL components
var urlPattern = RegexBuilder.Build(
    RegexBuilder.Group("protocol",
        RegexBuilder.CharacterSet("a-z", RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.Literal("://"),
    RegexBuilder.Group("domain",
        RegexBuilder.CharacterSet("a-zA-Z0-9.-", RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.Group("path",
        RegexBuilder.CharacterSet("/a-zA-Z0-9._~:?#@!$&'()*+,;=%-", RegexQuantifier.ZeroOrMore)
    )
);

var match = urlPattern.Match("https://example.com/path?q=1");
if (match.Success)
{
    Console.WriteLine($"Protocol: {match.Groups["protocol"].Value}");  // https
    Console.WriteLine($"Domain: {match.Groups["domain"].Value}");      // example.com
    Console.WriteLine($"Path: {match.Groups["path"].Value}");          // /path?q=1
}
```

---

## Phone Numbers

### US Phone Numbers

Matches various US phone number formats with optional country code.

```csharp
// Flexible US phone format
var phonePattern = RegexBuilder.Pattern()
    .Optional(o => o.Literal("+1").Optional(sep => sep.CharacterSet("- ")))
    .Group(g => g.Digits(3))               // Area code
    .Optional(o => o.CharacterSet("- ."))  // Separator
    .Group(g => g.Digits(3))               // Prefix
    .Optional(o => o.CharacterSet("- ."))  // Separator
    .Group(g => g.Digits(4))               // Line number
    .Build();

var phoneRegex = RegexBuilder.Build(phonePattern);

// Test it
Console.WriteLine(phoneRegex.IsMatch("555-123-4567"));      // ✓ True
Console.WriteLine(phoneRegex.IsMatch("+1-555-123-4567"));   // ✓ True
Console.WriteLine(phoneRegex.IsMatch("5551234567"));        // ✓ True
Console.WriteLine(phoneRegex.IsMatch("555.123.4567"));      // ✓ True
Console.WriteLine(phoneRegex.IsMatch("123-45-6789"));       // ✗ False (not phone format)
```

**✓ Matches:**

- `555-123-4567`
- `+1-555-123-4567`
- `5551234567`
- `555.123.4567`
- `(555) 123-4567` (with parentheses variant - needs adjustment)

**✗ Doesn't Match:**

- `12-345-6789` (invalid area code format)
- `555-12-34567` (wrong digit grouping)

### Phone with Parentheses

```csharp
// Format: (555) 123-4567
var phoneWithParens = RegexBuilder.Build(
    RegexBuilder.Optional(RegexBuilder.Literal("+1").Concatenate(
        RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.Optional)
    )),
    RegexBuilder.Literal("("),
    RegexBuilder.Digit(RegexQuantifier.Exactly(3)),
    RegexBuilder.Literal(")"),
    RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(3)),
    RegexBuilder.CharacterSet("- .", RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4))
);
```

---

## Postal Codes

### US ZIP Codes

```csharp
// Matches: 12345 or 12345-6789
var zipPattern = RegexBuilder.Pattern()
    .Start()
    .Digits(5)
    .Optional(o => o.Literal("-").Digits(4))
    .End()
    .Build();

var zipRegex = RegexBuilder.Build(zipPattern);

Console.WriteLine(zipRegex.IsMatch("12345"));        // ✓ True
Console.WriteLine(zipRegex.IsMatch("12345-6789"));   // ✓ True
Console.WriteLine(zipRegex.IsMatch("1234"));         // ✗ False (too short)
```

### Canadian Postal Codes

```csharp
// Format: A1A 1A1
var canadianPostal = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
    RegexBuilder.Digit(RegexQuantifier.Exactly(1)),
    RegexBuilder.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
    RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(1)),
    RegexBuilder.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
    RegexBuilder.Digit(RegexQuantifier.Exactly(1)),
    RegexBuilder.LineEnd()
);
```

**✓ Matches:**

- `K1A 0B1`
- `M5V3A8` (without space)

### UK Postcodes

```csharp
// Format: SW1A 1AA
var ukPostcode = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.CharacterSet("A-Z", new RegexQuantifier(1, 2)),
    RegexBuilder.Digit(RegexQuantifier.Exactly(1)),
    RegexBuilder.CharacterSet("A-Z0-9", RegexQuantifier.Optional),
    RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(1)),
    RegexBuilder.CharacterSet("A-Z", RegexQuantifier.Exactly(2)),
    RegexBuilder.LineEnd()
);
```

---

## Credit Cards

> **⚠️ Security Warning**: Never log, store, or transmit credit card numbers in plain text. Use these patterns for validation only, and always follow PCI DSS compliance standards.

### Visa

```csharp
// Visa: Starts with 4, 13-16 digits total
var visaPattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Literal("4"),
    RegexBuilder.Digit(new RegexQuantifier(12, 15)),
    RegexBuilder.LineEnd()
);

Console.WriteLine(visaPattern.IsMatch("4111111111111111"));  // ✓ True (test card)
```

### Mastercard

```csharp
// Mastercard: Starts with 51-55 or 2221-2720, 16 digits total
var mastercardPattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Alternate(
        RegexBuilder.Concatenate(
            RegexBuilder.Literal("5"),
            RegexBuilder.CharacterSet("1-5", RegexQuantifier.Exactly(1))
        ),
        RegexBuilder.Concatenate(
            RegexBuilder.Literal("22"),
            RegexBuilder.CharacterSet("2-9", RegexQuantifier.Exactly(1)),
            RegexBuilder.Digit(RegexQuantifier.Exactly(1))
        )
    ),
    RegexBuilder.Digit(RegexQuantifier.Exactly(12)),
    RegexBuilder.LineEnd()
);
```

### American Express

```csharp
// Amex: Starts with 34 or 37, 15 digits total
var amexPattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Literal("3"),
    RegexBuilder.CharacterSet("47", RegexQuantifier.Exactly(1)),
    RegexBuilder.Digit(RegexQuantifier.Exactly(13)),
    RegexBuilder.LineEnd()
);
```

### Generic Card with Separators

```csharp
// Matches: 4111-1111-1111-1111 or 4111 1111 1111 1111
var cardWithSeparators = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4)),
    RegexBuilder.CharacterSet("- ", RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4)),
    RegexBuilder.CharacterSet("- ", RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4)),
    RegexBuilder.CharacterSet("- ", RegexQuantifier.Optional),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4)),
    RegexBuilder.LineEnd()
);
```

---

## IP Addresses

### IPv4

```csharp
// Basic IPv4 pattern (simplified, doesn't validate ranges)
var ipv4Pattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Digit(new RegexQuantifier(1, 3)),
    RegexBuilder.Literal("."),
    RegexBuilder.Digit(new RegexQuantifier(1, 3)),
    RegexBuilder.Literal("."),
    RegexBuilder.Digit(new RegexQuantifier(1, 3)),
    RegexBuilder.Literal("."),
    RegexBuilder.Digit(new RegexQuantifier(1, 3)),
    RegexBuilder.LineEnd()
);

Console.WriteLine(ipv4Pattern.IsMatch("192.168.1.1"));    // ✓ True
Console.WriteLine(ipv4Pattern.IsMatch("10.0.0.1"));       // ✓ True
Console.WriteLine(ipv4Pattern.IsMatch("999.999.999.999")); // ✓ True (pattern doesn't validate range)
```

**Note**: This pattern validates the format but doesn't check if octets are in the valid range (0-255). For stricter validation, use additional logic.

### IPv4 with Range Validation (Classic API)

```csharp
// More accurate IPv4 validation
var octet = RegexBuilder.Alternate(
    RegexBuilder.Concatenate(
        RegexBuilder.Literal("25"),
        RegexBuilder.CharacterSet("0-5", RegexQuantifier.Exactly(1))
    ),
    RegexBuilder.Concatenate(
        RegexBuilder.Literal("2"),
        RegexBuilder.CharacterSet("0-4", RegexQuantifier.Exactly(1)),
        RegexBuilder.Digit(RegexQuantifier.Exactly(1))
    ),
    RegexBuilder.Concatenate(
        RegexBuilder.CharacterSet("01", RegexQuantifier.Optional),
        RegexBuilder.Digit(new RegexQuantifier(1, 2))
    )
);

var ipv4Strict = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    octet,
    RegexBuilder.Literal("."),
    octet,
    RegexBuilder.Literal("."),
    octet,
    RegexBuilder.Literal("."),
    octet,
    RegexBuilder.LineEnd()
);

Console.WriteLine(ipv4Strict.IsMatch("192.168.1.1"));      // ✓ True
Console.WriteLine(ipv4Strict.IsMatch("255.255.255.255"));  // ✓ True
Console.WriteLine(ipv4Strict.IsMatch("256.1.1.1"));        // ✗ False
```

---

## Dates and Time

### ISO 8601 Date (YYYY-MM-DD)

```csharp
var isoDate = RegexBuilder.Pattern()
    .Start()
    .Digits(4)                              // Year
    .Literal("-")
    .Digits(2)                              // Month
    .Literal("-")
    .Digits(2)                              // Day
    .End()
    .Build();

var dateRegex = RegexBuilder.Build(isoDate);

Console.WriteLine(dateRegex.IsMatch("2025-10-22"));  // ✓ True
Console.WriteLine(dateRegex.IsMatch("2025-1-1"));    // ✗ False (needs zero-padding)
```

### US Date (MM/DD/YYYY)

```csharp
var usDate = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Digit(new RegexQuantifier(1, 2)),
    RegexBuilder.Literal("/"),
    RegexBuilder.Digit(new RegexQuantifier(1, 2)),
    RegexBuilder.Literal("/"),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4)),
    RegexBuilder.LineEnd()
);

Console.WriteLine(usDate.IsMatch("10/22/2025"));  // ✓ True
Console.WriteLine(usDate.IsMatch("1/1/2025"));    // ✓ True
```

### Time (HH:MM or HH:MM:SS)

```csharp
var time24h = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Digit(RegexQuantifier.Exactly(2)),
    RegexBuilder.Literal(":"),
    RegexBuilder.Digit(RegexQuantifier.Exactly(2)),
    RegexBuilder.Optional(
        RegexBuilder.Concatenate(
            RegexBuilder.Literal(":"),
            RegexBuilder.Digit(RegexQuantifier.Exactly(2))
        )
    ),
    RegexBuilder.LineEnd()
);

Console.WriteLine(time24h.IsMatch("14:30"));     // ✓ True
Console.WriteLine(time24h.IsMatch("14:30:45"));  // ✓ True
```

---

## Identifiers and Codes

### UUID/GUID

```csharp
// Format: 550e8400-e29b-41d4-a716-446655440000
var uuidPattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.CharacterSet("a-fA-F0-9", RegexQuantifier.Exactly(8)),
    RegexBuilder.Literal("-"),
    RegexBuilder.CharacterSet("a-fA-F0-9", RegexQuantifier.Exactly(4)),
    RegexBuilder.Literal("-"),
    RegexBuilder.CharacterSet("a-fA-F0-9", RegexQuantifier.Exactly(4)),
    RegexBuilder.Literal("-"),
    RegexBuilder.CharacterSet("a-fA-F0-9", RegexQuantifier.Exactly(4)),
    RegexBuilder.Literal("-"),
    RegexBuilder.CharacterSet("a-fA-F0-9", RegexQuantifier.Exactly(12)),
    RegexBuilder.LineEnd()
);

Console.WriteLine(uuidPattern.IsMatch("550e8400-e29b-41d4-a716-446655440000"));  // ✓ True
```

### Hexadecimal String

```csharp
// Matches hex strings with optional 0x prefix
var hexPattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Optional(RegexBuilder.Literal("0x")),
    RegexBuilder.CharacterSet("a-fA-F0-9", RegexQuantifier.OneOrMore),
    RegexBuilder.LineEnd()
);

Console.WriteLine(hexPattern.IsMatch("0x1A2B3C"));  // ✓ True
Console.WriteLine(hexPattern.IsMatch("DEADBEEF"));  // ✓ True
```

### Social Security Number (US)

> **⚠️ Privacy Warning**: SSNs are highly sensitive. Never log, display, or store them without proper encryption and compliance measures.

```csharp
// Format: 123-45-6789
var ssnPattern = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Digit(RegexQuantifier.Exactly(3)),
    RegexBuilder.Literal("-"),
    RegexBuilder.Digit(RegexQuantifier.Exactly(2)),
    RegexBuilder.Literal("-"),
    RegexBuilder.Digit(RegexQuantifier.Exactly(4)),
    RegexBuilder.LineEnd()
);
```

---

## File Paths and Names

### Windows File Path

```csharp
// Format: C:\Users\Name\file.txt
var windowsPath = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
    RegexBuilder.Literal(":"),
    RegexBuilder.Concatenate(
        RegexBuilder.NonEscapedLiteral("\\\\"),
        RegexBuilder.CharacterSet(@"a-zA-Z0-9_\- .", RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.LineEnd()
);
```

### Unix File Path

```csharp
// Format: /home/user/file.txt
var unixPath = RegexBuilder.Build(
    RegexBuilder.LineStart(),
    RegexBuilder.Literal("/"),
    RegexBuilder.CharacterSet("a-zA-Z0-9_/.-", RegexQuantifier.OneOrMore),
    RegexBuilder.LineEnd()
);
```

### File Extension

```csharp
// Extract file extension
var fileExtPattern = RegexBuilder.Build(
    RegexBuilder.MetaCharacter(RegexMetaChars.AnyCharacter, RegexQuantifier.ZeroOrMore),
    RegexBuilder.Literal("."),
    RegexBuilder.Group("extension",
        RegexBuilder.CharacterSet("a-zA-Z0-9", RegexQuantifier.OneOrMore)
    ),
    RegexBuilder.LineEnd()
);

var match = fileExtPattern.Match("document.pdf");
Console.WriteLine(match.Groups["extension"].Value);  // pdf
```

---

## Customization Tips

### Making Patterns More Strict

```csharp
// Original: Allows any separator
.Optional(o => o.CharacterSet("- ."))

// Stricter: Only allows dash
.Literal("-")
```

### Making Patterns More Lenient

```csharp
// Original: Requires exact format
.Digits(3).Literal("-").Digits(3).Literal("-").Digits(4)

// More lenient: Optional separators
.Digits(3).Optional(o => o.CharacterSet("- .")).Digits(3).Optional(o => o.CharacterSet("- .")).Digits(4)
```

### Adding Anchors

```csharp
// Without anchors: Matches anywhere in string
var pattern = RegexBuilder.Pattern().Email().Build();

// With anchors: Must match entire string
var strictPattern = RegexBuilder.Pattern()
    .Start()
    .Email()
    .End()
    .Build();
```

---

## Performance Considerations

### Cache Compiled Patterns

```csharp
// ✓ Good: Static, compiled, reusable
private static readonly Regex EmailRegex = RegexBuilder.Build(
    RegexOptions.Compiled,
    CommonPatterns.Email()
);

// ✗ Avoid: Rebuilding every time
public bool Validate(string input)
{
    var regex = RegexBuilder.Build(CommonPatterns.Email());
    return regex.IsMatch(input);
}
```

### Use Appropriate Options

```csharp
// For case-insensitive matching
var regex = RegexBuilder.Build(
    RegexOptions.Compiled | RegexOptions.IgnoreCase,
    pattern
);

// For multiline text
var regex = RegexBuilder.Build(
    RegexOptions.Compiled | RegexOptions.Multiline,
    pattern
);
```

---

## Next Steps

- **[Getting Started](getting-started.md)** - Learn the basics of RegexBuilder
- **[API Guide](api-guide.md)** - Explore advanced features and complete API reference
- **[GitHub Examples](https://github.com/somenoe/RegexBuilder.NET9/tree/master/src/RegexBuilder.Tests)** - See more real-world examples

Happy pattern matching! 🎯
