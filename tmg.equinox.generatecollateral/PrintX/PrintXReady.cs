using Spire.Doc;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Doc.Fields;
using Spire.Doc.Documents;
using Hangfire.Logging;
using tmg.equinox.setting.Interface;
using tmg.equinox.applicationservices.interfaces;
using System.Reflection;

namespace tmg.equinox.generatecollateral
{
    public class PrintXReady
    {
        string _wordFileName;
        string _outputPath;
        private static readonly ILog _logger = LogProvider.For<PrintXReady>();
        private bool isFeatureEnable = false;
        ISettingManager _settingManager;
        const string CONST_DocCompliance = "DocCompliance";
        const string CONST_Pdf_Print_X_ON_OFF = "tmg.equinox.net.printx.OnOff";
        private ICollateralService _collateralService;
        private CollateralImageManager _collateralImageManager;
        ComplianceDetailInfo _complianceDetailInfo;
        public PrintXReady(string outputPath, string wordFileName, ISettingManager settingManager, ICollateralService collateralService,string licPath="", ComplianceDetailInfo complianceDetailInfo=null)
        {
            _wordFileName = wordFileName;
            _outputPath = outputPath;
            registerKey(licPath);
            _settingManager = settingManager;
            var value = _settingManager.GetSettingValue(CONST_DocCompliance, CONST_Pdf_Print_X_ON_OFF);
            if (value.ToUpper() == "ON")
                isFeatureEnable = true;
            _collateralImageManager = new  CollateralImageManager(collateralService);
            _complianceDetailInfo = complianceDetailInfo;
        }

