using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfLib
{
    public class NamedPipeServerAsync : IDisposable
    {

        string pipeName;
        static BinaryFormatter formatter = new BinaryFormatter();

        object lockCancel = new object();
        private CancellationTokenSource cancelWaitForConnection;
        bool closed = false;

        public delegate Task TokenReceivedAsync(Token token);
        public event TokenReceivedAsync TokenReceivedEventAsync;

        public NamedPipeServerAsync(string pipeName)
        {
            this.pipeName = pipeName;
        }


        public async Task StartAsync(bool oneMessage = false)
        {

            Token token = null;
            List<Task> tokenReceivedEventTasks = new List<Task>();

            do
            {
                using (var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                    lock (lockCancel)
                    {
                        if (closed) { return; } // Comment this out, run all tests, SendReceiveMultiple blocks. 
                        cancelWaitForConnection = new CancellationTokenSource();
                    }

                    try
                    {
                        await pipeServer.WaitForConnectionAsync(cancelWaitForConnection.Token).ConfigureAwait(false);
                    }
                    catch (System.OperationCanceledException) { closed = true; } // Thrown when CancallationTokenSource.Cancel() is called

                    lock (lockCancel)
                    {
                        cancelWaitForConnection = null;
                    }

                    if (!closed)
                    {
                        token = await Received(pipeServer).ConfigureAwait(false);
                    }

                    //NamedPipeClient.SendToken(pipeServer, token);

                    pipeServer.Close();

                }

                if (token != null && !token.End)
                {
                    // We do not await - Don't wait for the task to complete before waiting for another message
                    tokenReceivedEventTasks.Add((TokenReceivedEventAsync?.Invoke(token) ?? Task.CompletedTask));
                }
            }
            while (!(token?.End ?? false) && !oneMessage && !closed);

            // Don't exit the method though until all of the tasks have completed
            await Task.WhenAll(tokenReceivedEventTasks);

        }

        private static async Task<Token> Received(NamedPipeServerStream namedPipeServerStream)
        {
            int len = 0;

            len = namedPipeServerStream.ReadByte() * 256;
            len += namedPipeServerStream.ReadByte();
            byte[] inBuffer = new byte[len];
            await namedPipeServerStream.ReadAsync(inBuffer, 0, len).ConfigureAwait(false);

            Token token;

            using (var mem = new MemoryStream(inBuffer))
            {
                token = (Token)formatter.Deserialize(mem);
            }

            return token;


        }

        public void Dispose()
        {
            lock (lockCancel)
            {
                closed = true;
                if (cancelWaitForConnection != null)
                {
                    cancelWaitForConnection.Cancel();
                }
            }
        }
    }
}
