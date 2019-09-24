using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class PlanProductInfo
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, IsContainer = true)]
        public PlanProductInfoHeader PlanProductInfoHeader { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 12, IncrementDirection = IncrementDirection.Column, IncrementStep = 1, IsList = true)]
        public List<ProductInfo> ProductList { get; set; }
    }
}
