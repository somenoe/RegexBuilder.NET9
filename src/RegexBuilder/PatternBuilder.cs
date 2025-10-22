using System;
using System.Collections.Generic;
using System.Linq;

namespace RegexBuilder
{
    /// <summary>
    /// Provides a fluent API for building complex regex patterns with method chaining.
    /// </summary>
    public class PatternBuilder
    {
        private readonly List<RegexNode> _currentBranch;
        private readonly List<List<RegexNode>> _alternationBranches;
        private bool _hasAlternation;
        private bool _startAnchorAdded;
        private bool _endAnchorAdded;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBuilder"/> class.
        /// </summary>
        public PatternBuilder()
        {
            _currentBranch = new List<RegexNode>();
            _alternationBranches = new List<List<RegexNode>>();
            _hasAlternation = false;
            _startAnchorAdded = false;
            _endAnchorAdded = false;
        }

        /// <summary>
        /// Adds a start-of-line anchor (^) to the pattern.
        /// </summary>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Start()
        {
            if (_startAnchorAdded)
            {
                throw new InvalidOperationException("Start anchor has already been added.");
            }
            _currentBranch.Add(RegexBuilder.LineStart());
            _startAnchorAdded = true;
            return this;
        }

        /// <summary>
        /// Adds an end-of-line anchor ($) to the pattern.
        /// </summary>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder End()
        {
            if (_endAnchorAdded)
            {
                throw new InvalidOperationException("End anchor has already been added.");
            }
            _currentBranch.Add(RegexBuilder.LineEnd());
            _endAnchorAdded = true;
            return this;
        }

