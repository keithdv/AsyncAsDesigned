using AsyncAsDesigned.PerfLib;
using System;
using System.IO;
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

            var sendPipeName = NamedPipeClientSync.ClientToAppServerPipeName(clientNumber, uniquePipeName);
            var listenPipeName = NamedPipeClientSync.AppServerToClientPipeName(clientNumber, uniquePipeName);

            try
            {
                await PerfClient.RunAsync(numToSend, sendPipeName, listenPipeName);
            }
            catch (Exception ex)
            {
                File.AppendAllLines($@"..\Results.txt", new string[] { $"Client Error {sendPipeName} [{ex?.Message}]" });
                await Task.Delay(TimeSpan.FromDays(1)); // Hang and PowerShell script will Kill the process
            }
        }
    }
}

