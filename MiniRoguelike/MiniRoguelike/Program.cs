using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRoguelike
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("USAGE: <executable> <map file>");
                System.Console.WriteLine();
                System.Console.WriteLine("Map format: \n" + Map.Format());
                System.Console.WriteLine("Cells format: \n" + Map.Cell.Format());
                return;
            }
            string mapFilename = args[0];
            Map map;
            try
            {
                map = Map.LoadFile(mapFilename);
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine($"Failed to read map from file {mapFilename}, exception: {e.Message}");
                return;
            }

            Map.Point playerCoordinates = map.GetPlayerCoordinates();

            if (playerCoordinates.IsInvalid())
            {
                System.Console.WriteLine("Invalid map: nowhere to place the player");
            }

            Player player = new Player(map, playerCoordinates);

            ConsoleKeyInfo keyinfo;
            do
            {
                DrawMap(map);
                keyinfo = Console.ReadKey();
                switch (keyinfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        player.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        player.MoveRight();
                        break;
                    case ConsoleKey.UpArrow:
                        player.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        player.MoveDown();
                        break;
                    default:
                        System.Console.Write("\r");
                        System.Console.Write($"Unknown key pressed: {keyinfo.Key}. ");
                        System.Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);
        }

        private static void DrawMap(Map map)
        {
            System.Console.Clear();
            System.Console.CursorVisible = false;
            for (int y = 0; y < map.Height; ++y)
            {
                for (int x = 0; x < map.Width; ++x)
                {
                    System.Console.Write(map.GetCell(x, y).ToString());
                }
                System.Console.WriteLine();
            }
        }
    }
}
