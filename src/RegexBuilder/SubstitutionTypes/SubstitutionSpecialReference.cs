using System;

namespace RegexBuilder
{
    /// <summary>
    /// Defines the types of special substitution references.
    /// </summary>
    public enum SubstitutionType
    {
        /// <summary>
        /// Substitutes a copy of the whole match ($&amp;).
        /// </summary>
        WholeMatch,

        /// <summary>
        /// Substitutes all text before the match ($`).
        /// </summary>
        BeforeMatch,

        /// <summary>
        /// Substitutes all text after the match ($').
        /// </summary>
        AfterMatch,

        /// <summary>
        /// Substitutes the last group that was captured ($+).
        /// </summary>
        LastGroup,

        /// <summary>
        /// Substitutes the entire input string ($_).
        /// </summary>
        EntireInput,

        /// <summary>
        /// Substitutes a literal dollar sign ($$).
        /// </summary>
        LiteralDollar
    }

    /// <summary>
    /// Represents a special substitution reference (like $&amp;, $`, $', $+, $_, $$).
    /// </summary>
    public class SubstitutionSpecialReference : SubstitutionNode
    {
        /// <summary>
        /// Gets the type of special substitution.
        /// </summary>
        public SubstitutionType Type { get; }

        /// <summary>
        /// Initializes a new instance with the specified substitution type.
        /// </summary>
        /// <param name="type">The type of special substitution.</param>
        public SubstitutionSpecialReference(SubstitutionType type)
        {
            Type = type;
        }

        /// <summary>
        /// Converts this special reference to a substitution pattern string.
        /// </summary>
        /// <returns>A substitution pattern like "$&amp;", "$`", "$'", "$+", "$_", or "$$".</returns>
        public override string ToSubstitutionPattern()
        {
            return Type switch
            {
                SubstitutionType.WholeMatch => "$&",
                SubstitutionType.BeforeMatch => "$`",
                SubstitutionType.AfterMatch => "$'",
                SubstitutionType.LastGroup => "$+",
                SubstitutionType.EntireInput => "$_",
                SubstitutionType.LiteralDollar => "$$",
                _ => throw new InvalidOperationException($"Unknown substitution type: {Type}")
            };
        }
    }
}