        /// <summary>
        /// Adds a literal string to the pattern.
        /// </summary>
        /// <param name="text">The literal text to match.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Literal(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Literal text cannot be null or empty.", nameof(text));
            }
            _currentBranch.Add(RegexBuilder.Literal(text));
            return this;
        }

        /// <summary>
        /// Adds a digit pattern (\d) with optional quantifiers.
        /// </summary>
        /// <param name="min">Minimum number of digits (null for exactly one).</param>
        /// <param name="max">Maximum number of digits (null for exactly min, or unbounded if min is set).</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Digits(int? min = null, int? max = null)
        {
            var node = RegexBuilder.Digit();
            ApplyQuantifier(node, min, max);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds a letter pattern ([a-zA-Z]) with optional quantifiers.
        /// </summary>
        /// <param name="min">Minimum number of letters (null for exactly one).</param>
        /// <param name="max">Maximum number of letters (null for exactly min, or unbounded if min is set).</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Letters(int? min = null, int? max = null)
        {
            var node = RegexBuilder.CharacterSet("a-zA-Z", RegexQuantifier.None);
            ApplyQuantifier(node, min, max);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds a whitespace pattern (\s) with optional quantifiers.
        /// </summary>
        /// <param name="min">Minimum number of whitespace characters (null for exactly one).</param>
        /// <param name="max">Maximum number of whitespace characters (null for exactly min, or unbounded if min is set).</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Whitespace(int? min = null, int? max = null)
        {
            var node = RegexBuilder.Whitespace();
            ApplyQuantifier(node, min, max);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds a word character pattern (\w) with optional quantifiers.
        /// </summary>
        /// <param name="min">Minimum number of word characters (null for exactly one).</param>
        /// <param name="max">Maximum number of word characters (null for exactly min, or unbounded if min is set).</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder WordCharacter(int? min = null, int? max = null)
        {
            var node = RegexBuilder.WordCharacter();
            ApplyQuantifier(node, min, max);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds any character pattern (.) with optional quantifiers.
        /// </summary>
        /// <param name="min">Minimum number of any characters (null for exactly one).</param>
        /// <param name="max">Maximum number of any characters (null for exactly min, or unbounded if min is set).</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder AnyCharacter(int? min = null, int? max = null)
        {
            var node = RegexBuilder.MetaCharacter(".");
            ApplyQuantifier(node, min, max);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds a character set pattern with optional quantifiers.
        /// </summary>
        /// <param name="characterSet">The character set definition (e.g., "0-9a-f" for hex digits).</param>
        /// <param name="min">Minimum occurrences (null for exactly one).</param>
        /// <param name="max">Maximum occurrences (null for exactly min, or unbounded if min is set).</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder CharacterSet(string characterSet, int? min = null, int? max = null)
        {
            if (string.IsNullOrEmpty(characterSet))
            {
                throw new ArgumentException("Character set cannot be null or empty.", nameof(characterSet));
            }
            var node = RegexBuilder.CharacterSet(characterSet, RegexQuantifier.None);
            ApplyQuantifier(node, min, max);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds an existing RegexNode to the pattern.
        /// </summary>
        /// <param name="node">The RegexNode to add.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Pattern(RegexNode node)
        {
            ArgumentNullException.ThrowIfNull(node);
            _currentBranch.Add(node);
            return this;
        }

        /// <summary>
        /// Adds a capturing group to the pattern using a builder action.
        /// </summary>
        /// <param name="builderAction">Action that defines the group's content.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Group(Action<PatternBuilder> builderAction)
        {
            ArgumentNullException.ThrowIfNull(builderAction);

            var groupBuilder = new PatternBuilder();
            builderAction(groupBuilder);
            var groupContent = groupBuilder.BuildInternal();

            if (groupContent != null)
            {
                _currentBranch.Add(RegexBuilder.Group(groupContent));
            }

            return this;
        }

        /// <summary>
        /// Adds a non-capturing group to the pattern using a builder action.
        /// </summary>
        /// <param name="builderAction">Action that defines the group's content.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder NonCapturingGroup(Action<PatternBuilder> builderAction)
        {
            ArgumentNullException.ThrowIfNull(builderAction);

            var groupBuilder = new PatternBuilder();
            builderAction(groupBuilder);
            var groupContent = groupBuilder.BuildInternal();

            if (groupContent != null)
            {
                _currentBranch.Add(RegexBuilder.NonCapturingGroup(groupContent));
            }

            return this;
        }

        /// <summary>
        /// Makes the previous pattern optional (adds ? quantifier).
        /// </summary>
        /// <param name="builderAction">Action that defines the optional content.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Optional(Action<PatternBuilder> builderAction)
        {
            ArgumentNullException.ThrowIfNull(builderAction);

            var optionalBuilder = new PatternBuilder();
            builderAction(optionalBuilder);
            var optionalContent = optionalBuilder.BuildInternal();

            if (optionalContent != null)
            {
                // Wrap in non-capturing group and apply quantifier
                var groupedContent = RegexBuilder.NonCapturingGroup(optionalContent);
                groupedContent.Quantifier = RegexQuantifier.ZeroOrOne;
                _currentBranch.Add(groupedContent);
            }

            return this;
        }

        /// <summary>
        /// Adds an alternation (OR) pattern using a builder action.
        /// </summary>
        /// <param name="builderAction">Action that defines the alternative branch.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Or(Action<PatternBuilder> builderAction)
        {
            ArgumentNullException.ThrowIfNull(builderAction);

            // Save current branch
            if (_currentBranch.Count > 0)
            {
                _alternationBranches.Add(new List<RegexNode>(_currentBranch));
                _currentBranch.Clear();
            }

            // Build the alternative branch
            var alternativeBuilder = new PatternBuilder();
            builderAction(alternativeBuilder);
            var alternativeNode = alternativeBuilder.BuildInternal();

            if (alternativeNode != null)
            {
                _currentBranch.Add(alternativeNode);
            }

            _hasAlternation = true;
            return this;
        }

        /// <summary>
        /// Adds an alternation (OR) pattern using an existing RegexNode.
        /// </summary>
        /// <param name="node">The alternative pattern node.</param>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Or(RegexNode node)
        {
            ArgumentNullException.ThrowIfNull(node);

            // Save current branch
            if (_currentBranch.Count > 0)
            {
                _alternationBranches.Add(new List<RegexNode>(_currentBranch));
                _currentBranch.Clear();
            }

            _currentBranch.Add(node);
            _hasAlternation = true;
            return this;
        }

        /// <summary>
        /// Adds the common email pattern to the builder.
        /// </summary>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Email()
        {
            _currentBranch.Add(CommonPatterns.Email());
            return this;
        }

        /// <summary>
        /// Adds the common URL pattern to the builder.
        /// </summary>
        /// <returns>The current PatternBuilder instance for method chaining.</returns>
        public PatternBuilder Url()
        {
            _currentBranch.Add(CommonPatterns.Url());
            return this;
        }

        /// <summary>
        /// Builds and returns the final RegexNode representing the complete pattern.
        /// </summary>
        /// <returns>A RegexNode representing the built pattern, or null if no pattern was defined.</returns>
        public RegexNode Build()
        {
            return BuildInternal();
        }

        /// <summary>
        /// Internal method to build the RegexNode tree from the current state.
        /// </summary>
        private RegexNode BuildInternal()
        {
            // Handle empty builder
            if (_currentBranch.Count == 0 && _alternationBranches.Count == 0)
            {
                return null;
            }

            // Handle alternation
            if (_hasAlternation)
            {
                // Add current branch to alternation branches if it has content
                if (_currentBranch.Count > 0)
                {
                    _alternationBranches.Add(new List<RegexNode>(_currentBranch));
                }

                // Build each branch
                var branchNodes = _alternationBranches
                    .Where(branch => branch.Count > 0)
                    .Select(branch => branch.Count == 1 ? branch[0] : new RegexNodeConcatenation(branch.ToArray()))
                    .ToArray();

                if (branchNodes.Length == 0)
                {
                    return null;
                }

                if (branchNodes.Length == 1)
                {
                    return branchNodes[0];
                }

                // Wrap in alternation
                return RegexBuilder.Alternate(branchNodes);
            }

            // Handle simple concatenation
            if (_currentBranch.Count == 0)
            {
                return null;
            }

            if (_currentBranch.Count == 1)
            {
                return _currentBranch[0];
            }

            return new RegexNodeConcatenation(_currentBranch.ToArray());
        }

        /// <summary>
        /// Applies quantifier to a RegexNode if min/max values are provided.
        /// </summary>
        private static void ApplyQuantifier(RegexNode node, int? min, int? max)
        {
            if (min == null && max == null)
            {
                return; // No quantifier
            }

            if (min.HasValue && min.Value < 0)
            {
                throw new ArgumentException("Minimum quantifier cannot be negative.", nameof(min));
            }

            if (max.HasValue && max.Value < 0)
            {
                throw new ArgumentException("Maximum quantifier cannot be negative.", nameof(max));
            }

            if (min.HasValue && max.HasValue && max.Value < min.Value)
            {
                throw new ArgumentException("Maximum quantifier cannot be less than minimum.", nameof(max));
            }

            node.Quantifier = new RegexQuantifier(min, max);
        }
    }
}
