using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tmg.equinox.integration.qhplite.Ver2016.XMLExport
{
    public class XmlExportWriter<T>
    {

        public void SaveXml(T xmlObject, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            // To write to a file, create a StreamWriter object.
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, xmlObject);
                writer.Close();
            }
        }
    }
}
