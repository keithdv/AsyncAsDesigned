using AsyncAsDesigned.PerfLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfAppServer
{
    public static class PerfAppServer
    {

        static object statusLock = new object();
        static List<string> status = new List<string>();
        static Stopwatch sw = new Stopwatch();


        public static void Run()
        {
            Console.WriteLine("AppServer");
            object lockId = new object();
            int id = 0;
            
            NamedPipeServerAsync listenToClient = new NamedPipeServerAsync(NamedPipeClient.AppServerListenPipe);

            listenToClient.TokenReceivedEventAsync += (t) =>
            {

                if (!sw.IsRunning) { sw.Start(); }

                lock (lockId)
                {
                    t.AppServerID = id;
                    id++;
                }

                UpdateStatus(t, "R"); // R - Message Received

                // Spawn a new thread with each incoming message
                // So that it's fast!!! Right?
                Task.Run(() =>
                {

                    UpdateStatus(t, "T"); // T - Thread Started

                    NamedPipeServerAsync listenToDataServer = new NamedPipeServerAsync(t.DataServerToAppServer);

                    // When the response is received from the dataserver
                    // response to the client
                    listenToDataServer.TokenReceivedEventAsync += (t2) =>
                    {
                        UpdateStatus(t, "C"); // C - Respond to client
                        NamedPipeClient.Send(t2.AppServerToClient, t); // Blocks Thread until the message is sent to the client
                        return Task.CompletedTask;
                    };

                    NamedPipeClient.Send(NamedPipeClient.DataServerListenPipe, t); // Blocks Thread until the message is sent to the data server
                    UpdateStatus(t, "D"); // D - Waiting for DataServer (DataServer purposefully delays)

                    listenToDataServer.Start(true); // Blocks thread until the DataServer responds
                    UpdateStatus(t, "F"); // F - Finished
                });

                return Task.CompletedTask;

            };

            listenToClient.Start(); // Inifinte loop waiting for the client

        }

        public static async Task RunAsync()
        {
            Console.WriteLine("AppServer");

            NamedPipeServerAsync listenToClient = new NamedPipeServerAsync(NamedPipeClient.AppServerListenPipe);

            object lockId = new object();
            int id = 0;

            listenToClient.TokenReceivedEventAsync += (t) =>
            {

                if (!sw.IsRunning) { sw.Start(); }

                lock (lockId)
                {
                    t.AppServerID = id;
                    id++;
                }

                UpdateStatus(t, "R");

                Task.Run(async () =>
                {

                    UpdateStatus(t, "T");

                    NamedPipeServerAsync listenToDataServer = new NamedPipeServerAsync(t.DataServerToAppServer);

                    listenToDataServer.TokenReceivedEventAsync += async (t2) =>
                    {
                        UpdateStatus(t2, "C");
                        await NamedPipeClient.SendAsync(t2.AppServerToClient, t2).ConfigureAwait(false);
                    };

                    await NamedPipeClient.SendAsync(NamedPipeClient.DataServerListenPipe, t).ConfigureAwait(false);
                    UpdateStatus(t, "D");

                    await listenToDataServer.StartAsync(true).ConfigureAwait(false);
                    UpdateStatus(t, "F");
                });

                return Task.CompletedTask;
            };

            await listenToClient.StartAsync().ConfigureAwait(false);

        }

        private static void UpdateStatus(Token t, string s)
        {
            lock (statusLock)
            {
                while (t.AppServerID >= status.Count)
                {
                    status.Add("_");
                }

                status[t.AppServerID] = s;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write($"AppServer: ");

                for (var i = 0; i < status.Count; i++)
                {
                    var x = status[i];

                    switch (x)
                    {
                        case "R":
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            break;
                        case "D":
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case "T":
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case "C":
                            Console.BackgroundColor = ConsoleColor.Blue;
                            break;
                        case "F":
                            Console.BackgroundColor = ConsoleColor.White;
                            break;
                        default:
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                    }

                    if (i == t.AppServerID)
                    {
                        Console.Write(x);
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                }


                Console.BackgroundColor = ConsoleColor.Black;

                Console.Write($" Time: {sw.Elapsed.TotalSeconds} Thread: {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine();
            }
        }

    }
}
