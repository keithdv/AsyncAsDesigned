using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class NoConfigureAwait
    {

        public static async Task<int> Increment(int count)
        {
            Debug.WriteLine($"NoConfigureAwait.Increment Start: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            var delayTask = Task.Delay(100);
            await delayTask;

            Debug.WriteLine($"NoConfigureAwait.Increment After Task.Delay: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(2000);

            Debug.WriteLine($"NoConfigureAwait.Increment After 2nd Task.Delay: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            return count + 1;
        }

    }
}
