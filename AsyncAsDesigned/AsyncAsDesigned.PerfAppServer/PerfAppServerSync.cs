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

        public static void Run(int clientNumber, string clientListenPipeName, string clientSendPipeName, string dataserverListenPipeName, string dataserverSendPipeName)
        {
            Console.WriteLine($"AppServer Sync Start {clientNumber}");

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

                ConsoleOutput.UpdateStatus("AppServer Sync: ", t, "R"); // R - Message Received

                NamedPipeClientSync.Send(dataserverSendPipeName, t); // Blocks Thread until the message is sent to the data server

                ConsoleOutput.UpdateStatus("AppServer Sync: ", t, "D"); // D - Waiting for DataServer (DataServer purposefully delays)

            };

            listenToDataServer.TokenReceivedEvent += (t) =>
            {
                ConsoleOutput.UpdateStatus("AppServer Sync: ", t, "C"); // C - Respond to client
                NamedPipeClientSync.Send(clientSendPipeName, t); // Blocks Thread until the message is sent to the client
                ConsoleOutput.UpdateStatus("AppServer Sync: ", t, "F"); // F - Finished
            };


            Task[] listenTasks = new Task[2];

            listenTasks[0] = Task.Run(() => listenToDataServer.Start());
            listenTasks[1] = Task.Run(() => listenToClient.Start());

            Task.WaitAll(listenTasks);

            //Console.WriteLine($"AppServer Sync End {clientNumber}");

        }



    }
}
