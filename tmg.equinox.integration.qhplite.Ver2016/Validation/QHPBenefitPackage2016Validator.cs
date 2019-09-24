using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    public class QHPBenefitPackage2016Validator : QHPPackageValidatorBase2016
    {
         public QHPBenefitPackage2016Validator(PlanBenefitPackage package)
        {
            _package = package;
        }

        public override List<QHPValidationError> ValidateQHPPackage(PlanBenefitPackage package)
        {
            List<QHPValidationError> errors = new List<QHPValidationError>();
            ValidateForNoPlans(ref errors);
            if (errors.Count == 0)
            {
                ValidatePackageHeader(ref errors);
                ValidatePlans(ref errors);
                ValidateBenefits(ref errors);
                ValidateCostShares(ref errors);
            }
            return errors;
        }
    }
}
