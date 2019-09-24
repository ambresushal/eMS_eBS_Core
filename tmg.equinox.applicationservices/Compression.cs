using System.IO;
using System.IO.Compression;
using System.Text;

namespace tmg.equinox.applicationservices
{
    public class Compression
    {
        /// <summary>
        /// Compress data to byte array
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Compress JsonData string through ByteArray
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static byte[] CompressDataThroughByteArray(string data)
        {
            byte[] byteArrayToCompress = Encoding.ASCII.GetBytes(data);
            return Compress(byteArrayToCompress);
        }

        /// <summary>
        /// Decompress byte array to Json data
        /// </summary>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
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
    }
}