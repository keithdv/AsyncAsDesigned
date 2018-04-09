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

            async Task Listen(Token token)
            {

                var listen = new NamedPipeServerAsync(token.AppServerToClient);

                listen.TokenReceivedEventAsync += (t) =>
                {
                    UpdateStatus(token, "R");
                    return Task.CompletedTask;
                };

                await listen.StartAsync(true).ConfigureAwait(false);

            }


            for (var i = 0; i < numToSend; i++)
            {

                var token = new Token(i);

                await NamedPipeClient.SendAsync(NamedPipeClient.AppServerListenPipe, token).ConfigureAwait(false);

                UpdateStatus(token, "S");

                sendTasks.Add(Task.Run(() => Listen(token)));

            }

            await Task.WhenAll(sendTasks.ToArray()).ConfigureAwait(false);

            sw.Stop();

            await NamedPipeClient.SendAsync(NamedPipeClient.AppServerListenPipe, new Token(true)).ConfigureAwait(false);

            Console.WriteLine($"{sw.Elapsed.TotalSeconds}");
            //Console.ReadKey();

            File.AppendAllLines(@"..\Results.txt", new string[] { $"{numToSend}  {sw.Elapsed.TotalMilliseconds}" });

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