        private void registerKey(string licPath)
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (!string.IsNullOrEmpty(licPath))
            {
                path = licPath;
            }
            var licenseFile = new FileInfo(string.Format("{0}\\Spire.PDF_license.elic.xml", path));
            Spire.License.LicenseProvider.SetLicenseFile(licenseFile);
            Spire.License.LicenseProvider.LoadLicense();
            var licenseFile2 = new FileInfo(string.Format("{0}\\Spire.Doc_license.elic.xml",path));
            Spire.License.LicenseProvider.SetLicenseFile(licenseFile2);
            Spire.License.LicenseProvider.LoadLicense();
        }
        public void MakePrintXAsync()
        {
            Task.Run(() => MakePrintX());
        }
        public string MakePrintX()
        {
            try
            {
                if (isFeatureEnable == false)
                    return "";

                _logger.Debug(string.Format(@"MakePrintX: path:{0}:", _outputPath));
                var tempFileName = string.Format(@"{0}\\temp_{1}", _outputPath, _wordFileName);
                var wordPath = string.Format(@"{0}\\{1}", _outputPath, _wordFileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(wordPath);
                var newFileName = string.Format(@"{0}/{1}{2}_print.pdf", _outputPath, DateTime.Now.ToString("yy-MM-dd-THH-mm-ss"),fileNameWithoutExtension);
                using (Document doc = new Document())
                {

                    doc.LoadFromFile(wordPath);
                    List<Field> hyperlinks = FindAllHyperlinks(doc);

                    for (int i = hyperlinks.Count - 1; i >= 0; i--)
                    {
                        FlattenHyperlinks(hyperlinks[i]);
                    }

                    CollectImage(doc);

                    ToPdfParameterList toPdf = new ToPdfParameterList();
                    toPdf.IsEmbeddedAllFonts = true;
                    toPdf.PdfConformanceLevel = Spire.Pdf.PdfConformanceLevel.Pdf_X1A2001;
                    toPdf.DisableLink = true;
                    doc.KeepSameFormat = true;
                    doc.IsUpdateFields = true;
                    doc.BuiltinDocumentProperties.Title = _complianceDetailInfo.InfoTitle;
                    doc.SaveToFile(newFileName, toPdf);
                    //      doc.SaveToFile(tempFileName, toPdf);

                }
               //using (PdfNewDocument newDOC = new PdfNewDocument(PdfConformanceLevel.Pdf_X1A2001))
               // {
               //     newDOC.Conformance = PdfConformanceLevel.Pdf_X1A2001;
               //     PdfDocument OriginalDoc = new PdfDocument();
               //     newDOC.ColorSpace = PdfColorSpace.CMYK;
               //     OriginalDoc.LoadFromFile(tempFileName);
               //     var orderIndex = 0;
               //     foreach (PdfPageBase page in OriginalDoc.Pages)
               //     {
               //         foreach (var image in page.ImagesInfo)
               //         {
               //             _collateralImageManager.UpdateImage(image.Image, orderIndex);
               //             orderIndex++;
               //         }
                        
               //         float pageWidth = page.Size.Width + OriginalDoc.PageSettings.Margins.Left + OriginalDoc.PageSettings.Margins.Right;
               //         float pageHeight = page.Size.Height + OriginalDoc.PageSettings.Margins.Top + OriginalDoc.PageSettings.Margins.Bottom;
               //         PdfPageBase p = newDOC.Pages.Add(new SizeF(pageWidth, pageHeight));
               //         page.Canvas.ColorSpace = PdfColorSpace.CMYK;
               //         page.CreateTemplate().Draw(p, 0, 0);
               //     }

               //     try
               //     {                        
               //         var imageIndex = 0;
               //         foreach (PdfPageBase page in OriginalDoc.Pages)
               //         {
               //             var images = page.ExtractImages();
               //             imageIndex = 0;
               //             if (images != null)
               //             {
               //                 foreach (Image image in images)
               //                 {
               //                     var newImage = _collateralImageManager.GetCMYKImage(image.Tag.ToString());
               //                     if (newImage != null)
               //                     {
               //                         page.ReplaceImage(imageIndex, newImage);
               //                     }
               //                 }
               //             }
               //         }
               //     }
               //     catch(Exception ex)
               //     {
               //         _logger.ErrorException("PrintXReady : MakePrintX: Image", ex);
               //         _collateralImageManager.LogImages(ex);
               //     }

               //    newDOC.Save(newFileName);
               // }
                return newFileName;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("PrintXReady : MakePrintX", ex);
                throw new Exception("Unable to Create Printx Standard",ex);
            }
        }
        private void CollectImage(Document document)
        {
            
            var orderIndex = 0;
            foreach (Section section in document.Sections)
            {
                foreach (Paragraph paragraph in section.Paragraphs)
                {
                    foreach (DocumentObject obj in paragraph.ChildObjects)
                    {
                        if (obj is DocPicture)
                        {
                            var ob = (DocPicture)obj;                            
                            var imageName = "";


                            if (string.IsNullOrEmpty(ob.AlternativeText) != true)
                            {
                                imageName = ob.AlternativeText;
                            }
                            else
                            {
                                foreach (PropertyInfo property in typeof(DocPicture)
                                                        .GetProperties(
                                                            BindingFlags.Public |
                                                            BindingFlags.NonPublic |
                                                            BindingFlags.Instance))
                                {
                                    if (property.Name == "Name")
                                    {
                                        imageName = property.GetValue(obj, null).ToString();
                                    }
                                }
                            }
                            _collateralImageManager.AddImages(orderIndex, imageName);
                             orderIndex++;                            
                        }
                    }
                }

            }
        }
        private void FlattenHyperlinks(Field field)
        {
            int ownerParaIndex = field.OwnerParagraph.OwnerTextBody.ChildObjects.IndexOf(field.OwnerParagraph);
            int fieldIndex = field.OwnerParagraph.ChildObjects.IndexOf(field);
            Paragraph sepOwnerPara = field.Separator.OwnerParagraph;
            int sepOwnerParaIndex = field.Separator.OwnerParagraph.OwnerTextBody.ChildObjects.IndexOf(field.Separator.OwnerParagraph);
            int sepIndex = field.Separator.OwnerParagraph.ChildObjects.IndexOf(field.Separator);
            int endIndex = field.End.OwnerParagraph.ChildObjects.IndexOf(field.End);
            int endOwnerParaIndex = field.End.OwnerParagraph.OwnerTextBody.ChildObjects.IndexOf(field.End.OwnerParagraph);

            FormatFieldResultText(field.Separator.OwnerParagraph.OwnerTextBody, sepOwnerParaIndex, endOwnerParaIndex, sepIndex, endIndex);

            field.End.OwnerParagraph.ChildObjects.RemoveAt(endIndex);

            for (int i = sepOwnerParaIndex; i >= ownerParaIndex; i--)
            {
                if (i == sepOwnerParaIndex && i == ownerParaIndex)
                {
                    for (int j = sepIndex; j >= fieldIndex; j--)
                    {
                        field.OwnerParagraph.ChildObjects.RemoveAt(j);

                    }
                }
                else if (i == ownerParaIndex)
                {
                    for (int j = field.OwnerParagraph.ChildObjects.Count - 1; j >= fieldIndex; j--)
                    {
                        field.OwnerParagraph.ChildObjects.RemoveAt(j);
                    }

                }
                else if (i == sepOwnerParaIndex)
                {
                    for (int j = sepIndex; j >= 0; j--)
                    {
                        sepOwnerPara.ChildObjects.RemoveAt(j);
                    }
                }
                else
                {
                    field.OwnerParagraph.OwnerTextBody.ChildObjects.RemoveAt(i);
                }
            }
        }
        private void FormatFieldResultText(Body ownerBody, int sepOwnerParaIndex, int endOwnerParaIndex, int sepIndex, int endIndex)
        {
            for (int i = sepOwnerParaIndex; i <= endOwnerParaIndex; i++)
            {
                Paragraph para = ownerBody.ChildObjects[i] as Paragraph;
                if (i == sepOwnerParaIndex && i == endOwnerParaIndex)
                {
                    for (int j = sepIndex + 1; j < endIndex; j++)
                    {
                        FormatText(para.ChildObjects[j] as TextRange);
                    }

                }
                else if (i == sepOwnerParaIndex)
                {
                    for (int j = sepIndex + 1; j < para.ChildObjects.Count; j++)
                    {
                        FormatText(para.ChildObjects[j] as TextRange);
                    }
                }
                else if (i == endOwnerParaIndex)
                {
                    for (int j = 0; j < endIndex; j++)
                    {
                        FormatText(para.ChildObjects[j] as TextRange);
                    }
                }
                else
                {
                    for (int j = 0; j < para.ChildObjects.Count; j++)
                    {
                        FormatText(para.ChildObjects[j] as TextRange);
                    }
                }
            }
        }
        private void FormatText(TextRange tr)
        {
            if (tr != null)
            {
                tr.CharacterFormat.TextColor = Color.Black;
                tr.CharacterFormat.UnderlineStyle = UnderlineStyle.None;
            }
        }

        private List<Field> FindAllHyperlinks(Document document)
        {
            List<Field> hyperlinks = new List<Field>();

            foreach (Section section in document.Sections)
            {
                foreach (DocumentObject sec in section.Body.ChildObjects)
                {
                    if (sec.DocumentObjectType == DocumentObjectType.Paragraph)
                    {
                        foreach (DocumentObject para in (sec as Paragraph).ChildObjects)
                        {
                            if (para.DocumentObjectType == DocumentObjectType.Field)
                            {
                                Field field = para as Field;

                                if (field.Type == FieldType.FieldHyperlink)
                                {
                                    hyperlinks.Add(field);
                                }
                            }
                        }
                    }
                }
            }
            return hyperlinks;
        }
    }
}
