	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	
	namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
	{
	    public class DrugEHBDeductible
	    {
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BF")]
	        public string InNetworkIndividual { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BG")]
	        public string InNetworkFamily { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BH")]
	        public string InNetworkDefaultCoinsurance { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BI")]
	        public string InNetworkTier2Individual { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BJ")]
	        public string InNetworkTier2Family { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BK")]
	        public string InNetworkTier2DefaultCoinsurance { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BL")]
	        public string OutOfNetworkIndividual { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BM")]
	        public string OutOfNetworkFamily { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BN")]
	        public string CombinedInOutNetworkIndividual { get; set; }
	        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BO")]
	        public string CombinedInOutNetworkFamily { get; set; }
	    }
	}
