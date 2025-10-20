using System;

namespace RegexBuilder
{
    /// <summary>
    /// Represents a substitution that references a captured group by number or name.
    /// </summary>
    public class SubstitutionGroupReference : SubstitutionNode
    {
        /// <summary>
        /// Gets the group number (1-based), or null if referencing by name.
        /// </summary>
        public int? GroupNumber { get; }

        /// <summary>
        /// Gets the group name, or null if referencing by number.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Initializes a new instance that references a numbered group.
        /// </summary>
        /// <param name="groupNumber">The 1-based group number to reference.</param>
        public SubstitutionGroupReference(int groupNumber)
        {
            if (groupNumber < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(groupNumber), "Group number must be non-negative.");
            }
            GroupNumber = groupNumber;
            GroupName = null;
        }

        /// <summary>
        /// Initializes a new instance that references a named group.
        /// </summary>
        /// <param name="groupName">The name of the group to reference.</param>
        public SubstitutionGroupReference(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Group name cannot be null or whitespace.", nameof(groupName));
            }
            GroupName = groupName;
            GroupNumber = null;
        }

        /// <summary>
        /// Converts this group reference to a substitution pattern string.
        /// </summary>
        /// <returns>A substitution pattern like "$1" or "${groupName}".</returns>
        public override string ToSubstitutionPattern()
        {
            if (GroupNumber.HasValue)
            {
                return $"${GroupNumber.Value}";
            }
            else
            {
                return $"${{{GroupName}}}";
            }
        }
    }
}
