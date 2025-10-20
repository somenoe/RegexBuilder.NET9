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
    }
}
