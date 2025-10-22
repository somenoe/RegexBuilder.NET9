using System.Text.RegularExpressions;
using RB = global::RegexBuilder.RegexBuilder;
using RegexMetaChars = global::RegexBuilder.RegexMetaChars;
using RegexQuantifier = global::RegexBuilder.RegexQuantifier;
using SubstitutionBuilder = global::RegexBuilder.SubstitutionBuilder;

namespace RegexBuilder.Examples;

/// <summary>
/// Demonstrates all code examples from README.md
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== RB.NET9 README Examples ===\n");

        RunQuickStartExample();
        RunRealWorldExample();
        RunBasicPatternBuildingExamples();
        RunGroupingAndCapturingExamples();
        RunFluentPatternBuilderExamples();
        RunCommonPatternsExamples();
        RunBalancingGroupExample();
        RunUnicodeCategoryExample();
        RunInlineOptionGroupingExample();
        RunConditionalMatchExample();
        RunBacktrackingSuppressionExample();
        RunIpv4StrictExample();
        RunLookaroundAssertionsExample();
        RunBackreferenceExample();
        RunCharacterRangeExample();
        RunPhoneNumberExamples();
        RunPostalCodeExamples();
        RunDateTimeExamples();
        RunSubstitutionPatternExample();

        Console.WriteLine("\n=== All Examples Completed Successfully ===");
    }    /// <summary>
         /// Quick Start Example from README
         /// </summary>
    static void RunQuickStartExample()
    {
        Console.WriteLine("--- Quick Start: Email Regex ---");

        var emailRegex = RB.Build(
            RB.Group(
                "localPart",
                RB.CharacterSet(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)
            ),
            RB.Literal("@"),
            RB.Group(
                "domain",
                RB.Concatenate(
                    RB.CharacterSet(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore),
                    RB.Literal("."),
                    RB.CharacterSet(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)
                )
            )
        );

        var match = emailRegex.Match("user@example.com");
        if (match.Success)
        {
            Console.WriteLine($"  Local Part: {match.Groups["localPart"].Value}"); // user
            Console.WriteLine($"  Domain: {match.Groups["domain"].Value}");        // example.com
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Real-World Example: HTML href attribute extraction
    /// </summary>
    static void RunRealWorldExample()
    {
        Console.WriteLine("--- Real-World Example: Extract href Attributes ---");

        // Traditional regex (shown for comparison)
        Regex traditionalRegex = new Regex(
            "href\\s*=\\s*(?:[\"'](?<Target>[^\"']*)[\"']|(?<Target>\\S+))",
            RegexOptions.IgnoreCase
        );

        // With RegexBuilder (self-documenting)
        const string quotationMark = "\"";
        Regex hrefRegex = RB.Build(
            RegexOptions.IgnoreCase,
            RB.Literal("href"),
            RB.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrMore),
            RB.Literal("="),
            RB.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrMore),
            RB.Alternate(
                RB.Concatenate(
                    RB.NonEscapedLiteral(quotationMark),
                    RB.Group(
                        "Target",
                        RB.NegativeCharacterSet(quotationMark, RegexQuantifier.ZeroOrMore)
                    ),
                    RB.NonEscapedLiteral(quotationMark)
                ),
                RB.Group(
                    "Target",
                    RB.MetaCharacter(RegexMetaChars.NonwhiteSpace, RegexQuantifier.OneOrMore)
                )
            )
        );

        // Test both patterns
        string[] testHtml = new[]
        {
            "<a href=\"https://example.com\">Link 1</a>",
            "<a href='https://github.com'>Link 2</a>",
            "<a href=https://microsoft.com>Link 3</a>"
        };

        Console.WriteLine("  Testing traditional regex:");
        foreach (var html in testHtml)
        {
            var match = traditionalRegex.Match(html);
            if (match.Success)
            {
                Console.WriteLine($"    Found: {match.Groups["Target"].Value}");
            }
        }

        Console.WriteLine("\n  Testing RegexBuilder pattern:");
        foreach (var html in testHtml)
        {
            var match = hrefRegex.Match(html);
            if (match.Success)
            {
                Console.WriteLine($"    Found: {match.Groups["Target"].Value}");
            }
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Basic Pattern Building Examples from README
    /// </summary>
    static void RunBasicPatternBuildingExamples()
    {
        Console.WriteLine("--- Basic Pattern Building ---");

        // Simple pattern
        var simplePattern = RB.Build(
            RB.Literal("prefix-"),
            RB.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore)
        );
        Console.WriteLine("  Simple pattern matches:");
        Console.WriteLine($"    'prefix-123': {simplePattern.IsMatch("prefix-123")}");
        Console.WriteLine($"    'prefix-42': {simplePattern.IsMatch("prefix-42")}");
        Console.WriteLine($"    'prefix-abc': {simplePattern.IsMatch("prefix-abc")}");

        // With options
        var patternWithOptions = RB.Build(
            RegexOptions.IgnoreCase,
            RB.Literal("hello"),
            RB.Literal(" "),
            RB.Literal("world")
        );
        Console.WriteLine("\n  Pattern with IgnoreCase:");
        Console.WriteLine($"    'hello world': {patternWithOptions.IsMatch("hello world")}");
        Console.WriteLine($"    'HELLO WORLD': {patternWithOptions.IsMatch("HELLO WORLD")}");
        Console.WriteLine($"    'Hello World': {patternWithOptions.IsMatch("Hello World")}");

        Console.WriteLine();
    }

    /// <summary>
    /// Grouping and Capturing Examples from README
    /// </summary>
    static void RunGroupingAndCapturingExamples()
    {
        Console.WriteLine("--- Grouping and Capturing ---");

        // Capturing group
        var capturingPattern = RB.Build(
            RB.Group("name",
                RB.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)
            )
        );
        var match = capturingPattern.Match("Alice");
        Console.WriteLine($"  Capturing group 'name': {match.Groups["name"].Value}");

        // Non-capturing group
        var nonCapturingPattern = RB.Build(
            RB.NonCapturingGroup(
                RB.Alternate(
                    RB.Literal("cat"),
                    RB.Literal("dog")
                )
            )
        );
        Console.WriteLine("\n  Non-capturing group alternation:");
        Console.WriteLine($"    'cat': {nonCapturingPattern.IsMatch("cat")}");
        Console.WriteLine($"    'dog': {nonCapturingPattern.IsMatch("dog")}");
        Console.WriteLine($"    'bird': {nonCapturingPattern.IsMatch("bird")}");

        Console.WriteLine();
    }

    /// <summary>
    /// Fluent Pattern Builder Examples
    /// </summary>
    static void RunFluentPatternBuilderExamples()
    {
        Console.WriteLine("=== Fluent Pattern Builder Examples ===\n");

        // Example 1: Simple sequential pattern
        Console.WriteLine("Example 1: Sequential Pattern (ID followed by digits)");
        var idPattern = RB.Pattern()
            .Literal("ID-")
            .Digits(3, 5)
            .Build();

        var idRegex = RB.Build(idPattern);
        Console.WriteLine($"  Pattern: {idPattern.ToRegexPattern()}");
        Console.WriteLine($"  'ID-123' matches: {idRegex.IsMatch("ID-123")}");
        Console.WriteLine($"  'ID-12' matches: {idRegex.IsMatch("ID-12")}");

        // Example 2: Pattern with anchors
        Console.WriteLine("\nExample 2: Pattern with Anchors (start/end)");
        var anchoredPattern = RB.Pattern()
            .Start()
            .Literal("test")
            .Digits(1, 3)
            .End()
            .Build();

        var anchoredRegex = RB.Build(anchoredPattern);
        Console.WriteLine($"  Pattern: {anchoredPattern.ToRegexPattern()}");
        Console.WriteLine($"  'test123' matches: {anchoredRegex.IsMatch("test123")}");
        Console.WriteLine($"  'prefix test123 suffix' matches: {anchoredRegex.IsMatch("prefix test123 suffix")}");

        // Example 3: Pattern with alternation (OR)
        Console.WriteLine("\nExample 3: Alternation Pattern (ID or CODE)");
        var alternationPattern = RB.Pattern()
            .Start()
            .Literal("ID-")
            .Digits(3, 5)
            .Or(o => o.Literal("CODE-").Letters(2, 4))
            .End()
            .Build();

        var alternationRegex = RB.Build(alternationPattern);
        Console.WriteLine($"  Pattern: {alternationPattern.ToRegexPattern()}");
        Console.WriteLine($"  'ID-123' matches: {alternationRegex.IsMatch("ID-123")}");
        Console.WriteLine($"  'CODE-AB' matches: {alternationRegex.IsMatch("CODE-AB")}");
        Console.WriteLine($"  'OTHER-123' matches: {alternationRegex.IsMatch("OTHER-123")}");

        // Example 4: Pattern with groups
        Console.WriteLine("\nExample 4: Grouped Pattern");
        var groupPattern = RB.Pattern()
            .Literal("Name: ")
            .Group(g => g
                .Letters(1, null)
                .Whitespace()
                .Letters(1, null))
            .Build();

        var groupRegex = RB.Build(groupPattern);
        Console.WriteLine($"  Pattern: {groupPattern.ToRegexPattern()}");
        var match = groupRegex.Match("Name: John Smith");
        Console.WriteLine($"  'Name: John Smith' matches: {match.Success}");
        if (match.Success)
        {
            Console.WriteLine($"  Captured group: '{match.Groups[1].Value}'");
        }

        // Example 5: Optional pattern
        Console.WriteLine("\nExample 5: Optional Pattern (http/https)");
        var urlPattern = RB.Pattern()
            .Literal("http")
            .Optional(o => o.Literal("s"))
            .Literal("://")
            .WordCharacter(1, null)
            .Build();

        var urlRegex = RB.Build(urlPattern);
        Console.WriteLine($"  Pattern: {urlPattern.ToRegexPattern()}");
        Console.WriteLine($"  'https://example' matches: {urlRegex.IsMatch("https://example")}");
        Console.WriteLine($"  'http://example' matches: {urlRegex.IsMatch("http://example")}");

        // Example 6: Complex pattern with multiple operators
        Console.WriteLine("\nExample 6: Complex Phone Number Pattern");
        var phonePattern = RB.Pattern()
            .Optional(o => o.Literal("+1"))
            .Optional(o => o.Literal("-"))
            .Group(g => g.Digits(3))
            .Optional(o => o.Literal("-"))
            .Group(g => g.Digits(3))
            .Optional(o => o.Literal("-"))
            .Group(g => g.Digits(4))
            .Build();

        var phoneRegex = RB.Build(phonePattern);
        Console.WriteLine($"  Pattern: {phonePattern.ToRegexPattern()}");
        Console.WriteLine($"  '555-123-4567' matches: {phoneRegex.IsMatch("555-123-4567")}");
        Console.WriteLine($"  '+1-555-123-4567' matches: {phoneRegex.IsMatch("+1-555-123-4567")}");
        Console.WriteLine($"  '5551234567' matches: {phoneRegex.IsMatch("5551234567")}");

        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrate CommonPatterns usage (email/url)
    /// </summary>
    static void RunCommonPatternsExamples()
    {
        Console.WriteLine("--- CommonPatterns Examples ---");

        try
        {
            var emailRegex = RB.Build(global::RegexBuilder.CommonPatterns.Email());
            Console.WriteLine($"  email 'user@example.com': {emailRegex.IsMatch("user@example.com")}");

            var urlRegex = RB.Build(global::RegexBuilder.CommonPatterns.Url());
            Console.WriteLine($"  url 'https://github.com/example': {urlRegex.IsMatch("https://github.com/example")} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  CommonPatterns examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Balancing group example (match simple balanced parentheses)
    /// </summary>
    static void RunBalancingGroupExample()
    {
        Console.WriteLine("--- Balancing Group Example ---");

        try
        {
            var balancedParens = RB.Build(
                RB.Literal("("),
                RB.BalancingGroup("depth", "depth",
                    RB.NegativeCharacterSet("()", RegexQuantifier.ZeroOrMore)
                ),
                RB.Literal(")")
            );

            Console.WriteLine($"  '()' matches: {balancedParens.IsMatch("()")}");
            Console.WriteLine($"  '(text)' matches: {balancedParens.IsMatch("(text)")}");
            Console.WriteLine($"  '((nested))' matches: {balancedParens.IsMatch("((nested))")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Balancing example failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Unicode category example
    /// </summary>
    static void RunUnicodeCategoryExample()
    {
        Console.WriteLine("--- Unicode Category Example ---");

        try
        {
            var cyrillic = RB.Build(RB.UnicodeCategory("IsCyrillic", RegexQuantifier.OneOrMore));
            Console.WriteLine($"  Cyrillic matches '\u041f\u0440\u0438\u0432\u0435\u0442': {cyrillic.IsMatch("\u041f\u0440\u0438\u0432\u0435\u0442")} ");
            Console.WriteLine($"  Cyrillic matches 'Hello': {cyrillic.IsMatch("Hello")} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Unicode category example failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Inline option grouping example
    /// </summary>
    static void RunInlineOptionGroupingExample()
    {
        Console.WriteLine("--- Inline Option Grouping Example ---");

        try
        {
            var pattern = RB.Build(
                RB.Literal("ID:"),
                RB.InlineOptionGrouping(RegexOptions.IgnoreCase,
                    RB.Literal("abc")
                ),
                RB.Digit(RegexQuantifier.Exactly(3))
            );

            Console.WriteLine($"  'ID:abc123' matches: {pattern.IsMatch("ID:abc123")} ");
            Console.WriteLine($"  'ID:ABC123' matches: {pattern.IsMatch("ID:ABC123")} ");
            Console.WriteLine($"  'id:abc123' matches: {pattern.IsMatch("id:abc123")} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Inline option grouping failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Conditional match example
    /// </summary>
    static void RunConditionalMatchExample()
    {
        Console.WriteLine("--- Conditional Match Example ---");

        try
        {
            var pattern = RB.Build(
                // make named group optional using quantifier
                RB.Group("prefix", RB.Literal("PRE"), RegexQuantifier.ZeroOrOne),
                RB.Literal("-"),
                // use ConditionalMatch by group name
                RB.ConditionalMatch("prefix", RB.Literal("SUFFIX"), RB.Literal("suffix"))
            );

            Console.WriteLine($"  'PRE-SUFFIX' matches: {pattern.IsMatch("PRE-SUFFIX")} ");
            Console.WriteLine($"  '-suffix' matches: {pattern.IsMatch("-suffix")} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Conditional match failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Backtracking suppression (atomic group) example
    /// </summary>
    static void RunBacktrackingSuppressionExample()
    {
        Console.WriteLine("--- Backtracking Suppression Example ---");

        try
        {
            var atomic = RB.Build(
                RB.BacktrackingSuppression(
                    RB.Digit(RegexQuantifier.OneOrMore)
                )
            );

            Console.WriteLine($"  '12345' matches: {atomic.IsMatch("12345")} ");
            Console.WriteLine($"  'abc' matches: {atomic.IsMatch("abc")} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Backtracking suppression failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// IPv4 strict example (uses octet helper)
    /// </summary>
    static void RunIpv4StrictExample()
    {
        Console.WriteLine("--- IPv4 Strict Example ---");

        try
        {
            var octet = RB.Alternate(new RegexNode[] {
                RB.Concatenate(RB.Literal("25"), RB.CharacterSet("0-5", RegexQuantifier.Exactly(1))),
                RB.Concatenate(RB.Literal("2"), RB.CharacterSet("0-4", RegexQuantifier.Exactly(1)), RB.Digit(RegexQuantifier.Exactly(1))),
                RB.Concatenate(RB.CharacterSet("01", RegexQuantifier.ZeroOrOne), RB.Digit(new RegexQuantifier(1, 2)))
            });

            var ipv4Strict = RB.Build(
                RB.LineStart(),
                octet,
                RB.Literal("."),
                octet,
                RB.Literal("."),
                octet,
                RB.Literal("."),
                octet,
                RB.LineEnd()
            );

            Console.WriteLine($"  '192.168.1.1' matches: {ipv4Strict.IsMatch("192.168.1.1")} ");
            Console.WriteLine($"  '255.255.255.255' matches: {ipv4Strict.IsMatch("255.255.255.255")} ");
            Console.WriteLine($"  '256.1.1.1' matches: {ipv4Strict.IsMatch("256.1.1.1")} ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  IPv4 strict example failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Lookaround assertions examples (lookahead and lookbehind)
    /// </summary>
    static void RunLookaroundAssertionsExample()
    {
        Console.WriteLine("--- Lookaround Assertions Example ---");

        try
        {
            // Positive lookahead: match digit followed by a letter
            var posLookahead = RB.Build(
                RB.PositiveLookAhead(RB.Literal("a"), RB.Digit())
            );
            Console.WriteLine($"  PositiveLookAhead 'a5' matches: {posLookahead.IsMatch("a5")}");
            Console.WriteLine($"  PositiveLookAhead 'ab' matches: {posLookahead.IsMatch("ab")}");

            // Negative lookahead
            var negLookahead = RB.Build(
                RB.NegativeLookAhead(RB.Digit(), RB.Literal("test"))
            );
            Console.WriteLine($"  NegativeLookAhead 'test' matches: {negLookahead.IsMatch("test")}");
            Console.WriteLine($"  NegativeLookAhead 'test5' matches: {negLookahead.IsMatch("test5")}");

            // Positive lookbehind: match amount after $ sign
            var posLookbehind = RB.Build(
                RB.PositiveLookBehind(RB.Literal("$"), RB.Digit(RegexQuantifier.OneOrMore))
            );
            Console.WriteLine($"  PositiveLookBehind '$99' matches: {posLookbehind.IsMatch("$99")}");
            Console.WriteLine($"  PositiveLookBehind '99' matches: {posLookbehind.IsMatch("99")}");

            // Negative lookbehind
            var negLookbehind = RB.Build(
                RB.NegativeLookBehind(RB.Literal("$"), RB.Digit(RegexQuantifier.OneOrMore))
            );
            Console.WriteLine($"  NegativeLookBehind '$99' matches: {negLookbehind.IsMatch("$99")}");
            Console.WriteLine($"  NegativeLookBehind '99' matches: {negLookbehind.IsMatch("99")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Lookaround examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Backreference examples
    /// </summary>
    static void RunBackreferenceExample()
    {
        Console.WriteLine("--- Backreference Example ---");

        try
        {
            // Match repeated word
            var repeatedWord = RB.Build(
                RB.Group("word", RB.WordCharacter(RegexQuantifier.OneOrMore)),
                RB.Whitespace(),
                RB.GroupBackReference("word")
            );
            Console.WriteLine($"  'hello hello' matches: {repeatedWord.IsMatch("hello hello")}");
            Console.WriteLine($"  'hello world' matches: {repeatedWord.IsMatch("hello world")}");

            // Numbered backreference
            var numberedRef = RB.Build(
                RB.Group(RB.Digit(RegexQuantifier.Exactly(3))),
                RB.Literal("-"),
                RB.GroupBackReference(1, RegexQuantifier.None)
            );
            Console.WriteLine($"  '123-123' matches: {numberedRef.IsMatch("123-123")}");
            Console.WriteLine($"  '123-456' matches: {numberedRef.IsMatch("123-456")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Backreference examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// CharacterRange examples
    /// </summary>
    static void RunCharacterRangeExample()
    {
        Console.WriteLine("--- CharacterRange Example ---");

        try
        {
            // Simple character range
            var lowercase = RB.Build(
                RB.CharacterRange('a', 'z', false, RegexQuantifier.OneOrMore)
            );
            Console.WriteLine($"  'hello' matches: {lowercase.IsMatch("hello")}");
            Console.WriteLine($"  'HELLO' matches: {lowercase.IsMatch("HELLO")}");

            // Uppercase range
            var uppercase = RB.Build(
                RB.CharacterRange('A', 'Z', false, RegexQuantifier.OneOrMore)
            );
            Console.WriteLine($"  'WORLD' matches: {uppercase.IsMatch("WORLD")}");
            Console.WriteLine($"  'world' matches: {uppercase.IsMatch("world")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  CharacterRange examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Phone number validation examples from common-patterns.md
    /// </summary>
    static void RunPhoneNumberExamples()
    {
        Console.WriteLine("--- Phone Number Examples ---");

        try
        {
            // US phone with optional +1 and separators
            var phonePattern = RB.Pattern()
                .Optional(o => o.Literal("+1").Optional(sep => sep.CharacterSet("- ")))
                .Digits(3, 3)
                .Optional(o => o.CharacterSet("- ."))
                .Digits(3, 3)
                .Optional(o => o.CharacterSet("- ."))
                .Digits(4, 4)
                .Build();

            var phoneRegex = RB.Build(phonePattern);
            Console.WriteLine($"  '555-123-4567' matches: {phoneRegex.IsMatch("555-123-4567")}");
            Console.WriteLine($"  '+1-555-123-4567' matches: {phoneRegex.IsMatch("+1-555-123-4567")}");
            Console.WriteLine($"  '5551234567' matches: {phoneRegex.IsMatch("5551234567")}");
            Console.WriteLine($"  '555.123.4567' matches: {phoneRegex.IsMatch("555.123.4567")}");

            // Phone with parentheses: (555) 123-4567
            var phoneWithParens = RB.Build(
                RB.Group(
                    RB.Concatenate(
                        RB.Literal("+1"),
                        RB.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrOne)
                    ),
                    RegexQuantifier.ZeroOrOne
                ),
                RB.Literal("("),
                RB.Digit(RegexQuantifier.Exactly(3)),
                RB.Literal(")"),
                RB.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrOne),
                RB.Digit(RegexQuantifier.Exactly(3)),
                RB.CharacterSet("- .", RegexQuantifier.ZeroOrOne),
                RB.Digit(RegexQuantifier.Exactly(4))
            );
            Console.WriteLine($"  '(555) 123-4567' matches: {phoneWithParens.IsMatch("(555) 123-4567")}");
            Console.WriteLine($"  '+1 (555) 123-4567' matches: {phoneWithParens.IsMatch("+1 (555) 123-4567")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Phone number examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Postal code validation examples from common-patterns.md
    /// </summary>
    static void RunPostalCodeExamples()
    {
        Console.WriteLine("--- Postal Code Examples ---");

        try
        {
            // US ZIP: 12345 or 12345-6789
            var zipPattern = RB.Pattern()
                .Start()
                .Digits(5)
                .Optional(o => o.Literal("-").Digits(4))
                .End()
                .Build();

            var zipRegex = RB.Build(zipPattern);
            Console.WriteLine($"  US ZIP '12345' matches: {zipRegex.IsMatch("12345")}");
            Console.WriteLine($"  US ZIP '12345-6789' matches: {zipRegex.IsMatch("12345-6789")}");
            Console.WriteLine($"  US ZIP '1234' matches: {zipRegex.IsMatch("1234")}");

            // Canadian Postal: A1A 1A1
            var canadianPostal = RB.Build(
                RB.LineStart(),
                RB.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
                RB.Digit(RegexQuantifier.Exactly(1)),
                RB.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
                RB.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrOne),
                RB.Digit(RegexQuantifier.Exactly(1)),
                RB.CharacterSet("A-Z", RegexQuantifier.Exactly(1)),
                RB.Digit(RegexQuantifier.Exactly(1)),
                RB.LineEnd()
            );
            Console.WriteLine($"  Canadian 'K1A 0B1' matches: {canadianPostal.IsMatch("K1A 0B1")}");
            Console.WriteLine($"  Canadian 'M5V3A8' matches: {canadianPostal.IsMatch("M5V3A8")}");

            // UK Postcode: SW1A 1AA
            var ukPostcode = RB.Build(
                RB.LineStart(),
                RB.CharacterSet("A-Z", new RegexQuantifier(1, 2)),
                RB.Digit(RegexQuantifier.Exactly(1)),
                RB.CharacterSet("A-Z0-9", RegexQuantifier.ZeroOrOne),
                RB.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.ZeroOrOne),
                RB.Digit(RegexQuantifier.Exactly(1)),
                RB.CharacterSet("A-Z", RegexQuantifier.Exactly(2)),
                RB.LineEnd()
            );
            Console.WriteLine($"  UK 'SW1A 1AA' matches: {ukPostcode.IsMatch("SW1A 1AA")}");
            Console.WriteLine($"  UK 'M1 1AE' matches: {ukPostcode.IsMatch("M1 1AE")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Postal code examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Date and time validation examples from common-patterns.md
    /// </summary>
    static void RunDateTimeExamples()
    {
        Console.WriteLine("--- Date/Time Examples ---");

        try
        {
            // ISO 8601 Date: YYYY-MM-DD
            var isoDate = RB.Pattern()
                .Start()
                .Digits(4)
                .Literal("-")
                .Digits(2)
                .Literal("-")
                .Digits(2)
                .End()
                .Build();

            var isoDateRegex = RB.Build(isoDate);
            Console.WriteLine($"  ISO date '2025-10-22' matches: {isoDateRegex.IsMatch("2025-10-22")}");
            Console.WriteLine($"  ISO date '2025-1-1' matches: {isoDateRegex.IsMatch("2025-1-1")}");

            // US Date: MM/DD/YYYY
            var usDate = RB.Build(
                RB.LineStart(),
                RB.Digit(new RegexQuantifier(1, 2)),
                RB.Literal("/"),
                RB.Digit(new RegexQuantifier(1, 2)),
                RB.Literal("/"),
                RB.Digit(RegexQuantifier.Exactly(4)),
                RB.LineEnd()
            );
            Console.WriteLine($"  US date '10/22/2025' matches: {usDate.IsMatch("10/22/2025")}");
            Console.WriteLine($"  US date '1/1/2025' matches: {usDate.IsMatch("1/1/2025")}");

            // Time: HH:MM or HH:MM:SS
            var time24h = RB.Build(
                RB.LineStart(),
                RB.Digit(RegexQuantifier.Exactly(2)),
                RB.Literal(":"),
                RB.Digit(RegexQuantifier.Exactly(2)),
                RB.Group(
                    RB.Concatenate(
                        RB.Literal(":"),
                        RB.Digit(RegexQuantifier.Exactly(2))
                    ),
                    RegexQuantifier.ZeroOrOne
                ),
                RB.LineEnd()
            );
            Console.WriteLine($"  Time '14:30' matches: {time24h.IsMatch("14:30")}");
            Console.WriteLine($"  Time '14:30:45' matches: {time24h.IsMatch("14:30:45")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Date/Time examples failed: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Substitution Pattern Example from README
    /// </summary>
    static void RunSubstitutionPatternExample()
    {
        Console.WriteLine("--- Substitution Patterns ---");

        // Swap two words
        var pattern = RB.Build(
            RB.Group("word1", RB.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)),
            RB.Literal(" "),
            RB.Group("word2", RB.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore))
        );

        var replacement = SubstitutionBuilder.Build(
            SubstitutionBuilder.Group("word2"),
            SubstitutionBuilder.Literal(" "),
            SubstitutionBuilder.Group("word1")
        );

        string result = pattern.Replace("hello world", replacement);
        Console.WriteLine($"  Original: 'hello world'");
        Console.WriteLine($"  Swapped:  '{result}'"); // result = "world hello"

        // Test with more examples
        Console.WriteLine("\n  More swap examples:");
        Console.WriteLine($"    'foo bar' -> '{pattern.Replace("foo bar", replacement)}'");
        Console.WriteLine($"    'first second' -> '{pattern.Replace("first second", replacement)}'");

        Console.WriteLine();
    }
}
