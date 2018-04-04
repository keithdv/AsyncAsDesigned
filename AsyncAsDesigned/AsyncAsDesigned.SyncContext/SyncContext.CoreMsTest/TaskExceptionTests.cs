using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncContext.Lib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyncContext.CoreMsTest
{
    [TestClass]
    public class TaskExceptionTests
    {

        [TestMethod]
        public void TaskException_Method()
        {
            // Danger Point - No Exception Thrown!
            TaskException.Method();
        }

        [TestMethod]
        public async Task TaskException_MethodAsync()
        {

            Exception ex = null;

            try
            {
                await TaskException.MethodAsync();
            }
            catch (Exception ex2)
            {
                ex = ex2;
            }

            Assert.IsNotNull(ex);

        }

    }
}
