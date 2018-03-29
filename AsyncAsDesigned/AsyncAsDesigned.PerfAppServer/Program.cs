using AsyncAsDesigned.PerfLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfAppServer
{
    class Program
    {

        static async Task Main(string[] args)
        {
            // Run() fails - not enough threads to repond to the client
            //ThreadPool.SetMaxThreads(4, 4);
            

            await PerfAppServer.RunAsync();

            PerfAppServer.Run();
            await Task.CompletedTask;

        }
    }
}
