using System;
using System.IO;
using System.IO.Compression;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.infrastructure.util
{
    public class FileCompression : ICompressionBase
    {
        private string _unCompressedFilePath = string.Empty,
                       _compressedFilePath = string.Empty;

        public FileCompression(string unCompressedFilePath, string compressedFilePath)
        {
            _unCompressedFilePath = unCompressedFilePath;
            _compressedFilePath = compressedFilePath;
        }

        /// <summary>
        /// method for compressing a single file into a zip file
        /// </summary>
        /// <param name="unCompressedFilePath">the file we're compressing</param>
        /// <param name="compressedFilePath">the output zip file</param>
        /// <returns></returns>
        /// public bool
        public object Compress(byte[] data = null)
        {
            try
            {
                //open the file to be compressed
                using (var inFile = File.OpenRead(_unCompressedFilePath))
                {
                    //create a new stream from the resulting zip file to be created
                    using (var outFile = File.Create(_compressedFilePath))
                    {
                        //now create a GZipStream object for writing the input file to
                        using (var compress = new GZipStream(outFile, CompressionMode.Compress, false))
                        {
                            //buffer array to hold the input file in
                            byte[] buffer = new byte[inFile.Length];

                            //read the file into the FileStream
                            int read = inFile.Read(buffer, 0, buffer.Length);

                            //now loop (as long as we have data) and
                            //write to the GZipStream
                            while (read > 0)
                            {
                                compress.Write(buffer, 0, read);
                                read = inFile.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
                return true;
            }
            catch (IOException ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return false;
        }

        public object Compress(string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// method for decompressing a zip file
        /// </summary>
        /// <param name="compressedFilePath">the zip file we're decompressing</param>
        /// <param name="unCompressedFilePath">the destination</param>
        /// <returns></returns>
        public object Decompress(string data)
        {
            try
            {
                //open a FileStream from the file we're decompressing
                using (var inStream = new FileStream(_compressedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //create a FileStream for the resulting output file
                    using (var outStream = new FileStream(_unCompressedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        //Open a GZipStream from the source zip file
                        using (var zipStream = new GZipStream(inStream, CompressionMode.Decompress, true))
                        {
                            //byte arrya the size of our input file
                            byte[] buffer = new byte[inStream.Length];
                            while (true)
                            {
                                int count = zipStream.Read(buffer, 0, buffer.Length);
                                if (count != 0)
                                    outStream.Write(buffer, 0, count);
                                if (count != buffer.Length)
                                    // have reached the end
                                    break;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                return false;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}