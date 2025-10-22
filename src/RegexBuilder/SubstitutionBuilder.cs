using System;
using System.Linq;

namespace RegexBuilder
{
    /// <summary>
    /// Factory class for creating substitution/replacement pattern nodes.
    /// Use these methods to build replacement patterns for Regex.Replace() operations.
    /// </summary>
    public static class SubstitutionBuilder
    {
        #region Build Methods

        /// <summary>
        /// Builds a substitution pattern string from a single substitution node.
        /// </summary>
        /// <param name="node">The substitution node to convert.</param>
        /// <returns>A string that can be used as a replacement pattern in Regex.Replace().</returns>
        public static string Build(SubstitutionNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            return node.ToSubstitutionPattern();
        }

        /// <summary>
        /// Builds a substitution pattern string from multiple substitution nodes.
        /// The nodes are concatenated in the order provided.
        /// </summary>
        /// <param name="nodes">The substitution nodes to concatenate.</param>
        /// <returns>A string that can be used as a replacement pattern in Regex.Replace().</returns>
        public static string Build(params SubstitutionNode[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
            {
                throw new ArgumentException("At least one substitution node is required.", nameof(nodes));
            }
            var concatenation = new SubstitutionConcatenation(nodes);
            return concatenation.ToSubstitutionPattern();
        }

        #endregion

        #region Literal Text

        /// <summary>
        /// Creates a literal text substitution node. Dollar signs ($) are automatically escaped.
        /// </summary>
        /// <param name="text">The literal text to include in the replacement.</param>
        /// <returns>A substitution node representing the literal text.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.Literal("Hello ")
        /// );
        /// </code>
        /// </example>
        public static SubstitutionLiteral Literal(string text)
        {
            return new SubstitutionLiteral(text);
        }

        #endregion

        #region Group References

        /// <summary>
        /// Creates a substitution that references a numbered capturing group.
        /// </summary>
        /// <param name="groupNumber">The 1-based group number to reference.</param>
        /// <returns>A substitution node that outputs the value of the specified group.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.Group(2),
        ///     SubstitutionBuilder.Literal(" "),
        ///     SubstitutionBuilder.Group(1)
        /// );
        /// // Produces: "$2 $1"
        /// </code>
        /// </example>
        public static SubstitutionGroupReference Group(int groupNumber)
        {
            return new SubstitutionGroupReference(groupNumber);
        }

        /// <summary>
        /// Creates a substitution that references a named capturing group.
        /// </summary>
        /// <param name="groupName">The name of the group to reference.</param>
        /// <returns>A substitution node that outputs the value of the specified named group.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.Group("word2"),
        ///     SubstitutionBuilder.Literal(" "),
        ///     SubstitutionBuilder.Group("word1")
        /// );
        /// // Produces: "${word2} ${word1}"
        /// </code>
        /// </example>
        public static SubstitutionGroupReference Group(string groupName)
        {
            return new SubstitutionGroupReference(groupName);
        }

        #endregion

        #region Special References

        /// <summary>
        /// Creates a substitution that outputs the entire matched text.
        /// Equivalent to the $&amp; substitution pattern.
        /// </summary>
        /// <returns>A substitution node that outputs the whole match.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.Literal("["),
        ///     SubstitutionBuilder.WholeMatch(),
        ///     SubstitutionBuilder.Literal("]")
        /// );
        /// // Wraps the matched text in brackets
        /// </code>
        /// </example>
        public static SubstitutionSpecialReference WholeMatch()
        {
            return new SubstitutionSpecialReference(SubstitutionType.WholeMatch);
        }

        /// <summary>
        /// Creates a substitution that outputs all text before the match.
        /// Equivalent to the $` substitution pattern.
        /// </summary>
        /// <returns>A substitution node that outputs the text before the match.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.BeforeMatch()
        /// );
        /// // Replaces the match with everything that came before it
        /// </code>
        /// </example>
        public static SubstitutionSpecialReference BeforeMatch()
        {
            return new SubstitutionSpecialReference(SubstitutionType.BeforeMatch);
        }

        /// <summary>
        /// Creates a substitution that outputs all text after the match.
        /// Equivalent to the $' substitution pattern.
        /// </summary>
        /// <returns>A substitution node that outputs the text after the match.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.AfterMatch()
        /// );
        /// // Replaces the match with everything that comes after it
        /// </code>
        /// </example>
        public static SubstitutionSpecialReference AfterMatch()
        {
            return new SubstitutionSpecialReference(SubstitutionType.AfterMatch);
        }

        /// <summary>
        /// Creates a substitution that outputs the last captured group.
        /// Equivalent to the $+ substitution pattern.
        /// </summary>
        /// <returns>A substitution node that outputs the last captured group.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.LastCapturedGroup()
        /// );
        /// // Outputs the last group that was captured in the match
        /// </code>
        /// </example>
        public static SubstitutionSpecialReference LastCapturedGroup()
        {
            return new SubstitutionSpecialReference(SubstitutionType.LastGroup);
        }

        /// <summary>
        /// Creates a substitution that outputs the entire input string.
        /// Equivalent to the $_ substitution pattern.
        /// </summary>
        /// <returns>A substitution node that outputs the entire input string.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.EntireInput()
        /// );
        /// // Replaces the match with the entire input string
        /// </code>
        /// </example>
        public static SubstitutionSpecialReference EntireInput()
        {
            return new SubstitutionSpecialReference(SubstitutionType.EntireInput);
        }

        /// <summary>
        /// Creates a substitution that outputs a literal dollar sign ($).
        /// Equivalent to the $$ substitution pattern.
        /// Note: This is rarely needed as the Literal() method automatically escapes dollar signs.
        /// </summary>
        /// <returns>A substitution node that outputs a literal dollar sign.</returns>
        /// <example>
        /// <code>
        /// var replacement = SubstitutionBuilder.Build(
        ///     SubstitutionBuilder.LiteralDollar(),
        ///     SubstitutionBuilder.Group(1)
        /// );
        /// // Produces: "$$1" which outputs "$" followed by the value of group 1
        /// </code>
        /// </example>
        public static SubstitutionSpecialReference LiteralDollar()
        {
            return new SubstitutionSpecialReference(SubstitutionType.LiteralDollar);
        }

        #endregion

        #region Concatenation

        /// <summary>
        /// Creates a concatenation of multiple substitution nodes.
        /// This is equivalent to calling Build() with multiple nodes, but returns a SubstitutionNode
        /// that can be used as part of a larger substitution pattern.
        /// </summary>
        /// <param name="nodes">The substitution nodes to concatenate.</param>
        /// <returns>A substitution node representing the concatenation.</returns>
        /// <example>
        /// <code>
        /// var part1 = SubstitutionBuilder.Concatenate(
        ///     SubstitutionBuilder.Literal("("),
        ///     SubstitutionBuilder.Group(1),
        ///     SubstitutionBuilder.Literal(")")
        /// );
        /// var replacement = SubstitutionBuilder.Build(
        ///     part1,
        ///     SubstitutionBuilder.Literal(" "),
        ///     SubstitutionBuilder.Group(2)
        /// );
        /// </code>
        /// </example>
        public static SubstitutionConcatenation Concatenate(params SubstitutionNode[] nodes)
        {
            return new SubstitutionConcatenation(nodes);
        }

        #endregion
    }
}
