﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncContext.Lib
{
    public static class ExploreAsyncAwait
    {

        // Capture ExecutionContextC so the we are able to use it in ExecutionContext.Run to
        // observe the behavior of the three AsyncLocals
        public static ExecutionContext executionContextC_Capture;

        public static ThreadLocal<string> threadLocalA = new ThreadLocal<string>();
        public static ThreadLocal<string> threadLocalB = new ThreadLocal<string>();
        public static ThreadLocal<string> threadLocalC = new ThreadLocal<string>();

        //// AsyncLocal is within a container held within ExecutionContext
        public static AsyncLocal<string> asyncLocalA = new AsyncLocal<string>();
        public static AsyncLocal<string> asyncLocalB = new AsyncLocal<string>();
        public static AsyncLocal<string> asyncLocalC = new AsyncLocal<string>();

        public static async Task AsyncAwait_A(Action<string> output, bool continueOnCapturedSynchronizationContext = true, int pause = 1000)
        {
            // Logical Execution 1

            // Demonstration Purposes Only!!
            // Very very dangerous!!!
            // ExecutionContext.SuppressFlow();

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            threadLocalA.Value = "A"; asyncLocalA.Value = "A";
            LogicalExecution(1, output);

            // equivalent to await AsyncAwait_B(continueOnCapturedSynchronizationContext, pause);
            // you normally don't need to interact with the created task
            await AsyncAwait_B(output, continueOnCapturedSynchronizationContext, pause).ConfigureAwait(continueOnCapturedContext: continueOnCapturedSynchronizationContext); // Location Execution 2

            // Logical Execution 9
            LogicalExecution(9, output);

            // Point: Why do asyncLocalB and asyncLocalC not have a value?? AsyncLocalA does.

        }

        private static async Task AsyncAwait_B(Action<string> output, bool continueOnCapturedSynchronizationContext, int pause = 1000)
        {

            // Logical Execution 3
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            threadLocalB.Value = "B"; asyncLocalB.Value = "B";
            LogicalExecution(3, output);

            await AsyncAwait_C(output, continueOnCapturedSynchronizationContext, pause).ConfigureAwait(continueOnCapturedContext: continueOnCapturedSynchronizationContext); // Location Execution 4

            // Logical Execution 8
            LogicalExecution(8, output);

        }

        private static async Task AsyncAwait_C(Action<string> output, bool continueOnCapturedSynchronizationContext, int pause = 1000)
        {

            // Logical Execution 5
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-MX");
            threadLocalC.Value = "C"; asyncLocalC.Value = "C";
            LogicalExecution(5, output);

            output("Task.Delay");
            // Break during Delay
            // View Task Debugger - You can see the 3 tasks
            // View Thread Window - AsyncAwait_C().Wait() - Main Thread stuck at .Wait()
            //                    - await AsyncAwait_C - Main Thread doing nothing - waiting for messages
            await Task.Delay(millisecondsDelay: pause).ConfigureAwait(continueOnCapturedContext: continueOnCapturedSynchronizationContext); // Location Execution 6

            // Logical Execution 7
            LogicalExecution(7, output);

            // Point: In WPF with ConfigureAwait(true) ThreadLocal has all the values (ABC)

            Debug.WriteLine($"Logical Exeuction 7: Synchronization.Current == null: {(SynchronizationContext.Current == null)}");

            executionContextC_Capture = ExecutionContext.Capture();

        }

        public static async Task<int> ThrowException()
        {
            throw new Exception("Failure!");
            await Task.FromResult<int>(1);
        }

        public static void LogicalExecution(int logicalExecutionPoint, Action<string> output)
        {
            output($"Logical Execution {logicalExecutionPoint}: ThreadID {System.Threading.Thread.CurrentThread.ManagedThreadId} ThreadLocal {threadLocalA.Value}{threadLocalB.Value}{threadLocalC.Value} AsyncLocal: {asyncLocalA.Value}{asyncLocalB.Value}{asyncLocalC.Value} Culture-Code : {Thread.CurrentThread.CurrentCulture.ToString()}");
        }

    }
}
