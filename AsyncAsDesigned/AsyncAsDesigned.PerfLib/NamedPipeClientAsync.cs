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
    public static class NamedPipeClientAsync
    {

        public static async Task SendAsync(string pipeName, Token token)
        {

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous))
            {

                await pipeClient.ConnectAsync().ConfigureAwait(false);

                await SendTokenAsync(pipeClient, token).ConfigureAwait(false);

                pipeClient.Flush();
                pipeClient.Close();

            }
        }

        internal static async Task SendTokenAsync(Stream pipeClient, Token token)
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
            await pipeClient.WriteAsync(outBuffer, 0, len).ConfigureAwait(false);

        }

    }
}
