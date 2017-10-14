using System;

namespace MiniRoguelike
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: <executable> <map file>");
                Console.WriteLine();
                Console.WriteLine("Map format: \n" + Map.Format());
                Console.WriteLine("Cells format: \n" + Map.Cell.Format());
                return;
            }
            var mapFilename = args[0];
            Map map;
            try
            {
                map = Map.LoadFile(mapFilename);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to read map from file {mapFilename}, exception: {e.Message}");
                return;
            }

            var playerCoordinates = map.GetPlayerCoordinates();

            if (playerCoordinates.IsInvalid())
                Console.WriteLine("Invalid map: nowhere to place the player");

            var player = new Player(map, playerCoordinates);

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
                        Console.Write("\r");
                        Console.Write($"Unknown key pressed: {keyinfo.Key}. ");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            } while (keyinfo.Key != ConsoleKey.X);
        }

        private static void DrawMap(Map map)
        {
            Console.Clear();
            Console.CursorVisible = false;
            for (var y = 0; y < map.Height; ++y)
            {
                for (var x = 0; x < map.Width; ++x)
                {
                    Console.Write(map.GetCell(x, y).ToString());
                }
                Console.WriteLine();
            }
        }
    }
}