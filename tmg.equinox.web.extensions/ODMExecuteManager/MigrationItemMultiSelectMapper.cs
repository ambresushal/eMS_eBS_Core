using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationItemMultiSelectMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item)
        {
            string targetValue = "";
            string sourcePath = item.TableName + "." + item.ColumnName.Replace(" ", "");
            string targetPath = item.DocumentPath;
            var sourceToken = source.SelectToken(sourcePath);
            var targetToken = target.SelectToken(targetPath);
            targetValue = GenerateTargetValue(sourceToken, item);
            var prop = targetToken.Parent as JProperty;

            if (item.DocumentPath== "PostOOPThreshold.MedicareMedicaidTierCostSharingPostOOPThreshold[0].SelectdrugtypesinthisTierselectallthatapply"
                || item.DocumentPath== "SectionDPlanLevel.MaxEnrolleCostLimitNonNetwork.SelectthebenefitsthatapplytotheMaximumEnrolleeOutofPocketcost"
                || item.DocumentPath == "MedicareRxGeneral.MedicareRxGeneral1.Describethecomponentsofyournetworkselectallthatapply"
                || item.DocumentPath == "BasicEnhancedAlternative.AlternativeEnhancedAlternativeCharacteristics.IndicatetheareasthroughoutthePartDbenefitwherethereducedPartDcostshari"
                || item.DocumentPath == "OtherMedicarecoveredPreventiveServices.OtherMedicarecoveredPreventiveServicesBase3.SelectwhichServiceshaveaCopaymentSelectallthatapply"
                || item.DocumentPath == "OtherMedicarecoveredPreventiveServices.OtherMedicarecoveredPreventiveServicesBase4.SelectwhichServicesrequireaReferralSelectallthatapply"
                || item.DocumentPath == "OONGeneral.OONGeneralBase2.SelectthebenefitsthatapplytotheOONBenefits"
                || item.DocumentPath == "OONGeneral.OONGeneralBase2.SelectalloftheMedicarecoveredServiceCategoriestowhichtheOutofNetworkbe"
                || item.DocumentPath == "OONGeneral.OONGeneralBase2.SelectalloftheNonMedicarecoveredServiceCategoriestowhichtheOutofNetwor"
                || item.DocumentPath == "DentalPreventive.PreventiveDentalBase1.Selectenhancedbenefits"
                || item.DocumentPath == "DentalPreventive.PreventiveDentalBase4.SelectwhichPreventiveDentalServiceshaveaCopaymentSelectallthatapply"
                || item.DocumentPath == "ComprehensiveDental.ComprehensiveDentalBase1.Selectenhancedbenefits"
                || item.DocumentPath == "ComprehensiveDental.ComprehensiveDentalBase5.SelectwhichComprehensiveDentalServiceshaveaCopaymentSelectallthatapply"
                || item.DocumentPath == "EyeExams.EyeExamsBase2.SelectwhichEyeExamshaveaCopaymentSelectallthatapply"
                || item.DocumentPath == "EyeExams.EyeExamsBase2.SelectwhichEyeExamshaveaCoinsuranceSelectallthatapply"
                || item.DocumentPath == "EyeExams.EyeExamsBase1.Selectenhancedbenefit"
                || item.DocumentPath == "Eyewear.EyewearBase1.Selectenhancedbenefits"
                || item.DocumentPath == "Eyewear.EyewearBase3.SelectthetypeofEyewearwithIndividualMaxPlanBenefitCoverageamount"
                || item.DocumentPath == "Eyewear.EyewearBase5.SelectwhichEyewearBenefitshaveaCopaymentSelectallthatapply"
                || item.DocumentPath == "HearingExams.HearingExamsBase1.Selectenhancedbenefits"
                || item.DocumentPath == "HearingExams.HearingExamsBase3.SelectwhichHearingExamBenefitshaveaCopaymentSelectallthatapply"
                || item.DocumentPath == "HearingAids.HearingAidsBase1.Selectenhancedbenefits"
                || item.DocumentPath == "HearingAids.HearingAidsBase4.SelectwhichHearingAidsBenefitshaveaCopaymentSelectallthatapply"
                || item.PBPFile == "pbp_mrx"
                || item.PBPFile == "pbp_mrx_gapCoverage"
                || item.PBPFile == "pbp_mrx_p"
                || item.PBPFile == "pbp_mrx_tier"
                || item.ColumnName == "pbp_c_oon_outpt_bendesc_ben"
                || item.ColumnName == "pbp_c_pos_coins_ihs_ben_type"
                || item.ColumnName == "pbp_b7b_copay_ehc"
                || item.ColumnName == "pbp_b7b_coins_ehc"
                || (item.ColumnName == "pbp_b16a_bendesc_ehc" && item.TableName.Contains("STEP16A"))
                || (item.ColumnName == "pbp_b16a_coins_ehc" && item.TableName.Contains("STEP16A"))
                || (item.ColumnName == "pbp_b16a_copay_ehc" && item.TableName.Contains("STEP16A"))
                || (item.ColumnName == "pbp_b16b_bendesc_ehc" && item.TableName.Contains("STEP16B"))
                || (item.ColumnName == "pbp_b16b_coins_ehc" && item.TableName.Contains("STEP16B"))
                || (item.ColumnName == "pbp_b16b_copay_ehc" && item.TableName.Contains("STEP16B"))
                || (item.ColumnName == "pbp_b16a_bendesc_ehc" && item.TableName.Contains("PBPB16"))
                || (item.ColumnName == "pbp_b16a_coins_ehc" && item.TableName.Contains("PBPB16"))
                || (item.ColumnName == "pbp_b16a_copay_ehc" && item.TableName.Contains("PBPB16"))
                || (item.ColumnName == "pbp_b16b_bendesc_ehc" && item.TableName.Contains("PBPB16"))
                || (item.ColumnName == "pbp_b16b_coins_ehc" && item.TableName.Contains("PBPB16"))
                || (item.ColumnName == "pbp_b16b_copay_ehc" && item.TableName.Contains("PBPB16"))                
                //|| (item.ColumnName == "pbp_d_opt_deduct_cats" && item.TableName.Contains("PBPD_OPT"))
                || (item.ColumnName == "pbp_b7b_bendesc_ehc" && item.TableName.Contains("STEP7B"))
                || (item.ColumnName == "pbp_b7b_coins_ehc" && item.TableName.Contains("STEP7B"))
                || (item.ColumnName == "pbp_b7b_comb_ben_ehc" && item.TableName.Contains("STEP7B"))
                || (item.ColumnName == "pbp_b7b_copay_ehc" && item.TableName.Contains("STEP7B"))
                || (item.ColumnName == "pbp_b7f_bendesc_rf" && item.TableName.Contains("STEP7F"))
                || (item.ColumnName == "pbp_b7f_coins_ehc" && item.TableName.Contains("STEP7F"))
                || (item.ColumnName == "pbp_b7f_copay_ehc" && item.TableName.Contains("STEP7F"))
                || (item.ColumnName == "pbp_b10b_bendesc_mt_al" && item.TableName.Contains("STEP10B"))
                || (item.ColumnName == "pbp_b10b_bendesc_mt_pal" && item.TableName.Contains("STEP10B"))
                || (item.ColumnName == "pbp_b16a_coins_cserv_sc_pov" && item.TableName.Contains("STEP16A"))
                || (item.ColumnName == "pbp_b16a_copay_cserv_sc_pov" && item.TableName.Contains("STEP16A"))
                || (item.ColumnName == "pbp_b17a_bendesc_ehc" && item.TableName.Contains("STEP17A"))
                || (item.ColumnName == "pbp_b17a_coins_ehc" && item.TableName.Contains("STEP17A"))
                || (item.ColumnName == "pbp_b17a_copay_ehc" && item.TableName.Contains("STEP17A"))
                || (item.ColumnName == "pbp_b17b_bendesc_ehc" && item.TableName.Contains("STEP17B"))
                || (item.ColumnName == "pbp_b17b_coins_ehc" && item.TableName.Contains("STEP17B"))
                || (item.ColumnName == "pbp_b17b_copay_ehc" && item.TableName.Contains("STEP17B"))
                || (item.ColumnName == "PBP_B17B_INDV_MAXPLAN_BENDESC" && item.TableName.Contains("STEP17B"))
                || (item.ColumnName == "pbp_b18a_bendesc_ehc" && item.TableName.Contains("STEP18A"))
                || (item.ColumnName == "pbp_b18a_coins_ehc" && item.TableName.Contains("STEP18A"))
                || (item.ColumnName == "pbp_b18a_copay_ehc" && item.TableName.Contains("STEP18A"))
                || (item.ColumnName == "pbp_b18b_bendesc_ehc" && item.TableName.Contains("STEP18B"))
                || (item.ColumnName == "pbp_b18b_coins_ehc" && item.TableName.Contains("STEP18B"))
                || (item.ColumnName == "pbp_b18b_copay_ehc" && item.TableName.Contains("STEP18B"))
                || (item.ColumnName == "pbp_a_tier_bendesc_bens")
                //|| (item.ColumnName == "pbp_d_opt_secb_cats") && item.TableName.Contains("PBPD_OPT")
                //|| (item.ColumnName == "pbp_d_opt_other_benefits") && item.TableName.Contains("PBPD_OPT")
                )
            {
                string[] targetArray = Regex.Replace(targetValue, @"\s+", "").Split(',');
                var jarr = JArray.FromObject(targetArray);
                prop = targetToken.Parent as JProperty;
                prop.Value = jarr;
            }
            else
            {
                prop.Value = targetValue.ToString().Trim();
            }
        }

        private string GenerateTargetValue(JToken sourceToken, MigrationFieldItem item)
        {
            string sourceValue = sourceToken.ToString();
            string targetValue = "";
            string replace = "1";
            int index = 0;
            foreach (Char code in sourceValue)
            {
                if (code == '1')
                {
                    string codePlaceHolder = "".PadRight(sourceValue.Length, '0');
                    codePlaceHolder = codePlaceHolder.Insert(index, replace);
                    targetValue = targetValue + "," + codePlaceHolder.Substring(0, codePlaceHolder.Length - 1).ToString().Trim();
                }
                index++;
            }
            return targetValue.TrimStart(',');
        }

    }

}
