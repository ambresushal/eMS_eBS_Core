using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    interface IQHPValidator
    {
        List<QHPValidationError> ValidateQHPDocument(List<PlanBenefitPackage> packages);
    }
}
