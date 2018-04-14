using AsyncAsDesigned.PerfLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfClient
{
    public static class PerfClient
    {
        static object statusLock = new object();
        static string[] status;

        public static async Task RunAsync(int numToSend, string appServerPipeName)
        {

            object receivedLock = new object();
            int received = 0;

            Console.WriteLine($"Start Client {appServerPipeName}");

            status = new string[numToSend];

            for (var i = 0; i < status.Length; i++) { status[i] = "_"; }

            List<Task> sendTasks = new List<Task>();

            async Task Listen(Token token)
            {

                var listen = new NamedPipeServerAsync(token.AppServerToClient);

                listen.TokenReceivedEventAsync += (t) =>
                {
                    UpdateStatus(token, "R");
                    lock (receivedLock)
                    {
                        received++;
                    }
                    return Task.CompletedTask;
                };

                await listen.StartAsync(true).ConfigureAwait(false);

            }


            for (var i = 0; i < numToSend; i++)
            {

                var token = new Token(i);

                await NamedPipeClientAsync.SendAsync(appServerPipeName, token).ConfigureAwait(false);

                UpdateStatus(token, "S");

                sendTasks.Add(Listen(token));

            }

            await Task.WhenAll(sendTasks.ToArray()).ConfigureAwait(false);

            if (received != numToSend) { throw new Exception($"Failure: Number sent {numToSend} Number received {received}"); }

            // Send Token.End = true token to shut down the AppServer (but not the DataServer in case there are multiple clients)
            await NamedPipeClientAsync.SendAsync(appServerPipeName, new Token(true)).ConfigureAwait(false);

            Exception ex = null;

            Console.WriteLine($"End Client {appServerPipeName}");

        }

        private static void UpdateStatus(Token t, string s)
        {
#if DEBUG
            lock (statusLock)
            {
                status[t.ID] = s;
                ConsoleOutput.ConsoleWrite($"Client: {string.Concat(status.Take(t.ID))}");
                Console.BackgroundColor = ConsoleColor.White;
                ConsoleOutput.ConsoleWrite(status[t.ID]);
                Console.BackgroundColor = ConsoleColor.Black;
                ConsoleOutput.ConsoleWrite(string.Concat(status.Skip(t.ID + 1)));
                ConsoleOutput.ConsoleWriteLine(string.Empty);
            }

#endif

        }
    }
}
