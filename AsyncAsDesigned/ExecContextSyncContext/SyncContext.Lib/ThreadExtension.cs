using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SyncContext.Lib
{
    public static class ThreadExtension
    {
        public static ExecutionContext ExecutionContext(this Thread thread)
        {
            // Thread.ExecutionContext gives you a copy
            // Need the actual instance
            //m_ExecutionContext;
            var ecField = typeof(Thread).GetField("m_ExecutionContext", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (ExecutionContext)ecField.GetValue(thread) ?? thread.ExecutionContext;

        }
    }
}
