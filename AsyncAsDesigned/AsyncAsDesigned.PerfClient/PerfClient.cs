using AsyncAsDesigned.PerfLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfClient
{
    public static class PerfClient
    {
        static object statusLock = new object();
        static string[] status;
        static Stopwatch sw;
        public static async Task RunAsync(int numToSend)
        {
            Console.WriteLine("Client");

            await Task.Delay(1000);

            sw = new Stopwatch();

            sw.Start();

            status = new string[numToSend];

            for (var i = 0; i < status.Length; i++) { status[i] = "_"; }

            List<Task> sendTasks = new List<Task>();

            for (var i = 0; i < numToSend; i++)
            {

                var token = new Token(i);

                await NamedPipeClient.SendAsync(NamedPipeClient.AppServerListenPipe, token).ConfigureAwait(false);

                UpdateStatus(token, "S");

                // Question: Why isn't the client responding quicker??
                sendTasks.Add(Task.Run(async () =>
                {

                    UpdateStatus(token, "T");

                    var listen = new NamedPipeServerAsync(token.AppServerToClient);

                    listen.TokenReceivedEventAsync += (t) =>
                    {
                        UpdateStatus(token, "R");
                        return Task.CompletedTask;
                    };

                    await listen.StartAsync(true);

                }));
            }

            await Task.WhenAll(sendTasks.ToArray()).ConfigureAwait(false);

            sw.Stop();

            Console.WriteLine($"Completed {sw.Elapsed.Seconds} seconds");
            Console.ReadKey();

        }

        private static void UpdateStatus(Token t, string s)
        {
            lock (statusLock)
            {
                status[t.ID] = s;
                Console.Write($"Client: {string.Concat(status.Take(t.ID))}");
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(status[t.ID]);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(string.Concat(status.Skip(t.ID + 1)));
                Console.WriteLine();
            }
        }
    }
}
