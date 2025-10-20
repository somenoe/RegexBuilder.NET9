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
        RunSubstitutionPatternExample();

        Console.WriteLine("\n=== All Examples Completed Successfully ===");
    }

    /// <summary>
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
