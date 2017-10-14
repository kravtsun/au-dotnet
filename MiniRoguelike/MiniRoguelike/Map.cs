using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRoguelike
{
    internal partial class Map
    {
        private Cell[,] Cells { get; }

        public static Map LoadFile(string filename)
        {
            var fileStream = File.OpenRead(filename);
            StreamReader fileReader = new StreamReader(fileStream);
            string dimensionsLine = fileReader.ReadLine();
            if (dimensionsLine == null)
            {
                throw new IOException("dimensions line");
            }
            var dimensionsList = dimensionsLine.Split(' ').Select(int.Parse).ToList();
            if (dimensionsList.Count != 2)
            {
                throw new IOException("map dimensions read error");
            }

            int width = dimensionsList[0];
            int height = dimensionsList[1];

            if (width <= 0)
            {
                throw new IOException("map width read error");
            }

            if (height <= 0)
            {
                throw new IOException("map height read error");
            }

            var map = new Map(width, height);
            for (int y = 0; y < height; ++y)
            {
                string line = fileReader.ReadLine();
                if (line == null || line.Length != width)
                {
                    throw new IOException($"map error read in line: {y + 2}");
                }

                for (int x = 0; x < width; ++x)
                {
                    Cell cell = new Cell(line[x]);
                    map.SetCell(x, y, cell);
                }
            }
            return map;
        }

        private Map(int width, int height)
        {
            Cells = new Cell[height, width];
        }

        public int Width => Cells.GetLength(1);

        public int Height => Cells.GetLength(0);

        public Cell GetCell(int x, int y)
        {
            return Cells[y, x];
        }

        public void SetCell(int x, int y, Cell cell)
        {
            Cells[y, x] = cell;
        }

        public Point GetPlayerCoordinates()
        {
            int playersCount = Cells.Cast<Cell>().Count(cell => cell.GetCellType() == Cell.Type.Character);
            if (playersCount == 0)
            {
                throw new Exception("Invalid map: no players");
            }
            if (playersCount > 1)
            {
                throw new Exception("Invalid map: too many players");
            }

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (Cells[y, x].GetCellType() == Cell.Type.Character)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return Point.Invalid();
        }

        public static string Format()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("line #1: <width> <height>");
            sb.AppendLine("lines #2 - #<height>+1: <cells> ");
            return sb.ToString();
        }
    }
}
