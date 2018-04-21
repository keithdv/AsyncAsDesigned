//using System;
//using System.IO;
//using System.IO.Pipes;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AsyncAsDesigned.PerfLib
//{
//    public class NamedPipeServerTask : IDisposable
//    {

//        string pipeName;
//        object lockPipeServer = new object();
//        NamedPipeServerStream pipeServer;
//        static BinaryFormatter formatter = new BinaryFormatter();
//        bool isDisposed = false;

//        public delegate void TokenReceived(Token token);
//        public event TokenReceived TokenReceivedEvent;

//        public NamedPipeServerTask(string pipeName)
//        {
//            this.pipeName = pipeName;
//        }


//        public Task Start(bool oneMessage = false)
//        {


//            return Task.Run(() =>
//            {

//                Token token = null;

//                do
//                {
//                    lock (lockPipeServer)
//                    {
//                        pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
//                    }

//                    Task.Factory.FromAsync(pipeServer.BeginWaitForConnection, pipeServer.EndWaitForConnection, pipeServer)
//                        .ContinueWith(t =>
//                        {

//                            Received(t).ContinueWith(receivedToken =>
//                            {
//                                TokenReceivedEvent?.Invoke(receivedToken.Result);
//                            });

//                        }).Wait();


//                } while (!oneMessage && !isDisposed);

//            });

//        }

//        private static Task<Token> Received(Task t)
//        {
//            NamedPipeServerStream namedPipeServerStream = (NamedPipeServerStream)t.AsyncState;

//            int len = 0;

//            len = namedPipeServerStream.ReadByte() * 256;
//            len += namedPipeServerStream.ReadByte();
//            byte[] inBuffer = new byte[len];

//            return Task.Factory.FromAsync(namedPipeServerStream.BeginRead, namedPipeServerStream.EndRead, inBuffer, 0, len, null)
//                .ContinueWith((receviedTask) =>
//                {
//                    Token token;

//                    using (var mem = new MemoryStream(inBuffer))
//                    {
//                        token = (Token)formatter.Deserialize(mem);
//                    }

//                    namedPipeServerStream.Disconnect();
//                    namedPipeServerStream.Dispose();

//                    return token;
//                });

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
