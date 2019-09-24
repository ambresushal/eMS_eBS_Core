using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016;
using tmg.equinox.integration.qhplite.Ver2016.DocumentExporter;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;


namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class DocumentExporter2016URRTTemplate
    {
        #region Private Members
        private string _planURRTDocument { get; set; }
        private PlanURRTemplate _planURRTTemplate { get; set; }
        private IList<QhpDocumentViewModel> _qhpDocumentList { get; set; }
        string _saveFilePath = string.Empty;
        private IList<PlanBenefitPackage> _planBenefitPackageList { get; set; }
        Guid fileGuid;

        private class Constants
        {
            public const string URRTFolderPath = "\\App_Data\\URRT\\2015 Template\\";
            public const string UploadedURRTTemplateFolderPath = "\\App_Data\\URRT\\2015 Template\\";
            public const string ExcelExtension = ".xlsm";
        }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public DocumentExporter2016URRTTemplate(string folderPath, string urrtDocument, IList<QhpDocumentViewModel> qhpDocumentList)
        {
            this._planURRTDocument = urrtDocument;
            this._qhpDocumentList = qhpDocumentList;
            _saveFilePath = folderPath + Constants.UploadedURRTTemplateFolderPath;
        }
        #endregion Constructor

        #region Public Methods
        public string ExportToExcel()
        {
            CreateURRTTemplate();

            URRTModelToExcelMapper2016URRTTemplate mapper = new URRTModelToExcelMapper2016URRTTemplate(_planURRTTemplate, _saveFilePath);
            fileGuid = mapper.GetExcel();

            string guid = fileGuid.ToString();
            string fileFullName = _saveFilePath + guid + Constants.ExcelExtension;

            return fileFullName;
        }
        #endregion Public Methods

        #region Private Methods
        private void CreateBenefitPackages()
        {
            if (_planBenefitPackageList == null)
            {
                _planBenefitPackageList = new List<PlanBenefitPackage>();
                PlanBenefitPackageMapper mapper = new PlanBenefitPackageMapper();

                foreach (var document in _qhpDocumentList)
                {
                    PlanBenefitPackage package = mapper.GetBenefitPackage(document.DocumentName, document.DocumentData);
                    _planBenefitPackageList.Add(package);
                }
            }
        }

        private void CreateURRTTemplate()
        {
            CreateBenefitPackages();

            URRTJsonToModelMapper mapper = new URRTJsonToModelMapper();
            _planURRTTemplate = mapper.GetURRTTemplate(_planURRTDocument, _planBenefitPackageList);
        }
        #endregion Private Methods
    }
}
