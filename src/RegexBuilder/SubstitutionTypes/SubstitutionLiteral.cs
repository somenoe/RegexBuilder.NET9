using System;

namespace RegexBuilder
{
    /// <summary>
    /// Represents a literal text substitution node. Dollar signs ($) are automatically escaped.
    /// </summary>
    public class SubstitutionLiteral : SubstitutionNode
    {
        /// <summary>
        /// Gets the literal text value.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of SubstitutionLiteral with the specified text.
        /// </summary>
        /// <param name="text">The literal text to include in the substitution pattern.</param>
        public SubstitutionLiteral(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Converts this literal to a substitution pattern string, escaping dollar signs.
        /// </summary>
        /// <returns>The literal text with dollar signs escaped.</returns>
        public override string ToSubstitutionPattern()
        {
            // Escape dollar signs by doubling them
            return Text.Replace("$", "$$");
        }
    }
}
