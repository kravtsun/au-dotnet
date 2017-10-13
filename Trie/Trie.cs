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
            Vertex current = TraverseWord(element, true);
            if (current.IsTerminal)
            {
                return false;
            }
            current.IsTerminal = true;
            while (current != null)
            {
                ++current.SubTreeSize;
                current = current.Parent;
            }

            return true;
        }

        public bool Contains(string element)
        {
            Vertex current = TraverseWord(element, false);
            return current?.IsTerminal ?? false;
        }

        public int HowManyStartsWithPrefix(string prefix)
        {
            Vertex current = TraverseWord(prefix, false);
            return current?.SubTreeSize ?? 0;
        }

        public bool Remove(string element)
        {
            Vertex current = TraverseWord(element, false);
            if (current == null || !current.IsTerminal)
            {
                return false;
            }

            current.IsTerminal = false;
            for (int i = element.Length - 1; i >= 0; --i)
            {
                current.RemoveIfEmpty(element[i]);
                current = current.Parent;
            }

            --_root.SubTreeSize;
            if (_root.SubTreeSize == 0)
            {
                _root = null;
            }

            return true;
        }

        public int Size()
        {
            return _root?.SubTreeSize ?? 0;
        }

        private Vertex TraverseWord(string element, bool addIfNotExists)
        {
            if (_root == null)
            {
                if (addIfNotExists)
                {
                    _root = new Vertex(null); // _root only.
                }
                else
                {
                    return null;
                }
            }

            Vertex current = _root;

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

        private class Vertex
        {
            public Vertex(Vertex parent)
            {
                IsTerminal = false;
                SubTreeSize = 0;
                Parent = parent;
                Next = new Dictionary<char, Vertex>();
            }

            public bool IsTerminal { get; set; }

            public int SubTreeSize { get; set; }

            public Vertex Parent { get; }

            private IDictionary<char, Vertex> Next { get; }

            public bool ContainsNext(char c)
            {
                return Next.ContainsKey(c);
            }

            public Vertex GetNext(char c)
            {
                return Next[c];
            }

            public void SetNext(char c, Vertex newVertex)
            {
                Next[c] = newVertex;
            }


            public void RemoveIfEmpty(char stepChar)
            {
                --SubTreeSize;
                if (SubTreeSize == 0)
                {
                    Parent?.SetNext(stepChar, null);
                }
            }
        }
    }
}
