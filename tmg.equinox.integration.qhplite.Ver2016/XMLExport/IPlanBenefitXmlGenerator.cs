using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.XMLExport
{
    interface IPlanBenefitXmlGenerator
    {
        PlanBenefitTemplateVO GenerateXmlFromPlanBenefitPackages(List<PlanBenefitPackage> packages);
    }
}
