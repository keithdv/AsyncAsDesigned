using AsyncAsDesigned.PerfLib;
using System;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfAppServer
{
    class Program
    {

        static async Task Main(string[] args)
        {

            //await PerfAppServer.RunAsync();

            PerfAppServer.Run();
            await Task.CompletedTask;

        }
    }
}
