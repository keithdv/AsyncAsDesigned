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
    public static class PerfAppServerSync
    {
        public static int ID = 0;



        public static void Run(string pipeName)
        {
            Console.WriteLine($"Start AppServer Sync {pipeName}");

            object lockId = new object();

            NamedPipeServerSync listenToClient = new NamedPipeServerSync(pipeName);

            listenToClient.TokenReceivedEvent += (t) =>
            {

                // Start the time when the first value comes in from the first client
                if (!Program.Start.HasValue) { Program.Start = ConsoleOutput.StartTime = DateTime.Now; }

                lock (lockId)
                {
                    t.AppServerID = ID;
                    ID++;
                }

                ConsoleOutput.UpdateStatus(t, "R"); // R - Message Received

                // Spawn a new thread with each incoming message
                // So that it's fast!!! Right?
                return Task.Run(() =>
                {
                    ConsoleOutput.UpdateStatus(t, "T"); // T - Thread Started

                    NamedPipeServerSync listenToDataServer = new NamedPipeServerSync(t.DataServerToAppServer);

                    // When the response is received from the dataserver
                    // response to the client
                    listenToDataServer.TokenReceivedEvent += (t2) =>
                    {
                        return Task.Run(() =>
                        {
                            ConsoleOutput.UpdateStatus(t, "C"); // C - Respond to client
                            NamedPipeClientSync.Send(t2.AppServerToClient, t); // Blocks Thread until the message is sent to the client
                        });
                    };

                    NamedPipeClientSync.Send(NamedPipeClientSync.DataServerListenPipe, t); // Blocks Thread until the message is sent to the data server
                    ConsoleOutput.UpdateStatus(t, "D"); // D - Waiting for DataServer (DataServer purposefully delays)

                    listenToDataServer.Start(true);

                    ConsoleOutput.UpdateStatus(t, "F"); // F - Finished

                });

            };

            listenToClient.Start();

            Console.WriteLine($"End AppServer {pipeName}");

        }



    }
}
