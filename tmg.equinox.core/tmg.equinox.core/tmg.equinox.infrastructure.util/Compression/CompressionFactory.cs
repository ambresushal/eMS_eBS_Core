namespace tmg.equinox.infrastructure.util
{
    /// <summary>
    /// Using factory pattern.
    /// </summary>
    public static class CompressionFactory
    {
        public static ICompressionBase GetCompressionFactory(CompressionType type, byte[] data = null,
            string unCompressedFilePath = "", string compressedFilePath = "", string dataToBeCompressed = "")
        {
            switch (type)
            {
                case CompressionType.Memory:
                    return new MemoryCompression(data);
                case CompressionType.File:
                    return new FileCompression(unCompressedFilePath, compressedFilePath);
                case CompressionType.JSON:
                    return new JsonCompression();
            }
            return null;
        }
    }
}