using System;

namespace MiniRoguelike
{
    public partial class Program
    {
        private readonly Player _player;
        private readonly Map _map;

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
                map = new UpdateableMap(mapFilename);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to read map from file {mapFilename}, exception: {e.Message}");
                return;
            }

            var program = new Program(map);
            program.MainLoop();
        }

        private Program(Map map)
        {
            _map = map;
            try
            {
                var playerCoordinates = map.GetPlayerCoordinates();

                if (playerCoordinates.IsInvalid())
                {
                    Console.Error.WriteLine("Invalid map: nowhere to place the player");
                    return;
                }
                _player = new Player(map, playerCoordinates);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to create player, exception: {e.Message}");
            }
        }

        private void MainLoop()
        {
            using (var eventLoop = new EventLoop())
            {
                eventLoop.RegisterMove(
                    delegate (int dx, int dy)
                    {
                        _player.Walk(dx, dy);
                        _map.Draw();
                    }
                );

                eventLoop.RegisterUnknown(
                    delegate
                    {
                        WriteShortMessage("Unknown key pressed. Use arrows or press Esc to exit.");
                    }
                );

                eventLoop.RegisterExit(
                    delegate
                    {
                        Environment.Exit(0);
                    }
                );

                _map.Draw();
                eventLoop.Run();
            }
        }

        private void WriteShortMessage(string message)
        {
            var cursorX = Console.CursorLeft;
            var cursorY = Console.CursorTop;
            var messagePosition = new Map.Point(_map.Width + 1, _map.Height / 2);
            // clear line.
            Console.SetCursorPosition(messagePosition.X, messagePosition.Y);
            Console.WriteLine();

            // write message.
            Console.SetCursorPosition(messagePosition.X, messagePosition.Y);
            Console.Write(message);

            Console.CursorLeft = cursorX;
            Console.CursorTop = cursorY;
        }
    }
}