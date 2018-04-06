using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncContext.Lib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyncContext.CoreMsTest
{

    [TestClass]
    public class NullSyncContextBehaviorTests
    {

        [TestMethod]
        public async Task NullSyncContextBehavior_Null()
        {
            var nsc = new NullSyncContextBehavior();

            Task[] tasks = new Task[100];

            for(var i = 0; i < 100; i++)
            {
                tasks[i] = nsc.Increment();
            }

            await Task.WhenAll(tasks);

            Assert.AreEqual(100, nsc.Count);

        }



    }
}
