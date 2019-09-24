using GemBox.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.setting.Interface;
using WebSupergoo.ABCpdf11;
using WebSupergoo.ABCpdf11.Elements;
using WebSupergoo.ABCpdf11.Objects;
using WebSupergoo.ABCpdf11.Operations;

namespace tmg.equinox.generatecollateral
{
    public class ComplianceService : BaseCompliance
    {
        FilePath _filePath;
        ISettingManager _settingManager;
        const string CONST_DocCompliance = "DocCompliance";
        const string CONST_PdfConformance = "tmg.equinox.net.PdfConformance";
        const string CONST_PdfConformance_ON_OFF = "tmg.equinox.net.PdfConformance.OnOff";
        const string CONST_PdfConformance_Author = "tmg.equinox.net.PdfConformance.Author";
        bool isEnable = false;

        private static readonly ILog _logger = LogProvider.For<ComplianceService>();
        List<string> _PdfConformances;
        ComplianceDetailInfo _complianceDetailInfo;
        public ComplianceService(FilePath filePath, ISettingManager settingManager, ComplianceDetailInfo objComplianceDetailInfo)
        {
            XSettings.InstallLicense("X/VKS0cMn5FgsCJaa6uMaIL2KLdJQ4MYlq3wxL3FA0ojxkiVPH3rYMVWQ0lkwg8KCtU54j5AuSISr6MhQbF2xFAkfG2VAHo83DFMO/XgBjbi1y7S5MlUFrjUWBKMcmImUL1oUMFb8wtwCFVMoSiSIEERXiebQ2W5r8l4z1spFNjOxaIhuzL0pF7Bor9RgTVZcxNayGxeyX+VHxglENwhgH959XP2XZZgJumQOkjkRZvKLh0FtsqiAz4Xpzu+N+6ajpoO");

            _filePath = filePath;
            SetComplaincePath(_filePath.NewComplianceFileNamePath);
            _settingManager = settingManager;
            _PdfConformances = _settingManager.GetSettingValueSplit(CONST_DocCompliance, CONST_PdfConformance);
            var flag = _settingManager.GetSettingValue(CONST_DocCompliance, CONST_PdfConformance_ON_OFF);
            objComplianceDetailInfo.InfoAuthor = _settingManager.GetSettingValue(CONST_DocCompliance, CONST_PdfConformance_Author);
            _complianceDetailInfo = objComplianceDetailInfo;
            if (flag == "ON")
            {
                isEnable = true;
            }
        }
        public bool isFeatureEnable
        {
            get { return isEnable; }
        }
        public override void MakeCompliance()
        {
            if (isEnable == false)
            {
                return;
            }

            //    ValidateGemBoxPdf();
            // ConvertGemBoxPdfToWord();
            //RemoveAccessibilityFromWordToPDF();
            FixAcccesibility();

            var ctr = 1;
            foreach (var type in _PdfConformances)
            {
                MakePDFCompliance(getComplianceType(type), ctr);
                ctr++;
            }
        }

        public override void ValidateGemBoxPdf()
        {
            foreach (var type in _PdfConformances)
            {
                var pdfConformance = getComplianceType(type);
                if (pdfConformance != PdfConformance.PdfA2u)
                {
                    ValidatePDF(getComplianceType(type), _filePath.GetOriginalNewFileNamePDf);
                }
            }
        }
        public override void ValidateCompliancePdf()
        {
            foreach (var type in _PdfConformances)
            {
                ValidatePDF(getComplianceType(type), _filePath.NewComplianceFileNamePath);
            }
        }

