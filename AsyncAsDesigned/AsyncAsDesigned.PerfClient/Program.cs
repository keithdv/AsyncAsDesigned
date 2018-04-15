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

            if (args.Length != 3)
            {
                throw new Exception("Invalid number of command line arguments");
            }

            var numToSend = int.Parse(args[0]);
            var clientNumber = int.Parse(args[1]);
            string uniquePipeName = args[2];

            var pipeName = NamedPipeClientSync.AppServerListenPipe(clientNumber, uniquePipeName.ToString());

            await PerfClient.RunAsync(numToSend, pipeName);

        }
    }
}
