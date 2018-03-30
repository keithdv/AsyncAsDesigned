//using System;
//using System.IO;
//using System.IO.Pipes;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AsyncAsDesigned.PerfLib
//{
//    public class NamedPipeServer : IDisposable
//    {

//        string pipeName;
//        object lockPipeServer = new object();
//        NamedPipeServerStream pipeServer;
//        static BinaryFormatter formatter = new BinaryFormatter();
//        bool isDisposed = false;

//        public delegate void TokenReceived(Token token);
//        public event TokenReceived TokenReceivedEvent;

//        public NamedPipeServer(string pipeName)
//        {
//            this.pipeName = pipeName;
//        }


//        public void Start(bool oneMessage = false)
//        {
//            Listen(oneMessage);
//        }

//        private void Listen(bool oneMessage = false)
//        {
//            lock (lockPipeServer)
//            {
//                pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
//            }

//            Task.Factory.FromAsync(pipeServer.BeginWaitForConnection, pipeServer.EndWaitForConnection, pipeServer)
//                .ContinueWith(t =>
//                {
//                    if (t.Exception == null)
//                    {
//                        Token token;
//                        lock (lockPipeServer)
//                        {
//                            token = Received(t);
//                        }

//                        TokenReceivedEvent?.Invoke(token);

//                        if (!oneMessage && !isDisposed) { Listen(); } // Recursive loop unless we only want to receive one message
//                    }
//                }).Wait();

//        }

//        private static Token Received(Task t)
//        {
//            NamedPipeServerStream namedPipeServerStream = (NamedPipeServerStream)t.AsyncState;

//            int len = 0;

//            len = namedPipeServerStream.ReadByte() * 256;
//            len += namedPipeServerStream.ReadByte();
//            byte[] inBuffer = new byte[len];
//            namedPipeServerStream.Read(inBuffer, 0, len);

//            Token token;

//            using (var mem = new MemoryStream(inBuffer))
//            {
//                token = (Token)formatter.Deserialize(mem);
//            }

//            namedPipeServerStream.Disconnect();
//            namedPipeServerStream.Dispose();

//            return token;


//        }

//        public void Dispose()
//        {
//            lock (lockPipeServer)
//            {
//                isDisposed = true;
//                if (pipeServer != null)
//                {
//                    pipeServer.Dispose();
//                }
//            }
//        }
//    }
//}
