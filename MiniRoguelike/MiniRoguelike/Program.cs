using System;
using System.Diagnostics;

namespace MiniRoguelike
{
    public static class Program
    {
        private delegate void ArrowHandler();

        private class EventLoop
        {
            private event ArrowHandler UpPressed;
            private event ArrowHandler DownPressed;
            private event ArrowHandler LeftPressed;
            private event ArrowHandler RightPressed;
            private event ArrowHandler UnknownPressed;

            public void Run()
            {
                Debug.Assert(UpPressed != null, nameof(UpPressed) + " != null");
                Debug.Assert(DownPressed != null, nameof(DownPressed) + " != null");
                Debug.Assert(LeftPressed != null, nameof(LeftPressed) + " != null");
                Debug.Assert(RightPressed != null, nameof(RightPressed) + " != null");
                Debug.Assert(UnknownPressed != null, nameof(UnknownPressed) + " != null");

                while (true)
                {
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            UpPressed.Invoke();
                            break;
                        case ConsoleKey.DownArrow:
                            DownPressed.Invoke();
                            break;
                        case ConsoleKey.LeftArrow:
                            LeftPressed.Invoke();
                            break;
                        case ConsoleKey.RightArrow:
                            RightPressed.Invoke();
                            break;
                        case ConsoleKey.Escape:
                            return;
                        default:
                            UnknownPressed.Invoke();
                            break;
                    }
                }
            }

            public void RegisterUp(ArrowHandler handler) => UpPressed += handler;

            public void RegisterDown(ArrowHandler handler) => DownPressed += handler;

            public void RegisterLeft(ArrowHandler handler) => LeftPressed += handler;

            public void RegisterRight(ArrowHandler handler) => RightPressed += handler;

            public void RegisterUnknown(ArrowHandler handler) => UnknownPressed += handler;

            public void RegisterExit(ArrowHandler handler)
            {
                Console.CancelKeyPress += delegate
                {
                    handler.Invoke();
                };
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: <executable> <map file>");
                Console.WriteLine();
                Console.WriteLine("Map format: \n" + Map.Format());
                Console.WriteLine("Cells format: \n" + Map.Cell.CellFormat());
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
            var eventLoop = new EventLoop();

            eventLoop.RegisterUp(
                delegate
                {
                    player.MoveUp();
                    DrawMap(map);
                }
            );

            eventLoop.RegisterDown(
                delegate
                {
                    player.MoveDown();
                    DrawMap(map);
                }
            );

            eventLoop.RegisterLeft(
                delegate
                {
                    player.MoveLeft();
                    DrawMap(map);
                }
            );

            eventLoop.RegisterRight(
                delegate
                {
                    player.MoveRight();
                    DrawMap(map);
                }
            );

            eventLoop.RegisterUnknown(
                delegate
                {
                    Console.Write("Unknown key pressed. Use arrows or press Esc to exit. \n");
                }
            );

            eventLoop.RegisterExit(
                delegate
                {
                    Environment.Exit(0);
                }
            );

            DrawMap(map);

            eventLoop.Run();
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