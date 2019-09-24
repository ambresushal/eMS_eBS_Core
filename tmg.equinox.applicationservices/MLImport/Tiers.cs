using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.MLImport
{
    public enum Tiers
    {
        EffectiveDate = 0,
        TermDate = 1,
        StateRegion = 2,
        Plan = 3,
        PlanName = 4,

        PreferredNetworkNominalCopayTier1 = 5,
        PreferredNetworkNominalCopayTier2 = 6,
        PreferredNetworkNominalCopayTier3 = 7,
        PreferredNetworkNominalCopayTier4 = 8,
        PreferredNetworkNominalCopayTier5 = 9,

        NonPreferredNetworkNominalCopayTier1 = 10,
        NonPreferredNetworkNominalCopayTier2 = 11,
        NonPreferredNetworkNominalCopayTier3 = 12,
        NonPreferredNetworkNominalCopayTier4 = 13,
        NonPreferredNetworkNominalCopayTier5 = 14,

        DeductibleTiers = 15,
        ValueDeductible = 16,
        CoverageYN = 17,
        PreICLDrugCoveragethroughGap = 18,

        CoverageinthegapTier1 = 19,
        CoverageinthegapTier2 = 20,
        CoverageinthegapTier3 = 21,
        CoverageinthegapTier4 = 22,
        CoverageinthegapTier5 = 23,

        GapPreferredRxCostShareAverageTier1 = 24,
        GapPreferredRxCostShareAverageTier2 = 25,
        GapPreferredRxCostShareAverageTier3 = 26,
        GapPreferredRxCostShareAverageTier4 = 27,
        GapPreferredRxCostShareAverageTier5 = 28,

        GapNonPreferredRxCostShareAverageTier1 = 29,
        GapNonPreferredRxCostShareAverageTier2 = 30,
        GapNonPreferredRxCostShareAverageTier3 = 31,
        GapNonPreferredRxCostShareAverageTier4 = 32,
        GapNonPreferredRxCostShareAverageTier5 = 33,

        MailOrderMultiplier = 34,
        SupplementaldrugsCoverageYN = 35,
        SupplementaldrugNames = 36,
        SupplementaldrugTiers = 37,
        FormularyName = 38,
        Type = 39,

        PreferredRxCostShareAverageTier1 = 40,
        PreferredRxCostShareAverageTier2 = 41,
        PreferredRxCostShareAverageTier3 = 42,
        PreferredRxCostShareAverageTier4 = 43,
        PreferredRxCostShareAverageTier5 = 44,

        NonPreferredRxCostShareAverageTier1 = 45,
        NonPreferredRxCostShareAverageTier2 = 46,
        NonPreferredRxCostShareAverageTier3 = 47,
        NonPreferredRxCostShareAverageTier4 = 48,
        NonPreferredRxCostShareAverageTier5 = 49,

        OTCSteptherapy = 50,
        ExcludedDrugsEnhancedAlternativeONLY = 51,
        TypeofDrugs = 52,
        PrescriptionTier = 53,
        TierDescription = 54,


    }

}
