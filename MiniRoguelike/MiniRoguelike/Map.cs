using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniRoguelike
{
    internal partial class Map
    {
        private Cell[,] Cells { get; }
        protected int LeftBase { get; set; }
        protected int TopBase { get; set; }

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
                    if (cell.IsCharacter())
                    {
                        SetFocus(new Point(x, y));
                    }
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

        public void SetFocus(Point position)
        {
            LeftBase = OptimalBase(LeftBase, Width, ConsoleWidth(), position.X);
            TopBase = OptimalBase(TopBase, Height, ConsoleHeight(), position.Y);
        }

        private static int OptimalBase(int currentBase, int mapDimension, int windowDimension, int focusPosition)
        {
            if (mapDimension <= windowDimension)
            {
                return 0;
            }
            const int padding = 1;
            if (focusPosition - padding < currentBase)
            {
                return Math.Max(0, focusPosition - padding);
            }

            if (focusPosition + padding >= currentBase + windowDimension)
            {
                return Math.Min(mapDimension - windowDimension, focusPosition + padding - windowDimension + 1);
            }
            return currentBase;
        }
        
        public virtual void Draw()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            Console.CursorTop = 0;

            var heightLimit = Math.Min(Height - TopBase, ConsoleHeight());
            var widthLimit = Math.Min(Width - LeftBase, ConsoleWidth());
            Console.SetBufferSize(Width, Height);
            for (var y = 0; y < heightLimit; ++y)
            {
                var lineBuilder = new StringBuilder(Width);
                for (var x = 0; x < widthLimit; ++x)
                {
                    lineBuilder.Append(Cells[TopBase + y, LeftBase + x]);
                }
                Console.WriteLine(lineBuilder);
            }
        }

        private static int ConsoleWidth()
        {
            return Console.WindowWidth - 1;
        }

        private static int ConsoleHeight()
        {
            return Console.WindowHeight - 2;
        }
    }
}