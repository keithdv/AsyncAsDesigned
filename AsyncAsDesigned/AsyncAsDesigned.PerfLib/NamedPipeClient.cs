using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfLib
{
    public static class NamedPipeClient
    {

        public const string AppServerListenPipe = @"\\AsyncAsDesigned\AppServerListenPipe";
        public const string DataServerListenPipe = @"\\AsyncAsDesigned\DataServerListenPipe";

        public static async Task SendAsync(string pipeName, Token token)
        {

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous))
            {

                await pipeClient.ConnectAsync().ConfigureAwait(false);

                SendToken(pipeClient, token);

                pipeClient.Flush();
                pipeClient.Close();

            }
        }

        private static void SendToken(NamedPipeClientStream pipeClient, Token token)
        {

            BinaryFormatter formatter = new BinaryFormatter();

            byte[] outBuffer = null;

            using (var mem = new MemoryStream())
            {
                formatter.Serialize(mem, token);
                outBuffer = mem.ToArray();
                mem.Flush();
            }

            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }

            pipeClient.WriteByte((byte)(len / 256));
            pipeClient.WriteByte((byte)(len & 255));
            pipeClient.Write(outBuffer, 0, len);

        }

    }
}
