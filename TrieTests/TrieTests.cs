using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trie;

namespace TrieTests
{
    using TrieImpl = Trie.Trie;
    [TestClass()]
    public class TrieTests
    {
        [TestMethod()]
        public void UnicodeTest()
        {
            ITrie trie = new TrieImpl();
            const string s = "\uA0A2\uA0A2";
            Assert.IsTrue(trie.Add(s));
            Assert.IsTrue(trie.Contains(s));
            Assert.IsFalse(trie.Contains(s.Substring(0, 1)));
        }

        [TestMethod()]
        public void EmptyTest()
        {
            ITrie trie = new TrieImpl();
            const string empty = "";
            Assert.IsFalse(trie.Contains(empty));
            Assert.IsTrue(trie.Add(empty));
            Assert.IsTrue(trie.Contains(empty));
            Assert.IsTrue(trie.Remove(empty));
            Assert.IsFalse(trie.Remove(empty));
            Assert.IsFalse(trie.Contains(empty));
        }

        [TestMethod()]
        public void SimplePrefixTest()
        {
            ITrie trie = new TrieImpl();
            const string s = "abc";
            Assert.AreEqual(0, trie.Size());
            Assert.IsTrue(trie.Add(s));
            Assert.AreEqual(1, trie.Size());
            Assert.IsFalse(trie.Add(s));

            string prefix = s.Substring(0, s.Length - 1);
            Assert.IsTrue(trie.Add(prefix));
            Assert.IsTrue(trie.Remove(s));
            Assert.IsTrue(trie.Contains(prefix));
            Assert.IsFalse(trie.Contains(s));
        }

        [TestMethod()]
        public void HowManyStartsWithPrefixTest()
        {
            ITrie trie = new TrieImpl();
            Assert.IsTrue(trie.Add("abc"));
            Assert.AreEqual(1, trie.HowManyStartsWithPrefix("a"));
            Assert.IsFalse(trie.Add("abc"));
            Assert.AreEqual(1, trie.HowManyStartsWithPrefix("a"));
            Assert.AreEqual(1, trie.HowManyStartsWithPrefix("abc"));
            Assert.IsTrue(trie.Add("a"));
            Assert.AreEqual(2, trie.HowManyStartsWithPrefix("a"));
            Assert.IsFalse(trie.Add("a"));
            Assert.AreEqual(2, trie.HowManyStartsWithPrefix("a"));
        }
    }
}