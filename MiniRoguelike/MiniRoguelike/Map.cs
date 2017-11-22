using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniRoguelike
{
    internal partial class Map
    {
        private Cell[,] Cells { get; }

        public int Width => Cells.GetLength(1);

        public int Height => Cells.GetLength(0);

        public Map(string filename)
        {
            var fileStream = File.OpenRead(filename);
            var fileReader = new StreamReader(fileStream);
            var dimensionsLine = fileReader.ReadLine();
            if (dimensionsLine == null)
                throw new IOException("dimensions line");
            var dimensionsList = dimensionsLine.Split(' ').Select(int.Parse).ToList();
            if (dimensionsList.Count != 2)
                throw new IOException("map dimensions read error");

            var width = dimensionsList[0];
            var height = dimensionsList[1];

            if (width <= 0)
                throw new IOException("map width read error");

            if (height <= 0)
                throw new IOException("map height read error");

            Cells = new Cell[height, width];
            for (var y = 0; y < height; ++y)
            {
                var line = fileReader.ReadLine();
                if (line == null || line.Length != width)
                    throw new IOException($"map error read in line: {y + 2}");

                for (var x = 0; x < width; ++x)
                {
                    var cell = new Cell(line[x]);
                    Cells[y, x] = cell;
                }
            }
        }

        private Cell GetCell(Point position) => Cells[position.Y, position.X];

        protected virtual void SetCell(Point position, Cell cell) => Cells[position.Y, position.X] = cell;

        public Point GetPlayerCoordinates()
        {
            var playersCount = Cells.Cast<Cell>().Count(cell => cell.IsCharacter());
            if (playersCount == 0)
                throw new Exception("Invalid map: no players");
            if (playersCount > 1)
                throw new Exception("Invalid map: too many players");

            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    if (Cells[y, x].IsCharacter())
                    {
                        return new Point(x, y);
                    }
                }
            }
            return Point.Invalid();
        }

        public bool IsFreeCell(Point position) => GetCell(position).IsFree();

        public void SwapCells(Point first, Point second)
        {
            var firstCell = GetCell(first);
            var secondCell = GetCell(second);
            SetCell(first, secondCell);
            SetCell(second, firstCell);
        }

        public static string Format()
        {
            var sb = new StringBuilder();
            sb.AppendLine("line #1: <width> <height>");
            sb.AppendLine("lines #2 - #<height>+1: <cells> ");
            return sb.ToString();
        }

        public virtual void Draw()
        {
            Console.Clear();
            Console.CursorVisible = false;
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var p = new Point(x, y);
                    Console.Write(GetCell(p).ToString());
                }
                Console.WriteLine();
            }
        }
    }

    class UpdateableMap : Map
    {
        private bool _drawn;
        private List<CellChange> _cellChanges;

        public UpdateableMap(string filename) : base(filename)
        {
            _cellChanges = new List<CellChange>();
        }

        public override void Draw()
        {
            if (!_drawn)
            {
                base.Draw();
                _drawn = true;
            }
            foreach (var cellChange in _cellChanges)
            {
                var position = cellChange.Position;
                var cell = cellChange.NewCell;
                base.SetCell(position, cell);
                DrawCell(position, cell);
            }
        }

        protected override void SetCell(Point position, Cell cell)
        {
            if (_drawn)
            {
                _cellChanges.Add(new CellChange(position, cell));
            }
            else
            {
                base.SetCell(position, cell);
            }
        }

        private void DrawCell(Point position, Cell cell)
        {
            var cursorPosition = new Point(Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(cell.ToString());
            Console.SetCursorPosition(cursorPosition.X, cursorPosition.Y);
        }

        private struct CellChange
        {
            public Point Position { get; }
            public Cell NewCell { get; }

            public CellChange(Point position, Cell newCell)
            {
                Position = position;
                NewCell = newCell;
            }
        }
    }
}