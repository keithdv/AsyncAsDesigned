using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncAsDesigned.PerfLib
{
    public static class ConsoleOutput
    {

        public static void ConsoleWriteLine(string value)
        {

#if DEBUG

            Console.WriteLine(value);

#endif

        }

        public static void ConsoleWrite(string value)
        {

#if DEBUG

            Console.Write(value);

#endif

        }

    }
}
