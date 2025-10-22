using System;
using System.Globalization;

namespace RegexBuilder
{
    /// <summary>
    /// Represents a Unicode category escape sequence in a regular expression.
    /// Enables matching of Unicode letters, numbers, and other character categories.
    /// Supports both positive matching (\p{name}) and negative matching (\P{name}).
    /// </summary>
    public class RegexNodeUnicodeCategory : RegexNode
    {
        protected override bool AllowQuantifier
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the Unicode category name (e.g., "L", "N", "IsCyrillic").
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this category should be negated (use \P instead of \p).
        /// </summary>
        public bool IsNegated { get; set; }

        /// <summary>
        /// Initializes a new instance of the RegexNodeUnicodeCategory class.
        /// </summary>
        /// <param name="categoryName">The Unicode category name.</param>
        public RegexNodeUnicodeCategory(string categoryName)
            : this(categoryName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RegexNodeUnicodeCategory class.
        /// </summary>
        /// <param name="categoryName">The Unicode category name.</param>
        /// <param name="isNegated">If true, generates \P{name} instead of \p{name}.</param>
        public RegexNodeUnicodeCategory(string categoryName, bool isNegated)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
            }

            CategoryName = categoryName;
            IsNegated = isNegated;
        }

        /// <summary>
        /// Converts this node to a regular expression pattern string.
        /// </summary>
        /// <returns>The regular expression pattern string (e.g., "\p{L}", "\P{N}").</returns>
        public override string ToRegexPattern()
        {
            char categoryMarker = IsNegated ? 'P' : 'p';
            string result = string.Format(CultureInfo.InvariantCulture, "\\{0}{{{1}}}", categoryMarker, CategoryName);

            if (HasQuantifier)
            {
                result += Quantifier.ToRegexPattern();
            }

            return result;
        }
    }
}
