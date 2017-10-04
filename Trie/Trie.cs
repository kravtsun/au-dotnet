// <copyright file="Trie.cs" company="SPbAU">
// Copyright (c) SPbAU. All rights reserved.
// </copyright>

namespace Trie
{
    using System.Collections.Generic;

    public class Trie : ITrie
    {
        private Vertex _root;

        public bool Add(string element)
        {
            Vertex current = this.TraverseWord(element, true);
            if (current.IsTerminal)
            {
                return false;
            }
            else
            {
                current.IsTerminal = true;
                while (current != null)
                {
                    ++current.SubTreeSize;
                    current = current.Parent;
                }

                return true;
            }
        }

        public bool Contains(string element)
        {
            Vertex current = this.TraverseWord(element, false);
            return current != null && current.IsTerminal;
        }

        public int HowManyStartsWithPrefix(string prefix)
        {
            Vertex current = this.TraverseWord(prefix, false);
            return current?.SubTreeSize ?? 0;
        }

        public bool Remove(string element)
        {
            Vertex current = this.TraverseWord(element, false);
            if (current == null || !current.IsTerminal)
            {
                return false;
            }

            current.IsTerminal = false;
            for (int i = element.Length - 1; i >= 0; --i)
            {
                this.RemoveIfEmpty(current, element[i]);
                current = current.Parent;
            }

            --this._root.SubTreeSize;
            if (this._root.SubTreeSize == 0)
            {
                this._root = null;
            }

            return true;
        }

        public int Size()
        {
            return this._root?.SubTreeSize ?? 0;
        }

        private Vertex TraverseWord(string element, bool addIfNotExists)
        {
            if (this._root == null)
            {
                if (addIfNotExists)
                {
                    this._root = new Vertex(null); // root only.
                }
                else
                {
                    return null;
                }
            }

            Vertex current = this._root;

            foreach (char c in element)
            {
                if (!current.ContainsNext(c))
                {
                    current.SetNext(c, new Vertex(current));
                }

                current = current.GetNext(c);
            }

            return current;
        }

        private void RemoveIfEmpty(Vertex current, char stepChar)
        {
            --current.SubTreeSize;
            if (current.SubTreeSize == 0)
            {
                current.Parent?.SetNext(stepChar, null);
            }
        }

        private class Vertex
        {
            public Vertex(Vertex parent)
            {
                this.IsTerminal = false;
                this.SubTreeSize = 0;
                this.Parent = parent;
                this.Next = new Dictionary<char, Vertex>();
            }

            public bool IsTerminal { get; set; }

            public int SubTreeSize { get; set; }

            public Vertex Parent { get; }

            private IDictionary<char, Vertex> Next { get; }

            public bool ContainsNext(char c)
            {
                return this.Next.ContainsKey(c);
            }

            public Vertex GetNext(char c)
            {
                return this.Next[c];
            }

            public void SetNext(char c, Vertex newVertex)
            {
                this.Next[c] = newVertex;
            }
        }
    }
}
