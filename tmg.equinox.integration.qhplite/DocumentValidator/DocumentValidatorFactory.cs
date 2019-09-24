using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Version2016 = tmg.equinox.integration.qhplite.Ver2016;

namespace tmg.equinox.integration.qhplite.DocumentExporter
{
    public class DocumentValidatorFactory
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public DocumentValidatorFactory()
        {

        }
        #endregion Constructor

        #region Public Methods
        public IList<QHPValidationError> ValidateQhpDocument(int formDesignVersionId, DateTime formDesignVersionEffectiveDate, IList<QhpDocumentViewModel> qhpDocumentsList)
        {
            IList<QHPValidationError> validationErrorList = new List<QHPValidationError>();
            switch (formDesignVersionEffectiveDate.Year.ToString())
            {
                case "2016":
                     List<Version2016.QhpDocumentViewModel> qhpDocumentVersion2016List = (from q in qhpDocumentsList
                                                                                         select new Version2016.QhpDocumentViewModel
                                                                                         {
                                                                                             DocumentData = q.DocumentData,
                                                                                             DocumentName = q.DocumentName
                                                                                         }).ToList();
                    Ver2016.Validation.DocumentValidator validator2016 = new Version2016.Validation.DocumentValidator(qhpDocumentVersion2016List);
                    validationErrorList = (from v in validator2016.ValidateDocuments()
                                           select new QHPValidationError
                                           {
                                               ErrorCode = v.ErrorCode,
                                               ErrorMessage = v.ErrorMessage,
                                               ErrorType = v.ErrorType
                                           }).ToList();
                    break;
                default:
                    //Version2015.DocumentExporter.DocumentExporter2014BenefitPackageTemplatebuilder = new Version2015.DocumentExporter.DocumentExporter2014BenefitPackageTemplate(folderPath, qhpDocumentsList);
                    //filePath = builder.ExportToExcel();
                    ////TODO: commented out temporarily
                    //xmlGenerator = new PlanBenefit2014XmlGenerator();
                    //PlanBenefitTemplateVO xmlPackage = xmlGenerator.GenerateXmlFromPlanBenefitPackages(builder.GetPlanBenefitPackages());
                    //XmlExportWriter<PlanBenefitTemplateVO> writer = new XmlExportWriter<PlanBenefitTemplateVO>();
                    //writer.SaveXml(xmlPackage, filePath.Replace(".zip", ".xml"));
                    break;
            }
            return validationErrorList;
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
