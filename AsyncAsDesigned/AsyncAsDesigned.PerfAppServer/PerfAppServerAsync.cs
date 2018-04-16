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
    public static class PerfAppServerAsync
    {

        public static int ID = 0;

        public static async Task RunAsync(string clientListenPipeName, string clientSendPipeName, string dataserverListenPipeName, string dataserverSendPipeName)
        {
            Console.WriteLine($"AppServer Async Start {clientListenPipeName}");

            using (NamedPipeServerAsync listenToClient = new NamedPipeServerAsync(clientListenPipeName))
            {
                using (NamedPipeServerAsync listenToDataServer = new NamedPipeServerAsync(dataserverListenPipeName))
                {

                    object lockId = new object();

                    listenToClient.TokenReceivedEventAsync += (t) =>
                    {
                        // Start the time when the first value comes in from the first client
                        if (!Program.Start.HasValue) { Program.Start = ConsoleOutput.StartTime = DateTime.Now; }

                        lock (lockId)
                        {
                            t.AppServerID = ID;
                            ID++;
                        }

                        ConsoleOutput.UpdateStatus(t, "R");

                        async Task Respond()
                        {

                            ConsoleOutput.UpdateStatus(t, "T");

                            await NamedPipeClientAsync.SendAsync(dataserverSendPipeName, t).ConfigureAwait(false);

                            ConsoleOutput.UpdateStatus(t, "D");



                        }

                        return Respond();

                    };

                    listenToDataServer.TokenReceivedEventAsync += async (t) =>
                    {
                        ConsoleOutput.UpdateStatus(t, "C");
                        await NamedPipeClientAsync.SendAsync(clientSendPipeName, t).ConfigureAwait(false);
                        ConsoleOutput.UpdateStatus(t, "F");
                    };

                    Task[] listenTasks = new Task[2];

                    listenTasks[0] = listenToClient.StartAsync();
                    listenTasks[1] = listenToDataServer.StartAsync();

                    await Task.WhenAll(listenTasks).ConfigureAwait(false);
                }
            }

            Console.WriteLine($"AppServer Async End {clientListenPipeName}");

        }




    }
}
