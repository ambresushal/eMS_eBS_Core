using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Version2016 = tmg.equinox.integration.qhplite.Ver2016;

namespace tmg.equinox.integration.qhplite.DocumentBuilder
{
    public class DocumentBuilderFactory
    {
        public IList<QhpDocumentViewModel> BuildDocumentsFromQHPFile(int formDesignVersionId, DateTime formDesignVersionEffectiveDate, string defaultJSON, string qhpFilePath)
        {
            IList<QhpDocumentViewModel> documents = new List<QhpDocumentViewModel>();
            switch (formDesignVersionEffectiveDate.Year.ToString())
            {
                case "2016":
                    Version2016.DocumentBuilder.DocumentBuilder2016BenefitPackageTemplate builder2016 = new Version2016.DocumentBuilder.DocumentBuilder2016BenefitPackageTemplate();
                    documents = (from q in builder2016.BuildDocuments(qhpFilePath, defaultJSON)
                                 select new QhpDocumentViewModel
                                 {
                                     DocumentName = q.DocumentName,
                                     DocumentData = q.DocumentData
                                 }).ToList();
                    break;
                default:
                    //builder = new DocumentBuilder2014BenefitPackageTempalte();
                    //documents = builder.BuildDocuments(qhpFilePath, defaultJSON);
                    break;
            }
            return documents;
        }
    }
}