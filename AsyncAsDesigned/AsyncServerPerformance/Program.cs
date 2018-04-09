using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AsyncServerPerformance
{
    class Program
    {
        static void Main(string[] args)
        {

            ThreadPool.GetMinThreads(out var workerThreads, out var asyncThreads);


            ThreadPool.SetMaxThreads(workerThreads, asyncThreads);


            var done = 0;
            //var p = Process.GetCurrentProcess();

            //var t = Task.Run(() =>
            //{
            //    while (done < 100)
            //    {
            //        Task.Delay(100).Wait();

            //        Console.WriteLine($"ThreadCount: {p.Threads.Count} Done: {done}");
            //    }
            //});


            List<Tuple<int, int>> threadsA = new List<Tuple<int, int>>();

            object lockDone = new object();

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            for (var i = 0; i < 100; i++)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Thread.Sleep(1000);
                    lock (lockDone)
                    {
                        threadsA.Add(Tuple.Create(i, Thread.CurrentThread.ManagedThreadId));
                        done++;
                        if(done == 99)
                        {
                            autoResetEvent.Set();
                        }
                    }
                });
            }

            autoResetEvent.WaitOne();

            List<Task> tasks = new List<Task>();
            List<Tuple<int, int>> threadsB = new List<Tuple<int, int>>();


            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await Task.Delay(500);
                    lock (lockDone)
                    {
                        threadsB.Add(Tuple.Create(i, Thread.CurrentThread.ManagedThreadId));
                        done++;
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            threadsA.ForEach(i => Console.WriteLine($"A Index: {i.Item1} Thread {i.Item2}"));
            threadsB.ForEach(i => Console.WriteLine($"B Index: {i.Item1} Thread {i.Item2}"));

            Console.ReadKey();

        }
    }
}
