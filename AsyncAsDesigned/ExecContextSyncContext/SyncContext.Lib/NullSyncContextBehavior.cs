using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public class NullSyncContextBehavior
    {


        public int Count { get; private set; } = 0;

        public async Task Increment()
        {
            await Increment1();
        }

        public async Task Increment1()
        {
            await Increment2();
        }

        public async Task Increment2()
        {
            await Task.Delay(200);
            Count++;
        }

    }
}
