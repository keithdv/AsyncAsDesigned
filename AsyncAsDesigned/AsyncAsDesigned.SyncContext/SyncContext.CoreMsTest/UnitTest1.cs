using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncContext.Lib;
using System.Threading.Tasks;

namespace SyncContext.CoreMsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NoConfigureAwait_Wait()
        {

            var result = NoConfigureAwait.Increment(0).Result;

            Assert.AreEqual(1, result);

        }
    }
}
