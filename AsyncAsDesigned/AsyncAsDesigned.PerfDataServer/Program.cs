using AsyncAsDesigned.PerfLib;
using System;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfDataServer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            if(args.Length != 2)
            {
                throw new Exception("Invalid number of command line args");
            }

            int clientID = int.Parse(args[0]);
            string uniquePipeName = args[1];

            var pipeName = NamedPipeClientSync.DataServerListenPipe(clientID, uniquePipeName);
            Console.WriteLine($"DataServer: {pipeName}");

            await PerfDataServer.RunAsync(pipeName);
        }
    }
}
