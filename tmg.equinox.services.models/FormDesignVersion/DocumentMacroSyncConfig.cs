using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public static class DocumentMacroSyncConfig
    {

        private static Dictionary<string, List<string>> ProductKeys = null;
        public static List<string> GetConfiguredRepeaterKeys(string repeaterPath)
        {
            if (ProductKeys == null)
            {
                InitProductKeys();
            }
            if(ProductKeys.ContainsKey(repeaterPath))
                return ProductKeys[repeaterPath];
            return new List<string>();
        }

        private static void InitProductKeys()
        {
            ProductKeys = new Dictionary<string, List<string>>();
            ProductKeys.Add("ProductDefinition.FacetsProductInformation.FacetProductComponentsPDBC.PDBCPrefixList", new string[] { "PDBCType" }.ToList());
            ProductKeys.Add("BenefitSetNetwork.NetworkList", new string[] { "BenefitSetName" }.ToList());
            ProductKeys.Add("BenefitSetNetwork.FacetsVariableComponentPDVC.FacetProductVariableComponent", new string[] { "BenefitSet", "ProductVariableComponentTier", "ProductVariableComponentType", "SequenceNumber" }.ToList());
            ProductKeys.Add("ServiceGroup.ServiceGrouping", new string[] { "ServiceGroupHeader" }.ToList());
            ProductKeys.Add("ServiceGroup.ServiceGroupingDetails", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("ServiceGroup.AltRuleServiceGroupDetail", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowServiceGroup.ShadowServiceGrouping", new string[] { "ServiceGroupHeader" }.ToList());
            ProductKeys.Add("ShadowServiceGroup.ShadowServiceGroupDetail", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowServiceGroup.AltRuleShadowServiceGroupDetail", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("AdditionalServices.AdditionalServicesList", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("AdditionalServices.AdditionalServicesDetails", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("AdditionalServices.AltRuleAdditionalServicesDetails", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowAdditionalServices.ShadowAdditionalServicesList", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowAdditionalServices.ShadowAdditionalServicesDetails", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowAdditionalServices.AltruleShadowAdditionalServicesDetails", new string[] { "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "BenefitSetName" }.ToList());
            ProductKeys.Add("Deductibles.DeductiblesList", new string[] { "AccumNumber", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowDeductibles.ShadowDeductiblesList", new string[] { "AccumNumber", "BenefitSetName" }.ToList());
            ProductKeys.Add("Limits.LimitsList", new string[] { "AccumNumber", "BenefitSetName" }.ToList());
            ProductKeys.Add("Limits.FacetsLimits.LimitRulesLTLT", new string[] { "AccumNumber", "BenefitSetName" }.ToList());
            ProductKeys.Add("Limits.FacetsLimits.LimitServicesLTSE", new string[] { "AccumNumber", "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID" }.ToList());
            ProductKeys.Add("Limits.FacetsLimits.LimitProcedureTableLTIP", new string[] { "AccumNumber", "BenefitSetName", "RelatedProcedureCodeHigh", "RelatedProcedureCodeLow" }.ToList());
            ProductKeys.Add("Limits.FacetsLimits.LimitDiagnosisTableLTID", new string[] { "AccumNumber", "BenefitSetName", "RelatedDiagnosisCodeRelation", "RelatedDiagnosisType" }.ToList());
            ProductKeys.Add("Limits.FacetsLimits.LimitProviderTableLTPR", new string[] { "AccumNumber", "BenefitSetName", "ProviderType" }.ToList());
            ProductKeys.Add("ShadowLimits.ShadowLimitsList", new string[] { "AccumNumber", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowLimits.FacetsLimits.ShadowLimitRulesLTLT", new string[] { "AccumNumber", "BenefitSetName" }.ToList());
            ProductKeys.Add("ShadowLimits.FacetsLimits.ShadowLimitServicesLTSE", new string[] { "AccumNumber", "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID" }.ToList());
            ProductKeys.Add("ShadowLimits.FacetsLimits.ShadowLimitProcedureTableLTIP", new string[] { "AccumNumber", "BenefitSetName", "RelatedProcedureCodeHigh", "RelatedProcedureCodeLow" }.ToList());
            ProductKeys.Add("ShadowLimits.FacetsLimits.ShadowLimitDiagnosisTableLTID", new string[] { "AccumNumber", "BenefitSetName", "RelatedDiagnosisCodeRelation", "RelatedDiagnosisType" }.ToList());
            ProductKeys.Add("ShadowLimits.FacetsLimits.ShadowLimitProviderTableLTPR", new string[] { "AccumNumber", "BenefitSetName", "ProviderType" }.ToList());
            ProductKeys.Add("BenefitReview.BenefitReviewGrid", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID" }.ToList());
            ProductKeys.Add("BenefitReview.BenefitReviewGridTierData", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "TierNo" }.ToList());
            ProductKeys.Add("BenefitReview.BenefitReviewAltRulesGrid", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID" }.ToList());
            ProductKeys.Add("BenefitReview.BenefitReviewAltRulesGridTierData", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "TierNo" }.ToList());
            ProductKeys.Add("ShadowBenefitReview.ShadowBenefitReviewGrid", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID" }.ToList());
            ProductKeys.Add("ShadowBenefitReview.ShadowBenefitReviewGridTierData", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "TierNo" }.ToList());
            ProductKeys.Add("ShadowBenefitReview.ShadowBenefitReviewAltRulesGrid", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID" }.ToList());
            ProductKeys.Add("ShadowBenefitReview.ShadowBenefitReviewAltRulesGridTierData", new string[] { "BenefitSetName", "BenefitCategory1", "BenefitCategory2", "BenefitCategory3", "PlaceofService", "SESEID", "TierNo" }.ToList());
            ProductKeys.Add("BenefitSummary.BenefitSummaryTable", new string[] { "BenefitSummaryType", "BenefitSummaryDescription" }.ToList());
            ProductKeys.Add("BenefitSummary.BenefitSummaryText", new string[] { "BenefitSummaryType" }.ToList());
            ProductKeys.Add("BenefitSummary.BenefitSummaryDetailTable", new string[] { "DetailsType" }.ToList());
            ProductKeys.Add("BenefitSummary.AccumulationDataforDeductiblesandLimits", new string[] { "BenefitSetName", "TypeofAccumulator", "AccumNumber" }.ToList());
        }
    }
}
