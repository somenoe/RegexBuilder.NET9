namespace RegexBuilder
{
    /// <summary>
    /// Represents a substitution/replacement pattern node that can be used in regex replacement operations.
    /// This is the base class for all substitution pattern nodes.
    /// </summary>
    public abstract class SubstitutionNode
    {
        /// <summary>
        /// Converts this node to a substitution pattern string that can be used in Regex.Replace() operations.
        /// </summary>
        /// <returns>A string representation of the substitution pattern.</returns>
        public abstract string ToSubstitutionPattern();

        /// <summary>
        /// Returns the substitution pattern string.
        /// </summary>
        public override string ToString()
        {
            return ToSubstitutionPattern();
        }
    }
}
