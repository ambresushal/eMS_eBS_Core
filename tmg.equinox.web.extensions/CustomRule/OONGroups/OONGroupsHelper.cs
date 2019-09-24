using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.viewmodels;

namespace tmg.equinox.rules.oongroups
{
    public class OONGroupsHelper
    {

        public static void GetOONData(JToken formInstanceData, ref List<OONGroupEntryModel> entries)
        {
            foreach (var entry in entries)
            {
                var token = formInstanceData.SelectToken(entry.SOTFieldPath);
                if (token != null)
                {
                    string val = token.ToString();
                    if (entry.FieldSubType != "CONDITION")
                    {
                        val = val.Trim(',').Trim();
                        if (!String.IsNullOrEmpty(val))
                        {
                            if (val.Contains('%'))
                            {
                                entry.ValueType = "Percent";
                                val = val.Trim('%');
                                double numericValue;
                                if (Double.TryParse(val, out numericValue))
                                {
                                    entry.Value = Convert.ToInt32(numericValue);
                                }
                            }
                            else
                            {
                                entry.ValueType = "Value";
                                if(entry.FieldType == "PLANLIMIT" && entry.FieldSubType == "PLANMAX")
                                {
                                    try
                                    {
                                        entry.Value = string.IsNullOrWhiteSpace(val) ? Double.NaN : ExtractDoubleFromString(val.Trim());
                                    }
                                    catch(Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    val = val.Trim('$');
                                    double numericValue;
                                    if (Double.TryParse(val, out numericValue))
                                    {
                                        entry.Value = numericValue;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        entry.ConditionValue = token.ToString();
                    }
                }
            }
        }

        public static OONGroupTargetModel GenerateTargetEntry(List<OONGroupEntryModel> entries, string package, bool isPackage)
        {
            OONGroupTargetModel targetModel = new OONGroupTargetModel();
            if (isPackage == true)
            {
                targetModel.EnterLabelforthisGroupOptional = package;
            }
            else
            {
                targetModel.EnterLabelforthisGroupOptional = entries.First().BenefitGroup;
            }
            bool hasCoinsurance = OONGroupsHelper.HasCoinsurance(entries);
            if (hasCoinsurance == true)
            {
                targetModel.IsthereanOONCoinsuranceforthisGroup = "1";
            }
            else
            {
                targetModel.IsthereanOONCoinsuranceforthisGroup = "2";
            }
            bool hasCopay = OONGroupsHelper.HasCopay(entries);
            if (hasCopay == true)
            {
                targetModel.IsthereanOONCopaymentforthisGroup = "1";
            }
            else
            {
                targetModel.IsthereanOONCopaymentforthisGroup = "2";
            }

            if (hasCoinsurance == true)
            {
                double? maxCoinsurance = OONGroupsHelper.GetMaxCoinsurance(entries);
                if (maxCoinsurance.HasValue)
                {
                    targetModel.EnterMaximumCoinsurancePercentageforthisGroup = Convert.ToInt32(maxCoinsurance.Value).ToString();
                    targetModel.CoinsuranceMax = maxCoinsurance;
                }
                double? minCoinsurance = OONGroupsHelper.GetMinCoinsurance(entries);
                if (minCoinsurance.HasValue)
                {
                    targetModel.EnterMinimumCoinsurancePercentageforthisGroup = Convert.ToInt32(minCoinsurance.Value).ToString();
                    targetModel.CoinsuranceMin = minCoinsurance;
                }

            }
            if (hasCopay == true)
            {
                double? maxCopay = OONGroupsHelper.GetMaxCopay(entries);
                if (maxCopay.HasValue)
                {
                    targetModel.EnterMaximumCopaymentAmountforthisGroup = maxCopay.Value.ToString("F2");
                    targetModel.CopayMax = maxCopay;

                }
                double? minCopay = OONGroupsHelper.GetMinCopay(entries);
                if (minCopay.HasValue)
                {
                    targetModel.EnterMinimumCopaymentAmountforthisGroup = minCopay.Value.ToString("F2");
                    targetModel.CopayMin = minCopay;
                }
            }

            string[] medicareCodes = OONGroupsHelper.GetValueBenefitCodes(entries, "Medicare");
            targetModel.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = OONGroupsHelper.GenerateJSONArrayString(medicareCodes,true);

            string[] nonMedicareCodes = OONGroupsHelper.GetValueBenefitCodes(entries, "NonMedicare");
            targetModel.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes,true);

            double? deductible = OONGroupsHelper.GetDeductible(entries);
            if (deductible.HasValue)
            {
                targetModel.IsthereanOONDeductibleforthisgroup = "1";
                targetModel.EnterDeductibleAmountforthisgroup = deductible.Value.ToString("F2");
            }
            else
            {
                targetModel.IsthereanOONDeductibleforthisgroup = "2";
            }

            double? planMax = OONGroupsHelper.GetPlanMax(entries, isPackage);
            if (planMax.HasValue)
            {
                targetModel.Isthereamaximumplanbenefitcoverageamountforthisgroup = "1";
                targetModel.Indicatemaximumplanbenefitcoverageamount = planMax.Value.ToString("F2");
            }
            else
            {
                targetModel.Isthereamaximumplanbenefitcoverageamountforthisgroup = "2";
            }

            if(targetModel.IsthereanOONCoinsuranceforthisGroup == "1")
            {
                if(targetModel.CoinsuranceMin.HasValue == false)
                {
                    targetModel.CoinsuranceMin = targetModel.CoinsuranceMax;
                    targetModel.EnterMinimumCoinsurancePercentageforthisGroup = targetModel.EnterMaximumCoinsurancePercentageforthisGroup;
                }
                if(targetModel.CoinsuranceMax.HasValue == false)
                {
                    targetModel.CoinsuranceMax = targetModel.CoinsuranceMin;
                    targetModel.EnterMaximumCoinsurancePercentageforthisGroup = targetModel.EnterMinimumCoinsurancePercentageforthisGroup;
                }
            }

            if (targetModel.IsthereanOONCopaymentforthisGroup == "1")
            {
                if (targetModel.CopayMin.HasValue == false)
                {
                    targetModel.CopayMin = targetModel.CopayMax;
                    targetModel.EnterMinimumCopaymentAmountforthisGroup = targetModel.EnterMaximumCopaymentAmountforthisGroup;
                }
                if (targetModel.CopayMax.HasValue == false)
                {
                    targetModel.CopayMax = targetModel.CopayMin;
                    targetModel.EnterMaximumCopaymentAmountforthisGroup = targetModel.EnterMinimumCopaymentAmountforthisGroup;
                }
            }


            return targetModel;
        }

        public static bool HasCopay(List<OONGroupEntryModel> entries)
        {
            bool result;
            int count = entries.Where(a => a.Value != null && a.ValueType == "Value" && a.FieldType == "COSTSHARE").Count();
            if (count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        public static bool HasCoinsurance(List<OONGroupEntryModel> entries)
        {
            bool result;
            int count = entries.Where(a => a.Value != null && a.ValueType == "Percent" && a.FieldType == "COSTSHARE").Count();
            if (count > 0)
                result = true;
            else
                result = false;
            return result;
        }

        public static double? GetMinCoinsurance(List<OONGroupEntryModel> entries)
        {
            double? result = null;
            var coInsurances = entries.Where(a => a.Value != null && a.ValueType == "Percent" && a.FieldType == "COSTSHARE" && a.FieldSubType == "MIN");
            if (coInsurances.Count() > 0)
            {
                result = coInsurances.Min(a => a.Value);
            }
            return result;
        }

        public static double? GetMaxCoinsurance(List<OONGroupEntryModel> entries)
        {
            double? result = null;
            var coInsurances = entries.Where(a => a.Value != null && a.ValueType == "Percent" && a.FieldType == "COSTSHARE" && a.FieldSubType == "MAX");
            if (coInsurances.Count() > 0)
            {
                result = coInsurances.Max(a => a.Value);
            }
            return result;
        }

        public static double? GetMinCopay(List<OONGroupEntryModel> entries)
        {
            double? result = null;
            var copays = entries.Where(a => a.Value != null && a.ValueType == "Value" && a.FieldType == "COSTSHARE" && a.FieldSubType == "MIN");
            if (copays.Count() > 0)
            {
                result = copays.Min(a => a.Value);
            }
            return result;
        }

        public static double? GetMaxCopay(List<OONGroupEntryModel> entries)
        {
            double? result = null;
            var copays = entries.Where(a => a.Value != null && a.ValueType == "Value" && a.FieldType == "COSTSHARE" && a.FieldSubType == "MAX");
            if (copays.Count() > 0)
            {
                result = copays.Max(a => a.Value);
            }
            return result;
        }

        public static string[] GetValueBenefitCodes(List<OONGroupEntryModel> entries, string benefitType) 
        {
            return entries.Where(a => a.BenefitType == benefitType && a.Value.HasValue).Select(b => b.BenefitCode).Distinct().ToArray();
        }

        public static double? GetDeductible(List<OONGroupEntryModel> entries)
        {
            double? result = null;
            var deductibles = entries.Where(a => a.FieldType == "DEDUCTIBLE");
            if(deductibles.Count() > 0)
            {
                result = deductibles.First().Value;
            }
            return result;
        }

        public static double? GetPlanMax(List<OONGroupEntryModel> entries, bool isPackage)
        {
            double? result = null;
            if(isPackage == true)
            {
                var entryList = entries.Where(a => a.FieldType == "PLANLIMIT" && a.FieldSubType == "CONDITION" && isPackage && a.ConditionValue == "In-network services only");
                if (entryList.Count() > 0)
                {
                    result = entries.Where(a => a.FieldType == "PLANLIMIT" && a.FieldSubType == "PLANMAX").First().Value;
                }
            }
            else
            {
                var entryList = entries.Where(a => a.FieldType == "PLANLIMIT" && a.FieldSubType == "PLANMAX");
                if (entryList.Count() > 0)
                {
                    result = entries.Where(a => a.FieldType == "PLANLIMIT" && a.FieldSubType == "PLANMAX").First().Value;
                }
            }
            return result;
        }
 
        public static string GenerateJSONArrayString(string[] codes,bool addQuotes)
        {
            string csList = "";
            string quote = "";
            if(addQuotes)
            {
                quote = @"";
            }
            foreach (var code in codes)
            {
                if (!String.IsNullOrEmpty(code))
                {
                    csList = quote + csList + code + quote + ",";
                }
            }
            csList = csList.TrimEnd(',');

            string result = csList;
            return result;
        }
        public static Double ExtractDoubleFromString(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str.SkipWhile(c => c != '.' && !Char.IsDigit(c)))
            {
                if (c != '.' && !Char.IsDigit(c))
                {
                    return Convert.ToDouble(sb.ToString());
                }
                sb.Append(c);
            }
            
            return Convert.ToDouble(sb.ToString());
        }
    }
}
