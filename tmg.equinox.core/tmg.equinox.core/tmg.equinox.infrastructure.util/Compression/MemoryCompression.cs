using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace tmg.equinox.infrastructure.util
{
    public class MemoryCompression : ICompressionBase
    {
        private byte[] _data;
        public MemoryCompression(byte[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Compress byte array using gzip.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public object Compress(byte[] data = null)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Compress string using gzip.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public object Compress(string data)
        {
            byte[] byteArrayToCompress = Encoding.ASCII.GetBytes(data);
            return Compress(byteArrayToCompress);
        }

        /// <summary>
        /// Decompression byte array using gzip.
        /// </summary>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public object Decompress(string data)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(_data), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
