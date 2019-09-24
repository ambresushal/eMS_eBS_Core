using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class OutOfPocketExceptions
    {        
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "K")]
        public string ExcludedFromInNetworkMOOP { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "L")]
        public string ExcludedFromOutOfNetworkMOOP { get; set; }
    }
}
