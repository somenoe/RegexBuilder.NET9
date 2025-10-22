using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegexBuilder.Tests
{
    [TestClass]
    public class ExtensionMethodTests
    {
        private static readonly string[] s_oldValues1 = new[] { "a", "c" };
        private static readonly string[] s_newValues1 = new[] { "x", "cz" };
        private static readonly string[] s_oldValues2 = new[] { "x", "y" };
        private static readonly string[] s_newValues2 = new[] { "X", "Y" };
        private static readonly string[] s_oldValues3 = new[] { "a" };
        private static readonly string[] s_newValues3 = new[] { "b" };
        private static readonly string[] s_oldValues4 = new[] { "a", "b" };
        private static readonly string[] s_newValues4 = new[] { "a", "b" };
        private static readonly string[] s_newValues5 = new[] { "x" };

        [TestMethod]
        public void TestReplaceMany1()
        {
            StringBuilder stringBuilder = new StringBuilder("abc");
            stringBuilder.ReplaceMany(s_oldValues1, s_newValues1);
            Assert.AreEqual(stringBuilder.ToString(), "xbcz");
        }

        [TestMethod]
        public void TestReplaceMany2()
        {
            StringBuilder stringBuilder = new StringBuilder("abc");
            stringBuilder.ReplaceMany(s_oldValues2, s_newValues2);
            Assert.AreEqual(stringBuilder.ToString(), "abc");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceManyWithNullExtensionObject()
        {
            StringBuilder builder = null;
            // ReSharper disable ExpressionIsAlwaysNull
            builder.ReplaceMany(s_oldValues3, s_newValues3);
            // ReSharper restore ExpressionIsAlwaysNull
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceManyWithNullArguments1()
        {
            new StringBuilder("abc").ReplaceMany(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceManyWithNullArguments2()
        {
            new StringBuilder("abc").ReplaceMany(s_oldValues4, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceManyWithNullArguments3()
        {
            new StringBuilder("abc").ReplaceMany(null, s_newValues4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReplaceManyWithDifferentArgumentLength()
        {
            new StringBuilder("abc").ReplaceMany(s_oldValues4, s_newValues5);
        }
    }
}
