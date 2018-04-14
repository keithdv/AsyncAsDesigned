using AsyncAsDesigned.PerfLib;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfAppServer
{
    class Program
    {

        public static DateTime? Start, Stop;

        static async Task Main(string[] args)
        {

            // NEVER USE THIS IN PRODUCTION!!!
            // Dramatically improves the performance of Sync
            // This shows that Task.Run MAY use a new thread\
            // But .NET is well aware of the cost of threads
            // ThreadPool.SetMinThreads(40, 40);

            if (args.Length != 2)
            {
                throw new Exception("Invalid number of command line arguments");
            }

            bool isAsync = args[0] == "async";
            int numClients = int.Parse(args[1]);
            Task[] runTasks = new Task[numClients];

            for (var i = 0; i < numClients; i++)
            {

                var pipeName = string.Format(NamedPipeClientSync.AppServerListenPipe, i + 1);

                if (!isAsync)
                {
                    runTasks[i] = Task.Run(() => PerfAppServerSync.Run(pipeName));
                }
                else
                {
                    runTasks[i] = PerfAppServerAsync.RunAsync(pipeName);
                }
            }

            if (isAsync)
            {
                await Task.WhenAll(runTasks);
            }
            else
            {
                Task.WaitAll(runTasks);
            }

            Stop = DateTime.Now;

            // CLose the DataServer
            await NamedPipeClientAsync.SendAsync(NamedPipeClientSync.DataServerListenPipe, new Token(true)).ConfigureAwait(false);

            Console.WriteLine($"Clients: {numClients} Count: {(isAsync ? PerfAppServerAsync.ID : PerfAppServerSync.ID)} Elapsed Time: {(Stop.Value - Start.Value).TotalMilliseconds}");

#if !DEBUG
            File.AppendAllLines($@"..\Results.txt", new string[] { $"{(isAsync ? "Async" : "Sync")} {numClients} {(isAsync ? PerfAppServerAsync.ID : PerfAppServerSync.ID)} {(Stop.Value - Start.Value).TotalMilliseconds}" });
#endif

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
