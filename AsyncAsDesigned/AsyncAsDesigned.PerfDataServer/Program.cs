using AsyncAsDesigned.PerfLib;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfDataServer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            if (args.Length != 2)
            {
                throw new Exception("Invalid number of command line args");
            }

            int clientID = int.Parse(args[0]);
            string uniquePipeName = args[1];

            var listenPipeName = NamedPipeClientSync.AppServerToDataServerPipeName(clientID, uniquePipeName);
            var sendPipeName = NamedPipeClientSync.DataServerToAppServerPipeName(clientID, uniquePipeName);

            Console.WriteLine($"DataServer Start: {listenPipeName}");
            try
            {
                await PerfDataServer.RunAsync(listenPipeName, sendPipeName);
            }
            catch (Exception ex)
            {
                File.AppendAllLines($@"..\Results.txt", new string[] { $"DataServer Error {sendPipeName} [{ex?.Message}]" });
                await Task.Delay(TimeSpan.FromDays(1)); // Hang and PowerShell script will Kill the process
            }
        }
    }
}
