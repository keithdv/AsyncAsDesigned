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
        public static int Count = 0;

        public static void Run(string clientListenPipeName, string clientSendPipeName, string dataserverListenPipeName, string dataserverSendPipeName)
        {
            Console.WriteLine($"AppServer Sync Start {clientListenPipeName}");

            object lockId = new object();

            NamedPipeServerSync listenToClient = new NamedPipeServerSync(clientListenPipeName);
            NamedPipeServerSync listenToDataServer = new NamedPipeServerSync(dataserverListenPipeName);

            listenToClient.TokenReceivedEvent += (t) =>
            {

                // Start the time when the first value comes in from the first client
                if (!Program.Start.HasValue) { Program.Start = ConsoleOutput.StartTime = DateTime.Now; }

                lock (lockId)
                {
                    Count++;
                    t.AppServerID = Count;
                }

                ConsoleOutput.UpdateStatus(t, "R"); // R - Message Received

                // Spawn a new thread with each incoming message
                // So that it's fast!!! Right?
                return Task.Run(() =>
                {
                    ConsoleOutput.UpdateStatus(t, "T"); // T - Thread Started

                    NamedPipeClientSync.Send(dataserverSendPipeName, t); // Blocks Thread until the message is sent to the data server
                    ConsoleOutput.UpdateStatus(t, "D"); // D - Waiting for DataServer (DataServer purposefully delays)

                });

            };

            listenToDataServer.TokenReceivedEvent += (t) =>
            {
                return Task.Run(() =>
                {
                    ConsoleOutput.UpdateStatus(t, "C"); // C - Respond to client
                    NamedPipeClientSync.Send(clientSendPipeName, t); // Blocks Thread until the message is sent to the client
                    ConsoleOutput.UpdateStatus(t, "F"); // F - Finished
                });
            };


            Task[] listenTasks = new Task[2];

            listenTasks[0] = Task.Run(() => listenToDataServer.Start());
            listenTasks[1] = Task.Run(() => listenToClient.Start());

            Task.WaitAll(listenTasks);

            Console.WriteLine($"AppServer Sync End {clientListenPipeName}");

        }



    }
}
