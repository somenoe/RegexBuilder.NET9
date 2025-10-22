using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RegexBuilder.Tests
{
    [TestClass]
    public partial class SubstitutionBuilderTests
    {
        #region Literal Tests

        [TestMethod]
        public void Literal_SimpleText_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Literal("hello");
            Assert.AreEqual("hello", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void Literal_TextWithDollarSign_EscapesDollarSign()
        {
            var substitution = SubstitutionBuilder.Literal("Price: $100");
            Assert.AreEqual("Price: $$100", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void Literal_MultipleDollarSigns_EscapesAll()
        {
            var substitution = SubstitutionBuilder.Literal("$$ means $");
            Assert.AreEqual("$$$$ means $$", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Literal_NullText_ThrowsException()
        {
            SubstitutionBuilder.Literal(null!);
        }

        [TestMethod]
        public void Literal_EmptyString_RendersEmptyPattern()
        {
            var substitution = SubstitutionBuilder.Literal(string.Empty);
            Assert.AreEqual(string.Empty, substitution.ToSubstitutionPattern());
        }

        #endregion

        #region Numbered Group Reference Tests

        [TestMethod]
        public void GroupNumber_SingleDigit_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Group(1);
            Assert.AreEqual("$1", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void GroupNumber_MultipleDigits_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Group(123);
            Assert.AreEqual("$123", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void GroupNumber_Zero_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Group(0);
            Assert.AreEqual("$0", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GroupNumber_Negative_ThrowsException()
        {
            SubstitutionBuilder.Group(-1);
        }

        #endregion

        #region Named Group Reference Tests

        [TestMethod]
        public void GroupName_SimpleName_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Group("word");
            Assert.AreEqual("${word}", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void GroupName_ComplexName_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Group("word_1");
            Assert.AreEqual("${word_1}", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GroupName_NullName_ThrowsException()
        {
            SubstitutionBuilder.Group((string)null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GroupName_EmptyName_ThrowsException()
        {
            SubstitutionBuilder.Group("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GroupName_WhitespaceName_ThrowsException()
        {
            SubstitutionBuilder.Group("   ");
        }

        #endregion

        #region Special Reference Tests

        [TestMethod]
        public void WholeMatch_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.WholeMatch();
            Assert.AreEqual("$&", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void BeforeMatch_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.BeforeMatch();
            Assert.AreEqual("$`", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void AfterMatch_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.AfterMatch();
            Assert.AreEqual("$'", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void LastCapturedGroup_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.LastCapturedGroup();
            Assert.AreEqual("$+", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void EntireInput_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.EntireInput();
            Assert.AreEqual("$_", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void LiteralDollar_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.LiteralDollar();
            Assert.AreEqual("$$", substitution.ToSubstitutionPattern());
        }

        #endregion

        #region Concatenation Tests

        [TestMethod]
        public void Concatenate_MultipleNodes_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Concatenate(
                SubstitutionBuilder.Literal("Hello "),
                SubstitutionBuilder.Group(1),
                SubstitutionBuilder.Literal("!")
            );
            Assert.AreEqual("Hello $1!", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        public void Concatenate_WithSpecialReferences_RendersCorrectly()
        {
            var substitution = SubstitutionBuilder.Concatenate(
                SubstitutionBuilder.Literal("["),
                SubstitutionBuilder.WholeMatch(),
                SubstitutionBuilder.Literal("]")
            );
            Assert.AreEqual("[$&]", substitution.ToSubstitutionPattern());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Concatenate_NullArray_ThrowsException()
        {
            SubstitutionBuilder.Concatenate(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Concatenate_EmptyArray_ThrowsException()
        {
            SubstitutionBuilder.Concatenate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Concatenate_ArrayWithNullElement_ThrowsException()
        {
            SubstitutionBuilder.Concatenate(
                SubstitutionBuilder.Literal("test"),
                null!
            );
        }

        [TestMethod]
        public void SubstitutionConcatenation_IEnumerableConstructor_HappyPath()
        {
            // Test the IEnumerable-based constructor directly
            var nodes = new List<SubstitutionNode>
            {
                SubstitutionBuilder.Literal("Hello "),
                SubstitutionBuilder.Group(1),
                SubstitutionBuilder.Literal("!")
            };
            var concatenation = new SubstitutionConcatenation(nodes);
            Assert.AreEqual("Hello $1!", concatenation.ToSubstitutionPattern());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubstitutionConcatenation_IEnumerableConstructor_NullArgument_ThrowsException()
        {
            // Test the IEnumerable-based constructor with null
            new SubstitutionConcatenation((IEnumerable<SubstitutionNode>)null!);
        }

        #endregion

        #region Build Method Tests

        [TestMethod]
        public void Build_SingleNode_RendersCorrectly()
        {
            var pattern = SubstitutionBuilder.Build(SubstitutionBuilder.Group(1));
            Assert.AreEqual("$1", pattern);
        }

        [TestMethod]
        public void Build_MultipleNodes_RendersCorrectly()
        {
            var pattern = SubstitutionBuilder.Build(
                SubstitutionBuilder.Group(2),
                SubstitutionBuilder.Literal(" "),
                SubstitutionBuilder.Group(1)
            );
            Assert.AreEqual("$2 $1", pattern);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Build_SingleNullNode_ThrowsException()
        {
            SubstitutionBuilder.Build((SubstitutionNode)null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Build_NullArray_ThrowsException()
        {
            SubstitutionBuilder.Build((SubstitutionNode[])null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Build_EmptyArray_ThrowsException()
        {
            SubstitutionBuilder.Build(Array.Empty<SubstitutionNode>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Build_MixedValidAndNullNodes_ThrowsException()
        {
            // This should trigger the ArgumentException from SubstitutionConcatenation
            // when it detects null elements in the array
            SubstitutionBuilder.Build(
                SubstitutionBuilder.Literal("a"),
                null!
            );
        }

        #endregion

        #region Integration Tests with Regex.Replace

        [TestMethod]
        public void Integration_NumberedGroupSubstitution_SwapsTwoWords()
        {
            var pattern = RegexBuilder.Build(
                RegexBuilder.Group(RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)),
                RegexBuilder.Literal(" "),
                RegexBuilder.Group(RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore))
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
        public void Integration_NamedGroupSubstitution_SwapsTwoWords()
        {
            var pattern = RegexBuilder.Build(
                RegexBuilder.Group("word1", RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)),
                RegexBuilder.Literal(" "),
                RegexBuilder.Group("word2", RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore))
            );

            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.Group("word2"),
                SubstitutionBuilder.Literal(" "),
                SubstitutionBuilder.Group("word1")
            );

            string result = pattern.Replace("hello world", replacement);
            Assert.AreEqual("world hello", result);
        }

        [TestMethod]
        public void Integration_NamedGroupSubstitution_FormatsPhoneNumber()
        {
            var pattern = RegexBuilder.Build(
                RegexBuilder.Group("area", RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Exactly(3))),
                RegexBuilder.Literal("-"),
                RegexBuilder.Group("prefix", RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Exactly(3))),
                RegexBuilder.Literal("-"),
                RegexBuilder.Group("number", RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.Exactly(4)))
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
        public void Integration_WholeMatch_DuplicatesMatch()
        {
            var pattern = RegexBuilder.Build(RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore));
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.WholeMatch(),
                SubstitutionBuilder.Literal("-"),
                SubstitutionBuilder.WholeMatch()
            );

            string result = pattern.Replace("hello", replacement);
            Assert.AreEqual("hello-hello", result);
        }

        [TestMethod]
        public void Integration_WholeMatch_WrapsInBrackets()
        {
            var pattern = RegexBuilder.Build(RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore));
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.Literal("["),
                SubstitutionBuilder.WholeMatch(),
                SubstitutionBuilder.Literal("]")
            );

            string result = Regex.Replace("hello world", pattern.ToString(), replacement);
            Assert.AreEqual("[hello] [world]", result);
        }

        [TestMethod]
        public void Integration_LiteralDollar_InsertsMoneySymbol()
        {
            var pattern = RegexBuilder.Build(
                RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore)
            );

            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.LiteralDollar(),
                SubstitutionBuilder.WholeMatch()
            );

            string result = pattern.Replace("The price is 100", replacement);
            Assert.AreEqual("The price is $100", result);
        }

        [TestMethod]
        public void Integration_BeforeMatch_ReplacesWithPrefix()
        {
            var pattern = MyRegex();
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.BeforeMatch()
            );

            string result = pattern.Replace("AABBCC", replacement);
            Assert.AreEqual("AAAACC", result);
        }

        [TestMethod]
        public void Integration_AfterMatch_ReplacesWithSuffix()
        {
            var pattern = MyRegex();
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.AfterMatch()
            );

            string result = pattern.Replace("AABBCC", replacement);
            Assert.AreEqual("AACCCC", result);
        }

        [TestMethod]
        public void Integration_LastCapturedGroup_OutputsLastGroup()
        {
            var pattern = MyRegex1();
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.LastCapturedGroup()
            );

            string result = pattern.Replace("AABBCCDD", replacement);
            Assert.AreEqual("AACCDD", result);
        }

        [TestMethod]
        public void Integration_EntireInput_ReplacesWithWholeString()
        {
            var pattern = MyRegex2();
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.EntireInput()
            );

            string result = pattern.Replace("AABBCC", replacement);
            Assert.AreEqual("AAAABBCCCC", result);
        }

        [TestMethod]
        public void Integration_ComplexPattern_RestructuresData()
        {
            // Parse "LastName, FirstName" and convert to "FirstName LastName"
            var pattern = RegexBuilder.Build(
                RegexBuilder.Group("last", RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore)),
                RegexBuilder.Literal(", "),
                RegexBuilder.Group("first", RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore))
            );

            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.Group("first"),
                SubstitutionBuilder.Literal(" "),
                SubstitutionBuilder.Group("last")
            );

            string result = pattern.Replace("Smith, John", replacement);
            Assert.AreEqual("John Smith", result);
        }

        [TestMethod]
        public void Integration_MultipleReplacements_AppliesAll()
        {
            var pattern = RegexBuilder.Build(RegexBuilder.MetaCharacter(RegexMetaChars.WordCharacter, RegexQuantifier.OneOrMore));
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.Literal("<"),
                SubstitutionBuilder.WholeMatch(),
                SubstitutionBuilder.Literal(">")
            );

            string result = pattern.Replace("one two three", replacement);
            Assert.AreEqual("<one> <two> <three>", result);
        }

        [TestMethod]
        public void Integration_EscapedDollarInLiteral_OutputsCorrectly()
        {
            var pattern = RegexBuilder.Build(RegexBuilder.MetaCharacter(RegexMetaChars.Digit, RegexQuantifier.OneOrMore));
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.Literal("Price: $"),
                SubstitutionBuilder.WholeMatch()
            );

            string result = pattern.Replace("Total: 100", replacement);
            Assert.AreEqual("Total: Price: $100", result);
        }

        [TestMethod]
        public void Integration_CombineAllSpecialReferences_WorksCorrectly()
        {
            var pattern = MyRegex3();

            // This is a complex test combining multiple substitution types
            var replacement = SubstitutionBuilder.Build(
                SubstitutionBuilder.Literal("Input:["),
                SubstitutionBuilder.EntireInput(),
                SubstitutionBuilder.Literal("] Before:["),
                SubstitutionBuilder.BeforeMatch(),
                SubstitutionBuilder.Literal("] Match:["),
                SubstitutionBuilder.WholeMatch(),
                SubstitutionBuilder.Literal("] After:["),
                SubstitutionBuilder.AfterMatch(),
                SubstitutionBuilder.Literal("]")
            );

            // Test with "start hello world end"
            string result = pattern.Replace("start hello world end", replacement, 1);

            // Verify the replacement pattern was built correctly
            Assert.IsTrue(replacement.Contains("$_"), "Replacement should contain $_");
            Assert.IsTrue(replacement.Contains("$`"), "Replacement should contain $`");
            Assert.IsTrue(replacement.Contains("$&"), "Replacement should contain $&");
            Assert.IsTrue(replacement.Contains("$'"), "Replacement should contain $'");

            // Verify the actual result contains the expected substitution values
            Assert.IsTrue(result.Contains("Input:[start hello world end]"), $"Result should contain input. Result: {result}");
            Assert.IsTrue(result.Contains("Before:[start ]"), $"Result should contain before. Result: {result}");
            Assert.IsTrue(result.Contains("Match:[hello world]"), $"Result should contain match. Result: {result}");
            Assert.IsTrue(result.Contains("After:[ end]"), $"Result should contain after. Result: {result}");
        }

        [GeneratedRegex("B+")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"B+(C+)")]
        private static partial Regex MyRegex1();
        [GeneratedRegex("B+")]
        private static partial Regex MyRegex2();
        [GeneratedRegex(@"hello\sworld")]
        private static partial Regex MyRegex3();

        #endregion
    }
}
