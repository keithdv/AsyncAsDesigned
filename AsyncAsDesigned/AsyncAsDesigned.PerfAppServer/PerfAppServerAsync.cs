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

        public static async Task RunAsync(string pipeName)
        {
            Console.WriteLine($"Start AppServer Async {pipeName}");

            NamedPipeServerAsync listenToClient = new NamedPipeServerAsync(pipeName);

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


                    NamedPipeServerAsync listenToDataServer = new NamedPipeServerAsync(t.DataServerToAppServer);

                    listenToDataServer.TokenReceivedEventAsync += async (t2) =>
                    {
                        ConsoleOutput.UpdateStatus(t2, "C");
                        await NamedPipeClientAsync.SendAsync(t2.AppServerToClient, t2).ConfigureAwait(false);
                    };

                    await NamedPipeClientAsync.SendAsync(NamedPipeClientSync.DataServerListenPipe, t).ConfigureAwait(false);

                    ConsoleOutput.UpdateStatus(t, "D");

                    await listenToDataServer.StartAsync(true).ConfigureAwait(false);

                    ConsoleOutput.UpdateStatus(t, "F");

                }

                return Respond();

            };

            await listenToClient.StartAsync().ConfigureAwait(false);

            Console.WriteLine($"End AppServer {pipeName}");

        }




    }
}
