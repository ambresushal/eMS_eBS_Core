using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching.Interfaces
{
    internal interface ICachingObjectSerializer
    {
       
        Task<T> DeserializeAsync<T>(Stream stream);
        Task SerializeAsync<T>(T cachingObject, Stream stream);
        void Serialize<T>(T cachingObject, Stream stream);
        T Deserialize<T>(Stream stream);       

    }
}
