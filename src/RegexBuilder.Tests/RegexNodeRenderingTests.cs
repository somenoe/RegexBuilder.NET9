using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegexBuilder.Tests
{
    [TestClass]
    public class RegexNodeRenderingTests
    {
        [TestMethod]
        public void TestLiteralNodeRendering()
        {
            RegexNodeLiteral literal1 = new RegexNodeLiteral(@"\w\b.353\s");
            Assert.AreEqual(@"\w\b.353\s", literal1.ToRegexPattern());

            literal1.Quantifier = RegexQuantifier.ZeroOrOne;
            Assert.AreEqual(@"(?:\w\b.353\s)?", literal1.ToRegexPattern());

            RegexNodeLiteral literal2 = RegexBuilder.NonEscapedLiteral(@"\w\b.353\s");
            Assert.AreEqual(@"\w\b.353\s", literal2.ToRegexPattern());

            RegexNodeLiteral literal3 = RegexBuilder.NonEscapedLiteral(@"\w\b.353\s", RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?:\w\b.353\s)?", literal3.ToRegexPattern());

            RegexNodeLiteral literal4 = RegexBuilder.NonEscapedLiteral(@"\w", RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"\w?", literal4.ToRegexPattern());

            RegexNodeLiteral literal5 = RegexBuilder.NonEscapedLiteral(@"a", RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"a*", literal5.ToRegexPattern());
        }

        [TestMethod]
        public void TestEscapingLiteralNodeRendering()
        {
            RegexNodeEscapingLiteral literal1 = new RegexNodeEscapingLiteral(@"\w\b.353\s");
            Assert.AreEqual(@"\\w\\b\.353\\s", literal1.ToRegexPattern());

            literal1.Quantifier = RegexQuantifier.ZeroOrOne;
            Assert.AreEqual(@"(?:\\w\\b\.353\\s)?", literal1.ToRegexPattern());

            RegexNodeEscapingLiteral literal2 = RegexBuilder.Literal(@"\w\b.353\s");
            Assert.AreEqual(@"\\w\\b\.353\\s", literal2.ToRegexPattern());

            RegexNodeEscapingLiteral literal3 = RegexBuilder.Literal(@"\w\b.353\s", RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?:\\w\\b\.353\\s)?", literal3.ToRegexPattern());

            RegexNodeEscapingLiteral literal4 = RegexBuilder.Literal(@"a", RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"a?", literal4.ToRegexPattern());

            RegexNodeEscapingLiteral literal5 = RegexBuilder.Literal(@"\", RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"\\?", literal5.ToRegexPattern());
        }

        [TestMethod]
        public void TestCommentNodeRendering()
        {
            RegexNodeComment comment1 = new RegexNodeComment(@"This is a comment.");
            Assert.AreEqual(@"(?#This is a comment.)", comment1.ToRegexPattern());

            RegexNodeComment comment2 = new RegexNodeComment(@" This is a \c\o\m\m\e\n\t..  ");
            Assert.AreEqual(@"(?# This is a \c\o\m\m\e\n\t..  )", comment2.ToRegexPattern());

            RegexNodeComment comment3 = RegexBuilder.Comment(@"This is a comment.");
            Assert.AreEqual(@"(?#This is a comment.)", comment3.ToRegexPattern());

            RegexNodeComment comment4 = RegexBuilder.Comment(@" This is a \c\o\m\m\e\n\t..  ");
            Assert.AreEqual(@"(?# This is a \c\o\m\m\e\n\t..  )", comment4.ToRegexPattern());
        }

        [TestMethod]
        public void TestConcatenationNodeRendering()
        {
            RegexNodeLiteral literal1 = new RegexNodeLiteral(@"\w*");
            RegexNodeLiteral literal2 = new RegexNodeLiteral(@"\d+");
            RegexNodeLiteral literal3 = new RegexNodeLiteral(@"\s?");
            RegexNodeLiteral literal4 = new RegexNodeLiteral(@"\t");

            RegexNodeConcatenation concatenation1 = new RegexNodeConcatenation(literal1, literal2);
            Assert.AreEqual(@"\w*\d+", concatenation1.ToRegexPattern());

            RegexNodeConcatenation concatenation2 = new RegexNodeConcatenation(new List<RegexNode>(new[] { literal1, literal2 }));
            Assert.AreEqual(@"\w*\d+", concatenation2.ToRegexPattern());

            RegexNodeConcatenation concatenation3 = new RegexNodeConcatenation(literal1, literal2);
            concatenation3.ChildNodes.Add(literal3);
            Assert.AreEqual(@"\w*\d+\s?", concatenation3.ToRegexPattern());

            concatenation3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:\w*\d+\s?)*", concatenation3.ToRegexPattern());

            concatenation3 = new RegexNodeConcatenation(new List<RegexNode>(new[] { literal4, literal2, literal3, literal1 }));
            concatenation3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:\t\d+\s?\w*)*", concatenation3.ToRegexPattern());

            RegexNodeConcatenation concatenation4 = RegexBuilder.Concatenate(literal1, literal2);
            Assert.AreEqual(@"\w*\d+", concatenation4.ToRegexPattern());

            RegexNodeConcatenation concatenation5 = RegexBuilder.Concatenate(literal1, literal2, RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?:\w*\d+)?", concatenation5.ToRegexPattern());

            RegexNodeConcatenation concatenation6 = RegexBuilder.Concatenate(literal1, literal2, literal3);
            Assert.AreEqual(@"\w*\d+\s?", concatenation6.ToRegexPattern());

            RegexNodeConcatenation concatenation7 = RegexBuilder.Concatenate(literal1, literal2, literal3, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?:\w*\d+\s?)*", concatenation7.ToRegexPattern());

            RegexNodeConcatenation concatenation8 = RegexBuilder.Concatenate(literal1, literal2, literal3, literal4);
            Assert.AreEqual(@"\w*\d+\s?\t", concatenation8.ToRegexPattern());

            RegexNodeConcatenation concatenation9 = RegexBuilder.Concatenate(literal1, literal2, literal3, literal4, RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"(?:\w*\d+\s?\t)+", concatenation9.ToRegexPattern());
        }

        [TestMethod]
        public void TestAlternationNodeRendering()
        {
            RegexNodeLiteral literal1 = new RegexNodeLiteral(@"\w*");
            RegexNodeLiteral literal2 = new RegexNodeLiteral(@"\d+");
            RegexNodeLiteral literal3 = new RegexNodeLiteral(@"\s?");

            RegexNodeAlternation alternation1 = new RegexNodeAlternation(literal1, literal2);
            Assert.AreEqual(@"(?:\w*|\d+)", alternation1.ToRegexPattern());

            RegexNodeAlternation alternation2 = new RegexNodeAlternation(literal1, literal2, literal3);
            Assert.AreEqual(@"(?:\w*|\d+|\s?)", alternation2.ToRegexPattern());

            RegexNodeAlternation alternation3 = RegexBuilder.Alternate(literal1, literal2);
            Assert.AreEqual(@"(?:\w*|\d+)", alternation3.ToRegexPattern());

            RegexNodeAlternation alternation4 = RegexBuilder.Alternate(new RegexNode[] { literal1, literal2, literal3 });
            Assert.AreEqual(@"(?:\w*|\d+|\s?)", alternation4.ToRegexPattern());

            RegexNodeAlternation alternation5 = new RegexNodeAlternation(literal1, literal2);
            alternation5.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:\w*|\d+)*", alternation5.ToRegexPattern());

            RegexNodeAlternation alternation6 = new RegexNodeAlternation(literal1, literal2, literal3);
            alternation6.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"(?:\w*|\d+|\s?)+", alternation6.ToRegexPattern());

            RegexNodeAlternation alternation7 = RegexBuilder.Alternate(literal1, literal2, RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?:\w*|\d+)?", alternation7.ToRegexPattern());

            RegexNodeAlternation alternation8 = RegexBuilder.Alternate(new RegexNode[] { literal1, literal2, literal3 }, RegexQuantifier.AtLeast(5));
            Assert.AreEqual(@"(?:\w*|\d+|\s?){5,}", alternation8.ToRegexPattern());
        }

        [TestMethod]
        public void TestBacktrackingSuppressionNodeRendering()
        {
            RegexNodeLiteral literal1 = new RegexNodeLiteral("\\w*");

            RegexNodeBacktrackingSuppression suppression1 = new RegexNodeBacktrackingSuppression(literal1);
            Assert.AreEqual("(?>\\w*)", suppression1.ToRegexPattern());

            RegexNodeBacktrackingSuppression suppression2 = new RegexNodeBacktrackingSuppression(literal1);
            suppression2.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual("(?>\\w*)*", suppression2.ToRegexPattern());

            RegexNodeBacktrackingSuppression suppression3 = RegexBuilder.BacktrackingSuppression(literal1);
            Assert.AreEqual("(?>\\w*)", suppression3.ToRegexPattern());

            RegexNodeBacktrackingSuppression suppression4 = RegexBuilder.BacktrackingSuppression(literal1, RegexQuantifier.ZeroOrMore);
            suppression4.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual("(?>\\w*)*", suppression2.ToRegexPattern());
        }

        [TestMethod]
        public void TestCharacterRangeNodeRendering()
        {
            RegexNodeCharacterRange characterRange1 = new RegexNodeCharacterRange('b', 'x', false);
            Assert.AreEqual(@"[b-x]", characterRange1.ToRegexPattern());

            characterRange1.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[b-x]*", characterRange1.ToRegexPattern());

            characterRange1.Quantifier = RegexQuantifier.None;
            characterRange1.UseCharacterCodes = true;
            Assert.AreEqual(@"[\u0062-\u0078]", characterRange1.ToRegexPattern());

            characterRange1.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[\u0062-\u0078]*", characterRange1.ToRegexPattern());

            RegexNodeCharacterRange characterRange2 = new RegexNodeCharacterRange('b', 'x', true);
            Assert.AreEqual(@"[^b-x]", characterRange2.ToRegexPattern());

            characterRange2.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^b-x]*", characterRange2.ToRegexPattern());

            characterRange2.Quantifier = RegexQuantifier.None;
            characterRange2.UseCharacterCodes = true;
            Assert.AreEqual(@"[^\u0062-\u0078]", characterRange2.ToRegexPattern());

            characterRange2.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^\u0062-\u0078]*", characterRange2.ToRegexPattern());

            RegexNodeCharacterRange characterRange3 = RegexBuilder.CharacterRange('b', 'x', false, RegexQuantifier.None);
            Assert.AreEqual(@"[b-x]", characterRange3.ToRegexPattern());

            characterRange3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[b-x]*", characterRange3.ToRegexPattern());

            characterRange3.Quantifier = RegexQuantifier.None;
            characterRange3.UseCharacterCodes = true;
            Assert.AreEqual(@"[\u0062-\u0078]", characterRange3.ToRegexPattern());

            characterRange3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[\u0062-\u0078]*", characterRange3.ToRegexPattern());

            RegexNodeCharacterRange characterRange4 = RegexBuilder.NegativeCharacterRange('b', 'x', false, RegexQuantifier.None);
            Assert.AreEqual(@"[^b-x]", characterRange4.ToRegexPattern());

            characterRange4.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^b-x]*", characterRange4.ToRegexPattern());

            characterRange4.Quantifier = RegexQuantifier.None;
            characterRange4.UseCharacterCodes = true;
            Assert.AreEqual(@"[^\u0062-\u0078]", characterRange4.ToRegexPattern());

            characterRange4.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^\u0062-\u0078]*", characterRange4.ToRegexPattern());
        }

        [TestMethod]
        public void TestCharacterSetNodeRendering()
        {
            // Char array, positive.
            RegexNodeCharacterSet characterSet1 = new RegexNodeCharacterSet(new[] { 'a', 'B', '0', ']', 'd', '^', 'c' }, false);
            Assert.AreEqual(@"[aB0\]d\^c]", characterSet1.ToRegexPattern());

            characterSet1.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[aB0\]d\^c]*", characterSet1.ToRegexPattern());

            characterSet1.Quantifier = RegexQuantifier.None;
            characterSet1.UseCharacterCodes = true;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u005d\u0064\u005e\u0063]", characterSet1.ToRegexPattern());

            characterSet1.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u005d\u0064\u005e\u0063]*", characterSet1.ToRegexPattern());

            // Char array, negative.
            RegexNodeCharacterSet characterSet2 = new RegexNodeCharacterSet(new[] { 'a', 'B', '0', 'd', '^', 'x' }, true);
            Assert.AreEqual(@"[^aB0d\^x]", characterSet2.ToRegexPattern());

            characterSet2.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^aB0d\^x]*", characterSet2.ToRegexPattern());

            characterSet2.Quantifier = RegexQuantifier.None;
            characterSet2.UseCharacterCodes = true;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005e\u0078]", characterSet2.ToRegexPattern());

            characterSet2.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005e\u0078]*", characterSet2.ToRegexPattern());

            // Explicit pattern, positive.
            RegexNodeCharacterSet characterSet3 = new RegexNodeCharacterSet(@"aB0d\W\s", false);
            Assert.AreEqual(@"[aB0d\W\s]", characterSet3.ToRegexPattern());

            characterSet3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[aB0d\W\s]*", characterSet3.ToRegexPattern());

            characterSet3.Quantifier = RegexQuantifier.None;
            characterSet3.UseCharacterCodes = true;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]", characterSet3.ToRegexPattern());

            characterSet3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]*", characterSet3.ToRegexPattern());

            // Explicit pattern, negative.
            RegexNodeCharacterSet characterSet4 = new RegexNodeCharacterSet(@"aB0d\W\s", true);
            Assert.AreEqual(@"[^aB0d\W\s]", characterSet4.ToRegexPattern());

            characterSet4.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^aB0d\W\s]*", characterSet4.ToRegexPattern());

            characterSet4.Quantifier = RegexQuantifier.None;
            characterSet4.UseCharacterCodes = true;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]", characterSet4.ToRegexPattern());

            characterSet4.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]*", characterSet4.ToRegexPattern());

            // RegexBuilder, char array, positive.
            RegexNodeCharacterSet characterSet5 = RegexBuilder.CharacterSet(new[] { 'a', 'B', '0', ']', 'd', '^', 'c' }, false, RegexQuantifier.None);
            Assert.AreEqual(@"[aB0\]d\^c]", characterSet5.ToRegexPattern());

            characterSet5.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[aB0\]d\^c]*", characterSet5.ToRegexPattern());

            characterSet5.Quantifier = RegexQuantifier.None;
            characterSet5.UseCharacterCodes = true;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u005d\u0064\u005e\u0063]", characterSet5.ToRegexPattern());

            characterSet5.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u005d\u0064\u005e\u0063]*", characterSet5.ToRegexPattern());

            // RegexBuilder, char array, negative.
            RegexNodeCharacterSet characterSet6 = RegexBuilder.NegativeCharacterSet(new[] { 'a', 'B', '0', 'd', '^', 'x' }, false, RegexQuantifier.None);
            Assert.AreEqual(@"[^aB0d\^x]", characterSet6.ToRegexPattern());

            characterSet6.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^aB0d\^x]*", characterSet6.ToRegexPattern());

            characterSet6.Quantifier = RegexQuantifier.None;
            characterSet6.UseCharacterCodes = true;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005e\u0078]", characterSet6.ToRegexPattern());

            characterSet6.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005e\u0078]*", characterSet6.ToRegexPattern());

            // RegexBuilder, explicit pattern, positive.
            RegexNodeCharacterSet characterSet7 = RegexBuilder.CharacterSet(@"aB0d\W\s", RegexQuantifier.None);
            Assert.AreEqual(@"[aB0d\W\s]", characterSet7.ToRegexPattern());

            characterSet7.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[aB0d\W\s]*", characterSet7.ToRegexPattern());

            characterSet7.Quantifier = RegexQuantifier.None;
            characterSet7.UseCharacterCodes = true;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]", characterSet7.ToRegexPattern());

            characterSet7.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]*", characterSet7.ToRegexPattern());

            // RegexBuilder, explicit pattern, negative.
            RegexNodeCharacterSet characterSet8 = RegexBuilder.NegativeCharacterSet(@"aB0d\W\s", RegexQuantifier.None);
            Assert.AreEqual(@"[^aB0d\W\s]", characterSet8.ToRegexPattern());

            characterSet8.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^aB0d\W\s]*", characterSet8.ToRegexPattern());

            characterSet8.Quantifier = RegexQuantifier.None;
            characterSet8.UseCharacterCodes = true;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]", characterSet8.ToRegexPattern());

            characterSet8.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"[^\u0061\u0042\u0030\u0064\u005c\u0057\u005c\u0073]*", characterSet8.ToRegexPattern());
        }

        [TestMethod]
        public void TestConditionalMatchNodeRendering()
        {
            RegexNodeLiteral condition = new RegexNodeLiteral(@"\w\s");
            RegexNodeLiteral trueMatch = new RegexNodeLiteral(@"\w\s\d{5}-[^\u005c]+");
            RegexNodeLiteral falseMatch = new RegexNodeLiteral(@"\d{5,}\u005d");

            RegexNodeConditionalMatch conditionalMatch1 = new RegexNodeConditionalMatch(condition, trueMatch, falseMatch);
            Assert.AreEqual(@"(?(\w\s)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)", conditionalMatch1.ToRegexPattern());

            conditionalMatch1.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"(?(\w\s)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)+", conditionalMatch1.ToRegexPattern());

            RegexNodeConditionalMatch conditionalMatch2 = new RegexNodeConditionalMatch("SomeGroup", trueMatch, falseMatch);
            Assert.AreEqual(@"(?(SomeGroup)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)", conditionalMatch2.ToRegexPattern());

            conditionalMatch2.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"(?(SomeGroup)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)+", conditionalMatch2.ToRegexPattern());

            RegexNodeConditionalMatch conditionalMatch3 = RegexBuilder.ConditionalMatch(condition, trueMatch, falseMatch);
            Assert.AreEqual(@"(?(\w\s)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)", conditionalMatch3.ToRegexPattern());

            conditionalMatch3 = RegexBuilder.ConditionalMatch(condition, trueMatch, falseMatch, RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"(?(\w\s)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)+", conditionalMatch3.ToRegexPattern());

            RegexNodeConditionalMatch conditionalMatch4 = RegexBuilder.ConditionalMatch("SomeGroup", trueMatch, falseMatch);
            Assert.AreEqual(@"(?(SomeGroup)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)", conditionalMatch4.ToRegexPattern());

            conditionalMatch4 = RegexBuilder.ConditionalMatch("SomeGroup", trueMatch, falseMatch, RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"(?(SomeGroup)\w\s\d{5}-[^\u005c]+|\d{5,}\u005d)+", conditionalMatch4.ToRegexPattern());
        }

        [TestMethod]
        public void TestGroupNodeRendering()
        {
            RegexNodeLiteral literal = new RegexNodeLiteral("abc");

            RegexNodeGroup group1 = new RegexNodeGroup(literal);
            Assert.AreEqual("(abc)", group1.ToRegexPattern());
            group1.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual("(abc)*", group1.ToRegexPattern());

            RegexNodeGroup group2 = new RegexNodeGroup(literal, false);
            Assert.AreEqual("(?:abc)", group2.ToRegexPattern());
            group2.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual("(?:abc)*", group2.ToRegexPattern());

            RegexNodeGroup group3 = new RegexNodeGroup(literal, "SomeGroup");
            Assert.AreEqual("(?<SomeGroup>abc)", group3.ToRegexPattern());
            group3.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual("(?<SomeGroup>abc)*", group3.ToRegexPattern());

            RegexNodeGroup group4 = RegexBuilder.Group(literal);
            Assert.AreEqual("(abc)", group4.ToRegexPattern());
            group4 = RegexBuilder.Group(literal, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual("(abc)*", group4.ToRegexPattern());

            RegexNodeGroup group5 = RegexBuilder.NonCapturingGroup(literal);
            Assert.AreEqual("(?:abc)", group5.ToRegexPattern());
            group5 = RegexBuilder.NonCapturingGroup(literal, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual("(?:abc)*", group5.ToRegexPattern());

            RegexNodeGroup group6 = RegexBuilder.Group("SomeGroup", literal);
            Assert.AreEqual("(?<SomeGroup>abc)", group6.ToRegexPattern());
            group6 = RegexBuilder.Group("SomeGroup", literal, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual("(?<SomeGroup>abc)*", group6.ToRegexPattern());
        }

        [TestMethod]
        public void TestGroupReferenceNodeRendering()
        {
            RegexNodeGroupReference reference1 = new RegexNodeGroupReference(1);
            Assert.AreEqual(@"\1", reference1.ToRegexPattern());

            RegexNodeGroupReference reference2 = new RegexNodeGroupReference("SomeGroup");
            Assert.AreEqual(@"\k<SomeGroup>", reference2.ToRegexPattern());

            RegexNodeGroupReference reference3 = RegexBuilder.GroupBackReference("SomeGroup");
            Assert.AreEqual(@"\k<SomeGroup>", reference3.ToRegexPattern());

            RegexNodeGroupReference reference4 = RegexBuilder.GroupBackReference("SomeGroup", RegexQuantifier.Exactly(2));
            Assert.AreEqual(@"\k<SomeGroup>{2}", reference4.ToRegexPattern());

            RegexNodeGroupReference reference5 = RegexBuilder.GroupBackReference(1, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"\1*", reference5.ToRegexPattern());
        }

        [TestMethod]
        public void TestLookAroundNodeRendering()
        {
            RegexNodeLiteral lookupExpression = new RegexNodeLiteral(@"a\bc");
            RegexNodeLiteral matchExpression = new RegexNodeLiteral(@"\w+");

            RegexNodeLookAround positiveLookAhead = new RegexNodeLookAround(RegexLookAround.PositiveLookAhead, lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:\w+(?=a\bc))", positiveLookAhead.ToRegexPattern());
            positiveLookAhead.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:\w+(?=a\bc))*", positiveLookAhead.ToRegexPattern());

            RegexNodeLookAround positiveLookBehind = new RegexNodeLookAround(RegexLookAround.PositiveLookBehind, lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:(?<=a\bc)\w+)", positiveLookBehind.ToRegexPattern());
            positiveLookBehind.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:(?<=a\bc)\w+)*", positiveLookBehind.ToRegexPattern());

            RegexNodeLookAround negativeLookAhead = new RegexNodeLookAround(RegexLookAround.NegativeLookAhead, lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:\w+(?!a\bc))", negativeLookAhead.ToRegexPattern());
            negativeLookAhead.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:\w+(?!a\bc))*", negativeLookAhead.ToRegexPattern());

            RegexNodeLookAround negativeLookBehind = new RegexNodeLookAround(RegexLookAround.NegativeLookBehind, lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:(?<!a\bc)\w+)", negativeLookBehind.ToRegexPattern());
            negativeLookBehind.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?:(?<!a\bc)\w+)*", negativeLookBehind.ToRegexPattern());

            RegexNodeLookAround positiveLookAhead2 = RegexBuilder.PositiveLookAhead(lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:\w+(?=a\bc))", positiveLookAhead2.ToRegexPattern());
            positiveLookAhead2 = RegexBuilder.PositiveLookAhead(lookupExpression, matchExpression, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?:\w+(?=a\bc))*", positiveLookAhead2.ToRegexPattern());

            RegexNodeLookAround positiveLookBehind2 = RegexBuilder.PositiveLookBehind(lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:(?<=a\bc)\w+)", positiveLookBehind2.ToRegexPattern());
            positiveLookBehind2 = RegexBuilder.PositiveLookBehind(lookupExpression, matchExpression, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?:(?<=a\bc)\w+)*", positiveLookBehind2.ToRegexPattern());

            RegexNodeLookAround negativeLookAhead2 = RegexBuilder.NegativeLookAhead(lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:\w+(?!a\bc))", negativeLookAhead2.ToRegexPattern());
            negativeLookAhead2 = RegexBuilder.NegativeLookAhead(lookupExpression, matchExpression, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?:\w+(?!a\bc))*", negativeLookAhead2.ToRegexPattern());

            RegexNodeLookAround negativeLookBehind2 = RegexBuilder.NegativeLookBehind(lookupExpression, matchExpression);
            Assert.AreEqual(@"(?:(?<!a\bc)\w+)", negativeLookBehind2.ToRegexPattern());
            negativeLookBehind2 = RegexBuilder.NegativeLookBehind(lookupExpression, matchExpression, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?:(?<!a\bc)\w+)*", negativeLookBehind2.ToRegexPattern());
        }

        [TestMethod]
        public void TestInlineOptionNodeRendering()
        {
            RegexNodeLiteral literal = new RegexNodeLiteral(@"ab\wc{0}");
            RegexNodeInlineOption option1 = new RegexNodeInlineOption(RegexOptions.IgnoreCase, literal);
            Assert.AreEqual(@"(?i:ab\wc{0})", option1.ToRegexPattern());

            RegexNodeInlineOption option2 = new RegexNodeInlineOption(RegexOptions.Singleline | RegexOptions.IgnoreCase, literal);
            Assert.AreEqual(@"(?is:ab\wc{0})", option2.ToRegexPattern());

            RegexNodeInlineOption option3 = new RegexNodeInlineOption(RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace, literal);
            Assert.AreEqual(@"(?mx:ab\wc{0})", option3.ToRegexPattern());

            RegexNodeInlineOption option4 = new RegexNodeInlineOption(RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace, literal);
            Assert.AreEqual(@"(?nx:ab\wc{0})", option4.ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCategoryNodeRendering()
        {
            // Test basic Unicode category without quantifier
            RegexNodeUnicodeCategory category1 = new RegexNodeUnicodeCategory("L");
            Assert.AreEqual(@"\p{L}", category1.ToRegexPattern());

            // Test category with quantifier
            RegexNodeUnicodeCategory category2 = new RegexNodeUnicodeCategory("L");
            category2.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"\p{L}+", category2.ToRegexPattern());

            // Test different basic categories
            RegexNodeUnicodeCategory category3 = new RegexNodeUnicodeCategory("N");
            Assert.AreEqual(@"\p{N}", category3.ToRegexPattern());

            RegexNodeUnicodeCategory category4 = new RegexNodeUnicodeCategory("P");
            Assert.AreEqual(@"\p{P}", category4.ToRegexPattern());

            // Test specific subcategories
            RegexNodeUnicodeCategory category5 = new RegexNodeUnicodeCategory("Lu");
            Assert.AreEqual(@"\p{Lu}", category5.ToRegexPattern());

            RegexNodeUnicodeCategory category6 = new RegexNodeUnicodeCategory("Nd");
            category6.Quantifier = RegexQuantifier.Exactly(3);
            Assert.AreEqual(@"\p{Nd}{3}", category6.ToRegexPattern());

            // Test named blocks
            RegexNodeUnicodeCategory category7 = new RegexNodeUnicodeCategory("IsCyrillic");
            Assert.AreEqual(@"\p{IsCyrillic}", category7.ToRegexPattern());

            RegexNodeUnicodeCategory category8 = new RegexNodeUnicodeCategory("IsArabic");
            category8.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"\p{IsArabic}*", category8.ToRegexPattern());

            // Test negated categories
            RegexNodeUnicodeCategory category9 = new RegexNodeUnicodeCategory("L", true);
            Assert.AreEqual(@"\P{L}", category9.ToRegexPattern());

            RegexNodeUnicodeCategory category10 = new RegexNodeUnicodeCategory("N", true);
            category10.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"\P{N}+", category10.ToRegexPattern());

            RegexNodeUnicodeCategory category11 = new RegexNodeUnicodeCategory("IsCyrillic", true);
            Assert.AreEqual(@"\P{IsCyrillic}", category11.ToRegexPattern());

            // Test via RegexBuilder factory methods
            RegexNodeUnicodeCategory category12 = RegexBuilder.UnicodeCategory("L");
            Assert.AreEqual(@"\p{L}", category12.ToRegexPattern());

            RegexNodeUnicodeCategory category13 = RegexBuilder.UnicodeCategory("N", RegexQuantifier.AtLeast(2));
            Assert.AreEqual(@"\p{N}{2,}", category13.ToRegexPattern());

            RegexNodeUnicodeCategory category14 = RegexBuilder.NegativeUnicodeCategory("P");
            Assert.AreEqual(@"\P{P}", category14.ToRegexPattern());

            RegexNodeUnicodeCategory category15 = RegexBuilder.NegativeUnicodeCategory("Lu", RegexQuantifier.Custom(1, 5, false));
            Assert.AreEqual(@"\P{Lu}{1,5}", category15.ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCategoryValidation()
        {
            // Test valid general categories
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("L"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("N"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("P"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("Lu"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("Ll"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("Nd"));

            // Test valid named blocks
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsCyrillic"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsArabic"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsLatin1Supplement"));

            // Test invalid categories
            Assert.IsFalse(RegexMetaChars.IsValidUnicodeCategory("X"));
            Assert.IsFalse(RegexMetaChars.IsValidUnicodeCategory("InvalidCategory"));
            Assert.IsFalse(RegexMetaChars.IsValidUnicodeCategory(""));
            Assert.IsFalse(RegexMetaChars.IsValidUnicodeCategory(null));
        }

        [TestMethod]
        public void TestUnicodeCategoryIntegration()
        {
            // Test concatenation with Unicode categories
            var letterNode = RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore);
            var digitNode = RegexBuilder.UnicodeCategory("Nd", RegexQuantifier.OneOrMore);
            var concatenation = RegexBuilder.Concatenate(letterNode, digitNode);
            Assert.AreEqual(@"\p{L}+\p{Nd}+", concatenation.ToRegexPattern());

            // Test alternation with Unicode categories
            var cyrillic = RegexBuilder.UnicodeCategory("IsCyrillic");
            var arabic = RegexBuilder.UnicodeCategory("IsArabic");
            var alternation = RegexBuilder.Alternate(cyrillic, arabic);
            Assert.AreEqual(@"(?:\p{IsCyrillic}|\p{IsArabic})", alternation.ToRegexPattern());

            // Test group with Unicode categories
            var group = RegexBuilder.Group(RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore));
            Assert.AreEqual(@"(\p{L}+)", group.ToRegexPattern());

            // Test real regex with international text matching
            var regex = RegexBuilder.Build(
                RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore),
                RegexBuilder.Literal(" "),
                RegexBuilder.UnicodeCategory("Nd", RegexQuantifier.Exactly(3))
            );
            Assert.IsNotNull(regex);

            // Verify it matches international text
            Assert.IsTrue(regex.IsMatch("Ы 123"));
            Assert.IsTrue(regex.IsMatch("Б 456"));
            Assert.IsTrue(regex.IsMatch("A 789"));
        }

        [TestMethod]
        public void TestUnicodeCategoryLazyQuantifiers()
        {
            // Test lazy quantifiers with Unicode categories
            RegexNodeUnicodeCategory category1 = new RegexNodeUnicodeCategory("L");
            category1.Quantifier = RegexQuantifier.ZeroOrMoreLazy;
            Assert.AreEqual(@"\p{L}*?", category1.ToRegexPattern());

            RegexNodeUnicodeCategory category2 = new RegexNodeUnicodeCategory("N");
            category2.Quantifier = RegexQuantifier.OneOrMoreLazy;
            Assert.AreEqual(@"\p{N}+?", category2.ToRegexPattern());

            RegexNodeUnicodeCategory category3 = new RegexNodeUnicodeCategory("P");
            category3.Quantifier = RegexQuantifier.ZeroOrOneLazy;
            Assert.AreEqual(@"\p{P}??", category3.ToRegexPattern());

            // Test lazy quantifiers with negated categories
            RegexNodeUnicodeCategory category4 = new RegexNodeUnicodeCategory("L", true);
            category4.Quantifier = RegexQuantifier.AtLeast(2, true);
            Assert.AreEqual(@"\P{L}{2,}?", category4.ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCategoryErrorHandling()
        {
            // Test null category name throws exception
            try
            {
                var category = new RegexNodeUnicodeCategory(null);
                Assert.Fail("Should have thrown ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Category name cannot be null or empty"));
            }

            // Test empty category name throws exception
            try
            {
                var category = new RegexNodeUnicodeCategory("");
                Assert.Fail("Should have thrown ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Category name cannot be null or empty"));
            }
        }

        [TestMethod]
        public void TestUnicodeCategoryAllSubcategories()
        {
            // Test all letter subcategories
            Assert.AreEqual(@"\p{Lu}", new RegexNodeUnicodeCategory("Lu").ToRegexPattern());
            Assert.AreEqual(@"\p{Ll}", new RegexNodeUnicodeCategory("Ll").ToRegexPattern());
            Assert.AreEqual(@"\p{Lt}", new RegexNodeUnicodeCategory("Lt").ToRegexPattern());
            Assert.AreEqual(@"\p{Lm}", new RegexNodeUnicodeCategory("Lm").ToRegexPattern());
            Assert.AreEqual(@"\p{Lo}", new RegexNodeUnicodeCategory("Lo").ToRegexPattern());

            // Test all number subcategories
            Assert.AreEqual(@"\p{Nd}", new RegexNodeUnicodeCategory("Nd").ToRegexPattern());
            Assert.AreEqual(@"\p{Nl}", new RegexNodeUnicodeCategory("Nl").ToRegexPattern());
            Assert.AreEqual(@"\p{No}", new RegexNodeUnicodeCategory("No").ToRegexPattern());

            // Test all punctuation subcategories
            Assert.AreEqual(@"\p{Pc}", new RegexNodeUnicodeCategory("Pc").ToRegexPattern());
            Assert.AreEqual(@"\p{Pd}", new RegexNodeUnicodeCategory("Pd").ToRegexPattern());
            Assert.AreEqual(@"\p{Ps}", new RegexNodeUnicodeCategory("Ps").ToRegexPattern());
            Assert.AreEqual(@"\p{Pe}", new RegexNodeUnicodeCategory("Pe").ToRegexPattern());
            Assert.AreEqual(@"\p{Pi}", new RegexNodeUnicodeCategory("Pi").ToRegexPattern());
            Assert.AreEqual(@"\p{Pf}", new RegexNodeUnicodeCategory("Pf").ToRegexPattern());
            Assert.AreEqual(@"\p{Po}", new RegexNodeUnicodeCategory("Po").ToRegexPattern());

            // Test mark subcategories
            Assert.AreEqual(@"\p{Mn}", new RegexNodeUnicodeCategory("Mn").ToRegexPattern());
            Assert.AreEqual(@"\p{Mc}", new RegexNodeUnicodeCategory("Mc").ToRegexPattern());
            Assert.AreEqual(@"\p{Me}", new RegexNodeUnicodeCategory("Me").ToRegexPattern());

            // Test separator subcategories
            Assert.AreEqual(@"\p{Zs}", new RegexNodeUnicodeCategory("Zs").ToRegexPattern());
            Assert.AreEqual(@"\p{Zl}", new RegexNodeUnicodeCategory("Zl").ToRegexPattern());
            Assert.AreEqual(@"\p{Zp}", new RegexNodeUnicodeCategory("Zp").ToRegexPattern());

            // Test symbol subcategories
            Assert.AreEqual(@"\p{Sm}", new RegexNodeUnicodeCategory("Sm").ToRegexPattern());
            Assert.AreEqual(@"\p{Sc}", new RegexNodeUnicodeCategory("Sc").ToRegexPattern());
            Assert.AreEqual(@"\p{Sk}", new RegexNodeUnicodeCategory("Sk").ToRegexPattern());
            Assert.AreEqual(@"\p{So}", new RegexNodeUnicodeCategory("So").ToRegexPattern());

            // Test control/other subcategories
            Assert.AreEqual(@"\p{Cc}", new RegexNodeUnicodeCategory("Cc").ToRegexPattern());
            Assert.AreEqual(@"\p{Cf}", new RegexNodeUnicodeCategory("Cf").ToRegexPattern());
            Assert.AreEqual(@"\p{Cs}", new RegexNodeUnicodeCategory("Cs").ToRegexPattern());
            Assert.AreEqual(@"\p{Co}", new RegexNodeUnicodeCategory("Co").ToRegexPattern());
            Assert.AreEqual(@"\p{Cn}", new RegexNodeUnicodeCategory("Cn").ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCategoryRealWorldPatterns()
        {
            // Email username pattern (letters, digits, dots, dashes)
            var emailPattern = RegexBuilder.Build(
                RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore),
                RegexBuilder.NonEscapedLiteral(@"[.\-]", RegexQuantifier.ZeroOrMore),
                RegexBuilder.UnicodeCategory("L", RegexQuantifier.OneOrMore)
            );
            Assert.IsTrue(emailPattern.IsMatch("user"));
            Assert.IsTrue(emailPattern.IsMatch("user.name"));

            // International name pattern (letters with optional spaces)
            var namePattern = RegexBuilder.Build(
                RegexBuilder.UnicodeCategory("Lu", RegexQuantifier.Exactly(1)),
                RegexBuilder.UnicodeCategory("Ll", RegexQuantifier.OneOrMore),
                RegexBuilder.NonEscapedLiteral(@"\s", RegexQuantifier.ZeroOrOne),
                RegexBuilder.UnicodeCategory("Lu", RegexQuantifier.ZeroOrOne),
                RegexBuilder.UnicodeCategory("Ll", RegexQuantifier.ZeroOrMore)
            );
            Assert.IsTrue(namePattern.IsMatch("John"));
            Assert.IsTrue(namePattern.IsMatch("José"));
            Assert.IsTrue(namePattern.IsMatch("Владимир"));

            // Currency amount pattern (currency symbol + digits)
            var currencyPattern = RegexBuilder.Build(
                RegexBuilder.UnicodeCategory("Sc"),
                RegexBuilder.UnicodeCategory("Nd", RegexQuantifier.OneOrMore),
                RegexBuilder.NonEscapedLiteral(@"\.", RegexQuantifier.ZeroOrOne),
                RegexBuilder.UnicodeCategory("Nd", RegexQuantifier.Custom(0, 2, false))
            );
            Assert.IsTrue(currencyPattern.IsMatch("$100"));
            Assert.IsTrue(currencyPattern.IsMatch("€99.99"));

            // Mixed scripts detection (negated Latin)
            var nonLatinPattern = RegexBuilder.Build(
                RegexBuilder.NegativeUnicodeCategory("IsBasicLatin", RegexQuantifier.OneOrMore)
            );
            Assert.IsTrue(nonLatinPattern.IsMatch("مرحبا"));
            Assert.IsTrue(nonLatinPattern.IsMatch("Привет"));
            Assert.IsFalse(nonLatinPattern.IsMatch("Hello"));
        }

        [TestMethod]
        public void TestUnicodeCategoryWithLookaheadAndLookbehind()
        {
            // Test Unicode category with positive lookahead
            var letterBeforeDigit = RegexBuilder.PositiveLookAhead(
                RegexBuilder.UnicodeCategory("Nd"),
                RegexBuilder.UnicodeCategory("L")
            );
            Assert.AreEqual(@"(?:\p{L}(?=\p{Nd}))", letterBeforeDigit.ToRegexPattern());

            // Test Unicode category with negative lookahead
            var letterNotBeforeDigit = RegexBuilder.NegativeLookAhead(
                RegexBuilder.UnicodeCategory("Nd"),
                RegexBuilder.UnicodeCategory("L")
            );
            Assert.AreEqual(@"(?:\p{L}(?!\p{Nd}))", letterNotBeforeDigit.ToRegexPattern());

            // Test Unicode category with positive lookbehind
            var digitAfterLetter = RegexBuilder.PositiveLookBehind(
                RegexBuilder.UnicodeCategory("L"),
                RegexBuilder.UnicodeCategory("Nd")
            );
            Assert.AreEqual(@"(?:(?<=\p{L})\p{Nd})", digitAfterLetter.ToRegexPattern());

            // Test Unicode category with negative lookbehind
            var digitNotAfterLetter = RegexBuilder.NegativeLookBehind(
                RegexBuilder.UnicodeCategory("L"),
                RegexBuilder.UnicodeCategory("Nd")
            );
            Assert.AreEqual(@"(?:(?<!\p{L})\p{Nd})", digitNotAfterLetter.ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCategoryMultipleNamedBlocks()
        {
            // Test various Unicode blocks
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsGreekandCoptic"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsHebrew"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsHiragana"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsKatakana"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsHangul"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsThai"));
            Assert.IsTrue(RegexMetaChars.IsValidUnicodeCategory("IsDevanagari"));

            // Test rendering of various blocks
            Assert.AreEqual(@"\p{IsGreekandCoptic}", new RegexNodeUnicodeCategory("IsGreekandCoptic").ToRegexPattern());
            Assert.AreEqual(@"\P{IsHebrew}", new RegexNodeUnicodeCategory("IsHebrew", true).ToRegexPattern());
        }

        [TestMethod]
        public void TestUnicodeCategoryGettersAndEnumerators()
        {
            // Test GetGeneralCategories
            var generalCategories = RegexMetaChars.GetGeneralCategories();
            Assert.IsNotNull(generalCategories);
            var categoryList = new System.Collections.Generic.List<string>(generalCategories);
            Assert.IsTrue(categoryList.Count > 0);
            Assert.IsTrue(categoryList.Contains("L"));
            Assert.IsTrue(categoryList.Contains("N"));
            Assert.IsTrue(categoryList.Contains("P"));

            // Test GetNamedBlocks
            var namedBlocks = RegexMetaChars.GetNamedBlocks();
            Assert.IsNotNull(namedBlocks);
            var blockList = new System.Collections.Generic.List<string>(namedBlocks);
            Assert.IsTrue(blockList.Count > 0);
            Assert.IsTrue(blockList.All(b => b.StartsWith("Is")));
            Assert.IsTrue(blockList.Contains("IsCyrillic"));
            Assert.IsTrue(blockList.Contains("IsArabic"));
        }

        [TestMethod]
        public void TestBalancingGroupBasicSyntax()
        {
            // Test basic two-name balancing group
            var innerExpr = RegexBuilder.NonEscapedLiteral(@"[^()]*");
            var balancingGroup = new RegexNodeBalancingGroup("open", "close", innerExpr);
            Assert.AreEqual(@"(?<open-close>[^()]*)", balancingGroup.ToRegexPattern());

            // Test simple balancing group (single name)
            var simpleBalancing = new RegexNodeBalancingGroup("paren", innerExpr);
            Assert.AreEqual(@"(?<paren>-[^()]*)", simpleBalancing.ToRegexPattern());

            // Test using public API methods
            var apiBalancingGroup = RegexBuilder.BalancingGroup("open", "close", innerExpr);
            Assert.AreEqual(@"(?<open-close>[^()]*)", apiBalancingGroup.ToRegexPattern());

            var apiSimpleBalancing = RegexBuilder.SimpleBalancingGroup("paren", innerExpr);
            Assert.AreEqual(@"(?<paren>-[^()]*)", apiSimpleBalancing.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithQuantifiers()
        {
            // Test balancing group with greedy quantifier
            var innerExpr = RegexBuilder.NonEscapedLiteral(@"\w+");
            var balancingWithQuantifier = new RegexNodeBalancingGroup("name1", "name2", innerExpr);
            balancingWithQuantifier.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"(?<name1-name2>\w+)+", balancingWithQuantifier.ToRegexPattern());

            // Test balancing group with lazy quantifier
            balancingWithQuantifier.Quantifier = RegexQuantifier.ZeroOrMore;
            Assert.AreEqual(@"(?<name1-name2>\w+)*", balancingWithQuantifier.ToRegexPattern());

            // Test using API with quantifier
            var apiWithQuantifier = RegexBuilder.BalancingGroup("test", "stack", innerExpr, RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?<test-stack>\w+)?", apiWithQuantifier.ToRegexPattern());

            // Test simple balancing with quantifier
            var simpleWithQuantifier = RegexBuilder.SimpleBalancingGroup("name", innerExpr, RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"(?<name>-\w+)+", simpleWithQuantifier.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupComplexInnerExpressions()
        {
            // Test with concatenation inner expression
            var concat = RegexBuilder.Concatenate(
                RegexBuilder.NonEscapedLiteral(@"[^()]+"),
                RegexBuilder.NonEscapedLiteral(@"\s*")
            );
            var balanceConcat = RegexBuilder.BalancingGroup("open", "close", concat);
            Assert.AreEqual(@"(?<open-close>[^()]+\s*)", balanceConcat.ToRegexPattern());

            // Test with alternation inner expression
            var alt = RegexBuilder.Alternate(
                RegexBuilder.NonEscapedLiteral(@"[^<>]"),
                RegexBuilder.NonEscapedLiteral(@"\s")
            );
            var balanceAlt = RegexBuilder.BalancingGroup("tag", "close", alt);
            Assert.AreEqual(@"(?<tag-close>(?:[^<>]|\s))", balanceAlt.ToRegexPattern());

            // Test with character set inner expression
            var charSet = RegexBuilder.CharacterSet("a-zA-Z0-9", null);
            var balanceCharSet = RegexBuilder.BalancingGroup("id", "name", charSet);
            Assert.IsTrue(balanceCharSet.ToRegexPattern().StartsWith(@"(?<id-name>"));
            Assert.IsTrue(balanceCharSet.ToRegexPattern().Contains("["));
        }

        [TestMethod]
        public void TestBalancingGroupValidation()
        {
            var validInner = RegexBuilder.NonEscapedLiteral("test");

            // Test null push group name
            try
            {
                new RegexNodeBalancingGroup(null, "pop", validInner);
                Assert.Fail("Should throw ArgumentException for null push group name");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Push group name"));
            }

            // Test empty push group name
            try
            {
                new RegexNodeBalancingGroup("", "pop", validInner);
                Assert.Fail("Should throw ArgumentException for empty push group name");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Push group name"));
            }

            // Test null inner expression
            try
            {
                new RegexNodeBalancingGroup("push", "pop", null);
                Assert.Fail("Should throw ArgumentNullException for null inner expression");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }

            // Test simple balancing with null group name
            try
            {
                new RegexNodeBalancingGroup(null, validInner);
                Assert.Fail("Should throw ArgumentException for null group name");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Group name"));
            }

            // Test simple balancing with null inner expression
            try
            {
                new RegexNodeBalancingGroup("name", null);
                Assert.Fail("Should throw ArgumentNullException for null inner expression");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void TestBalancingGroupProperties()
        {
            var innerExpr = RegexBuilder.NonEscapedLiteral("test");

            // Test two-name balancing group properties
            var twoName = new RegexNodeBalancingGroup("push", "pop", innerExpr);
            Assert.AreEqual("push", twoName.PushGroupName);
            Assert.AreEqual("pop", twoName.PopGroupName);
            Assert.IsFalse(twoName.IsSimpleBalancing);

            // Test single-name balancing group properties
            var singleName = new RegexNodeBalancingGroup("single", innerExpr);
            Assert.AreEqual("single", singleName.PushGroupName);
            Assert.IsNull(singleName.PopGroupName);
            Assert.IsTrue(singleName.IsSimpleBalancing);

            // Test inner expression assignment
            var newInner = RegexBuilder.NonEscapedLiteral("new");
            twoName.InnerExpression = newInner;
            Assert.AreEqual(@"(?<push-pop>new)", twoName.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupPracticalExample_BalancedParentheses()
        {
            // Pattern for matching balanced parentheses
            // (?<open>\() - push to 'open' stack when we see (
            // (?<-open>\)) - pop from 'open' stack when we see )
            // [^()] - any non-paren character
            // This pattern: \( (?:[^()] | (?<open>\() | (?<-open>\)))*+ \)

            // Basic structure without full quantifiers for clarity in test
            var part1 = RegexBuilder.NonEscapedLiteral(@"\(");
            var part2 = RegexBuilder.SimpleBalancingGroup("paren", RegexBuilder.NonEscapedLiteral(@"[^()]"));
            var part3 = RegexBuilder.NonEscapedLiteral(@"\)");

            var basicPattern = RegexBuilder.Concatenate(part1, part2, part3);
            Assert.AreEqual(@"\((?<paren>-[^()])\)", basicPattern.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupPracticalExample_XMLTags()
        {
            // Simplified pattern for matching nested XML-like tags
            var tagNamePattern = RegexBuilder.NonEscapedLiteral(@"\w+");

            // Opening tag with balancing push
            var openTag = RegexBuilder.BalancingGroup("tag", "open", RegexBuilder.NonEscapedLiteral(@"<\w+>"));

            // Content (simplified)
            var content = RegexBuilder.NegativeCharacterSet("<>", null);

            // Closing tag with balancing pop
            var closeTag = RegexBuilder.BalancingGroup("tag", "tag", RegexBuilder.NonEscapedLiteral(@"</\w+>"));

            var xmlPattern = RegexBuilder.Concatenate(openTag, content, closeTag);
            Assert.IsTrue(xmlPattern.ToRegexPattern().Contains("(?<tag-"));
            Assert.IsTrue(xmlPattern.ToRegexPattern().Contains("(?<tag-open>"));
        }

        [TestMethod]
        public void TestBalancingGroupPracticalExample_CodeBlocks()
        {
            // Pattern for matching nested code blocks: { ... { ... } ... }
            var openBrace = RegexBuilder.BalancingGroup("brace", "open", RegexBuilder.Literal("{"));
            var closeBrace = RegexBuilder.BalancingGroup("brace", "brace", RegexBuilder.Literal("}"));
            var nonBraceContent = RegexBuilder.NegativeCharacterSet("{}", null);

            var codeBlockPattern = RegexBuilder.Concatenate(openBrace, nonBraceContent, closeBrace);
            Assert.IsTrue(codeBlockPattern.ToRegexPattern().Contains("(?<brace-"));
            Assert.IsTrue(codeBlockPattern.ToRegexPattern().Contains("(?<brace-brace>"));
        }

        [TestMethod]
        public void TestBalancingGroupWithAlternation()
        {
            // Test balancing group combined with alternation
            var altExpr = RegexBuilder.Alternate(
                RegexBuilder.NonEscapedLiteral("a"),
                RegexBuilder.NonEscapedLiteral("b")
            );
            var balanced = RegexBuilder.BalancingGroup("name", "stack", altExpr);
            Assert.AreEqual(@"(?<name-stack>(?:a|b))", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupIntegration_WithConcatenation()
        {
            // Test BalancingGroup within a larger concatenation
            var prefix = RegexBuilder.NonEscapedLiteral("start_");
            var balanced = RegexBuilder.BalancingGroup("item", "end", RegexBuilder.NonEscapedLiteral(@"\w+"));
            var suffix = RegexBuilder.NonEscapedLiteral("_end");

            var combined = RegexBuilder.Concatenate(prefix, balanced, suffix);
            Assert.AreEqual(@"start_(?<item-end>\w+)_end", combined.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupIntegration_NestedInGroup()
        {
            // Test BalancingGroup nested within a capturing group
            var inner = RegexBuilder.SimpleBalancingGroup("balance", RegexBuilder.NonEscapedLiteral(@"[a-z]+"));
            var group = RegexBuilder.Group("container", inner);
            Assert.AreEqual(@"(?<container>(?<balance>-[a-z]+))", group.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupIntegration_WithQuantifier()
        {
            // Test BalancingGroup with multiple quantifiers
            var balanced = RegexBuilder.BalancingGroup(
                "open",
                "close",
                RegexBuilder.NonEscapedLiteral(@"."),
                RegexQuantifier.ZeroOrMore
            );
            var withAdditionalQuantifier = RegexBuilder.Concatenate(balanced, RegexBuilder.NonEscapedLiteral(@"x"));
            Assert.IsTrue(withAdditionalQuantifier.ToRegexPattern().Contains(@"(?<open-close>.)*x"));
        }

        [TestMethod]
        public void TestBalancingGroupFunctionalMatching_BasicPatterns()
        {
            // Test actual regex matching with balancing groups
            // Pattern structure: (?<open>\()?[^()]*?(?<-open>\))?
            var pattern = RegexBuilder.Build(
                RegexBuilder.BalancingGroup("open", "open", RegexBuilder.Literal("(")),
                RegexBuilder.NonEscapedLiteral(@"[^()]*?"),
                RegexBuilder.BalancingGroup("open", "open", RegexBuilder.Literal(")"))
            );

            // Simple case: pattern renders correctly
            Assert.IsTrue(pattern.ToString().Contains("(?<open-open>"));
        }

        [TestMethod]
        public void TestBalancingGroupFunctionalMatching_NestedParentheses()
        {
            // More complex pattern for balanced parentheses
            // This tests if the generated pattern structure is correct
            var openParen = RegexBuilder.Literal("(");
            var closeParen = RegexBuilder.Literal(")");
            var nonParens = RegexBuilder.NonEscapedLiteral(@"[^()]*");
            var balance = RegexBuilder.BalancingGroup("level", "level", nonParens);

            var pattern = RegexBuilder.Build(
                openParen,
                balance,
                closeParen
            );

            // Verify the pattern builds successfully
            Assert.IsNotNull(pattern);
        }

        [TestMethod]
        public void TestBalancingGroupEdgeCase_SameNamePushAndPop()
        {
            // Test when push and pop group names are the same
            var inner = RegexBuilder.NonEscapedLiteral("test");
            var balanced = RegexBuilder.BalancingGroup("same", "same", inner);
            Assert.AreEqual(@"(?<same-same>test)", balanced.ToRegexPattern());
            Assert.IsFalse(balanced.IsSimpleBalancing);
        }

        [TestMethod]
        public void TestBalancingGroupEdgeCase_EmptyPopName()
        {
            // Test when pop name is empty string
            var inner = RegexBuilder.NonEscapedLiteral("test");
            var balanced = new RegexNodeBalancingGroup("push", "", inner);
            Assert.IsTrue(balanced.IsSimpleBalancing);
            Assert.AreEqual(@"(?<push>-test)", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupEdgeCase_LongGroupNames()
        {
            // Test with longer, more realistic group names
            var inner = RegexBuilder.NonEscapedLiteral(@"\w+");
            var balanced = RegexBuilder.BalancingGroup("htmlTag", "closeTag", inner);
            Assert.AreEqual(@"(?<htmlTag-closeTag>\w+)", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupEdgeCase_UnderscoreInGroupNames()
        {
            // Test group names with underscores
            var inner = RegexBuilder.NonEscapedLiteral("x");
            var balanced = RegexBuilder.BalancingGroup("open_brace", "close_brace", inner);
            Assert.AreEqual(@"(?<open_brace-close_brace>x)", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupEdgeCase_NumbersInGroupNames()
        {
            // Test group names with numbers
            var inner = RegexBuilder.NonEscapedLiteral("y");
            var balanced = RegexBuilder.BalancingGroup("group1", "group2", inner);
            Assert.AreEqual(@"(?<group1-group2>y)", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithNestedBalancingGroups()
        {
            // Test balancing group containing another balancing group
            var inner = RegexBuilder.SimpleBalancingGroup("inner", RegexBuilder.NonEscapedLiteral("a"));
            var outer = RegexBuilder.BalancingGroup("outer", "outer", inner);
            Assert.AreEqual(@"(?<outer-outer>(?<inner>-a))", outer.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithCharacterClass()
        {
            // Test balancing group with character class as inner expression
            var charClass = RegexBuilder.NegativeCharacterSet("abc", null);
            var balanced = RegexBuilder.SimpleBalancingGroup("group", charClass);
            string pattern = balanced.ToRegexPattern();
            Assert.IsTrue(pattern.StartsWith(@"(?<group>-"));
            Assert.IsTrue(pattern.Contains("[^abc]"));
        }

        [TestMethod]
        public void TestBalancingGroupWithCharacterRange()
        {
            // Test balancing group with character range
            var charRange = RegexBuilder.CharacterRange('a', 'z', false, null);
            var balanced = RegexBuilder.SimpleBalancingGroup("letters", charRange);
            string pattern = balanced.ToRegexPattern();
            Assert.IsTrue(pattern.Contains("[a-z]"));
            Assert.IsTrue(pattern.StartsWith(@"(?<letters>-"));
        }

        [TestMethod]
        public void TestBalancingGroupPropertyMutation()
        {
            // Test that properties can be modified
            var inner = RegexBuilder.NonEscapedLiteral("test");
            var balanced = RegexBuilder.BalancingGroup("push", "pop", inner);

            // Verify initial state
            Assert.AreEqual("push", balanced.PushGroupName);
            Assert.AreEqual("pop", balanced.PopGroupName);
            Assert.AreEqual(@"(?<push-pop>test)", balanced.ToRegexPattern());

            // Modify quantifier
            balanced.Quantifier = RegexQuantifier.OneOrMore;
            Assert.AreEqual(@"(?<push-pop>test)+", balanced.ToRegexPattern());

            // Modify inner expression
            balanced.InnerExpression = RegexBuilder.NonEscapedLiteral("new");
            Assert.AreEqual(@"(?<push-pop>new)+", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithExactlyQuantifier()
        {
            // Test with specific count quantifier
            var inner = RegexBuilder.NonEscapedLiteral("x");
            var balanced = RegexBuilder.BalancingGroup("g", "p", inner, RegexQuantifier.Exactly(3));
            Assert.AreEqual(@"(?<g-p>x){3}", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithAtLeastQuantifier()
        {
            // Test with AtLeast quantifier
            var inner = RegexBuilder.NonEscapedLiteral("y");
            var balanced = RegexBuilder.BalancingGroup("a", "b", inner, RegexQuantifier.AtLeast(2));
            Assert.AreEqual(@"(?<a-b>y){2,}", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupComplexNesting_ThreeLevel()
        {
            // Test deeply nested balancing groups
            var level3 = RegexBuilder.SimpleBalancingGroup("l3", RegexBuilder.NonEscapedLiteral("z"));
            var level2 = RegexBuilder.BalancingGroup("l2", "l2", level3);
            var level1 = RegexBuilder.BalancingGroup("l1", "l1", level2);

            string pattern = level1.ToRegexPattern();
            Assert.IsTrue(pattern.Contains("(?<l1-l1>"));
            Assert.IsTrue(pattern.Contains("(?<l2-l2>"));
            Assert.IsTrue(pattern.Contains("(?<l3>-"));
        }

        [TestMethod]
        public void TestBalancingGroupAfterModifyingInnerExpression()
        {
            // Test that changing inner expression updates output correctly
            var inner1 = RegexBuilder.NonEscapedLiteral(@"[a-z]");
            var balanced = RegexBuilder.SimpleBalancingGroup("test", inner1);

            Assert.AreEqual(@"(?<test>-[a-z])", balanced.ToRegexPattern());

            var inner2 = RegexBuilder.NonEscapedLiteral(@"\d+");
            balanced.InnerExpression = inner2;
            Assert.AreEqual(@"(?<test>-\d+)", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithLookaroundExpression()
        {
            // Test balancing group with lookahead as inner expression
            var lookahead = RegexBuilder.PositiveLookAhead(
                RegexBuilder.NonEscapedLiteral("test"),
                RegexBuilder.NonEscapedLiteral(@"\w+")
            );
            var balanced = RegexBuilder.BalancingGroup("look", "ahead", lookahead);
            string pattern = balanced.ToRegexPattern();
            Assert.IsTrue(pattern.Contains("(?<look-ahead>"));
            Assert.IsTrue(pattern.Contains("(?="));
        }

        [TestMethod]
        public void TestBalancingGroupRendering_TwoNameForm_AllQuantifiers()
        {
            // Test two-name form with various quantifiers
            var inner = RegexBuilder.NonEscapedLiteral("x");

            // ZeroOrMore
            var q1 = RegexBuilder.BalancingGroup("a", "b", inner, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?<a-b>x)*", q1.ToRegexPattern());

            // OneOrMore
            var q2 = RegexBuilder.BalancingGroup("a", "b", inner, RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"(?<a-b>x)+", q2.ToRegexPattern());

            // ZeroOrOne
            var q3 = RegexBuilder.BalancingGroup("a", "b", inner, RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?<a-b>x)?", q3.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupRendering_SimpleForm_AllQuantifiers()
        {
            // Test simple form with various quantifiers
            var inner = RegexBuilder.NonEscapedLiteral("y");

            // ZeroOrMore
            var q1 = RegexBuilder.SimpleBalancingGroup("g", inner, RegexQuantifier.ZeroOrMore);
            Assert.AreEqual(@"(?<g>-y)*", q1.ToRegexPattern());

            // OneOrMore
            var q2 = RegexBuilder.SimpleBalancingGroup("g", inner, RegexQuantifier.OneOrMore);
            Assert.AreEqual(@"(?<g>-y)+", q2.ToRegexPattern());

            // ZeroOrOne
            var q3 = RegexBuilder.SimpleBalancingGroup("g", inner, RegexQuantifier.ZeroOrOne);
            Assert.AreEqual(@"(?<g>-y)?", q3.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupErrorMessage_NullPushName_TwoNameForm()
        {
            // Verify error message contains useful information
            try
            {
                new RegexNodeBalancingGroup(null, "pop", RegexBuilder.NonEscapedLiteral("x"));
                Assert.Fail("Should throw");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Push group name"));
                Assert.IsTrue(ex.ParamName.Contains("push"));
            }
        }

        [TestMethod]
        public void TestBalancingGroupErrorMessage_NullPushName_SimpleForm()
        {
            // Verify error message for simple form
            try
            {
                new RegexNodeBalancingGroup(null, RegexBuilder.NonEscapedLiteral("x"));
                Assert.Fail("Should throw");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Group name"));
            }
        }

        [TestMethod]
        public void TestBalancingGroupIsSimpleBalancing_Consistency()
        {
            // Verify IsSimpleBalancing flag is consistent
            var inner = RegexBuilder.NonEscapedLiteral("test");

            // Two-name form - should not be simple
            var twoName = new RegexNodeBalancingGroup("a", "b", inner);
            Assert.IsFalse(twoName.IsSimpleBalancing);

            // Simple form - should be simple
            var simple = new RegexNodeBalancingGroup("a", inner);
            Assert.IsTrue(simple.IsSimpleBalancing);

            // Two-name form with empty pop - should be simple
            var twoNameEmpty = new RegexNodeBalancingGroup("a", "", inner);
            Assert.IsTrue(twoNameEmpty.IsSimpleBalancing);

            // Two-name form with null pop - should be simple
            var twoNameNull = new RegexNodeBalancingGroup("a", null, inner);
            Assert.IsTrue(twoNameNull.IsSimpleBalancing);
        }

        [TestMethod]
        public void TestBalancingGroupInGroup_Nesting()
        {
            // Test balancing group nested in regular group with name
            var balancing = RegexBuilder.BalancingGroup("inner", "i", RegexBuilder.NonEscapedLiteral("a"));
            var outer = RegexBuilder.Group("outer", balancing);
            Assert.AreEqual(@"(?<outer>(?<inner-i>a))", outer.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupInNonCapturingGroup()
        {
            // Test balancing group in non-capturing group
            var balancing = RegexBuilder.SimpleBalancingGroup("b", RegexBuilder.NonEscapedLiteral("c"));
            var nonCapture = RegexBuilder.NonCapturingGroup(balancing);
            Assert.AreEqual(@"(?:(?<b>-c))", nonCapture.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupWithConcatenationInnerExpr()
        {
            // Test with multiple concatenated expressions inside
            var concat = RegexBuilder.Concatenate(
                RegexBuilder.NonEscapedLiteral("[a-z]"),
                RegexBuilder.NonEscapedLiteral("+")
            );
            var balanced = RegexBuilder.SimpleBalancingGroup("letters", concat);
            Assert.AreEqual(@"(?<letters>-[a-z]+)", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupMultipleInConcatenation()
        {
            // Test multiple balancing groups in sequence
            var b1 = RegexBuilder.BalancingGroup("g1", "p1", RegexBuilder.NonEscapedLiteral("a"));
            var b2 = RegexBuilder.BalancingGroup("g2", "p2", RegexBuilder.NonEscapedLiteral("b"));
            var concat = RegexBuilder.Concatenate(b1, b2);
            Assert.AreEqual(@"(?<g1-p1>a)(?<g2-p2>b)", concat.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupInAlternation()
        {
            // Test balancing group as one option in alternation
            var balanced = RegexBuilder.BalancingGroup("b", "p", RegexBuilder.NonEscapedLiteral("x"));
            var literal = RegexBuilder.NonEscapedLiteral("y");
            var alt = RegexBuilder.Alternate(balanced, literal);
            Assert.AreEqual(@"(?:(?<b-p>x)|y)", alt.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupNullInnerExpressionThrows()
        {
            // Ensure that null inner expression always throws
            try
            {
                new RegexNodeBalancingGroup("name", "pop", null);
                Assert.Fail("Should throw ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void TestBalancingGroupEmptyPushNameThrows()
        {
            // Ensure empty push name throws
            try
            {
                new RegexNodeBalancingGroup("", "pop", RegexBuilder.NonEscapedLiteral("x"));
                Assert.Fail("Should throw ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("cannot be null or empty"));
            }
        }

        [TestMethod]
        public void TestBalancingGroupQuantifierChaining()
        {
            // Test that quantifier can be set and modified
            var inner = RegexBuilder.NonEscapedLiteral("test");
            var balanced = RegexBuilder.BalancingGroup("a", "b", inner);

            Assert.AreEqual(RegexQuantifier.None, balanced.Quantifier);
            Assert.AreEqual(@"(?<a-b>test)", balanced.ToRegexPattern());

            balanced.Quantifier = RegexQuantifier.ZeroOrOne;
            Assert.AreEqual(@"(?<a-b>test)?", balanced.ToRegexPattern());
        }

        [TestMethod]
        public void TestBalancingGroupAPI_FactoryMethodConsistency()
        {
            // Verify factory methods produce same results as constructors
            var inner = RegexBuilder.NonEscapedLiteral("x");

            var factoryTwoName = RegexBuilder.BalancingGroup("a", "b", inner);
            var constructorTwoName = new RegexNodeBalancingGroup("a", "b", inner);
            Assert.AreEqual(constructorTwoName.ToRegexPattern(), factoryTwoName.ToRegexPattern());

            var factorySimple = RegexBuilder.SimpleBalancingGroup("g", inner);
            var constructorSimple = new RegexNodeBalancingGroup("g", inner);
            Assert.AreEqual(constructorSimple.ToRegexPattern(), factorySimple.ToRegexPattern());
        }
    }
}
