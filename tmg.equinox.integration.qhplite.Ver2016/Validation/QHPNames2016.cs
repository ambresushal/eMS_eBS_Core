using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    public class QHPNames2016
    {
        private static List<string> _issuerStates = new List<string> 
        { "AL","AK","AZ","AR","CA","CO","CT","DE","FL","GA","HI","ID","IL","IN","IA","KS","KY","LA","ME","MD","MA",
          "MI","MN","MS","MO","MT","NE","NV","NH","NJ","NM","NY","NC","ND","OH","OK","OR","PA","RI","SC","SD", 
          "TN","TX","UT","VT","VA","WA","WV","WI","WY"};
        public static List<string> IssuerStates
        {
            get
            {
                return _issuerStates;
            }
        }

        private static List<string> _marketCoverages = new List<string> { "Individual", "SHOP (Small Group)" };
        public static List<string> MarketCoverages
        {
            get
            {
                return _marketCoverages;
            }
        }

        private static List<string> _yesNo = new List<string> { "Yes", "No" };
        public static List<string> YesNo
        {
            get
            {
                return _yesNo;
            }
        }

        private static List<string> _invalidTins = new List<string> { "07", "08", "09", "17", "18", "19", "28", "29", "49", "69", "70", "78", "79", "89", "96", "97" };
        public static List<string> InvalidTins
        {
            get
            {
                return _invalidTins;
            }
        }

        private static List<string> _newExistingPlans = new List<string> { "New", "Existing" };
        public static List<string> NewExistingPlans
        {
            get
            {
                return _newExistingPlans;
            }
        }

        private static List<string> _planTypes = new List<string> { "Indemnity", "PPO", "HMO", "POS", "EPO" };
        public static List<string> PlanTypes
        {
            get
            {
                return _planTypes;
            }
        }

        private static List<string> _metalLevels = new List<string> { "Bronze", "Silver", "Gold", "Platinum", "Catastrophic" };
        public static List<string> MetalLevels
        {
            get
            {
                return _metalLevels;
            }
        }

        private static List<string> _metalLevelsDentalOnly = new List<string> { "High", "Low" };
        public static List<string> MetalLevelsDentalOnly
        {
            get
            {
                return _metalLevelsDentalOnly;
            }
        }

        private static List<string> _qhpNonQhp = new List<string> { "On the Exchange", "Off the Exchange", "Both" };
        public static List<string> QhpNonQhp
        {
            get
            {
                return _qhpNonQhp;
            }
        }

        private static List<string> _childOnlyOfferings = new List<string> { "Allows Adult and Child-Only", "Allows Adult-Only", "Allows Child-Only" };
        public static List<string> ChildOnlyOfferings
        {
            get
            {
                return _childOnlyOfferings;
            }
        }

        private static List<string> _diseasePrograms = new List<string> { "Asthma", "Heart Disease", "Depression", "Diabetes", "High Blood Pressure & High Cholesterol", "Low Back Pain", "Pain Management", "Pregnancy" };
        public static List<string> DiseasePrograms
        {
            get
            {
                return _diseasePrograms;
            }
        }

        private static List<string> _guaranteedVsEstimatedRate = new List<string> { "Guaranteed Rate", "Estimated Rate" };
        public static List<string> GuaranteedVsEstimatedRate
        {
            get
            {
                return _guaranteedVsEstimatedRate;
            }
        }

    }
}
