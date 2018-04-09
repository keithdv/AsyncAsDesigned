using AsyncAsDesigned.PerfLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Try to get multiple clients to start together
            await Task.Delay(3000 + (1000 - DateTime.Now.Millisecond));
            var start = DateTime.Now;

            if (args.Length != 2)
            {
                throw new Exception("Invalid number of command line arguments");
            }

            var numToSend = int.Parse(args[0]);
            var clientNumber = int.Parse(args[1]);

            await PerfClient.RunAsync(numToSend, $@"{NamedPipeClient.AppServerListenPipe}\{clientNumber}");

        }
    }
}
