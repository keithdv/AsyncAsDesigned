using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncContext.CoreMsTest
{
    [TestClass]
    public class TaskCompletionSourceDemo
    {

        [TestMethod]
        public void TaskCompletionSourceDemo_AutoResetEvent()
        {
            var count = 0;
            AutoResetEvent canContinue = new AutoResetEvent(false);

            Timer timer = new Timer((o) =>
            {
                count++;
                if (count == 4)
                {
               
                    canContinue.Set();
                }
            }, null, 100, 100);

            canContinue.WaitOne();

            Assert.IsTrue(count == 4);

        }

        [TestMethod]
        public async Task TaskCompletionSourceDemo_TaskCompletionSource()
        {
            var count = 0;
            TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();

            Timer timer = new Timer((o) =>
            {
                count++;
                if (count == 4)
                {
                    taskCompletionSource.SetResult(4);
                }
            }, null, 100, 100);

            await taskCompletionSource.Task;

            Assert.IsTrue(count == 4);

        }
    }
}


