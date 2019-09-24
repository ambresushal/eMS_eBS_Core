using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.sbccalculator.Model
{
    public class CoverageExample
    {
        public string Sequence { get; set; }
        public string ReferenceID { get; set; }
        public string CoverageType { get; set; }
        public string DateofService { get; set; }
        public string ICD9DiagnosisCode { get; set; }
        public string ICD10DiagnosisCode { get; set; }
        public string BillingCode { get; set; }
        public string ProviderType { get; set; }
        public string Category { get; set; }
        public string AllowedAmount { get; set; }
        public string CalculatedAllowedAmount { get; set; }
        public string CostShareApplies { get; set; }
        public string BenefitServiceCode { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string Deductible{get; set;}
        public string Copay { get; set; }
        public string Coinsurance{get; set;}
        public string ProcessingRule{get; set;}
        public string MemberCostDeductible{get; set;}
        public string MemberCostCopay{get; set;}
        public string MemberCostCoinsurance{get; set;}
        public string RowIDProperty{get; set;}
        public string RxService { get; set; }
        public string RxTierType { get; set; }
        public string Covered { get; set; }
        public string RemainingDeductible { get; set; }
        public string ManualOverride { get; set; }
        public string Excluded { get; set; }
    }
}
