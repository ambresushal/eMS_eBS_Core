using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.DocumentExporter;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    public class DocumentValidator
    {
        List<QhpDocumentViewModel> _documents;
        private List<PlanBenefitPackage> _planBenefitPackageList { get; set; }

        public DocumentValidator(List<QhpDocumentViewModel> documents)
        {
            _documents = documents;
        }

        public List<QHPValidationError> ValidateDocuments()
        {
            List<QHPValidationError> errors = new List<QHPValidationError>();
            List<PlanBenefitPackage> packages = GetPlanBenefitPackages(_documents);
            DocumentValidator2016BenefitPackageTemplate validator = new DocumentValidator2016BenefitPackageTemplate(packages);
            errors = validator.ValidateQHPDocument();
            return errors;
        }

        private List<PlanBenefitPackage> GetPlanBenefitPackages(List<QhpDocumentViewModel> qhpDocumentList)
        {
            if (_planBenefitPackageList == null)
            {
                _planBenefitPackageList = new List<PlanBenefitPackage>();
                PlanBenefitPackageMapper mapper = new PlanBenefitPackageMapper();
                foreach (var document in _documents)
                {
                    PlanBenefitPackage package = mapper.GetBenefitPackage(document.DocumentName, document.DocumentData);
                    _planBenefitPackageList.Add(package);
                }
            }
            return _planBenefitPackageList.ToList();
        }
    }

}
