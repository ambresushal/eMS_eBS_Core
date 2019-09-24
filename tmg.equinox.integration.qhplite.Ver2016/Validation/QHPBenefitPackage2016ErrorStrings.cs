using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    internal class QHPBenefitPackage2016ErrorStrings
    {
        internal const string IssuerIDInvalid = "Invalid, enter a 5-digit Issuer ID";
        internal const string IssuerIDNotSync = "Invalid, Issuer ID must be the same on all benefit packages";
        internal const string FromListInvalid = "Invalid, select from the list";
        internal const string IssuerStateNotSync = "Invalid, State must be the same on all benefit packages";
        internal const string NoPlans = "There are no plans";
        internal const string MarketCoverageNotSync = "Invalid, Market must be the same on all benefit packages";
        internal const string DentalOnlyNotSync = "Invalid, Dental Only Indicator must be the same on all benefit packages";
        internal const string TINNotSync = "Invalid, TIN must be the same on all benefit packages";
        internal const string TINInvalid = "Invalid, enter a valid TIN number";
        internal const string DuplicatePlanId = "Duplicate Plan ID";
        internal const string PlanIDInvalid = "Invalid, enter a valid Plan ID";
        internal const string MarketingNameInvalid = "Invalid, enter a Plan Marketing Name";
        internal const string ProductIDInvalid = "Invalid, enter a valid Product ID";
        internal const string ProductIDNoPlanIDInvalid = "Invalid, the Product ID must include the Plan ID";
        internal const string HPIDInvalid = "Invalid, enter a valid HPID";
        internal const string NetworkIDInvalid = "Invalid, enter a valid Network ID";
        internal const string ServiceAreaIDInvalid = "Invalid, enter a valid Service Area ID";
        internal const string FormularyIDInvalid = "Invalid, enter a valid Formulary ID";
        internal const string ReferralRequiredInvalid = "Invalid, if a Referral is Required for Specialists, you must list the Specialist(s) requiring referrals";
        internal const string ReferralNotRequiredInvalid = "Invalid, if a Referral Required for Specialists is No, then this field must be blank";
        internal const string LimitedCSPVNumericInvalid = "Invalid, enter a numeric value";
        internal const string LimitedCSPVCatastrophicInvalid = "Invalid, must be $0 for Catastrophic plans";
        internal const string HSAHRAContribInvalid = "Invalid, this field is only required if market is Small Group";
        internal const string HSAHRAContribSmallGroupInvalid = "Invalid, this field is required if market is Small Group, select from list";
        internal const string HSAHRAContribAmountYesInvalid = "Invalid, this field is required if HSA/HRA Employer Contribution is Yes, enter a whole dollar amount";
        internal const string HSAHRAContribAmountNoInvalid = "Invalid, this field is only required if HSA/HRA Employer Contribution is Yes";
        internal const string ChildOnlyOfferingInvalid = "Invalid, Stand Alone Dental plans cannot have Adult-Only enrollment";
        internal const string ChildOnlyPlanIDRequiredInvalid = "Invalid, you must give the Child Only Plan ID when Child-Only Offering is 'Allows Adult-Only'";
        internal const string ChildOnlyPlanIDNoMatchInvalid = "Invalid, the Adult-Only Plan ID cannot be used for the Child-Only Plan ID";
        internal const string ChildOnlyPlanIDNotRequiredInvalid = "Invalid, must be blank if this is a Child Only Plan";
        internal const string ChildOnlyPlanIDNotRequiredCOInvalid = "Invalid, must be blank if Child Only Enrollment is Allowed";
        internal const string ChildOnlyPlanIDInPlansInvalid = "Invalid, the Child Only Plan ID must be a Plan ID in this benefits package";
        internal const string DiseaseProgramsInvalid = "is an invalid value, select from the list";
        internal const string EHBApportionmentForPediatricDentalReqInvalid = "Invalid, a dollar amount is required if this plan is a Stand Alone Dental plan";
        internal const string EHBApportionmentForPediatricDentalNotReqInvalid = "Invalid, this field is only required for Stand Alone Dental plans";
        internal const string GuaranteedVsEstimatedReqInvalid = "Invalid, this field is required for Stand Alone Dental plans, select from list";
        internal const string GuaranteedVsEstimatedNotReqInvalid = "Invalid, this field is required only for Stand Alone Dental plans";
        internal const string CoinsForSpecialityDrugsInvalid = "Invalid, enter a whole dollar amount";
        internal const string AVCalculatorDesignInvalid = "Invalid, enter a number 1-10 only";
        internal const string PlanEffectiveDateInvalid = "Invalid, enter a valid date";
        internal const string PlanEffectiveDate0101Invalid = "Invalid, QHP's must have a Plan Effective date of Jan 1st";

    }
}
