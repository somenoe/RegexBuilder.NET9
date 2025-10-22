using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexBuilder
{
    /// <summary>
    /// Represents a concatenation of multiple substitution nodes.
    /// </summary>
    public class SubstitutionConcatenation : SubstitutionNode
    {
        /// <summary>
        /// Gets the child substitution nodes.
        /// </summary>
        public IReadOnlyList<SubstitutionNode> ChildNodes { get; }

        /// <summary>
        /// Initializes a new instance with the specified child nodes.
        /// </summary>
        /// <param name="childNodes">The substitution nodes to concatenate.</param>
        public SubstitutionConcatenation(params SubstitutionNode[] childNodes)
        {
            ArgumentNullException.ThrowIfNull(childNodes);
            if (childNodes.Length == 0)
            {
                throw new ArgumentException("At least one child node is required.", nameof(childNodes));
            }
            if (childNodes.Any(n => n == null))
            {
                throw new ArgumentException("Child nodes cannot contain null elements.", nameof(childNodes));
            }
            ChildNodes = Array.AsReadOnly(childNodes);
        }

        /// <summary>
        /// Initializes a new instance with the specified child nodes.
        /// </summary>
        /// <param name="childNodes">The substitution nodes to concatenate.</param>
        public SubstitutionConcatenation(IEnumerable<SubstitutionNode> childNodes)
            : this(childNodes?.ToArray() ?? throw new ArgumentNullException(nameof(childNodes)))
        {
        }

        /// <summary>
        /// Converts this concatenation to a substitution pattern string.
        /// </summary>
        /// <returns>The concatenated substitution patterns of all child nodes.</returns>
        public override string ToSubstitutionPattern()
        {
            var builder = new StringBuilder();
            foreach (var node in ChildNodes)
            {
                builder.Append(node.ToSubstitutionPattern());
            }
            return builder.ToString();
        }
    }
}
