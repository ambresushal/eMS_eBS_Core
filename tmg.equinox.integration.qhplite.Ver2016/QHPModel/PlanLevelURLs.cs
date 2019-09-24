using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanLevelURLs
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AH")]
        public string URLforEnrollmentPayment { get; set; }        
    }
}
