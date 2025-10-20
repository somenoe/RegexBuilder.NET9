using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegexBuilder.Tests
{
    [TestClass]
    public class RegexBuilderTests
    {
        [TestMethod]
        public void TestAsciiCharacterRendering()
        {
            RegexNode node1 = RegexBuilder.AsciiCharacter(0x30);
            Assert.AreEqual(@"\x30", node1.ToRegexPattern());

            RegexNode node2 = RegexBuilder.AsciiCharacter(0x7F, RegexQuantifier.Custom(1, 4, true));
            Assert.AreEqual(@"(?:\x7f){1,4}?", node2.ToRegexPattern());

            RegexNode node3 = RegexBuilder.AsciiCharacter(0x0B, RegexQuantifier.Exactly(5));
            Assert.AreEqual(@"(?:\x0b){5}", node3.ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCharacterRendering()
        {
            RegexNode node1 = RegexBuilder.UnicodeCharacter(0x1234);
            Assert.AreEqual(@"\u1234", node1.ToRegexPattern());

            RegexNode node2 = RegexBuilder.UnicodeCharacter(0x7F03, RegexQuantifier.Custom(1, 4, true));
            Assert.AreEqual(@"(?:\u7f03){1,4}?", node2.ToRegexPattern());

            RegexNode node3 = RegexBuilder.UnicodeCharacter(0x0BA5, RegexQuantifier.Exactly(5));
            Assert.AreEqual(@"(?:\u0ba5){5}", node3.ToRegexPattern());
        }

        [TestMethod]
        public void TestMetaCharacterRendering()
        {
            RegexNode node1 = RegexBuilder.MetaCharacter(RegexMetaChars.NonWordBoundary);
            Assert.AreEqual(@"\B", node1.ToRegexPattern());

            RegexNode node2 = RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Custom(1, 4, true));
            Assert.AreEqual(@"\d{1,4}?", node2.ToRegexPattern());

            RegexNode node3 = RegexBuilder.MetaCharacter(RegexMetaChars.WhiteSpace, RegexQuantifier.Exactly(5));
            Assert.AreEqual(@"\s{5}", node3.ToRegexPattern());
        }

        [TestMethod]
        public void TestBuildMethod1()
        {
            RegexNode literal = RegexBuilder.Literal("abc");
            RegexNode characterRange = RegexBuilder.CharacterRange('a', 'f', false, RegexQuantifier.None);
            RegexNode character = RegexBuilder.MetaCharacter(RegexMetaChars.LineEnd);

            Regex regex = RegexBuilder.Build(literal, characterRange, character);
            Assert.AreEqual("abc[a-f]$", regex.ToString());
        }

        [TestMethod]
        public void TestBuildMethod2()
        {
            RegexNode literal = RegexBuilder.Literal("abc");
            RegexNode characterRange = RegexBuilder.CharacterRange('a', 'f', false, RegexQuantifier.None);
            RegexNode character = RegexBuilder.MetaCharacter(RegexMetaChars.LineEnd);
            RegexNodeConcatenation concatenation = new RegexNodeConcatenation(literal, characterRange, character);

            Regex regex = RegexBuilder.Build(concatenation);
            Assert.AreEqual("abc[a-f]$", regex.ToString());
        }

        [TestMethod]
        public void TestBuildMethod3()
        {
            RegexNode literal = RegexBuilder.Literal("abc");
            RegexNode characterRange = RegexBuilder.CharacterRange('a', 'f', false, RegexQuantifier.None);
            RegexNode character = RegexBuilder.MetaCharacter(RegexMetaChars.LineEnd);

            Regex regex = RegexBuilder.Build(RegexOptions.Compiled, literal, characterRange, character);
            Assert.IsTrue((regex.Options & RegexOptions.Compiled) == RegexOptions.Compiled);
            Assert.AreEqual("abc[a-f]$", regex.ToString());
        }

        [TestMethod]
        public void TestBuildMethod4()
        {
            RegexNode literal = RegexBuilder.Literal("abc");
            RegexNode characterRange = RegexBuilder.CharacterRange('a', 'f', false, RegexQuantifier.None);
            RegexNode character = RegexBuilder.MetaCharacter(RegexMetaChars.LineEnd);
            RegexNodeConcatenation concatenation = new RegexNodeConcatenation(literal, characterRange, character);

            Regex regex = RegexBuilder.Build(RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline, concatenation);
            Assert.IsTrue((regex.Options & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace);
            Assert.IsTrue((regex.Options & RegexOptions.Singleline) == RegexOptions.Singleline);
            Assert.AreEqual("abc[a-f]$", regex.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestBuildMethodValidation1()
        {
            RegexBuilder.Build((RegexNode)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestBuildMethodValidation2()
        {
            RegexBuilder.Build((RegexNode[])null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestBuildMethodValidation3()
        {
            RegexBuilder.Build(RegexOptions.None, (RegexNode)null);
        }

        #region Convenience Shortcut Methods Tests

        #region Character Class Shortcuts Tests

        [TestMethod]
        public void TestDigitShortcut()
        {
            RegexNode node = RegexBuilder.Digit();
            Assert.AreEqual(@"\d", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestDigitShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.Digit(RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"\d*", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonDigitShortcut()
        {
            RegexNode node = RegexBuilder.NonDigit();
            Assert.AreEqual(@"\D", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonDigitShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.NonDigit(RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"\D+", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestWhitespaceShortcut()
        {
            RegexNode node = RegexBuilder.Whitespace();
            Assert.AreEqual(@"\s", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestWhitespaceShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.Whitespace(RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"\s?", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonWhitespaceShortcut()
        {
            RegexNode node = RegexBuilder.NonWhitespace();
            Assert.AreEqual(@"\S", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonWhitespaceShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.NonWhitespace(RegexQuantifier.Exactly(3));
            Assert.AreEqual(@"\S{3}", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestWordCharacterShortcut()
        {
            RegexNode node = RegexBuilder.WordCharacter();
            Assert.AreEqual(@"\w", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestWordCharacterShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.WordCharacter(RegexQuantifier.Custom(1, 5, false));
            Assert.AreEqual(@"\w{1,5}", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonWordCharacterShortcut()
        {
            RegexNode node = RegexBuilder.NonWordCharacter();
            Assert.AreEqual(@"\W", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonWordCharacterShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.NonWordCharacter(RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"\W*", node.ToRegexPattern());
        }

        #endregion Character Class Shortcuts Tests

        #region Anchor Shortcuts Tests

        [TestMethod]
        public void TestLineStartShortcut()
        {
            RegexNode node = RegexBuilder.LineStart();
            Assert.AreEqual("^", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestLineEndShortcut()
        {
            RegexNode node = RegexBuilder.LineEnd();
            Assert.AreEqual("$", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestStringStartShortcut()
        {
            RegexNode node = RegexBuilder.StringStart();
            Assert.AreEqual(@"\A", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestStringEndShortcut()
        {
            RegexNode node = RegexBuilder.StringEnd();
            Assert.AreEqual(@"\Z", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestStringEndAbsoluteShortcut()
        {
            RegexNode node = RegexBuilder.StringEndAbsolute();
            Assert.AreEqual(@"\z", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestWordBoundaryShortcut()
        {
            RegexNode node = RegexBuilder.WordBoundary();
            Assert.AreEqual(@"\b", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestNonWordBoundaryShortcut()
        {
            RegexNode node = RegexBuilder.NonWordBoundary();
            Assert.AreEqual(@"\B", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestMatchPointAnchorShortcut()
        {
            RegexNode node = RegexBuilder.MatchPointAnchor();
            Assert.AreEqual(@"\G", node.ToRegexPattern());
        }

        #endregion Anchor Shortcuts Tests

        #region Escape Character Shortcuts Tests

        [TestMethod]
        public void TestBellCharacterShortcut()
        {
            RegexNode node = RegexBuilder.BellCharacter();
            Assert.AreEqual(@"\a", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestBellCharacterShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.BellCharacter(RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"\a?", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestFormFeedShortcut()
        {
            RegexNode node = RegexBuilder.FormFeed();
            Assert.AreEqual(@"\f", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestFormFeedShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.FormFeed(RegexQuantifier.Exactly(2));
            Assert.AreEqual(@"\f{2}", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestVerticalTabShortcut()
        {
            RegexNode node = RegexBuilder.VerticalTab();
            Assert.AreEqual(@"\v", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestVerticalTabShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.VerticalTab(RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"\v*", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestEscapeCharacterShortcut()
        {
            RegexNode node = RegexBuilder.EscapeCharacter();
            Assert.AreEqual(@"\e", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestEscapeCharacterShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.EscapeCharacter(RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"\e+", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestOctalCharacterShortcut()
        {
            RegexNode node = RegexBuilder.OctalCharacter(255);
            Assert.AreEqual(@"\377", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestOctalCharacterShortcutSmallValue()
        {
            RegexNode node = RegexBuilder.OctalCharacter(10);
            Assert.AreEqual(@"\012", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestOctalCharacterShortcutWithQuantifier()
        {
            RegexNode node = RegexBuilder.OctalCharacter(65, RegexQuantifier.Exactly(3));
            Assert.AreEqual(@"(?:\101){3}", node.ToRegexPattern());
        }

        #endregion Escape Character Shortcuts Tests

        #region Integration Tests

        [TestMethod]
        public void TestShortcutsInConcatenation()
        {
            RegexNode pattern = RegexBuilder.Concatenate(
                new[]
                {
                    RegexBuilder.LineStart() as RegexNode,
                    RegexBuilder.Digit(RegexQuantifier.OneOrMore) as RegexNode,
                    RegexBuilder.Whitespace() as RegexNode,
                    RegexBuilder.WordCharacter(RegexQuantifier.ZeroOrMore) as RegexNode,
                    RegexBuilder.LineEnd() as RegexNode
                }
            );
            Assert.AreEqual(@"^\d+\s\w*$", pattern.ToRegexPattern());
        }

        [TestMethod]
        public void TestShortcutsInGroup()
        {
            RegexNode pattern = RegexBuilder.Group(
                "word",
                RegexBuilder.Concatenate(
                    RegexBuilder.WordCharacter(RegexQuantifier.OneOrMore),
                    RegexBuilder.NonWhitespace()
                )
            );
            Assert.AreEqual(@"(?<word>\w+\S)", pattern.ToRegexPattern());
        }

        [TestMethod]
        public void TestCharacterClassShortcutMatching()
        {
            Regex regex = RegexBuilder.Build(RegexBuilder.Digit(RegexQuantifier.OneOrMore));
            Assert.IsTrue(regex.IsMatch("12345"));
            Assert.IsFalse(regex.IsMatch("abcde"));
        }

        [TestMethod]
        public void TestAnchorShortcutMatching()
        {
            Regex regex = RegexBuilder.Build(
                RegexBuilder.StringStart(),
                RegexBuilder.WordCharacter(RegexQuantifier.OneOrMore),
                RegexBuilder.StringEnd()
            );
            Assert.IsTrue(regex.IsMatch("HelloWorld"));
            Assert.IsFalse(regex.IsMatch("Hello World"));
        }

        [TestMethod]
        public void TestEscapeShortcutMatching()
        {
            Regex regex = RegexBuilder.Build(
                RegexBuilder.Digit(),
                RegexBuilder.FormFeed(),
                RegexBuilder.Digit()
            );
            Assert.IsTrue(regex.IsMatch("1\f2"));
            Assert.IsFalse(regex.IsMatch("1 2"));
        }

        #endregion Integration Tests

        #endregion Convenience Shortcut Methods Tests

        #region Inline Option Grouping Tests

        [TestMethod]
        public void TestInlineOptionGroupingSingleOptionIgnoreCase()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(RegexOptions.IgnoreCase, RegexBuilder.Literal("hello"));
            Assert.AreEqual("(?i:hello)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingSingleOptionMultiline()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(RegexOptions.Multiline, RegexBuilder.Literal("test"));
            Assert.AreEqual("(?m:test)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingSingleOptionSingleline()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(RegexOptions.Singleline, RegexBuilder.Literal("expr"));
            Assert.AreEqual("(?s:expr)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingSingleOptionExplicitCapture()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(RegexOptions.ExplicitCapture, RegexBuilder.Literal("pattern"));
            Assert.AreEqual("(?n:pattern)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingSingleOptionIgnorePatternWhitespace()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(RegexOptions.IgnorePatternWhitespace, RegexBuilder.Literal("spaced"));
            Assert.AreEqual("(?x:spaced)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingMultipleOptionsIgnoreCaseMultiline()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(
                RegexOptions.IgnoreCase | RegexOptions.Multiline,
                RegexBuilder.Literal("text")
            );
            Assert.AreEqual("(?im:text)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingMultipleOptionsAll()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace,
                RegexBuilder.Literal("all")
            );
            Assert.AreEqual("(?imsnx:all)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingWithDisabledOptions()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(
                RegexOptions.IgnoreCase,
                RegexOptions.Multiline,
                RegexBuilder.Literal("expr")
            );
            Assert.AreEqual("(?i-m:expr)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingMultipleEnabledMultipleDisabled()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(
                RegexOptions.IgnoreCase | RegexOptions.Singleline,
                RegexOptions.Multiline | RegexOptions.ExplicitCapture,
                RegexBuilder.Literal("complex")
            );
            Assert.AreEqual("(?is-mn:complex)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingNoneEnabledWithDisabled()
        {
            RegexNode node = RegexBuilder.InlineOptionGrouping(
                RegexOptions.None,
                RegexOptions.IgnoreCase,
                RegexBuilder.Literal("disable")
            );
            Assert.AreEqual("(?-i:disable)", node.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionGroupingWithComplexExpression()
        {
            RegexNode complexExpr = new RegexNodeConcatenation(
                RegexBuilder.Literal("hello"),
                RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore)
            );
            RegexNode node = RegexBuilder.InlineOptionGrouping(RegexOptions.IgnoreCase, complexExpr);
            Assert.AreEqual("(?i:hello\\d+)", node.ToRegexPattern());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInlineOptionGroupingInvalidOptionCompiled()
        {
            RegexBuilder.InlineOptionGrouping(RegexOptions.Compiled, RegexBuilder.Literal("test"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInlineOptionGroupingInvalidOptionRightToLeft()
        {
            RegexBuilder.InlineOptionGrouping(RegexOptions.RightToLeft, RegexBuilder.Literal("test"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInlineOptionGroupingInvalidOptionECMAScript()
        {
            RegexBuilder.InlineOptionGrouping(RegexOptions.ECMAScript, RegexBuilder.Literal("test"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInlineOptionGroupingInvalidOptionCultureInvariant()
        {
            RegexBuilder.InlineOptionGrouping(RegexOptions.CultureInvariant, RegexBuilder.Literal("test"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInlineOptionGroupingNullExpression()
        {
            RegexBuilder.InlineOptionGrouping(RegexOptions.IgnoreCase, null);
        }

        [TestMethod]
        public void TestInlineOptionGroupingCaseInsensitiveMatching()
        {
            Regex regex = RegexBuilder.Build(
                RegexBuilder.InlineOptionGrouping(
                    RegexOptions.IgnoreCase,
                    RegexBuilder.Literal("hello")
                )
            );
            Assert.IsTrue(regex.IsMatch("hello"));
            Assert.IsTrue(regex.IsMatch("HELLO"));
            Assert.IsTrue(regex.IsMatch("HeLLo"));
        }

        [TestMethod]
        public void TestInlineOptionGroupingMultilineMatching()
        {
            string input = "line1\nline2";
            Regex regex = RegexBuilder.Build(
                RegexBuilder.InlineOptionGrouping(
                    RegexOptions.Multiline,
                    new RegexNodeConcatenation(
                        RegexBuilder.MetaCharacter(RegexMetaChars.LineStart),
                        RegexBuilder.Literal("line")
                    )
                )
            );
            Assert.IsTrue(regex.IsMatch(input));
        }

        [TestMethod]
        public void TestInlineOptionGroupingSinglelineMatching()
        {
            string input = "start\nmiddle\nend";
            Regex regex = RegexBuilder.Build(
                RegexBuilder.InlineOptionGrouping(
                    RegexOptions.Singleline,
                    new RegexNodeConcatenation(
                        RegexBuilder.Literal("start"),
                        RegexBuilder.MetaCharacter(RegexMetaChars.AnyCharacter, RegexQuantifier.ZeroOrMore),
                        RegexBuilder.Literal("end")
                    )
                )
            );
            Assert.IsTrue(regex.IsMatch(input));
        }

        #endregion Inline Option Grouping Tests
    }
}
