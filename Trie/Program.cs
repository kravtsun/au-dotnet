// <copyright file="Program.cs" company="SPbAU">
// Copyright (c) SPbAU. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Trie
{
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

            Debug.Assert(trie.Add(string.Empty), "Error in PR review testcase");

            string s = "\uA0A2\uA0A2";

            // can fail with IndexOutOfRangeException
            Debug.Assert(trie.Add(s), "Error with unicode symbols");
        }
    }
}
