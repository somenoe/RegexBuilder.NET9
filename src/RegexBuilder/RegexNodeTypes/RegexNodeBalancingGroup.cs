using System;
using System.Globalization;

namespace RegexBuilder
{
    /// <summary>
    /// Represents a balancing group construct in a regular expression.
    /// Balancing groups are used to match nested/balanced constructs such as parentheses, XML tags, or code blocks.
    /// Supports both two-name form (?<name1-name2>expr) and single-name form (?<name>-expr).
    /// </summary>
    /// <remarks>
    /// Balancing groups work by maintaining a stack of captured groups. The syntax (?<name1-name2>expr)
    /// captures to 'name1' and pops 'name2' from the stack. This is useful for matching balanced constructs.
    /// Example: \( (?:[^()] | (?<paren>\() | (?<-paren>\)))*+ \)  matches balanced parentheses.
    /// </remarks>
    public class RegexNodeBalancingGroup : RegexNode
    {
        private RegexNode innerExpression;

        protected override bool AllowQuantifier
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the name of the group to push onto the stack.
        /// In (?<name1-name2>expr), this is 'name1'.
        /// </summary>
        public string PushGroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group to pop from the stack.
        /// In (?<name1-name2>expr), this is 'name2'.
        /// If null or empty, only push operation is performed: (?<name>-expr).
        /// </summary>
        public string PopGroupName { get; set; }

        /// <summary>
        /// Gets or sets the inner expression to match within the balancing group.
        /// </summary>
        public RegexNode InnerExpression
        {
            get { return innerExpression; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "InnerExpression cannot be null.");
                }
                innerExpression = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a simple balancing group (single name only).
        /// Simple balancing groups use syntax: (?<name>-expr)
        /// </summary>
        public bool IsSimpleBalancing
        {
            get { return string.IsNullOrEmpty(PopGroupName); }
        }

        /// <summary>
        /// Initializes a new instance of the RegexNodeBalancingGroup class with two group names.
        /// Creates a balancing group with syntax: (?<pushName-popName>expr)
        /// </summary>
        /// <param name="pushGroupName">The name of the group to push onto the stack.</param>
        /// <param name="popGroupName">The name of the group to pop from the stack.</param>
        /// <param name="innerExpression">The inner expression to match.</param>
        public RegexNodeBalancingGroup(string pushGroupName, string popGroupName, RegexNode innerExpression)
        {
            if (string.IsNullOrEmpty(pushGroupName))
            {
                throw new ArgumentException("Push group name cannot be null or empty.", "pushGroupName");
            }
            if (innerExpression == null)
            {
                throw new ArgumentNullException("innerExpression");
            }

            PushGroupName = pushGroupName;
            PopGroupName = popGroupName;
            InnerExpression = innerExpression;
        }

        /// <summary>
        /// Initializes a new instance of the RegexNodeBalancingGroup class with a single group name.
        /// Creates a simple balancing group with syntax: (?<name>-expr)
        /// </summary>
        /// <param name="groupName">The name of the group to push onto the stack.</param>
        /// <param name="innerExpression">The inner expression to match.</param>
        public RegexNodeBalancingGroup(string groupName, RegexNode innerExpression)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentException("Group name cannot be null or empty.", "groupName");
            }
            if (innerExpression == null)
            {
                throw new ArgumentNullException("innerExpression");
            }

            PushGroupName = groupName;
            PopGroupName = null;
            InnerExpression = innerExpression;
        }

        /// <summary>
        /// Converts this node to a regular expression pattern string.
        /// </summary>
        /// <returns>
        /// A regular expression pattern string.
        /// Returns (?<name1-name2>expr) for two-name balancing groups.
        /// Returns (?<name>-expr) for simple balancing groups.
        /// </returns>
        public override string ToRegexPattern()
        {
            string result;

            if (IsSimpleBalancing)
            {
                // Simple balancing group: (?<name>-expr)
                result = string.Format(
                    CultureInfo.InvariantCulture,
                    "(?<{0}>-{1})",
                    PushGroupName,
                    InnerExpression.ToRegexPattern());
            }
            else
            {
                // Two-name balancing group: (?<name1-name2>expr)
                result = string.Format(
                    CultureInfo.InvariantCulture,
                    "(?<{0}-{1}>{2})",
                    PushGroupName,
                    PopGroupName,
                    InnerExpression.ToRegexPattern());
            }

            if (HasQuantifier)
            {
                result += Quantifier.ToRegexPattern();
            }

            return result;
        }
    }
}
