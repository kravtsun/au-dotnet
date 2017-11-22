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

            Program program;
            try
            {
                program = new Program(map);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error while creating context: {e.Message}");
                return;
            }
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
                throw new Exception($"Failed to create player, exception: {e.Message}");
            }
        }

        private void MainLoop()
        {
            var randomGenerator = new Random();
            using (var eventLoop = new EventLoop())
            {
                eventLoop.RegisterMove(
                    delegate (int dx, int dy)
                    {
                        _player.Walk(dx, dy);
                        _map.Draw();
                        var random = randomGenerator.Next(100);
                        string message;
                        switch (random)
                        {
                            case 0:
                                message = "You're doing well!";
                                break;
                            case 1:
                                message = "You're awesome!";
                                break;
                            case 2:
                                message = "Spectacular!";
                                break;
                            case 3:
                                message = "Fantastic!";
                                break;
                            case 4:
                                message = "Great!";
                                break;
                            case 5:
                                message = "Well done!";
                                break;
                            case 6:
                                message = "Keep going and be brave!";
                                break;
                            case 7:
                                message = "Interesting...";
                                break;
                            case 8:
                                message = "Nothing can stop you!";
                                break;
                            case 9:
                                message = "What a lovely day!";
                                break;
                            default:
                                message = "";
                                break;
                        }
                        WriteShortMessage(message);
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
                        eventLoop.Finish = true;
                    }
                );

                _map.Draw();

                eventLoop.Run();
            }
        }

        private static void WriteShortMessage(string message)
        {
            var cursorX = Console.CursorLeft;
            var cursorY = Console.CursorTop;
            var messagePosition = new Point(0, Console.WindowHeight - 2);
            // clear line.
            Console.SetCursorPosition(messagePosition.X, messagePosition.Y);
            Console.Write(new string(' ', Console.WindowWidth - 1));

            // write message.
            Console.SetCursorPosition(messagePosition.X, messagePosition.Y);
            Console.Write(message);

            Console.CursorLeft = cursorX;
            Console.CursorTop = cursorY;
        }
    }
}