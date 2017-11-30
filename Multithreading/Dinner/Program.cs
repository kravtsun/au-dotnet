using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Resource hierarchy solution of 
// https://en.wikipedia.org/wiki/Dining_philosophers_problem
namespace Dinner
{
    internal static class Program
    {
        private static void Main()
        {
            // number of philosopthers and forks (as stated in task).
            const int philosophersCount = 5;

            IList<Fork> forks = new List<Fork>();
            for (int i = 0; i < philosophersCount; ++i)
            {
                forks.Add(new Fork(i));
            }

            var tasks = new List<Task>();
            var cancellationToken = new CancellationTokenSource().Token;
            for (int i = 0; i < philosophersCount; ++i)
            {
                var firstFork = forks[i];
                var secondFork = forks[(i + 1) % philosophersCount];

                var philosopher = new Philosopher(i, firstFork, secondFork);

                var task = new Task(() =>
                    {
                        philosopher.Log("starting task");
                        int cnt = 0;
                        while (true)
                        {
                            philosopher.Log($"Stage #{cnt}");
                            cnt++;

                            philosopher.Eat();
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            philosopher.Think();
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }
                    , cancellationToken, TaskCreationOptions.LongRunning);
                tasks.Add(task);
            }

            Console.WriteLine("Press [Enter] to start and [Enter] once more to stop what will happen soon...");
            Console.ReadLine();
            foreach (var task in tasks)
            {
                task.Start();
            }
            Console.ReadLine();
        }
    }
}
