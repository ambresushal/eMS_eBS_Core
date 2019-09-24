using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace tmg.equinox.integration.domain.services
{
    public class FileConvertor<T> where T: class
    {
        public string CSV(List<T> entities)
        {
            StringBuilder sb = new StringBuilder();

            PropertyInfo[] propInfos = typeof(T).GetProperties();
            for (int i = 0; i <= propInfos.Length - 1; i++)
            {
                sb.Append(propInfos[i].Name);

                if (i < propInfos.Length - 1)
                {
                    sb.Append("|");
                }
            }

            sb.AppendLine();

            for (int i = 0; i <= entities.Count - 1; i++)
            {
                T item = entities[i];
                for (int j = 0; j <= propInfos.Length - 1; j++)
                {
                    object o = item.GetType().GetProperty(propInfos[j].Name).GetValue(item, null);
                    if (o != null)
                    {
                        string value = o.ToString();

                        if (value.Contains("|"))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }

                        sb.Append(value);
                    }

                    if (j < propInfos.Length - 1)
                    {
                        sb.Append("|");
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string XML(List<T> entities)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
                xmlSerializer.Serialize(stringWriter, entities);
                return stringWriter.ToString();
            }
        }
    }
}
