using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.rules.oongroups
{
    public class OONGroupTargetModel
    {
        public string EnterLabelforthisGroupOptional { get; set; }
        public string SelectthebenefitsthatapplytotheOONGroups { get; set; }
        public string SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis { get; set; }
        public string SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort { get; set; }
        public string Isthereamaximumplanbenefitcoverageamountforthisgroup { get; set; }
        public string Indicatemaximumplanbenefitcoverageamount { get; set; }
        public string IsthereanOONCoinsuranceforthisGroup { get; set; }
        public string EnterMinimumCoinsurancePercentageforthisGroup { get; set; }
        public string EnterMaximumCoinsurancePercentageforthisGroup { get; set; }
        public string IsthereanOONCopaymentforthisGroup { get; set; }
        public string EnterMinimumCopaymentAmountforthisGroup { get; set; }
        public string EnterMaximumCopaymentAmountforthisGroup { get; set; }
        public string IsthereanOONDeductibleforthisgroup { get; set; }
        public string EnterDeductibleAmountforthisgroup { get; set;}

        public double? CoinsuranceMin { get; set; }
        public double? CoinsuranceMax { get; set; }
        public double? CopayMax { get; set; }
        public double? CopayMin { get; set; }
    }
}
