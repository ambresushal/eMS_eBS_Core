using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;

namespace tmg.equinox.caching.Stores
{
    internal class CachingObjectSerializer : ICachingObjectSerializer
    {
        #region Public/ Protected / Private Member Variables
        private bool doBufferContent;        
        private IFormatter formatter = null;

        #endregion

        #region Constructor /Dispose

        public CachingObjectSerializer(bool bufferContent):this()
		{
            doBufferContent = bufferContent;
		}

        public CachingObjectSerializer()
        {
            formatter = new BinaryFormatter();
        }       
        #endregion Constructor

        #region Public Methods
        
        public Task SerializeAsync<T>(T cachingObject, Stream stream)
        {
            return Task.Run(() => { Serialize<T>(cachingObject, stream); });
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {          
            return Task.FromResult<T>(Deserialize<T>(stream)) as Task<T>;
        }  
        public void Serialize<T>(T cachingObject, Stream stream)
        {                      
            formatter.Serialize(stream, cachingObject);
        }

        public T Deserialize<T>(Stream stream)
        {            
           return (T)formatter.Deserialize(stream);
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods

        #region Helper Methods
        #endregion

    }
}
