using System;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class NoConfigureAwait
    {

        public static async Task<int> Increment(int count)
        {
            return await NestedMethod1(count);
        }

        private static async Task<int> NestedMethod1(int count)
        {
            return await NestedMethod2(count);
        }

        private static async Task<int> NestedMethod2(int count)
        {
            await Task.Delay(1000);
            return count + 1;
        }

    }
}
