using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace tmg.equinox.generatecollateral
{
    public static class Helper
    {
        public static XElement GetCustomExcelParts(string strInputFilePath)
        {
            XElement rootElements;
            using (MemoryStream stream = GetStreamOfFile(strInputFilePath))
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    rootElements = GetCustomXmlPartsRootElement(wordDoc);
                }
            }
            return rootElements;
        }

        public static MemoryStream GetStreamOfFile(string strInputFilePath)
        {
            MemoryStream stream = new MemoryStream();
            if (File.Exists(strInputFilePath))
            {
                //Storing file content into byteArray
                byte[] byteArray;

                var fs = new FileStream(strInputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (var sr = new StreamReader(fs))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        sr.BaseStream.CopyTo(ms);
                        byteArray = ms.ToArray();
                    }
                }

                if (byteArray != null && byteArray.Length > 0)
                {
                    try
                    {
                        stream.Write(byteArray, 0, (int)byteArray.Length);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
                throw new FileNotFoundException();

            return stream;
        }

        private static XElement GetCustomXmlPartsRootElement(WordprocessingDocument wordDoc)
        {
            XElement rootElements = null;
            foreach (CustomXmlPart part in wordDoc.MainDocumentPart.CustomXmlParts)
            {
                XDocument objXDoc = XDocument.Load(XmlReader.Create(part.GetStream(FileMode.Open)));

                XElement objXMLFirstElement = objXDoc.Descendants("DataSourceXML").Elements("XML").FirstOrDefault();
                if (objXMLFirstElement != null)
                {
                    rootElements = XElement.Parse(objXMLFirstElement.Value.Trim());
                    break;
                }
            }
            return rootElements;
        }

        public static string RemoveContentControl(string strInputFilePath, string fileName)
        {   
            string strNewFileName = strInputFilePath;
            using (MemoryStream stream = GetStreamOfFile(strInputFilePath))
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    List<SdtBlock> sdtList = wordDoc.MainDocumentPart.Document.Descendants<SdtBlock>().ToList();
                    foreach (SdtBlock sdt in sdtList)
                    {
                        sdt.SdtContentBlock.RemoveAllChildren();
                        sdt.RemoveAllChildren();
                        sdt.Remove();
                    }
                }

                strNewFileName = System.IO.Path.GetDirectoryName(strInputFilePath) + "\\" + fileName
                                + DateTime.Now.ToString("yy-MM-dd-THH-mm-ss") + System.IO.Path.GetExtension(strInputFilePath);
                File.WriteAllBytes(strNewFileName, stream.ToArray());
            }

            if(strNewFileName != strInputFilePath)
                File.Delete(strInputFilePath);  

            return strNewFileName;
        }
    }

    public enum GenerateFormat
    {
        //Generate Collateral Document in Pdf format
        PdfFormat = 0,
        //Generate Collateral Document in Word format
        WordFormat = 1,
    }
    
}
