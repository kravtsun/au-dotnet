// <copyright file="Trie.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Trie
{
    public class Trie : ITrie
    {
        private Vertex root;

        public Trie()
        {
            this.root = null;
        }

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
                    current.SubTreeSize++;
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
            return current == null ? 0 : current.SubTreeSize;
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

            this.root.SubTreeSize--;
            if (this.root.SubTreeSize == 0)
            {
                this.root = null;
            }

            return true;
        }

        public int Size()
        {
            return this.root == null ? 0 : this.root.SubTreeSize;
        }

        private Vertex TraverseWord(string element, bool addIfNotExists)
        {
            if (this.root == null)
            {
                if (addIfNotExists)
                {
                    this.root = new Vertex(null); // root only.
                }
                else
                {
                    return null;
                }
            }

            Vertex current = this.root;

            for (int i = 0; i < element.Length; ++i)
            {
                char c = element[i];
                if (current.GetNext(c) == null)
                {
                    if (addIfNotExists)
                    {
                        current.SetNext(c, new Vertex(current));
                    }
                    else
                    {
                        return null;
                    }
                }

                current = current.GetNext(c);
            }

            return current;
        }

        private void RemoveIfEmpty(Vertex current, char stepChar)
        {
            current.SubTreeSize--;
            if (current.SubTreeSize == 0 && current.Parent != null)
            {
                current.Parent.SetNext(stepChar, null);
            }
        }

        private class Vertex
        {
            private const int CHARPOWER = 2 * 256;
            private Vertex[] next = null;

            public Vertex(Vertex parent)
            {
                this.IsTerminal = false;
                this.next = new Vertex[CHARPOWER];
                this.Parent = parent;
                this.SubTreeSize = 0;
            }

            public bool IsTerminal { get; set; }

            public int SubTreeSize { get; set; }

            public Vertex Parent { get; set; }

            public Vertex GetNext(char c)
            {
                return this.next[c];
            }

            public void SetNext(char c, Vertex newVertex)
            {
                this.next[c] = newVertex;
            }
        }
    }
}
