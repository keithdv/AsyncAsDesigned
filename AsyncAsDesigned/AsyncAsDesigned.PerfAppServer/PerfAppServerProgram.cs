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
            //ThreadPool.SetMinThreads(20, 20);

            if (args.Length != 3)
            {
                throw new Exception("Invalid number of command line arguments");
            }

            bool isAsync = args[0] == "async";
            int numClients = int.Parse(args[1]);
            string uniquePipeName = args[2];

            try
            {

                Task[] runTasks = new Task[numClients];

                for (var i = 0; i < numClients; i++)
                {
                    var clientToAppServerPipeName = NamedPipeClientSync.ClientToAppServerPipeName(i + 1, uniquePipeName);
                    var appServerToClientPipeName = NamedPipeClientSync.AppServerToClientPipeName(i + 1, uniquePipeName);
                    var appServerToDataServerPipeName = NamedPipeClientSync.AppServerToDataServerPipeName(i + 1, uniquePipeName);
                    var dataServerToAppServerPipeName = NamedPipeClientSync.DataServerToAppServerPipeName(i + 1, uniquePipeName);
                    var clientNum = i + 1;

                    if (!isAsync)
                    {
                        runTasks[i] = Task.Run(() => PerfAppServerSync.Run(clientNum, clientToAppServerPipeName, appServerToClientPipeName, dataServerToAppServerPipeName, appServerToDataServerPipeName));
                    }
                    else
                    {
                        runTasks[i] = PerfAppServerAsync.RunAsync(clientNum, clientToAppServerPipeName, appServerToClientPipeName, dataServerToAppServerPipeName, appServerToDataServerPipeName);
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
                //            await NamedPipeClientAsync.SendAsync(NamedPipeClientSync.DataServerListenPipe(uniquePipeName), new Token(true)).ConfigureAwait(false);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"AppServer {args[0]} Completed: Clients: {numClients} Count: {(isAsync ? PerfAppServerAsync.ID : PerfAppServerSync.Count)} Elapsed Time: {(Stop.Value - Start.Value).TotalMilliseconds}");
                Console.ResetColor();

#if !DEBUG
                File.AppendAllLines($@"..\Results.txt", new string[] { $"{(isAsync ? "Async" : "Sync")} {numClients} {(isAsync ? PerfAppServerAsync.ID : PerfAppServerSync.Count)} {(Stop.Value - Start.Value).TotalMilliseconds}" });
#endif

#if DEBUG
                Console.ReadKey();
#endif
            }
            catch (Exception ex)
            {
                File.AppendAllLines($@"..\Results.txt", new string[] { $"AppServer Error {numClients} [{ex?.Message}]" });
                await Task.Delay(TimeSpan.FromDays(1)); // Hang and PowerShell script will Kill the process
            }
        }
    }
}

