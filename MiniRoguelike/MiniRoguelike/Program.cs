using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MiniRoguelike
{
    public static class Program
    {
        private delegate void MoveHandler(int dx, int dy);

        private delegate void EventHandler();

        private class EventLoop : IDisposable
        {
            private event MoveHandler MovePressed;
            private event EventHandler UnknownPressed;
            private readonly List<MoveHandler> _moveHandlers;
            private readonly List<ConsoleCancelEventHandler> _cancelEventHandlers;
            private readonly List<EventHandler> _unknownEventHandlers;

            public EventLoop()
            {
                _moveHandlers = new List<MoveHandler>();
                _cancelEventHandlers = new List<ConsoleCancelEventHandler>();
                _unknownEventHandlers = new List<EventHandler>();
            }

            public void Run()
            {
                Debug.Assert(MovePressed != null, nameof(MovePressed) + " != null");
                Debug.Assert(UnknownPressed != null, nameof(UnknownPressed) + " != null");

                do
                {
                    while (!Console.KeyAvailable)
                    {
                        Thread.Yield();
                    }
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            MovePressed.Invoke(0, -1);
                            break;
                        case ConsoleKey.DownArrow:
                            MovePressed.Invoke(0, +1);
                            break;
                        case ConsoleKey.LeftArrow:
                            MovePressed.Invoke(-1, 0);
                            break;
                        case ConsoleKey.RightArrow:
                            MovePressed.Invoke(+1, 0);
                            break;
                        case ConsoleKey.Escape:
                            return;
                        default:
                            UnknownPressed.Invoke();
                            break;
                    }
                } while (true);
            }

            public void RegisterMove(MoveHandler handler)
            {
                _moveHandlers.Add(handler);
                MovePressed += handler;
            }

            public void RegisterExit(EventHandler handler)// => Console.CancelKeyPress += handler;
            {
                _cancelEventHandlers.Add(delegate
                {
                    handler.Invoke();
                });
                Console.CancelKeyPress += _cancelEventHandlers.Last();
            }

            public void RegisterUnknown(EventHandler handler)
            {
                _unknownEventHandlers.Add(handler);
                UnknownPressed += handler;
            }

            public void Dispose()
            {
                foreach (var moveHandler in _moveHandlers)
                {
                    MovePressed -= moveHandler;
                }
                foreach (var eventHandler in _unknownEventHandlers)
                {
                    UnknownPressed -= eventHandler;
                }
                foreach (var cancelEventHandler in _cancelEventHandlers)
                {
                    Console.CancelKeyPress -= cancelEventHandler;
                }
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
            {
                Console.WriteLine("Invalid map: nowhere to place the player");
            }

            var player = new Player(map, playerCoordinates);
            using (var eventLoop = new EventLoop())
            {
                eventLoop.RegisterMove(
                    delegate(int dx, int dy)
                    {
                        player.Walk(dx, dy);
                        // TODO fix the whole map redrawing.
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