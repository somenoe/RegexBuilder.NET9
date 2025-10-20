using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RegexBuilder
{
    /// <summary>
    /// Represents an inline option grouping node with support for enabling and disabling options.
    /// Generates patterns like (?i:expr), (?im:expr), or (?i-m:expr).
    /// </summary>
    public class RegexNodeInlineOptionGrouping : RegexNode
    {
        private RegexOptions enabledOptions;
        private RegexOptions disabledOptions;
        private RegexNode innerExpression;

        protected override bool AllowQuantifier
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the options to be enabled for this group.
        /// </summary>
        public RegexOptions EnabledOptions
        {
            get { return enabledOptions; }
            set
            {
                ValidateOptions(value);
                enabledOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the options to be disabled for this group.
        /// </summary>
        public RegexOptions DisabledOptions
        {
            get { return disabledOptions; }
            set
            {
                ValidateOptions(value);
                disabledOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the inner expression for this grouping.
        /// </summary>
        public RegexNode InnerExpression
        {
            get { return innerExpression; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "InnerExpression cannot be null");
                }
                innerExpression = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the RegexNodeInlineOptionGrouping class.
        /// </summary>
        /// <param name="enabledOptions">Options to enable.</param>
        /// <param name="innerExpression">Inner expression.</param>
        public RegexNodeInlineOptionGrouping(RegexOptions enabledOptions, RegexNode innerExpression)
            : this(enabledOptions, RegexOptions.None, innerExpression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RegexNodeInlineOptionGrouping class with both enabled and disabled options.
        /// </summary>
        /// <param name="enabledOptions">Options to enable.</param>
        /// <param name="disabledOptions">Options to disable.</param>
        /// <param name="innerExpression">Inner expression.</param>
        public RegexNodeInlineOptionGrouping(RegexOptions enabledOptions, RegexOptions disabledOptions, RegexNode innerExpression)
        {
            EnabledOptions = enabledOptions;
            DisabledOptions = disabledOptions;
            InnerExpression = innerExpression;
        }

        public override string ToRegexPattern()
        {
            string enabledPart = FormatOptions(EnabledOptions);
            string disabledPart = FormatOptions(DisabledOptions);

            string optionsString;
            if (string.IsNullOrEmpty(disabledPart))
            {
                optionsString = enabledPart;
            }
            else if (string.IsNullOrEmpty(enabledPart))
            {
                optionsString = "-" + disabledPart;
            }
            else
            {
                optionsString = enabledPart + "-" + disabledPart;
            }

            string result = string.Format(
                CultureInfo.InvariantCulture,
                "(?{0}:{1})",
                optionsString,
                InnerExpression.ToRegexPattern()
            );
            return result;
        }

        /// <summary>
        /// Formats RegexOptions into a string of option characters (imsnx).
        /// </summary>
        private static string FormatOptions(RegexOptions options)
        {
            return string.Concat(
                ((options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase) ? "i" : "",
                ((options & RegexOptions.Multiline) == RegexOptions.Multiline) ? "m" : "",
                ((options & RegexOptions.Singleline) == RegexOptions.Singleline) ? "s" : "",
                ((options & RegexOptions.ExplicitCapture) == RegexOptions.ExplicitCapture) ? "n" : "",
                ((options & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace) ? "x" : ""
            );
        }

        /// <summary>
        /// Validates that the given options are valid for inline usage.
        /// </summary>
        private static void ValidateOptions(RegexOptions options)
        {
            if (options == RegexOptions.None)
            {
                // None is allowed for disabled options
                return;
            }

            string invalidOptionString = null;
            if ((options & RegexOptions.Compiled) == RegexOptions.Compiled)
            {
                invalidOptionString = "Compiled";
            }
            if ((options & RegexOptions.RightToLeft) == RegexOptions.RightToLeft)
            {
                invalidOptionString = "RightToLeft";
            }
            if ((options & RegexOptions.ECMAScript) == RegexOptions.ECMAScript)
            {
                invalidOptionString = "ECMAScript";
            }
            if ((options & RegexOptions.CultureInvariant) == RegexOptions.CultureInvariant)
            {
                invalidOptionString = "CultureInvariant";
            }

            if (invalidOptionString != null)
            {
                throw new ArgumentException(invalidOptionString + " option is not available in inline mode");
            }
        }
    }
}
