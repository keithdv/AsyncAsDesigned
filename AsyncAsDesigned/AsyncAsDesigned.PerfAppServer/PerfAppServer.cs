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

                UpdateStatus(t, "R");

                Task.Run(() =>
                {

                    UpdateStatus(t, "T");

                    NamedPipeServerAsync listenToDataServer = new NamedPipeServerAsync(t.DataServerToAppServer);

                    listenToDataServer.TokenReceivedEventAsync += (t2) =>
                    {
                        UpdateStatus(t, "C");
                        NamedPipeClient.SendAsync(t2.AppServerToClient, t).Wait();
                        return Task.CompletedTask;
                    };

                    NamedPipeClient.SendAsync(NamedPipeClient.DataServerListenPipe, t).Wait();
                    UpdateStatus(t, "D");

                    listenToDataServer.StartAsync(true).Wait();
                    UpdateStatus(t, "F");
                });

                return Task.CompletedTask;

            };

            listenToClient.StartAsync().Wait();

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
