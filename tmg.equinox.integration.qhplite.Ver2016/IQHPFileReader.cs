using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public interface IQHPFileReader
    {
        List<PlanBenefitPackage> GetPackagesFromFile(string fileName);
    }
}
