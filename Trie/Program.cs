// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Trie
{
    using System.Diagnostics;

    public class Program
    {
        public static void Main(string[] args)
        {
            TestSimple();
        }

        private static void TestSimple()
        {
            Trie trie = new Trie();
            trie.Add("abc");
            string errorMessage = "Error in HowManyStartsWithPrefix";
            Debug.Assert(trie.HowManyStartsWithPrefix("a") == 1, errorMessage);
            trie.Add("abc");
            Debug.Assert(trie.HowManyStartsWithPrefix("a") == 1, errorMessage);
            Debug.Assert(trie.HowManyStartsWithPrefix("abc") == 1, errorMessage);
            trie.Add("a");
            Debug.Assert(trie.HowManyStartsWithPrefix("a") == 2, errorMessage);
            trie.Add("a");
            Debug.Assert(trie.HowManyStartsWithPrefix("a") == 2, errorMessage);
        }
    }
}
