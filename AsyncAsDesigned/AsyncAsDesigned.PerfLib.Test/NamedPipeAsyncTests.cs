using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfLib.Test
{
    [TestClass]
    public class NamedPipeAsyncTests
    {
        string pipeName;
        NamedPipeServerAsync namedPipeServer;

        [TestInitialize]
        public void TestInitialize()
        {
            pipeName = Guid.NewGuid().ToString();
            namedPipeServer = new NamedPipeServerAsync(pipeName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            namedPipeServer.Dispose();
        }

        [TestMethod]
        public async Task SendReceive()
        {

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Token token = new Token(1, 1);
            Token result = null;

            namedPipeServer.TokenReceivedEventAsync += (t) =>
            {
                result = t;
                autoResetEvent.Set();
                return Task.CompletedTask;
            };

            var task = Task.Run(() => namedPipeServer.StartAsync());

            await NamedPipeClient.SendAsync(pipeName, token);

            autoResetEvent.WaitOne(1000);

            Assert.IsNotNull(result);
            Assert.AreEqual(token.UniqueID, result.UniqueID);

            namedPipeServer.Dispose();
            await task; // wait for the thread to finished. 

        }

        [TestMethod]
        public async Task SendReceiveMultiple()
        {

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Token[] token = new Token[] { new Token(1, 2), new Token(2, 2) };
            List<Token> result = new List<Token>();

            namedPipeServer.TokenReceivedEventAsync += (t) =>
            {
                result.Add(t);
                if (result.Count >= 2)
                {
                    autoResetEvent.Set();
                }
                return Task.CompletedTask;
            };

            var task = Task.Run(() => namedPipeServer.StartAsync());


            await NamedPipeClient.SendAsync(pipeName, token[0]).ConfigureAwait(false);
            await NamedPipeClient.SendAsync(pipeName, token[1]).ConfigureAwait(false);

            autoResetEvent.WaitOne(1000);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(token[0].UniqueID, result[0].UniqueID);
            Assert.AreEqual(token[1].UniqueID, result[1].UniqueID);

            namedPipeServer.Dispose();
            await task.ConfigureAwait(false);

        }


        [TestMethod]
        public async Task SendReceiveMultiple_TaskCompletionSource()
        {

            Token[] token = new Token[] { new Token(1, 2), new Token(2, 2) };
            List<Token> result = new List<Token>();
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            namedPipeServer.TokenReceivedEventAsync += (t) =>
            {
                result.Add(t);
                if (result.Count >= 2)
                {
                    taskCompletionSource.SetResult(true);
                }
                return Task.CompletedTask;
            };

            var task = Task.Run(() => namedPipeServer.StartAsync());


            await NamedPipeClient.SendAsync(pipeName, token[0]);
            await NamedPipeClient.SendAsync(pipeName, token[1]);

            await taskCompletionSource.Task;

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(token[0].UniqueID, result[0].UniqueID);
            Assert.AreEqual(token[1].UniqueID, result[1].UniqueID);

            namedPipeServer.Dispose();
            await task;

        }

        [TestMethod]
        public async Task ReceiveOneMessage()
        {
            // Receive one message than stop

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Token token = new Token(1, 2);
            Token result = null;
            bool listenThreadCompleted = false;

            namedPipeServer.TokenReceivedEventAsync += (t) =>
            {
                result = t;
                return Task.CompletedTask;
            };

            var task = Task.Run(async () =>
            {
                await namedPipeServer.StartAsync(true);
                listenThreadCompleted = true;
                autoResetEvent.Set();
            });

            await NamedPipeClient.SendAsync(pipeName, token);

            autoResetEvent.WaitOne(1000);

            Assert.IsTrue(listenThreadCompleted);

            namedPipeServer.Dispose();
            await task;

        }
    }
}
