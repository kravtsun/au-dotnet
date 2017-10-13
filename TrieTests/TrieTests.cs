using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trie;

namespace TrieTests
{
    using TrieImpl = Trie.Trie;
    [TestClass]
    public class TrieTests
    {
        private ITrie _trie;

        [TestInitialize]
        public void SetUp()
        {
            _trie = new TrieImpl();
        }

        [TestMethod]
        public void UnicodeTest()
        {
            
            const string s = "\uA0A2\uA0A2";
            Assert.IsTrue(_trie.Add(s));
            Assert.IsTrue(_trie.Contains(s));
            Assert.IsFalse(_trie.Contains(s.Substring(0, 1)));
        }

        [TestMethod]
        public void EmptyTest()
        {
            const string empty = "";
            Assert.IsFalse(_trie.Contains(empty));
            Assert.IsTrue(_trie.Add(empty));
            Assert.IsTrue(_trie.Contains(empty));
            Assert.IsTrue(_trie.Remove(empty));
            Assert.IsFalse(_trie.Remove(empty));
            Assert.IsFalse(_trie.Contains(empty));
        }

        [TestMethod]
        public void SimplePrefixTest()
        {
            const string s = "abc";
            Assert.AreEqual(0, _trie.Size());
            Assert.IsTrue(_trie.Add(s));
            Assert.AreEqual(1, _trie.Size());
            Assert.IsFalse(_trie.Add(s));

            string prefix = s.Substring(0, s.Length - 1);
            Assert.IsTrue(_trie.Add(prefix));
            Assert.IsTrue(_trie.Remove(s));
            Assert.IsTrue(_trie.Contains(prefix));
            Assert.IsFalse(_trie.Contains(s));
        }

        [TestMethod]
        public void HowManyStartsWithPrefixTest()
        {
            Assert.IsTrue(_trie.Add("abc"));
            Assert.AreEqual(1, _trie.HowManyStartsWithPrefix("a"));
            Assert.IsFalse(_trie.Add("abc"));
            Assert.AreEqual(1, _trie.HowManyStartsWithPrefix("a"));
            Assert.AreEqual(1, _trie.HowManyStartsWithPrefix("abc"));
            Assert.IsTrue(_trie.Add("a"));
            Assert.AreEqual(2, _trie.HowManyStartsWithPrefix("a"));
            Assert.IsFalse(_trie.Add("a"));
            Assert.AreEqual(2, _trie.HowManyStartsWithPrefix("a"));
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsTrue(_trie.Add("ab"));
            Assert.IsTrue(_trie.Add("ac"));
            Assert.IsFalse(_trie.Contains("a"));
        }
    }
}