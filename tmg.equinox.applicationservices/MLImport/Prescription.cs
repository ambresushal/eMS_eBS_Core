using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.MLImport
{
    public enum Prescription
    {
        EffectiveDate = 0,
        TermDate = 1,
        StateRegion = 2,
        Plan = 3,
        PlanName = 4,

        PreferredNetworkNominalCopay = 5,
        NonPreferredNetworkNominalCopay = 6,        

        DeductibleTiers = 7,
        ValueDeductible = 8,        

        CoverageYN = 9,
        PreICLDrugCoveragethroughGap = 10,
        Coverageinthegap = 11,

        GapPreferredRxCostShareAverage = 12,
        GapNonPreferredRxCostShareAverage = 13,
        MailOrderMultiplier = 14,

        SupplementaldrugsCoverageYN = 15,
        SupplementaldrugNames = 16,
        SupplementaldrugTiers = 17,

        FormularyName = 18,
        Type = 19,

        PreferredRxCostShareAverage = 20,
        NonPreferredRxCostShareAverage = 21,

        OTCSteptherapy = 22,
        ExcludedDrugsEnhancedAlternativeONLY = 23,


        PreferredRetailCostSharing1Month = 24,
        PreferredRetailCostSharing3Month = 25,

        StandardRetailCostSharing1Month = 26,
        StandardRetailCostSharing3Month = 27,

        StandardMailOrderCostSharing1Month = 28,
        StandardMailOrderCostSharing3Month = 29,

        PreferredMailOrderCostSharing1Month = 30,
        PreferredMailOrderCostSharing3Month = 31,

        OutofNetworkPharmacy = 32,
        LongTermCarePharmacy = 33,

        GapCoveragePreferredRetailCostSharing1Month = 34,
        GapCoveragePreferredRetailCostSharing3Month = 35,

        GapCoverageStandardRetailCostSharing1Month = 36,
        GapCoverageStandardRetailCostSharing3Month = 37,

        GapCoverageStandardMailOrderCostSharing1Month = 38,
        GapCoverageStandardMailOrderCostSharing3Month = 39,

        GapCoveragePreferredMailOrderCostSharing1Month = 40,
        GapCoveragePreferredMailOrderCostSharing3Month = 41,

        GapCoverageOutofNetworkPharmacy = 42,
        GapCoverageLongTermCarePharmacy = 43,
        TypeofDrugs = 44,
        PrescriptionTier = 45,
        TierDescription = 46,
    };

}
