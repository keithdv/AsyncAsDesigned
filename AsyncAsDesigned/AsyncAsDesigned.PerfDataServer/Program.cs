using System;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfDataServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await PerfDataServer.RunAsync();
        }
    }
}
