using System;

namespace RegexBuilder
{
    /// <summary>
    /// Provides factory methods for commonly used regex patterns.
    /// </summary>
    public static class CommonPatterns
    {
        /// <summary>
        /// Returns a RegexNode that matches a basic email address pattern.
        /// Pattern matches: localpart@domain.tld
        /// - Local part: alphanumeric characters, dots, hyphens, underscores, percent, and plus signs
        /// - Domain: alphanumeric characters and hyphens, separated by dots
        /// - TLD: 2-6 alphabetic characters
        /// </summary>
        /// <returns>A RegexNode representing an email address pattern.</returns>
        /// <example>
        /// <code>
        /// var regex = RegexBuilder.Build(CommonPatterns.Email());
        /// bool isValid = regex.IsMatch("user@example.com"); // true
        /// </code>
        /// </example>
        public static RegexNode Email()
        {
            // Local part: [a-zA-Z0-9._%+-]+
            var localPart = RegexBuilder.CharacterSet("a-zA-Z0-9._%+-", RegexQuantifier.OneOrMore);

            // @ symbol
            var atSymbol = RegexBuilder.Literal("@");

            // Domain part: [a-zA-Z0-9.-]+
            var domainPart = RegexBuilder.CharacterSet("a-zA-Z0-9.-", RegexQuantifier.OneOrMore);

            // Literal dot before TLD
            var dot = RegexBuilder.Literal(".");

            // TLD: [a-zA-Z]{2,6}
            var tld = RegexBuilder.CharacterSet("a-zA-Z", new RegexQuantifier(2, 6));

            return new RegexNodeConcatenation(localPart, atSymbol, domainPart, dot, tld);
        }

        /// <summary>
        /// Returns a RegexNode that matches a URL pattern.
        /// Pattern matches URLs with optional protocol, domain, and path.
        /// - Protocol: http://, https://, or ftp:// (optional)
        /// - Domain: standard domain format with optional port
        /// - Path: optional path, query string, and fragment
        /// </summary>
        /// <returns>A RegexNode representing a URL pattern.</returns>
        /// <example>
        /// <code>
        /// var regex = RegexBuilder.Build(CommonPatterns.Url());
        /// bool isValid = regex.IsMatch("https://example.com/path"); // true
        /// </code>
        /// </example>
        public static RegexNode Url()
        {
            // Protocol: (https?|ftp)://
            var httpS = RegexBuilder.Literal("s");
            httpS.Quantifier = RegexQuantifier.ZeroOrOne;

            var httpProtocol = new RegexNodeConcatenation(
                RegexBuilder.Literal("http"),
                httpS,
                RegexBuilder.Literal("://")
            );

            var ftpProtocol = RegexBuilder.Literal("ftp://");
            var protocol = new RegexNodeAlternation(httpProtocol, ftpProtocol);
            protocol.Quantifier = RegexQuantifier.ZeroOrOne;

            // Domain: [a-zA-Z0-9][-a-zA-Z0-9]*(\.[a-zA-Z0-9][-a-zA-Z0-9]*)*
            var domainChar = RegexBuilder.CharacterSet("a-zA-Z0-9-", RegexQuantifier.OneOrMore);

            var domainSegmentChar = RegexBuilder.CharacterSet("a-zA-Z0-9-", RegexQuantifier.OneOrMore);

            var domainSegment = new RegexNodeConcatenation(
                RegexBuilder.Literal("."),
                domainSegmentChar
            );
            domainSegment.Quantifier = RegexQuantifier.ZeroOrMore;

            var domain = new RegexNodeConcatenation(domainChar, domainSegment);

            // Optional port: (:[0-9]+)?
            var portNumber = RegexBuilder.CharacterSet("0-9", RegexQuantifier.OneOrMore);

            var port = new RegexNodeConcatenation(
                RegexBuilder.Literal(":"),
                portNumber
            );
            port.Quantifier = RegexQuantifier.ZeroOrOne;

            // Optional path: (/[a-zA-Z0-9\-._~:/?#[\]@!$&'()*+,;=]*)?
            var pathChars = RegexBuilder.CharacterSet("a-zA-Z0-9\\-._~:/?#\\[\\]@!$&'()*+,;=", RegexQuantifier.ZeroOrMore);

            var path = new RegexNodeConcatenation(
                RegexBuilder.Literal("/"),
                pathChars
            );
            path.Quantifier = RegexQuantifier.ZeroOrOne;

            return new RegexNodeConcatenation(protocol, domain, port, path);
        }
    }
}
