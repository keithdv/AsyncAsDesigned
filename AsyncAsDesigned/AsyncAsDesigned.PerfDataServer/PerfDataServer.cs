using AsyncAsDesigned.PerfLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfDataServer
{
    public static class PerfDataServer
    {

        static object statusLock = new object();
        static string[] status;

        public static async Task RunAsync()
        {

            Console.WriteLine("DataServer");

            NamedPipeServerAsync listenToAppServer = new NamedPipeServerAsync(NamedPipeClient.DataServerListenPipe);

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            listenToAppServer.TokenReceivedEventAsync += (t) =>
            {
                lock (statusLock)
                {
                    if (status == null)
                    {
                        status = new string[t.Total];
                        for (var i = 0; i < status.Length; i++) { status[i] = "_"; }
                    }
                }

                UpdateStatus(t, "R");

                Task.Run(async () =>
                {
                    UpdateStatus(t, "T");
                    await Task.Delay(300);
                    UpdateStatus(t, "D");
                    await NamedPipeClient.SendAsync(t.DataServerToAppServer, t);
                    UpdateStatus(t, "F");
                });
                return Task.CompletedTask;
            };

            await listenToAppServer.StartAsync();

        }

        private static void UpdateStatus(Token t, string s)
        {
            lock (statusLock)
            {
                status[t.UniqueID] = s;
                Console.Write($"DataServer: {string.Concat(status.Take(t.UniqueID - 1))}");
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(status[t.UniqueID]);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(string.Concat(status.Skip(t.UniqueID + 1)));
                Console.WriteLine();
            }
        }
    }


}
