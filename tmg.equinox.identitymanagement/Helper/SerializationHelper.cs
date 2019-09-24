using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.identitymanagement
{
    /// <summary>
    /// Provides the serialization abilities through binary formatter.
    /// </summary>
    public static class SerializationHelper
    {
        public static string SerializeWithBinaryFormatter<TObject>(TObject objectToBeSerialized)
        {            
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, objectToBeSerialized);
            byte[] serializedObject = memoryStream.GetBuffer();
            string userData = Convert.ToBase64String(serializedObject);
            return userData;           
        }

        public static TObject DeSerializeWithBinaryFormatter<TObject>(string objectToBeDeSerialized)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            byte[] byteArray = Convert.FromBase64String(objectToBeDeSerialized);

            MemoryStream serializationStream = new MemoryStream(byteArray);
            TObject returnObject = (TObject)formatter.Deserialize(serializationStream);
            return returnObject;
                    
        }
    }
}
