using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class SBCScenario
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public HavingABaby HavingABaby { get; set; }

        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public HavingDiabetes HavingDiabetes { get; set; }

        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public HavingATreatment TreatmentOfSimpleFracture { get; set; }
    }
}
