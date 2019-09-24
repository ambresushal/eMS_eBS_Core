using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class MigrationItemMultiSelectMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item, MigrationPlanItem plan)
        {
            string targetValue = "";
            string sourcePath = item.TableName + "." + item.ColumnName.Replace(" ", "");
            string targetPath = item.DocumentPath;
            var sourceToken = source.SelectToken(sourcePath);
            var targetToken = target.SelectToken(targetPath);
            targetValue = GenerateTargetValue(sourceToken, item);
            var prop = targetToken.Parent as JProperty;

            if (item.DocumentPath == "MedicareRxGeneral.MedicareRxGeneral1.Describethecomponentsofyournetworkselectallthatapply"
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
                || (item.ColumnName== "pbp_b16a_coins_ehc" && item.TableName.Contains("STEP16A"))
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
                )
            {
                string[] targetArray = Regex.Replace(targetValue, @"\s+", "").Split(',');
                var jarr = JArray.FromObject(targetArray);
                prop = targetToken.Parent as JProperty;
                prop.Value = jarr;

                string logMessage = item.TableName + "," + item.ColumnName + "," + item.DocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
            }
            else
            {
                prop.Value = targetValue.ToString().Trim();

                string logMessage = item.TableName + "," + item.ColumnName + "," + item.DocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
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
