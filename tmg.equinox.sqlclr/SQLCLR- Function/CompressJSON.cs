using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.IO;
using System.IO.Compression;
using System.Text;
public partial class UserDefinedFunctions
{
    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static string GZip(string formData)
    {
        string ZipIt = null;
        if (!string.IsNullOrEmpty(formData))
        {
            byte[] buffer = Encoding.UTF8.GetBytes(formData);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }

                ms.Position = 0;
                byte[] compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);

                byte[] gzBuffer = new byte[compressed.Length + 4];
                System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
                ZipIt = Convert.ToBase64String(gzBuffer);
            }
        }
        return ZipIt;
    }

    [SqlFunction(IsDeterministic = true, IsPrecise = true)]
    public static string UnZip(string compressedText)
    {
        String UnZipIt = null;
        if (!string.IsNullOrEmpty(compressedText))
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                UnZipIt = Encoding.UTF8.GetString(buffer);
            }
        }
        return UnZipIt;
    }


}
