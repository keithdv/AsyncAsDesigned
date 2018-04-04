using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class TaskException
    {


        public static async void Method()
        {
            // Bad!
            await NestedMethod();
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
