using SyncContext.Lib;
using System;

namespace SyncContext.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var result = NoConfigureAwait.Increment(0).Result;

            Console.WriteLine($"Complete {result}");
            Console.ReadKey();

        }
    }
}
