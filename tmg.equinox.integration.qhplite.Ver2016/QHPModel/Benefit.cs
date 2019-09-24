using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class Benefit
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public BenefitInformation BenefitInformation { get; set; }
    }
}
