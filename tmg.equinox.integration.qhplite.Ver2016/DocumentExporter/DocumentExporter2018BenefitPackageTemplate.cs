using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;


namespace tmg.equinox.integration.qhplite.Ver2016.DocumentExporter
{
    /// <summary>
    /// This class creates the Benefit Package Template Excel for 2014 Version
    /// </summary>
    public class DocumentExporter2018BenefitPackageTemplate : IDocumentExporter
    {
        #region Private Memebers
        private IList<QhpDocumentViewModel> _documentList { get; set; }
        string _helperfilePath = string.Empty;
        string _filePath = string.Empty;
        private IList<PlanBenefitPackage> _planBenefitPackageList { get; set; }

        private class Constants
        {
            public const string QhpFolderPath = "\\App_Data\\Qhp\\2018 Template\\";
            public const string UploadedQhpTemplateFolderPath = "\\App_Data\\Qhp\\2018 Template\\";
            public const string PlanBenefitAddInName = "PlansBenefitsAddIn.xlam";
            public const string AvCalculatorName = "av-calculator-final.xlsm";
            public const string ExcelExportName = "Benefit Package Template.xlsm";
        }
        #endregion Private Members

        #region Constructor
        public DocumentExporter2018BenefitPackageTemplate(string folderPath, IList<QhpDocumentViewModel> qhpDocumentList)
        {
            this._documentList = qhpDocumentList;
            _helperfilePath = folderPath + Constants.QhpFolderPath;
            _filePath = folderPath + Constants.UploadedQhpTemplateFolderPath;
        }
        #endregion Constructor

        #region Public Methods
        public string ExportToExcel()
        {
            QHPModelToExcelMapper2018BenefitPackageTemplate mapper = new QHPModelToExcelMapper2018BenefitPackageTemplate(GetPlanBenefitPackages(), _helperfilePath, _filePath);
            Guid fileGuid = mapper.GetExcel();

            string guid = fileGuid.ToString();
            string fileName = guid + ".xlsm";
            string zipFileFullName = _filePath + guid + ".zip";
            string workBookFile = _helperfilePath + "//" + _planBenefitPackageList.Count() + " Benefit Package//workbook.xml";
            string fullFilePath = Path.Combine(_filePath + fileName);

            ZipArchiveMode mode = ZipArchiveMode.Create;
            if (System.IO.File.Exists(zipFileFullName) == true)
            {
                mode = ZipArchiveMode.Update;
            }
            using (ZipArchive zipFile = ZipFile.Open(zipFileFullName, mode))
            {
                if (mode == ZipArchiveMode.Create)
                {
                    zipFile.CreateEntryFromFile(Path.Combine(_helperfilePath, Constants.PlanBenefitAddInName), "qhp/" + Constants.PlanBenefitAddInName);
                    zipFile.CreateEntryFromFile(Path.Combine(_helperfilePath, Constants.AvCalculatorName), "qhp/" + Constants.AvCalculatorName);
                }
                else
                {
                    var entry = from ent in zipFile.Entries where ent.FullName == "qhp/" + Constants.ExcelExportName select ent;
                    if (entry != null && entry.Count() > 0)
                    {
                        entry.First().Delete();
                    }
                }
                zipFile.CreateEntryFromFile(Path.Combine(_filePath, fileName), "qhp/" + Constants.ExcelExportName);
            }

            return zipFileFullName;
        }

        public List<PlanBenefitPackage> GetPlanBenefitPackages()
        {
            if (_planBenefitPackageList == null)
            {
                _planBenefitPackageList = new List<PlanBenefitPackage>();
                CreatePlanBenefitPackages();
            }
            return _planBenefitPackageList.ToList();
        }
        #endregion Public Methods

        #region Private Methods
        private void CreatePlanBenefitPackages()
        {
            PlanBenefitPackageMapper mapper = new PlanBenefitPackageMapper();
            foreach (var document in _documentList)
            {
                PlanBenefitPackage package = mapper.GetBenefitPackage(document.DocumentName, document.DocumentData);
                _planBenefitPackageList.Add(package);
            }
        }

        private void RemoveCDataFromSheet(ZipArchive zipFile, string sheetName)
        {
            var entry = from ent in zipFile.Entries where ent.FullName == sheetName select ent;
            if (entry != null && entry.Count() > 0)
            {
                string textContent = "";
                using (StreamReader reader = new StreamReader(entry.First().Open()))
                {
                    textContent = reader.ReadToEnd();
                }
                textContent = textContent.Replace("<![CDATA[", "");
                textContent = textContent.Replace("]]>", "");
                entry.First().Delete();
                ZipArchiveEntry newEntry = zipFile.CreateEntry(sheetName);
                using (StreamWriter writer = new StreamWriter(newEntry.Open()))
                {
                    writer.Write(textContent);
                }
            }
        }
        #endregion Private Methods
    }
}
