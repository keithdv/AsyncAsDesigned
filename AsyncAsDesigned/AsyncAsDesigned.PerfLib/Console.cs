using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsyncAsDesigned.PerfLib
{
    public static class ConsoleOutput
    {
        public static DateTime StartTime; // Lazy
        static object statusLock = new object();
        static List<string> status = new List<string>();

        public static void ConsoleWriteLine(string value)
        {

#if DEBUG
            lock (statusLock)
            {
                Console.WriteLine(value);
            }
#endif

        }

        public static void ConsoleWrite(string value)
        {

#if DEBUG
            lock (statusLock)
            {
                Console.Write(value);
            }
#endif

        }

        public static void UpdateStatus(Token t, string s)
        {

#if DEBUG
            if (!t.End)
            {
                lock (statusLock)
                {
                    while (t.AppServerID >= status.Count)
                    {
                        status.Add("_");
                    }

                    status[t.AppServerID] = s;

                    Console.BackgroundColor = ConsoleColor.Black;
                    ConsoleOutput.ConsoleWrite($"AppServer: ");

                    for (var i = 0; i < status.Count; i++)
                    {
                        var x = status[i];

                        switch (x)
                        {
                            case "R":
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                break;
                            case "D":
                                Console.BackgroundColor = ConsoleColor.Yellow;
                                break;
                            case "T":
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                            case "C":
                                Console.BackgroundColor = ConsoleColor.Blue;
                                break;
                            case "F":
                                Console.BackgroundColor = ConsoleColor.White;
                                break;
                            default:
                                Console.BackgroundColor = ConsoleColor.Black;
                                break;
                        }

                        if (i == t.AppServerID)
                        {
                            ConsoleOutput.ConsoleWrite(x);
                        }
                        else
                        {
                            ConsoleOutput.ConsoleWrite(" ");
                        }

                    }


                    Console.BackgroundColor = ConsoleColor.Black;

                    ConsoleOutput.ConsoleWrite($" Time: {(DateTime.Now - StartTime).TotalSeconds} Thread: {Thread.CurrentThread.ManagedThreadId}");
                    ConsoleOutput.ConsoleWriteLine(string.Empty);

                }
            }
#endif

        }

    }
}
