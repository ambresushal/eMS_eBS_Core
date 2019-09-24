using GemBox.Document;
using GemBox.Document.Tables;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Collateral;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.generatecollateral
{
    public class GenerateCollateral
    {
        string _dataJson;
        string _designJson;
        string _strInputFilePath;
        string _outputpath;
        string _imageFilePath = string.Empty;
        ISettingManager _settingManager;
        IFormInstanceService _formInstanceService;
        HTMLComplianceFormatter _HTMLComplianceFormatter;
        applicationservices.viewmodels.FolderVersion.FormInstanceExportPDF _formInstanceDetails;
        ICollateralService _collateralService;

        private static readonly ILog _logger = LogProvider.For<GenerateCollateral>();

        public applicationservices.viewmodels.FolderVersion.FormInstanceExportPDF FormInstanceDetails
        {
            get
            {
                return _formInstanceDetails;
            }
            set
            {
                _formInstanceDetails = _HTMLComplianceFormatter.FormInstanceDetails = value;
            }
        }

        public GenerateCollateral(string path, string outputpath, ISettingManager settingManager, ICollateralService collateralService)
        {
            _strInputFilePath = path;
            _outputpath = outputpath;
            _settingManager = settingManager;
            _collateralService = collateralService;
        }
        public GenerateCollateral(string path, string dataJson, string designJson, string outputpath)
        {
            _dataJson = RemoveUnAcceptedTrackChanges(dataJson);
            _designJson = designJson;
            _strInputFilePath = path;
            _outputpath = outputpath == string.Empty ? System.IO.Path.GetDirectoryName(_strInputFilePath) : outputpath;
            _HTMLComplianceFormatter.FormInstanceDetails = FormInstanceDetails;
        }

        public GenerateCollateral(string path, string dataJson, string designJson, string outputpath, string imageFilePath, ISettingManager settingManager, IFormInstanceService formInstanceService, ICollateralService collateralService, bool isCalledFromService = false)
        {
            _dataJson = RemoveUnAcceptedTrackChanges(dataJson); ;
            _designJson = designJson;
            _strInputFilePath = path;
            _imageFilePath = imageFilePath;
            _outputpath = outputpath == string.Empty ? System.IO.Path.GetDirectoryName(_strInputFilePath) : outputpath;
            _settingManager = settingManager;
            _formInstanceService = formInstanceService;
            _HTMLComplianceFormatter = new HTMLComplianceFormatter(isCalledFromService);
            _HTMLComplianceFormatter.FormInstanceDetails = FormInstanceDetails;
            _collateralService = collateralService;
        }

        public string Generate()
        {
            if (File.Exists(_strInputFilePath))
            {
                try
                {

                    // If using Professional version, put your serial key below.
                    ComponentInfo.SetLicense("DMPZ-896E-ELBX-2DWD");
                    //ComponentInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;

                    //Fetch the CustomExcel Parts form OpenXml Dlls, the data having information about tags
                    XElement rootElements = Helper.GetCustomExcelParts(_strInputFilePath);

                    //Load the document using gemBox
                    DocumentModel document = DocumentModel.Load(_strInputFilePath);
                    // DocumentModel.Load("C:\\Users\\myadav\\Desktop\\HNE\\Config File\\PBP\\Work\\test.html").Save("C:\\Users\\myadav\\Desktop\\HNE\\Config File\\PBP\\Work\\test.docx");

                    // document.DocumentProperties.BuiltIn[BuiltInDocumentProperty.Title] = System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath);
                    //   document.DocumentProperties.BuiltIn[BuiltInDocumentProperty.Author] = System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath);

                    //Take a list of Sections from document which have the tag placed, then further process for HTML Data
                    List<Paragraph> paracollection = new List<Paragraph>();
                    SectionCollection objSectionColl = document.Sections;
                    foreach (Section objsection in objSectionColl)
                    {
                        //HeaderFooter Section
                        HeaderFooterCollection objHeaderFooterColl = objsection.HeadersFooters;
                        ProcessHeaderFooter(objsection, objHeaderFooterColl, rootElements);
                        //Block Section
                        BlockCollection objBlockColl = objsection.Blocks;
                        ProcessBlockData(paracollection, objBlockColl, rootElements);
                        ProcessTableBlockData(paracollection, objBlockColl, rootElements);
                    }

                    //Process, to replace tags with html data
                    foreach (Paragraph para in paracollection)
                    {
                        var h = para.Inlines.Where(x => x is Run)
                                                .ToList();

                        foreach (Inline v in h.ToList())
                        {
                            bool isHtmlProcessed = ProcessRunWithHTMLData(v, rootElements);
                        }
                        //If html data is processed then inline collection get changed, and if we try to iterated it throws index exception
                        //So if html data is processed loop has been breaked.                         
                    }

                    //Calling Table of Entries Updated function, this will updated the entries of newly created HTML Data paragraphs
                    ProcessTableOfContent(document);

                    foreach (Table t in document.GetChildElements(true, ElementType.Table))
                    {
                        foreach (Run run in t.GetChildElements(true, ElementType.Run))
                        {
                            if (run.CharacterFormat.FontName == "'times new roman'")
                                run.CharacterFormat.FontName = "times new roman";
                            if (run.CharacterFormat.FontName.Contains("'"))
                                run.CharacterFormat.FontName = run.CharacterFormat.FontName.Replace("'", string.Empty);
                        }
                    }

                    foreach (Run run in document.GetChildElements(true, ElementType.Run))
                    {
                        if (run.CharacterFormat.FontName == "'times new roman'")
                            run.CharacterFormat.FontName = "times new roman";
                        if (run.CharacterFormat.FontName.Contains("'"))
                            run.CharacterFormat.FontName = run.CharacterFormat.FontName.Replace("'", string.Empty);
                    }

                    //Saving File Name
                    string strNewFileName = _outputpath + System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath)
                      + DateTime.Now.ToString("yy-MM-dd-THH-mm-ss") + ".Docx";

                    document.Save(strNewFileName);


                    return strNewFileName;

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return string.Empty;
        }
        private ColleteralFilePath MakeCompliance(string outputpath, string fileName, int formInstanceId, string userName, string title, string originalNewFileNamePDf, int collateralProcessQueue1Up, string licPath, bool ignorePrintx = false)
        {
            var colleteralPath = new ColleteralFilePath() { PDF = "", PrintX = "" };
            bool isComplianceSuccessfull = false;
            try
            {
                ///not allowed from UI
                if (collateralProcessQueue1Up == 0 && formInstanceId != 0)
                {
                    colleteralPath.PDF = originalNewFileNamePDf;
                    return colleteralPath;
                }

                ComplianceDetailInfo objComplianceDetailInfo = new generatecollateral.ComplianceDetailInfo();
                if (FormInstanceDetails != null)
                {
                    objComplianceDetailInfo.InfoTitle = System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath) + "_" + FormInstanceDetails.FormName;
                    //objComplianceDetailInfo.InfoAuthor = FormInstanceDetails.FormName;
                }
                else
                {
                    objComplianceDetailInfo.InfoTitle = System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath);
                }
                var accessibilityService = new ComplianceService(new FilePath(fileName, outputpath, title, originalNewFileNamePDf), _settingManager, objComplianceDetailInfo);
                if (accessibilityService.isFeatureEnable)
                {
                    accessibilityService.MakeCompliance();
                    colleteralPath.PDF = originalNewFileNamePDf;
                    if (formInstanceId != 0)
                    {
                        SaveComplianceValidationlogAsync(accessibilityService, formInstanceId, userName, collateralProcessQueue1Up);
                    }
                    colleteralPath.PDF = accessibilityService.ComplianceFilePath;
                    isComplianceSuccessfull = true;
                    if (ignorePrintx == false)
                    {
                        var printx = new PrintXReady(outputpath, fileName, _settingManager, _collateralService, licPath, objComplianceDetailInfo);
                        colleteralPath.PrintX = printx.MakePrintX();
                    }
                    return colleteralPath;
                }

                return colleteralPath;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("508 Compliance Error.", ex);
                var validationLogErrors = new List<FormInstanceComplianceValidationlog>();
                var errors = new List<ValidationError>();
                errors.Add(new ValidationError
                {
                    Error = string.Format("{0}:Unable to Fix Compliance Issue {1} ", formInstanceId.ToString(), ex.Message),
                    No = 1,
                    ComplianceType = "",
                });

                Fill(validationLogErrors, errors, "Error", formInstanceId, userName, collateralProcessQueue1Up);
                _formInstanceService.UpdateFormInstanceComplianceValidationlog(formInstanceId, validationLogErrors, collateralProcessQueue1Up);
            }
            if (isComplianceSuccessfull == false)
            {
                colleteralPath.PDF = originalNewFileNamePDf;
            }
            colleteralPath.PrintX = originalNewFileNamePDf;
            return colleteralPath;
        }
        private void SaveComplianceValidationlogAsync(ComplianceService complianceService, int formInstanceId, string userName, int collateralProcessQueue1Up)
        {
            var validationErrors = new List<FormInstanceComplianceValidationlog>();
            Fill(validationErrors, complianceService.Errors, "PreValidationError", formInstanceId, userName, collateralProcessQueue1Up);
            Fill(validationErrors, complianceService.Warnings, "Warning", formInstanceId, userName, collateralProcessQueue1Up);
            Fill(validationErrors, complianceService.ChangesMadeAfterCompliance, "Resolved", formInstanceId, userName, collateralProcessQueue1Up);
            Fill(validationErrors, complianceService.ErrorExistsAfterCompliance, "NotResolved", formInstanceId, userName, collateralProcessQueue1Up);
            _formInstanceService.UpdateFormInstanceComplianceValidationlog(formInstanceId, validationErrors, collateralProcessQueue1Up);
        }

        private List<FormInstanceComplianceValidationlog> Fill(
            List<FormInstanceComplianceValidationlog> validationErrors, List<ValidationError> complianceServiceError, string type, int formInstanceId, string userName, int collateralProcessQueue1Up)
        {
            foreach (var error in complianceServiceError)
            {
                validationErrors.Add(new FormInstanceComplianceValidationlog
                {
                    AddedDate = DateTime.Now,
                    ComplianceType = error.ComplianceType,
                    FormInstanceID = formInstanceId,
                    No = error.No,
                    ValidationType = type,
                    Error = error.Error,
                    AddedBy = userName,
                    CollateralProcessQueue1Up = collateralProcessQueue1Up
                });
            }
            return validationErrors;
        }
        public string GenerateWord(string filpath)
        {
            string newfilepath = UpdateTableOfContent(filpath);
            string strFileWord = Helper.RemoveContentControl(newfilepath, System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath));
            return strFileWord;
        }

        private string UpdateTableOfContent(string filepath)
        {
            DocumentModel d = DocumentModel.Load(filepath);
            string strNewFileName = string.Empty;
            if (d.GetChildElements(true, ElementType.TableOfEntries).Count() > 0)
            {
                ProcessTableOfContent(d);
                try
                {
                    d.GetPaginator(new PaginatorOptions() { UpdateFields = true });
                }
                catch (Exception ex)
                {

                }
                foreach (Run run in d.GetChildElements(true, ElementType.Run))
                {
                    if (run.Text == "No table of contents entries found.")
                        run.Text = string.Empty;
                }

                strNewFileName = _outputpath + System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath)
                                   + DateTime.Now.ToString("yy-MM-dd-THH-mm-ss") + System.IO.Path.GetExtension(_strInputFilePath);

            }

            for (int i = 0; i < d.Sections.Count; ++i)
            {
                Section section = d.Sections[i];
                if (string.IsNullOrWhiteSpace(section.Content.ToString()) && section.GetChildElements(true, ElementType.Picture).Count() == 0)
                {
                    d.Sections.RemoveAt(i);
                    --i;
                }
            }

            if (!string.IsNullOrEmpty(strNewFileName))
            {
                d.Save(strNewFileName);

                if (File.Exists(filepath))
                    File.Delete(filepath);

                return strNewFileName;
            }
            else
                return filepath;
        }

        public ServiceResult GeneratePDF(string path, string licPath)
        {

            ServiceResult result = new ServiceResult();
            var serviceResultItem = new ServiceResultItem();
            try
            {
                var pdf = GeneratePDF(path, 0, "Admin", 0, "", licPath);

                result.Result = ServiceResultStatus.Success;
                serviceResultItem.Messages = new string[2];

                var pathArry = pdf.PDF.Split('\\');


                serviceResultItem.Messages[0] = string.Format("../Content/CollateralImages/{0}/{1}", pathArry[pathArry.Length - 2], pathArry[pathArry.Length - 1]);
                pathArry = pdf.PrintX.Split('/');
                serviceResultItem.Messages[1] = string.Format("../Content/CollateralImages/{0}", pathArry[pathArry.Length - 1]);

                IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                serviceResultItemList.Add(serviceResultItem);
                result.Items = serviceResultItemList;
                return result;
            }
            catch (Exception ex)
            {
                ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult GeneratePDFInDatasBase(string path, string licPath, string rootpath, byte[] file, byte[] file1,  UploadReportViewModel uploadReportViewModel)
        {

            ServiceResult result = new ServiceResult();
            var serviceResultItem = new ServiceResultItem();
            try
            {
                ColleteralFilePath pdf = new ColleteralFilePath();
                if (uploadReportViewModel.AlreadyConverted508 == false)
                {
                    pdf = GeneratePDF(path, 0, "Admin", 0, "", licPath, true);
                }
                else
                {   
                    pdf.PDF = path;
                    pdf.PrintX = "";
                    uploadReportViewModel.File508 = file.ToArray();
                }
                result.Result = ServiceResultStatus.Success;
                serviceResultItem.Messages = new string[2];

                var pathArry = pdf.PDF.Split('\\');

                if (uploadReportViewModel.AlreadyConverted508 == false)
                {

                    serviceResultItem.Messages[0] = string.Format("../Content/CollateralImages/{0}/{1}", pathArry[pathArry.Length - 2], pathArry[pathArry.Length - 1]);
                    MemoryStream ms1 = Helper.GetStreamOfFile(string.Format(@"{2}\Content\CollateralImages\\{0}\\{1}", pathArry[pathArry.Length - 2],
                        pathArry[pathArry.Length - 1], rootpath));
                    uploadReportViewModel.File508 = ms1.ToArray();
                    pathArry = pdf.PrintX.Split('/');
                    serviceResultItem.Messages[1] = string.Format("../Content/CollateralImages/{0}", pathArry[pathArry.Length - 1]);
                }
               
     
                //Mapping data from UI to ViewModel
                //TODO
                uploadReportViewModel.WordFile = file;
                uploadReportViewModel.PrintxFile = file1;
                
                   
                
                _collateralService.UploadCollateral(uploadReportViewModel);

                IList<ServiceResultItem> serviceResultItemList = new List<ServiceResultItem>();
                serviceResultItemList.Add(serviceResultItem);
                result.Items = serviceResultItemList;
                return result;
            }
            catch (Exception ex)
            {
                ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                result = ex.ExceptionMessages();
            }
            return result;
        }


        public ColleteralFilePath GeneratePDF(string filpath, int formInstanceId, string userName, int collateralProcessQueue1Up, string title = "", string licPath = "", bool ignorePrintX = false)
        {
            ComponentInfo.SetLicense("DMPZ-896E-ELBX-2DWD");
            DocumentModel document = DocumentModel.Load(filpath, GemBox.Document.LoadOptions.DocxDefault);
            document.ViewOptions.Zoom = 80;
            foreach (ParagraphFormat format in document.GetChildElements(true, ElementType.Paragraph)
                                                            .Cast<Paragraph>()
                                                            .Select(p => p.ParagraphFormat)
                                                            .Where(f => f.LineSpacing == 0 && f.LineSpacingRule == LineSpacingRule.Exactly))
            {
                format.LineSpacingRule = LineSpacingRule.AtLeast;
            }

            string strNewFileNamePDf = _outputpath + System.IO.Path.GetFileNameWithoutExtension(_strInputFilePath) + DateTime.Now.ToString("yy-MM-dd-THH-mm-ss") + ".Pdf";
            document.Save(strNewFileNamePDf, GemBox.Document.SaveOptions.PdfDefault);
            //_logger.Debug(string.Format(@"{0}:strNewFileNamePDf:{1}:_outputpath:{2}", formInstanceId.ToString(), strNewFileNamePDf, _outputpath));

            var pathWithOutFileName = System.IO.Path.GetDirectoryName(filpath);//have Doc file
            if (title == "")
                title = System.IO.Path.GetFileNameWithoutExtension(filpath);

            var colleteralFilePath = MakeCompliance(pathWithOutFileName, System.IO.Path.GetFileName(filpath), formInstanceId, userName, title, strNewFileNamePDf, collateralProcessQueue1Up, licPath, ignorePrintX);

            return colleteralFilePath;
        }

        private void SetHeaderRowForTable(Table[] tables)
        {
            //Function will repeated table header on next page if the table data goes to second page
            if (tables != null)
            {
                foreach (Table objTable in tables)
                {
                    if (objTable.Rows.Count > 0)
                        objTable.Rows[0].RowFormat.RepeatOnEachPage = true;
                }
            }
        }

        private void ProcessTableOfContent(DocumentModel document)
        {
            try
            {
                foreach (TableOfEntries listele in document.GetChildElements(true, ElementType.TableOfEntries))
                {
                    if (listele != null && listele is TableOfEntries)
                    {
                        listele.Update();
                    }
                }
                //document.GetPaginator(new PaginatorOptions() { UpdateFields = true });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Collateral : ProcessTableOfContent", ex);
            }
        }

        private void ProcessBlockData(List<Paragraph> paracollection, BlockCollection objBlockColl, XElement rootElements)
        {
            //taking the paragraph collection into list to process further for HTML data
            //this is done because if Gemobox process tags with html data one by one it affects the paragraph collection, and throws index exception
            if (objBlockColl != null)
            {
                foreach (Block objBlock in objBlockColl)
                {
                    if (objBlock is Paragraph)
                    {
                        Paragraph objParagraph = objBlock as Paragraph;
                        if (objParagraph != null)
                        {
                            InlineCollection objInlineCollection = objParagraph.Inlines;

                            foreach (Inline objInline in objInlineCollection)
                            {
                                if (objInline is Run)
                                {
                                    Run objRun = objInline as Run;

                                    if (!string.IsNullOrEmpty(objRun.Text))
                                    {

                                        if (!paracollection.Contains(objParagraph))
                                            paracollection.Add(objParagraph);
                                    }
                                }
                                if (objInline is TextBox)
                                {
                                    foreach (Run run in objInline.GetChildElements(true, ElementType.Run))
                                    {
                                        if (!string.IsNullOrEmpty(run.Text))
                                        {
                                            if (run.Parent != null && run.Parent is Paragraph && !paracollection.Contains(run.Parent as Paragraph))
                                                paracollection.Add(run.Parent as Paragraph);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ProcessHeaderFooter(Section objsection, HeaderFooterCollection objHeaderFooterColl, XElement rootElements)
        {
            //Taking collection of Header and  Footer sections,  if both has the tags then it will be replaced with HTML data
            List<Paragraph> paracollectionHeaderFooter = new List<Paragraph>();
            if (objHeaderFooterColl != null)
            {
                foreach (HeaderFooter objHeaderFooter in objHeaderFooterColl)
                {
                    BlockCollection objHFBlockColl = objHeaderFooter.Blocks;
                    foreach (Block objHFBlock in objHFBlockColl)
                    {
                        if (objHFBlock is Paragraph)
                        {
                            Paragraph objHFParagraph = objHFBlock as Paragraph;
                            if (objHFParagraph != null)
                            {
                                InlineCollection objHFInlineCollection = objHFParagraph.Inlines;
                                foreach (Inline objHFInline in objHFInlineCollection)
                                {
                                    if (objHFInline is Run)
                                    {
                                        Run objHFRun = objHFInline as Run;
                                        if (!string.IsNullOrEmpty(objHFRun.Text))
                                        {
                                            if (!paracollectionHeaderFooter.Contains(objHFParagraph))
                                                paracollectionHeaderFooter.Add(objHFParagraph);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Processing for HTML Data

            foreach (HeaderFooter objHeaderFooter in objHeaderFooterColl)
            {
                BlockCollection objHFBlockColl = objHeaderFooter.Blocks;
                ProcessTableBlockData(paracollectionHeaderFooter, objHFBlockColl, rootElements);
            }

            //Processing for HTML Data
            foreach (Paragraph para in paracollectionHeaderFooter)
            {
                var h = para.Inlines.Where(x => x is Run)
                                               .ToList();

                foreach (Inline v in h.ToList())
                {
                    bool isHtmlProcessed = ProcessRunWithHTMLData(v, rootElements);
                }
                //If html data is processed then inline collection get changed, and if we try to iterated it throws index exception
                //So if html data is processed loop has been breaked.
            }

        }

        private bool ProcessRunWithHTMLData(Inline objInline, XElement rootElements)
        {
            bool ishtmlProcessed = false;
            if (objInline is Run)
            {
                Run objRun = objInline as Run;
                //Checking the tag text is present into rootElements, if yes then only HTML tags is replaced.
                StringBuilder strValue = GetHtmlText(objRun.Text, rootElements);
                HtmlLoadOptions htmloption = new HtmlLoadOptions();
                htmloption.PreserveUnsupportedFeatures = false;

                if (strValue != null && !string.IsNullOrEmpty(strValue.ToString()))
                {
                    ContentRange htmlPlaceholder = objRun.Content;
                    if (strValue.ToString() == "<html></html>")
                    {
                        //var paragraphBreakRange = new ContentRange(objRun.Content.Start, objRun.Content.End);
                        //paragraphBreakRange.Delete();
                        objRun.Parent.Content.Delete();
                        objRun.Content.Delete();
                    }
                    else
                    {
                        Paragraph p = objRun.Parent as Paragraph;
                        htmlPlaceholder.LoadText(string.Empty);

                        //foreach(Element r in p.Inlines[0].GetChildElements(true).ToList())
                        //{
                        //    if(r is Run)
                        //        r.Content.Delete();
                        //}

                        //if (p.ParagraphFormat.Style != null && p.ParagraphFormat.Style.ParagraphFormat != null
                        //    && p.ParagraphFormat.Style.Name != "Normal")
                        //{
                        //    string str = strValue.ToString().Replace("</p", "</span");
                        //    str = str.Replace("<p", "<span");
                        //    str = str.Replace("<span style=\"margin: 0in 0in 0pt; line-height: 0%; font-size: 11pt; font-family: Calibri, sans-serif;\">&nbsp;</span>", string.Empty);
                        //    //p.ParagraphFormat.LineSpacing = 0;
                        //    //str = Regex.Replace(strValue.ToString(), @"<[^>]*(>|$)|&nbsp;|&zwnj;|&raquo;|&laquo;", string.Empty).Trim();
                        //    //htmlPlaceholder.Start.LoadText(strValue, GemBox.Document.LoadOptions.TxtDefault);
                        //    htmlPlaceholder.LoadText(str, GemBox.Document.LoadOptions.HtmlDefault);
                        //}
                        //else

                        htmlPlaceholder.LoadText(strValue.ToString(), htmloption);
                    }
                    ishtmlProcessed = true;
                }
            }
            return ishtmlProcessed;
        }

        private StringBuilder GetHtmlText(string placeHolderTagName, XElement rootElements)
        {
            StringBuilder htmlValue = new StringBuilder();
            try
            {
                //Check The tag contains any child tags, if yes then take all tags from customxmlParts Root node
                XElement currentNode = null;
                string strXPath = placeHolderTagName.Replace(rootElements.Name.LocalName, "").Trim();
                currentNode = rootElements.XPathSelectElement(strXPath);
                if (currentNode != null)
                {
                    string jsonPath = placeHolderTagName.Replace("/", ".");
                    jsonPath = jsonPath.StartsWith("root.") ? jsonPath.Substring(5) : jsonPath;

                    if (currentNode.HasElements)
                    {
                        foreach (var test11 in currentNode.Elements())
                        {
                            string strTagValue = test11.Name.LocalName.ToString();
                            if (!string.IsNullOrEmpty(strTagValue))
                                htmlValue.Append(GetHTMLString(jsonPath + "." + strTagValue));
                        }
                    }
                    else
                        htmlValue.Append(GetHTMLString(jsonPath));
                }
            }
            catch (Exception ex)
            {

            }
            return htmlValue;
        }

        private string GetHTMLString(string xPath)
        {
            JObject formData = JObject.Parse(_dataJson);
            dynamic elementData = formData.SelectToken(xPath);
            string html = elementData == null ? string.Empty : elementData;

            if (html.Contains("../Content/tinyMce"))
                html = html.Replace("../Content/tinyMce/", _imageFilePath);

            //if (!html.Contains("<p"))
            //    html = "<p style=\"margin: 0in 0in 0pt; line-height: 0%; \">" + html + "</p>";

            html = html.Replace("<h4", "<h1").Replace("</h4>", "</h1>");
            html = html.Replace("<h5", "<h2").Replace("</h5>", "</h2>");
            html = html.Replace("<h6", "<h3").Replace("</h6>", "</h3>");

            var checkHtml = html;
            _HTMLComplianceFormatter.FixAllHtmlFormat(checkHtml);

            return html == string.Empty ? "<html></html>" : html;
        }

        private void UpdateTableOfEntriesObjects(DocumentModel document)
        {
            //Update Table of entries object with entries created during HTML data processing
            foreach (Section objsection in document.Sections)
            {
                var objTableOfEntries = objsection.Blocks.Where(p => p.GetType() == typeof(TableOfEntries));
                //Check if Blocks contains Table of Entries, if the object is not null then process further
                if (objTableOfEntries != null)
                {
                    List<int> indexList = new List<int>();
                    foreach (var objcurrTOE in objTableOfEntries)
                    {
                        if (objcurrTOE != null && objcurrTOE is TableOfEntries)
                        {
                            TableOfEntries objTabE = objcurrTOE as TableOfEntries;
                            if (objTabE != null)
                            {
                                int index = objsection.Blocks.IndexOf(objTabE);
                                indexList.Add(index);
                            }
                        }
                    }
                    foreach (int indexToDelete in indexList)
                    {
                        objsection.Blocks.RemoveAt(indexToDelete);
                        objsection.Blocks.Insert(indexToDelete, new TableOfEntries(document, FieldType.TOC));
                    }
                }
            }
        }

        private void ProcessCloneOperationForTOC(DocumentModel document)
        {
            Dictionary<int, Section> toeSectionDic = new Dictionary<int, Section>();

            foreach (Section objSection in document.Sections)
            {
                var objTableOfEntries = objSection.Blocks.Where(p => p.GetType() == typeof(TableOfEntries));
                //Check if Blocks contains Table of Entries, if the object is not null then process further
                if (objTableOfEntries != null)
                {
                    bool allowToProcess = false;
                    foreach (var objcurrTOE in objTableOfEntries)
                    {
                        if (objcurrTOE != null && objcurrTOE is TableOfEntries)
                        {
                            allowToProcess = true;
                            break;
                        }
                    }
                    if (allowToProcess)
                    {
                        int indexVal = document.Sections.IndexOf(objSection);
                        if (!toeSectionDic.ContainsValue(objSection) && !toeSectionDic.ContainsKey(indexVal))
                        {
                            toeSectionDic.Add(indexVal, objSection.Clone(true));
                        }
                    }
                }
            }
            foreach (KeyValuePair<int, Section> objSectionVal in toeSectionDic)
            {
                ProcessCloneTOCObjectToNewDoc(objSectionVal.Value, document, objSectionVal.Key);
            }
        }

        private void ProcessCloneTOCObjectToNewDoc(Section objSection, DocumentModel baseDocument, int sectionIndex)
        {
            //Create Source Document for exporting section for TOE Process
            DocumentModel sourceDocument = new DocumentModel();

            // Mapping Base Document with Source Document
            var mappingBase = new ImportMapping(baseDocument, sourceDocument, false);
            //Exporting Base section to Destination Section
            Section destinationSection = sourceDocument.Import(objSection, true, mappingBase);
            destinationSection.PageSetup.PageStartingNumber = objSection.PageSetup.PageStartingNumber;
            sourceDocument.Sections.Add(destinationSection);
            //Processing Table of Contents for Newly created Document
            ProcessTableOfContent(sourceDocument);

            //Importing Updated section with proper TOC from Source to Base Document

            // Mapping Source Document with Base Document
            var mappingSource = new ImportMapping(sourceDocument, baseDocument, false);
            //Removing Current Section from Base Document
            baseDocument.Sections.RemoveAt(sectionIndex);

            // Import all sections from source document. And inserting it on same location where previous section was present in Base Document
            foreach (Section sourceSection in sourceDocument.Sections)
            {
                Section baseSectionObj = baseDocument.Import(sourceSection, true, mappingSource);
                baseDocument.Sections.Insert(sectionIndex, baseSectionObj);
                sectionIndex++;
            }
        }


        private void ProcessTableBlockData(List<Paragraph> paracollection, BlockCollection objBlockColl, XElement rootElements)
        {
            //taking the paragraph collection into list to process further for HTML data
            //this is done because if Gemobox process tags with html data one by one it affects the paragraph collection, and throws index exception
            if (objBlockColl != null)
            {
                foreach (Block obj in objBlockColl)
                {
                    if (obj is Table)
                    {
                        Table table = obj as Table;
                        foreach (TableRow row in table.Rows)
                            foreach (TableCell cell in row.Cells)
                                ProcessBlockData(paracollection, cell.Blocks, rootElements);
                    }
                }
            }
        }

        private string RemoveUnAcceptedTrackChanges(string formData)
        {
            int start;
            while ((start = formData.IndexOf("<insert")) >= 0)
            {
                int end = formData.IndexOf("</insert>", start);
                formData = formData.Remove(start, end - start + "</insert>".Length);
            }

            start = 0;
            while ((start = formData.IndexOf("<delete")) >= 0)
            {
                int end = formData.IndexOf(">", start);
                formData = formData.Remove(start, end - start + ">".Length);
            }

            //remove Model Language tag before generating document.
            start = 0;
            while ((start = formData.IndexOf("<mlstart")) >= 0)
            {
                int end = formData.IndexOf(">", start);
                formData = formData.Remove(start, end - start + ">".Length);
            }

            formData = formData.Replace("</delete>", "").Replace("</mlstart>", "");

            return formData;
        }
    }



}
