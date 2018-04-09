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
        static List<string> status = new List<string>();

        public static async Task RunAsync()
        {

            ConsoleOutput.ConsoleWriteLine("DataServer");

            NamedPipeServerAsync listenToAppServer = new NamedPipeServerAsync(NamedPipeClient.DataServerListenPipe);

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            listenToAppServer.TokenReceivedEventAsync += (t) =>
            {

                UpdateStatus(t, "R");

                async Task respond()
                {
                    UpdateStatus(t, "T");
                    await Task.Delay(300).ConfigureAwait(false);
                    UpdateStatus(t, "D");
                    await NamedPipeClient.SendAsync(t.DataServerToAppServer, t).ConfigureAwait(false);
                    UpdateStatus(t, "F");
                };

                Task.Run(() => respond());

                return Task.CompletedTask;

            };

            await listenToAppServer.StartAsync().ConfigureAwait(false);

        }

        private static void UpdateStatus(Token t, string s)
        {
#if DEBUG
            lock (statusLock)
            {
                while(t.AppServerID >= status.Count)
                {
                    status.Add("_");
                }

                status[t.AppServerID] = s;

                ConsoleOutput.ConsoleWrite($"DataServer: {string.Concat(status.Take(t.AppServerID - 1))}");
                Console.BackgroundColor = ConsoleColor.White;
                ConsoleOutput.ConsoleWrite(status[t.AppServerID]);
                Console.BackgroundColor = ConsoleColor.Black;
                ConsoleOutput.ConsoleWrite(string.Concat(status.Skip(t.AppServerID + 1)));
                ConsoleOutput.ConsoleWriteLine(string.Empty);
            }
#endif
        }
    }


}