        private PdfConformance getComplianceType(string type)
        {
            //            pdfa1b,pdfa1a,pdfa2b,pdfa2u,pdfa2a

            var value = type.ToLower();

            switch (value)
            {
                case "pdfa1b":
                    return PdfConformance.PdfA1b;
                case "pdfa1a":
                    return PdfConformance.PdfA1a;
                case "pdfa2b":
                    return PdfConformance.PdfA2b;
                case "pdfa2u":
                    return PdfConformance.PdfA2u;
                case "pdfa2a":
                    return PdfConformance.PdfA2a;
                default:
                    return PdfConformance.PdfA1a;
            }
        }
        private string getComplianceType(PdfConformance type)
        {
            //            pdfa1b,pdfa1a,pdfa2b,pdfa2u,pdfa2a

            switch (type)
            {
                case PdfConformance.PdfA1b:
                    return "pdfa1b";
                case PdfConformance.PdfA1a:
                    return "pdfa1a";
                case PdfConformance.PdfA2b:
                    return "pdfa2b";
                case PdfConformance.PdfA2u:
                    return "pdfa2u";
                case PdfConformance.PdfA2a:
                    return "pdfa2a";
                default:
                    return "pdfA1a";
            }
        }
        private void ValidatePDF(PdfConformance pdfConformance, string path)
        {
            _logger.Debug(string.Format(@"ValidatePDF: path:{0}:", path));

            using (PdfValidationOperation theOperation = new PdfValidationOperation())
            {
                theOperation.Conformance = pdfConformance;
                var xr = new XReadOptions();
                xr.ReadModule = ReadModuleType.MSOffice;
                Doc theDoc = theOperation.Read(path, xr);
                theDoc.Dispose();

                if (theOperation.Errors.Count > 0)
                {
                    FillErrors(theOperation.Errors, getComplianceType(pdfConformance));
                }
                if (theOperation.Warnings.Count > 0)
                {
                    FillWarnings(theOperation.Warnings, getComplianceType(pdfConformance));
                }
            }
        }

        private void RemoveChapterLevelBookmarkfromDoc()
        {
            try
            {
                DocumentModel document = DocumentModel.Load(_filePath.CurrentPathWithDocFileName);

                List<string> markForDeleteBookmark = new List<string>();
                foreach (BookmarkStart bookMarkStart in document.GetChildElements(true, ElementType.BookmarkStart))
                {
                    if (bookMarkStart != null)
                    {
                        //make sure template must contain bookmark name with chap
                        if (bookMarkStart.Name.Length > 4)
                        {
                            Int32 num;
                            int.TryParse(bookMarkStart.Name.ToLower().Substring(4), out num);

                            if (bookMarkStart.Name.ToLower().StartsWith("chap") && num > 0)
                            {
                                markForDeleteBookmark.Add(bookMarkStart.Name);
                            }
                        }
                    }
                }

                foreach (var bookMarkStart in markForDeleteBookmark)
                {
                    var bookMark = document.Bookmarks.Where(m => m.Name == bookMarkStart).FirstOrDefault();
                    if (bookMark != null)
                    {
                        document.Bookmarks.Remove(bookMark);
                    }
                }

                document.Save(_filePath.NewDocFileNameWithPath);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ComplianceService : RemoveChapterLevelBookmark", ex);
            }
        }


        private void FixAcccesibility()
        {
            //   RemoveChapterLevelBookmarkfromDoc();

            var tags = new Structure(true);
            tags.ComplianceDetailInfoData = _complianceDetailInfo;
            tags.Load(_filePath.CurrentPathWithDocFileName);
            tags.FixAccessibility(_filePath.NewAccessibilityPdfFileNamePath, _filePath.Title);

        }


        private void FixAnnotOnLink()
        {
            var tags = new Structure();
            tags.Load(_filePath.NewAccessibilityPdfFileNamePath);
            tags.FixAnnotOnLink();
            tags.FixAnnotOnFigure();
        }

        private void MakePDFCompliance(PdfConformance pdfConformance, int ctr)
        {
            var newFilePath = _filePath.GetNewComplianceFileNamePath(ctr);
            SetComplaincePath(newFilePath);
            using (var theDoc = new Doc())
            {
                var xr = new XReadOptions();
                xr.ReadModule = ReadModuleType.Default;
                xr.Repair = true;
                theDoc.Read(_filePath.PreviouNewComplianceFileNamePath, xr);

                using (PdfConformityOperation theOperation = new PdfConformityOperation())
                {
                    theOperation.Conformance = pdfConformance;
                    theOperation.Conformity = PdfConformity.NoJavaScriptAction;
                    theOperation.Options = PdfConformityOperationOptions.KeepDocumentModDate;
                    theOperation.Save(theDoc, newFilePath);

                    if (theOperation.Errors.Count > 0)
                    {
                        FillChangeErrorLogs(theOperation.Errors, getComplianceType(pdfConformance));
                    }
                    if (theOperation.Messages.Count > 0)
                    {
                        FillChangeLogs(theOperation.Messages, getComplianceType(pdfConformance));
                    }
                }
            }
        }

    }

    public class ComplianceDetailInfo
    {
        public string InfoAuthor { get; set; }
        public string InfoTitle { get; set; }
    }
}

