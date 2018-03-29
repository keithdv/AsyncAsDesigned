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
        public static async Task RunAsync()
        {
            Console.WriteLine("Client");

            await Task.Delay(1000);

            sw = new Stopwatch();

            sw.Start();

            int numToSend = 20;

            status = new string[numToSend];

            for (var i = 0; i < status.Length; i++) { status[i] = "_"; }

            List<Task> sendTasks = new List<Task>();

            for (var i = 0; i < numToSend; i++)
            {

                var token = new Token(i, numToSend);

                await NamedPipeClient.SendAsync(NamedPipeClient.AppServerListenPipe, token);

                UpdateStatus(token, "S");

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

            await Task.WhenAll(sendTasks.ToArray());

            sw.Stop();

            Console.WriteLine($"Completed {sw.Elapsed.Seconds} seconds");
            Console.ReadKey();

        }

        private static void UpdateStatus(Token t, string s)
        {
            lock (statusLock)
            {
                status[t.UniqueID] = s;
                Console.Write($"Client: {string.Concat(status.Take(t.UniqueID))}");
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(status[t.UniqueID]);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(string.Concat(status.Skip(t.UniqueID + 1)));
                Console.WriteLine();
            }
        }
    }
}
