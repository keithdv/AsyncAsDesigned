//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AsyncAsDesigned.PerfLib.Test
//{
//    [TestClass]
//    public class NamedPipeTests
//    {
//        string pipeName;
//        NamedPipeServer namedPipeServer;

//        [TestInitialize]
//        public void TestInitialize()
//        {
//            pipeName = Guid.NewGuid().ToString();
//            namedPipeServer = new NamedPipeServer(pipeName);
//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            namedPipeServer.Dispose();
//        }

//        [TestMethod]
//        public async Task SendReceive()
//        {

//            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
//            Token token = new Token();
//            Token result = null;

//            namedPipeServer.TokenReceivedEvent += (t) =>
//            {
//                result = t;
//                autoResetEvent.Set();
//            };

//            var task = Task.Run(() => namedPipeServer.Start());

//            NamedPipeClient.Send(pipeName, token);

//            autoResetEvent.WaitOne(1000);

//            //            namedPipeServer.Dispose(); // Important so that the async operation is allowed to finish

//            Assert.IsNotNull(result);
//            Assert.AreEqual(token.UniqueID, result.UniqueID);

//            namedPipeServer.Dispose();
//            await task;

//        }

//        [TestMethod]
//        public async Task SendReceiveMultiple()
//        {

//            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
//            Token[] token = new Token[] { new Token(), new Token() };
//            List<Token> result = new List<Token>();

//            namedPipeServer.TokenReceivedEvent += (t) =>
//            {
//                result.Add(t);
//                if (result.Count >= 2)
//                {
//                    autoResetEvent.Set();
//                }
//            };

//            var task = Task.Run(() => namedPipeServer.Start());

//            NamedPipeClient.Send(pipeName, token[0]);
//            NamedPipeClient.Send(pipeName, token[1]);

//            autoResetEvent.WaitOne(1000);

//            Assert.AreEqual(2, result.Count);
//            Assert.AreEqual(token[0].UniqueID, result[0].UniqueID);
//            Assert.AreEqual(token[1].UniqueID, result[1].UniqueID);

//            namedPipeServer.Dispose();
//            await task;

//        }

//        [TestMethod]
//        public async Task ReceiveOneMessage()
//        {
//            // Receive one message than stop

//            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
//            Token token = new Token();
//            Token result = null;
//            bool listenThreadCompleted = false;

//            namedPipeServer.TokenReceivedEvent += (t) =>
//            {
//                result = t;
//            };

//            var task = Task.Run(() =>
//            {
//                namedPipeServer.Start(true);
//                listenThreadCompleted = true;
//                autoResetEvent.Set();
//            });

//            NamedPipeClient.Send(pipeName, token);

//            autoResetEvent.WaitOne(1000);

//            Assert.IsTrue(listenThreadCompleted);

//            namedPipeServer.Dispose();
//            await task;

//        }
//    }
//}
