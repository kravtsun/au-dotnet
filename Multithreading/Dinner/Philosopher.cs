using System;
using System.Threading;
using static System.Console;

namespace Dinner
{
    internal class Philosopher
    {
        private int Id { get; }
        private readonly IFork _first;
        private readonly IFork _second;
        private static readonly Random Random = new Random();
        private const int ActivityMaximumTimeMillis = 1000;

        public Philosopher(int id, IFork left, IFork right)
        {
            Id = id;
            if (left.Id < right.Id)
            {
                _first = left;
                _second = right;
            }
            else
            {
                _first = right;
                _second = left;
            }
        }

        public void Think()
        {
            Log("eating", true);
            Thread.Sleep(Random.Next(ActivityMaximumTimeMillis));
            Log("eating", false);
        }

        public void Eat()
        {
            while (!_first.Get())
            {
            }
            while (!_second.Get())
            {
            }

            Log("eating", true);
            Thread.Sleep(Random.Next(ActivityMaximumTimeMillis));
            Log("eating", false);

            _first.Put();
            _second.Put();
        }

        public void Log(string message)
        {
            WriteLine($"Philosopher#{Id}: {message}");
        }

        private void Log(string activity, bool isStart)
        {
            var activityString = isStart ? "starts" : "finished";
            Log($"{activityString} {activity}");
        }
    }
}
