using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MiniRoguelike
{
    public partial class Program
    {
        private delegate void MoveHandler(int dx, int dy);

        private delegate void EventHandler();

        private class EventLoop : IDisposable
        {
            public bool Finish { private get; set; }

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

                while (!Finish)
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
                            Finish = true;
                            break;
                        default:
                            UnknownPressed.Invoke();
                            break;
                    }
                }
            }

            public void RegisterMove(MoveHandler handler)
            {
                _moveHandlers.Add(handler);
                MovePressed += handler;
            }

            public void RegisterExit(EventHandler handler)
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
    }
}