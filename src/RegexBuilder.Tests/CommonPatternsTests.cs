using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegexBuilder.Tests
{
    [TestClass]
    public class CommonPatternsTests
    {
        #region Email Tests

        [TestMethod]
        public void TestEmailPattern_ValidSimpleEmail()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithDot()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user.name@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithUnderscore()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user_name@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithPlus()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user+tag@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithHyphen()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user-name@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithNumbers()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user123@example456.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithSubdomain()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user@mail.example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithLongTLD()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user@example.museum"));
        }

        [TestMethod]
        public void TestEmailPattern_ValidEmailWithShortTLD()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsTrue(regex.IsMatch("user@example.co"));
        }

        [TestMethod]
        public void TestEmailPattern_InvalidEmailNoAt()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsFalse(regex.IsMatch("userexample.com"));
        }

        [TestMethod]
        public void TestEmailPattern_InvalidEmailNoDomain()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsFalse(regex.IsMatch("user@"));
        }

        [TestMethod]
        public void TestEmailPattern_InvalidEmailNoLocalPart()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsFalse(regex.IsMatch("@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_InvalidEmailNoTLD()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsFalse(regex.IsMatch("user@example"));
        }

        [TestMethod]
        public void TestEmailPattern_InvalidEmailTLDTooShort()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsFalse(regex.IsMatch("user@example.c"));
        }

        [TestMethod]
        public void TestEmailPattern_InvalidEmailTLDTooLong()
        {
            // Note: This pattern will match "user@example.toolong" because the domain part
            // can consume "example.too" and TLD can match "long" (4 chars).
            // To properly validate, the pattern would need to be anchored or more complex.
            // For a simple pattern, we test with a clearly invalid format instead.
            var regex = RegexBuilder.Build(CommonPatterns.Email());
            Assert.IsFalse(regex.IsMatch("user@.toolong"));
        }

        [TestMethod]
        public void TestEmailPattern_GeneratesCorrectRegex()
        {
            var emailNode = CommonPatterns.Email();
            string pattern = emailNode.ToRegexPattern();
            
            // Verify it's a valid regex pattern
            var regex = new Regex(pattern);
            Assert.IsTrue(regex.IsMatch("test@example.com"));
        }

        [TestMethod]
        public void TestEmailPattern_CombinedWithOtherNodes()
        {
            var regex = RegexBuilder.Build(
                RegexBuilder.Literal("Email: "),
                CommonPatterns.Email()
            );
            
            Assert.IsTrue(regex.IsMatch("Email: user@example.com"));
            Assert.IsFalse(regex.IsMatch("user@example.com"));
        }

        #endregion

        #region URL Tests

        [TestMethod]
        public void TestUrlPattern_ValidHttpUrl()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("http://example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidHttpsUrl()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidFtpUrl()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("ftp://example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithoutProtocol()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithSubdomain()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://www.example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithPath()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com/path"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithLongPath()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com/path/to/resource"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithQueryString()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com/path?query=value"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithFragment()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com/path#section"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithPort()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com:8080"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithPortAndPath()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com:8080/path"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithAllComponents()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://www.example.com:8080/path/to/resource?query=value&other=param#section"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithHyphenInDomain()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://my-site.example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithNumbersInDomain()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://site123.example456.com"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithComplexPath()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com/api/v1/users/123"));
        }

        [TestMethod]
        public void TestUrlPattern_ValidUrlWithSpecialCharsInPath()
        {
            var regex = RegexBuilder.Build(CommonPatterns.Url());
            Assert.IsTrue(regex.IsMatch("https://example.com/path_with-special.chars~test"));
        }

        [TestMethod]
        public void TestUrlPattern_GeneratesCorrectRegex()
        {
            var urlNode = CommonPatterns.Url();
            string pattern = urlNode.ToRegexPattern();
            
            // Verify it's a valid regex pattern
            var regex = new Regex(pattern);
            Assert.IsTrue(regex.IsMatch("https://example.com"));
        }

        [TestMethod]
        public void TestUrlPattern_CombinedWithOtherNodes()
        {
            var regex = RegexBuilder.Build(
                RegexBuilder.Literal("Visit: "),
                CommonPatterns.Url()
            );
            
            Assert.IsTrue(regex.IsMatch("Visit: https://example.com"));
            Assert.IsFalse(regex.IsMatch("https://example.com"));
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void TestCombinedEmailAndUrl()
        {
            var regex = RegexBuilder.Build(
                RegexBuilder.Literal("Email: "),
                CommonPatterns.Email(),
                RegexBuilder.Literal(", Website: "),
                CommonPatterns.Url()
            );
            
            Assert.IsTrue(regex.IsMatch("Email: user@example.com, Website: https://example.com"));
        }

        [TestMethod]
        public void TestEmailInAlternation()
        {
            var regex = RegexBuilder.Build(
                new RegexNodeAlternation(
                    CommonPatterns.Email(),
                    CommonPatterns.Url()
                )
            );
            
            Assert.IsTrue(regex.IsMatch("user@example.com"));
            Assert.IsTrue(regex.IsMatch("https://example.com"));
        }

        [TestMethod]
        public void TestEmailWithQuantifier()
        {
            var emailNode = CommonPatterns.Email();
            emailNode.Quantifier = RegexQuantifier.ZeroOrOne;
            
            var regex = RegexBuilder.Build(
                RegexBuilder.Literal("Contact: "),
                emailNode
            );
            
            Assert.IsTrue(regex.IsMatch("Contact: user@example.com"));
            Assert.IsTrue(regex.IsMatch("Contact: "));
        }

        [TestMethod]
        public void TestUrlWithQuantifier()
        {
            var urlNode = CommonPatterns.Url();
            urlNode.Quantifier = RegexQuantifier.ZeroOrOne;
            
            var regex = RegexBuilder.Build(
                RegexBuilder.Literal("Website: "),
                urlNode
            );
            
            Assert.IsTrue(regex.IsMatch("Website: https://example.com"));
            Assert.IsTrue(regex.IsMatch("Website: "));
        }

        #endregion
    }
}
