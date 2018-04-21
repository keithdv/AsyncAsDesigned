using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class TaskExtensions
    {
        public static ExecutionContext ExecutionContext(this Task task)
        {
            // Task.ExecutionContext
            // Demonstration Purposes Only

            var ecProp = typeof(Task).GetProperty("CapturedContext", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (ExecutionContext) ecProp?.GetValue(task) ?? throw new Exception("Unable to read CapturedContext property");
        }
    }
}
