using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace tmg.equinox.integration.qhplite.Ver2016.XMLExport
{
    public class XmlExportWriter2016<T>
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

        public string GetXmlString(T xmlObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = false;
            settings.NewLineHandling = NewLineHandling.None;
            settings.NewLineOnAttributes = false;
            using (XmlWriter xw = XmlTextWriter.Create(sb, settings))
            {
                serializer.Serialize(xw, xmlObject);
                xw.Flush();
                xw.Close();
            }
            return sb.ToString();
        }
    }
}
