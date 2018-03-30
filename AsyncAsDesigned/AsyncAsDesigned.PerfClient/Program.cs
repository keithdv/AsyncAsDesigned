using System;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if(args.Length < 1 || !int.TryParse(args[0], out int result))
            {
                result = 5;
            }

            await PerfClient.RunAsync(result);
        }
    }
}
