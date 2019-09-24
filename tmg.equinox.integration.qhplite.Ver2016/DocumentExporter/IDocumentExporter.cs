using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.DocumentExporter
{
    public interface IDocumentExporter
    {
        string ExportToExcel();
        List<PlanBenefitPackage> GetPlanBenefitPackages();
    }
}
