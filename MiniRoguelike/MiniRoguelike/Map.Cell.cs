using System;
using System.ComponentModel;
using System.Text;

namespace MiniRoguelike
{
    internal partial class Map
    {
        public struct Cell
        {
            public enum Type
            {
                Empty,
                Wall,
                Character
            }

            private static Type TypeFromChar(char c)
            {
                switch (c)
                {
                    case '.':
                        return Type.Empty;
                    case '#':
                        return Type.Wall;
                    case '@':
                        return Type.Character;
                    default:
                        throw new InvalidEnumArgumentException($"TypeFromChar: {c}");
                }
            }

            private static char CharFromType(Type type)
            {
                switch (type)
                {
                    case Type.Wall:
                        return '#';
                    case Type.Empty:
                        return '.';
                    case Type.Character:
                        return '@';
                    default:
                        throw new InvalidEnumArgumentException($"CharFromType: {type}");
                }
            }

            private readonly Type _type;

            public Cell(Type type)
            {
                _type = type;
            }

            public Cell(char code)
            {
                _type = TypeFromChar(code);
            }

            public Type GetCellType()
            {
                return _type;
            }

            public bool IsFree()
            {
                return _type.Equals(Type.Empty);
            }

            public override string ToString()
            {
                return $"{CharFromType(_type)}";
            }

            public static string Format()
            {
                var sb = new StringBuilder();
                foreach (Type cellType in System.Enum.GetValues(typeof(Type)))
                {
                    string cellTypeName = System.Enum.GetName(typeof(Type), cellType);
                    sb.Append($"{CharFromType(cellType)} - {cellTypeName};\n");
                }
                return sb.ToString();
            }
        }
    }
}