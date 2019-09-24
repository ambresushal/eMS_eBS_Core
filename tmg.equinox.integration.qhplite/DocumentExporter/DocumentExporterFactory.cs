using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Version2016 = tmg.equinox.integration.qhplite.Ver2016;

namespace tmg.equinox.integration.qhplite.DocumentExporter
{
    public class DocumentExporterFactory
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public DocumentExporterFactory()
        {

        }
        #endregion Constructor

        #region Public Methods
        public string GetQHPXmlFromFolderVersion(IList<QhpDocumentViewModel> qhpDocumentsList, DateTime effectiveDate)
        {
            string xmlOutput = "";

            IList<Version2016.QhpDocumentViewModel> qhpDocumentVersion2016List = (from q in qhpDocumentsList
                                                                                  select new Version2016.QhpDocumentViewModel
                                                                                  {
                                                                                      DocumentData = q.DocumentData,
                                                                                      DocumentName = q.DocumentName
                                                                                  }).ToList();
            Version2016.DocumentExporter.DocumentExporter2016BenefitPackageTemplate exportTemplate2016 = new Version2016.DocumentExporter.DocumentExporter2016BenefitPackageTemplate("", qhpDocumentVersion2016List);
            Version2016.XMLExport.PlanBenefit2016XmlGenerator xmlGenerator2016 = new Version2016.XMLExport.PlanBenefit2016XmlGenerator();
            Version2016.XMLExport.PlanBenefitTemplateVO planBenefitTemplate2016 = xmlGenerator2016.GenerateXmlFromPlanBenefitPackages(exportTemplate2016.GetPlanBenefitPackages());
            Version2016.XMLExport.XmlExportWriter2016<Version2016.XMLExport.PlanBenefitTemplateVO> xmlWriter = new Version2016.XMLExport.XmlExportWriter2016<Version2016.XMLExport.PlanBenefitTemplateVO>();
            xmlOutput = xmlWriter.GetXmlString(planBenefitTemplate2016);

            return xmlOutput;
        }

        public string ExportQhpFileFromDocuments(int formDesignVersionId, DateTime formDesignVersionEffectiveDate, string folderPath, IList<QhpDocumentViewModel> qhpDocumentsList)
        {
            string filePath = null;

            IList<Version2016.QhpDocumentViewModel> qhpDocumentVersion2016List = (from q in qhpDocumentsList
                                                                                  select new Version2016.QhpDocumentViewModel
                                                                                  {
                                                                                      DocumentData = q.DocumentData,
                                                                                      DocumentName = q.DocumentName
                                                                                  }).ToList();
            switch (formDesignVersionEffectiveDate.Year.ToString())
            {
                case "2020":
                    Version2016.DocumentExporter.DocumentExporter2020BenefitPackageTemplate export2020 = new Version2016.DocumentExporter.DocumentExporter2020BenefitPackageTemplate(folderPath, qhpDocumentVersion2016List);
                    filePath = export2020.ExportToExcel();
                    break;
                default:
                    Version2016.DocumentExporter.DocumentExporter2018BenefitPackageTemplate export2016 = new Version2016.DocumentExporter.DocumentExporter2018BenefitPackageTemplate(folderPath, qhpDocumentVersion2016List);
                    filePath = export2016.ExportToExcel();
                    break;
            }


            return filePath;
        }

        public string ExportURRTFileFromDocuments(int urrtFormDesignVersionId, DateTime planBenefitFormDesignVersionEffectiveDate, string folderPath, string urrtDocument, IList<QhpDocumentViewModel> qhpDocumentsList)
        {
            string filePath = null;

            IList<Version2016.QhpDocumentViewModel> qhpDocumentVersion2016List = (from q in qhpDocumentsList
                                                                                  select new Version2016.QhpDocumentViewModel
                                                                                  {
                                                                                      DocumentData = q.DocumentData,
                                                                                      DocumentName = q.DocumentName
                                                                                  }).ToList();
            tmg.equinox.integration.qhplite.Ver2016.DocumentExporter2016URRTTemplate export2016 = new tmg.equinox.integration.qhplite.Ver2016.DocumentExporter2016URRTTemplate(folderPath, urrtDocument, qhpDocumentVersion2016List);
            filePath = export2016.ExportToExcel();
            //export2015.generateXML();

            return filePath;
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
