using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.anocchart.Model
{
    public class InpatientHospitalStays
    {
        public string CostShareKey { get; set; }
        public string Tier { get; set; }
        public string Interval { get; set; }
        public string ThisYearBeginDay { get; set; }
        public string ThisYearEndDay { get; set; }
        public string ThisYearAmount { get; set; }
        public string NextYearBeginDay { get; set; }
        public string NextYearEndDay { get; set; }
        public string NextYearAmount { get; set; }
        public string ThisYearIntervalCount { get; set; }
        public string NextYearIntervalCount { get; set; }

        public string AdditionalDaysThisYearBeginDay { get; set; }
        public string AdditionalDaysThisYearEndDay { get; set; }
        public string AdditionalDaysThisYearAmount { get; set; }

        public string AdditionalDaysNextYearBeginDay { get; set; }
        public string AdditionalDaysNextYearEndDay { get; set; }
        public string AdditionalDaysNextYearAmount { get; set; }

        public string AdditionalDaysThisIntervalCount { get; set; }
        public string AdditionalDaysNextIntervalCount { get; set; }
    }
}
