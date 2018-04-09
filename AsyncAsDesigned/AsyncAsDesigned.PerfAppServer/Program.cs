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

            if (args.Length != 2)
            {
                throw new Exception("Invalid number of command line arguments");
            }

            // Run() fails - not enough threads to repond to the client
            //ThreadPool.SetMaxThreads(4, 4);

            bool isAsync = args[0] == "async";
            int numClients = int.Parse(args[1]);
            Task[] runTasks = new Task[numClients];

            for (var i = 0; i < numClients; i++)
            {
                var pipeName = $@"{NamedPipeClient.AppServerListenPipe}\{i + 1}";

                if (!isAsync)
                {
                    runTasks[i] = Task.Run(() => PerfAppServer.Run(pipeName));
                }
                else
                {
                    runTasks[i] = Task.Run(() => PerfAppServer.RunAsync(pipeName));
                }
            }

            await Task.WhenAll(runTasks);

            Stop = DateTime.Now;

            // CLose the DataServer
            await NamedPipeClient.SendAsync(NamedPipeClient.DataServerListenPipe, new Token(true)).ConfigureAwait(false);

            Console.WriteLine($"Clients: {numClients} Count: {PerfAppServer.ID} Elapsed Time: {(Stop.Value - Start.Value).TotalMilliseconds}");

            File.AppendAllLines($@"..\Results.txt", new string[] { $"{(isAsync ? "Async" : "Sync")} {numClients} {PerfAppServer.ID} {(Stop.Value - Start.Value).TotalMilliseconds}" });

        }
    }
}
