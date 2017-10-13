// <copyright file="ITrie.cs" company="SPbAU">
// Copyright (c) SPbAU. All rights reserved.
// </copyright>

namespace Trie
{
    public interface ITrie
    {
        /// <summary>
        /// Expected complexity: O(|element|)
        /// </summary>
        /// <param name="element">specified string</param>
        /// <returns>true if this trie did not already contain the specified element</returns>
        bool Add(string element);

        /// <summary>
        /// Expected complexity: O(|element|)
        /// </summary>
        /// <param name="element">specified string</param>
        /// <returns>true if this trie has already been called Contains with the same element.</returns>
        bool Contains(string element);

        /// <summary>
        /// Expected complexity: O(|element|)
        /// </summary>
        /// <param name="element">specified string</param>
        /// <returns>Returns true if this trie contained the specified element</returns>
        bool Remove(string element);

        /// <summary>
        /// Expected complexity: O(1)
        /// </summary>
        /// <returns>Number of unique strings added into trie</returns>
        int Size();

        /// <summary>
        /// Expected complexity: O(|prefix|)
        /// </summary>
        /// <param name="prefix">specified string</param>
        /// <returns>Number of unique strings with specified prefix contained in trie.</returns>
        int HowManyStartsWithPrefix(string prefix);
    }
}
