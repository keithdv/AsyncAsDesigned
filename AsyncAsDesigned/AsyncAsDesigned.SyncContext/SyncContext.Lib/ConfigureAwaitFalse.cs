using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class ConfigureAwaitFalse
    {

        public static async Task<int> Increment(int count)
        {
            Debug.WriteLine($"ConfigureAwaitFalse.Increment Start: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            var delayTask = Task.Delay(2000);
            await delayTask.ConfigureAwait(false);

            Debug.WriteLine($"ConfigureAwaitFalse.Increment After Task.Delay: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(2000).ConfigureAwait(false);

            Debug.WriteLine($"ConfigureAwaitFalse.NestedMethod1 After Task.Delay: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            return count + 1;
        }

    }
}
