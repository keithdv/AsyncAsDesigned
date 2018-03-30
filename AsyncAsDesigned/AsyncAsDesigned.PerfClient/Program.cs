using System;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await PerfClient.RunAsync();
        }
    }
}
