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

        public static async Task RunAsync(int numToSend, string sendToAppServerPipeName, string listenToAppServerPipeName)
        {

            object receivedLock = new object();
            int received = 0;

            Console.WriteLine($"Client Start {sendToAppServerPipeName}");

            status = new string[numToSend];

            for (var i = 0; i < status.Length; i++) { status[i] = "_"; }

            List<Task> sendTasks = new List<Task>();

            using (var listen = new NamedPipeServerAsync(listenToAppServerPipeName))
            {
                List<int> tokens = new List<int>();
                TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

                listen.TokenReceivedEventAsync += (t) =>
                {
                    UpdateStatus(t, "R");
                    if (!t.End)
                    {
                        lock (receivedLock)
                        {
                            received++;
                            tokens.Remove(t.ID);
                            if (tokens.Count == 0)
                            {
                                taskCompletionSource.SetResult(null);
                            }
                        }
                    }

                    return Task.CompletedTask;
                };


                var listenTask = listen.StartAsync();

                for (var i = 0; i < numToSend; i++)
                {

                    var token = new Token(i);
                    tokens.Add(token.ID);

                    UpdateStatus(token, "S");

                    sendTasks.Add(NamedPipeClientAsync.SendAsync(sendToAppServerPipeName, token));

                }


                await Task.WhenAll(sendTasks.ToArray());
                await taskCompletionSource.Task;

                Console.WriteLine($"Client Send End Token {sendToAppServerPipeName}");

                var endToken = new Token(true);
                await NamedPipeClientAsync.SendAsync(sendToAppServerPipeName, endToken).ConfigureAwait(false);
                await listenTask; // Wait for the End Token to be received and exit the pipe infinit loop


            }


            // Send Token.End = true token to shut down the AppServer (but not the DataServer in case there are multiple clients)

            if (received != numToSend) { throw new Exception($"Failure: Number sent {numToSend} Number received {received}"); }

            Console.WriteLine($"Client End {sendToAppServerPipeName}");

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
