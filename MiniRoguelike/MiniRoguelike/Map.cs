using System;
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

        public static Map LoadFile(string filename)
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

            var map = new Map(width, height);
            for (var y = 0; y < height; ++y)
            {
                var line = fileReader.ReadLine();
                if (line == null || line.Length != width)
                    throw new IOException($"map error read in line: {y + 2}");

                for (var x = 0; x < width; ++x)
                {
                    var cell = new Cell(line[x]);
                    map.SetCell(x, y, cell);
                }
            }
            return map;
        }

        public Cell GetCell(int x, int y) => Cells[y, x];

        public void SetCell(int x, int y, Cell cell) => Cells[y, x] = cell;

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

        public static string Format()
        {
            var sb = new StringBuilder();
            sb.AppendLine("line #1: <width> <height>");
            sb.AppendLine("lines #2 - #<height>+1: <cells> ");
            return sb.ToString();
        }

        private Map(int width, int height)
        {
            Cells = new Cell[height, width];
        }
    }
}