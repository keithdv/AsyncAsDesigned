using SyncContext.Lib;
using System;
using System.Threading.Tasks;

namespace SyncContext.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Which Exercise to run? (1 - )");
            int result;
            while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out result)) { Console.WriteLine("Invalid Entry"); }

            switch (result)
            {
                case 1:
                    // Exercise 1 - Baseline
                    await ExploreAsyncAwait.AsyncAwait_A(Output);
                    break;
                case 2:
                    // Exercise 2 - Task Debugger Window
                    await ExploreAsyncAwait.AsyncAwait_A(output: Output, pause: 10000);
                    break;
                case 3:
                    // Exercise 3 - Don't await
                    // Note the warning - Don't ignore these
                    ExploreAsyncAwait.AsyncAwait_A(Output);
                    break;
                case 4:
                    // Exercise 4 - Don't await, exception lost
                    ExploreAsyncAwait.ThrowException();
                    break;
                case 6:
                    // Exercise 4 - .Wait()
                    // Works
                    ExploreAsyncAwait.AsyncAwait_A(output: Output, pause: 5000).Wait();
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Incorrect");
                    Console.ReadKey();
                    break;
            }


        }

        private static void Output(string message)
        {
            Console.WriteLine(message);
        }
    }
}
