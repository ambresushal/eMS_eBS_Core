using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.ExitValidate
{
    public class ExitValidatePlanAreaFileMapping
    {
        public Dictionary<string, string> _dictPBPRegions = new Dictionary<string, string>();
        public Dictionary<string, string> _dictPBPPlans = new Dictionary<string, string>();
        public Dictionary<string, string> _dictPlanAreas = new Dictionary<string, string>();

        public ExitValidatePlanAreaFileMapping()
        {
            this._dictPBPRegions.Add("PBP_A_CONTRACT_NUMBER", "SectionA.SectionA1.ContractNumber");
            this._dictPBPRegions.Add("PBP_A_PLAN_IDENTIFIER", "SectionA.SectionA1.PlanID");
            this._dictPBPRegions.Add("PBP_A_SEGMENT_ID", "SectionA.SectionA1.SegmentID");
            this._dictPBPRegions.Add("PBP_A_SQUISH_ID", "SectionA.AdditionalFields.PBPASQUISHID");
            this._dictPBPRegions.Add("PBP_A_REGION_TYPE", "");
            this._dictPBPRegions.Add("PBP_A_REGION_CODE", "SectionA.SectionA1.OrganizationType");
            this._dictPBPRegions.Add("PBP_A_REGION_NAME", "SectionA.SectionA1.PlanGeographicName");
            this._dictPBPRegions.Add("PBP_A_SERVICE_AREA", "");
            this._dictPBPPlans.Add("PBP_A_CONTRACT_NUMBER", "SectionA.SectionA1.ContractNumber");
            this._dictPBPPlans.Add("PBP_A_PLAN_IDENTIFIER", "SectionA.SectionA1.PlanID");
            this._dictPBPPlans.Add("PBP_A_SEGMENT_ID", "SectionA.SectionA1.SegmentID");
            this._dictPBPPlans.Add("PBP_A_SQUISH_ID", "SectionA.AdditionalFields.PBPASQUISHID");
            //this._dictPBPPlans.Add("PBP_A_SEGMENT_NAME", "SectionA.SectionA1.SegmentName");
            //this._dictPBPPlans.Add("PBP_A_PLAN_GEOG_NAME", "SectionA.SectionA1.PlanGeographicName");
            //this._dictPBPPlans.Add("PBP_A_PLAN_NAME", "SectionA.SectionA1.PlanName");
            this._dictPBPPlans.Add("PBP_A_PLAN_TYPE", "SectionA.SectionA1.PlanType");
            //this._dictPBPPlans.Add("PBP_A_ORG_TYPE", "SectionA.SectionA1.OrganizationType");
            //this._dictPBPPlans.Add("PBP_A_ORG_NAME", "SectionA.SectionA1.OrganizationLegalName");
            //this._dictPBPPlans.Add("PBP_A_ORG_MARKETING_NAME", "SectionA.SectionA1.OrganizationMarketingName");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_CURMBR_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_CURMBR_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_CUR_LOC_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_CUR_LOC_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_PROMBR_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_PROMBR_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_PRO_LOC_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_TTYTDD_PRO_LOC_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_CURMBR_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_CURMBR_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_CURMBR_LOC_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_CURMBR_LOC_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_PROMBR_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_PROMBR_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_PROMBR_LOC_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_PROMBR_LOC_PHONE_EXT", "");
            this._dictPBPPlans.Add("PBP_A_PD_CURMBR_PHONE", "SectionA.SectionA3.CustomerServiceContactPhoneNumberforCurrentPartDMedicareBeneficiaries");
            this._dictPBPPlans.Add("PBP_A_PD_CURMBR_PHONE_EXT", "SectionA.SectionA3.PNCPMBExtension");
            this._dictPBPPlans.Add("PBP_A_PD_CURMBR_LOC_PHONE", "SectionA.SectionA3.CustomerServiceContactLocalPhoneNumberforCurrentPartDMedicareBeneficia");
            this._dictPBPPlans.Add("PBP_A_PD_CURMBR_LOC_PHONE_EXT", "SectionA.SectionA3.LPNCPMBExtension");
            this._dictPBPPlans.Add("PBP_A_PD_PROMBR_PHONE", "SectionA.SectionA3.CustomerServiceContactPhoneNumberforProspectivePartDMedicareBeneficiar");
            this._dictPBPPlans.Add("PBP_A_PD_PROMBR_PHONE_EXT", "SectionA.SectionA3.LPNPPMB");
            this._dictPBPPlans.Add("PBP_A_PD_PROMBR_LOC_PHONE", "SectionA.SectionA4.CustomerServiceContactLocalPhoneNumberforProspectivePartDMedicareBenef");
            this._dictPBPPlans.Add("PBP_A_PD_PROMBR_LOC_PHONE_EXT", "SectionA.SectionA4.LPPPMBExtension");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_CURMBR_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_CURMBR_PHN_EXT", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_CUR_LOC_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_CUR_LOC_PHN_EX", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_PROMBR_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_PROMBR_PHN_EXT", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_PRO_LOC_PHONE", "");
            this._dictPBPPlans.Add("PBP_A_PD_TTYTDD_PRO_LOC_PHN_EX", "");
            this._dictPBPPlans.Add("PBP_DELETE_FLAG", "");
            this._dictPBPPlans.Add("PBP_A_EGHP_YN", "");
            this._dictPBPPlans.Add("PBP_A_SERVICE_HOURS", "");
            this._dictPBPPlans.Add("PBP_A_SERVICE_HOURS_2", "");
            this._dictPBPPlans.Add("PBP_A_SPECIAL_NEED_FLAG", "SectionA.SectionA2.IsthisaSpecialNeedsPlan");
            this._dictPBPPlans.Add("PBP_A_SPECIAL_NEED_PLAN_TYPE", "SectionA.SectionA2.SpecialNeedsPlanType");
            this._dictPBPPlans.Add("PBP_A_SNP_INSTITUTIONAL_TYPE", "SectionA.SectionA2.SpecialNeedsInstitutionalType");
            this._dictPBPPlans.Add("PBP_A_NATIONAL_YN", "");
            this._dictPBPPlans.Add("MRX_DRUG_BEN_YN", "SectionA.AdditionalFields.DoesyourplanofferaMedicarePrescriptiondrugPartDRXbenefit");
            this._dictPBPPlans.Add("PBP_A_ORG_WEBSITE", "SectionA.SectionA1.OrganizationWebSite");
            this._dictPBPPlans.Add("PBP_A_NETWORK_FLAG", "SectionA.SectionA1.Isthisanetworkplan");
            this._dictPBPPlans.Add("PBP_A_SNP_COND", "SectionA.SectionA2.SpecialNeedsPlanType");
            this._dictPBPPlans.Add("PBP_A_SNP_PCT", "SectionA.SectionA2.SpecialNeedsInstitutionalType");
            this._dictPBPPlans.Add("PBP_A_DSNP_ZERODOLLAR", "SectionA.SectionA2.IsthisDSNPplanaMedicarezerodollarcostsharingplanthisdoesnotapplytoPart");
            this._dictPBPPlans.Add("PBP_A_PHYS_WEB_ADDR", "SectionA.SectionA3.PhysicianWebsiteAddress");
            this._dictPBPPlans.Add("PBP_A_FORMULARY_WEB_ADDR", "SectionA.SectionA3.FormularyWebsiteAddress");
            this._dictPBPPlans.Add("PBP_A_CONTRACT_PARTD_FLAG", "");
            this._dictPBPPlans.Add("PBP_A_PLATINO_FLAG", "");
            this._dictPBPPlans.Add("PBP_A_PHARMACY_WEBSITE", "SectionA.SectionA3.ParticipatingPharmacyWebsiteAddress");
            this._dictPlanAreas.Add("PBP_A_CONTRACT_NUMBER", "SectionA.SectionA1.ContractNumber");
            this._dictPlanAreas.Add("PBP_A_PLAN_IDENTIFIER", "SectionA.SectionA1.PlanID");
            this._dictPlanAreas.Add("PBP_A_SEGMENT_ID", "SectionA.SectionA1.SegmentID");
            this._dictPlanAreas.Add("PBP_A_SQUISH_ID", "SectionA.AdditionalFields.PBPASQUISHID");
            this._dictPlanAreas.Add("PBP_A_SERVICE_AREA", "");
            this._dictPlanAreas.Add("PBP_A_COUNTY_CODE", "");
            this._dictPlanAreas.Add("PBP_A_REGION_TYPE", "");
            this._dictPlanAreas.Add("PBP_A_REGION_CODE", "SectionA.SectionA1.OrganizationType");
            this._dictPlanAreas.Add("PBP_A_COUNTY_NAME", "");
            this._dictPlanAreas.Add("PBP_A_STATE_CODE", "");
            this._dictPlanAreas.Add("PBP_A_STATEWIDE_FLAG", "");
        }

        public string GetPBPRegions(Dictionary<string, string> dict)
        {
            string pbpRegionQuery = "INSERT INTO pbpregions (";
            string ColumnList = string.Join(",", dict.Select(x => x.Key).ToArray());
            string ValuePathList = string.Join(",", dict.Select(x => string.Concat("'", string.Concat(x.Value.Replace("'","''"), "'"))).ToArray());

            pbpRegionQuery = string.Concat(pbpRegionQuery, ColumnList);
            pbpRegionQuery = string.Concat(pbpRegionQuery, ") VALUES (");
            pbpRegionQuery = string.Concat(pbpRegionQuery, ValuePathList);
            pbpRegionQuery = string.Concat(pbpRegionQuery, ")");

            return pbpRegionQuery;
        }

        public string GetPBPPlans(Dictionary<string, string> dict)
        {
            string pbpRegionQuery = "INSERT INTO pbpplans (";
            string ColumnList = string.Join(",", dict.Select(x => x.Key).ToArray());
            string ValuePathList = string.Join(",", dict.Select(x => string.Concat("'", string.Concat(x.Value, "'"))).ToArray());

            pbpRegionQuery = string.Concat(pbpRegionQuery, ColumnList);
            pbpRegionQuery = string.Concat(pbpRegionQuery, ") VALUES (");
            pbpRegionQuery = string.Concat(pbpRegionQuery, ValuePathList);
            pbpRegionQuery = string.Concat(pbpRegionQuery, ")");

            return pbpRegionQuery;
        }

        public string GetPlanAreas(Dictionary<string, string> dict)
        {
            string pbpRegionQuery = "INSERT INTO PLAN_AREAS (";
            string ColumnList = string.Join(",", dict.Select(x => x.Key).ToArray());
            string ValuePathList = string.Join(",", dict.Select(x => string.Concat("'", string.Concat(x.Value, "'"))).ToArray());

            pbpRegionQuery = string.Concat(pbpRegionQuery, ColumnList);
            pbpRegionQuery = string.Concat(pbpRegionQuery, ") VALUES (");
            pbpRegionQuery = string.Concat(pbpRegionQuery, ValuePathList);
            pbpRegionQuery = string.Concat(pbpRegionQuery, ")");

            return pbpRegionQuery;
        }

    }
}
