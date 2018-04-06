using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class TaskException
    {


        public static void Method()
        {
            // Very Bad! Exceptions will be lost
            // Do not do this
            // await or .Result/.Wait()
            NestedMethod();
        }

        public static async Task MethodAsync()
        {
            await NestedMethod();
        }


        private static async Task NestedMethod()
        {
            await Task.Delay(1000);
            throw new Exception();
        }

    }
}
