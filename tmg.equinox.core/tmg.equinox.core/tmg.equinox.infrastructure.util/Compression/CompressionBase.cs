using System;

namespace tmg.equinox.infrastructure.util
{
    public interface ICompressionBase : IDisposable
    {
        object Compress(byte[] data = null);

        object Compress(string data = "");

        object Decompress(string data = "");

    }
}