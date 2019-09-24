using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationItemMultiValueCSVMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item)
        {
            string sourcePath = item.TableName + "." + item.ColumnName.Replace(" ", "");
            string targetPath = item.DocumentPath;
            var sourceToken = source.SelectToken(sourcePath);
            var targetToken = target.SelectToken(targetPath);
            string targetValue = GenerateTargetValue(sourceToken, item);
            var prop = targetToken.Parent as JProperty;
            if (item.ColumnName == "pbp_a_tier_mc_bendesc_cats" || item.ColumnName == "pbp_a_ffs_bid_b_ref_cats"||item.ColumnName == "pbp_b14c_bendesc_ehc"
                || item.ColumnName == "pbp_a_ffs_bid_b_auth_cats" || item.ColumnName == "pbp_c_oon_out_mc_bendesc_cats" || item.ColumnName == "pbp_a_tier_nmc_bendesc_cats"
                || item.ColumnName == "pbp_c_oon_out_nmc_bendesc_cats" || item.ColumnName == "pbp_d_opt_other_benefits" || item.ColumnName == "pbp_c_oon_nmc_bendesc_cats" || item.ColumnName == "pbp_c_oon_mc_bendesc_cats"
                || item.ColumnName == "pbp_c_pos_mc_bendesc_subcats" || item.ColumnName == "pbp_c_pos_nmc_bendesc_subcats" || item.ColumnName == "pbp_c_pos_maxplan_mc_subcats" || item.ColumnName == "pbp_c_pos_maxplan_nmc_subcats"
                || item.ColumnName == "pbp_b14c_maxplan_ehc" || item.ColumnName == "pbp_d_inn_maxplan_nm_cats" || item.ColumnName == "pbp_d_oon_maxplan_nm_cats"
                || item.ColumnName == "pbp_d_inn_max_enr_nm_cat_ex" || item.ColumnName == "pbp_d_comb_max_enr_inn_nmcatex" || item.ColumnName == "pbp_d_opt_secb_cats"
                || item.ColumnName == "pbp_d_mmp_plancov_bendesc_cats" || item.ColumnName == "pbp_d_mmp_medcaid_bendesc_cats"|| item.ColumnName == "PBP_C_POS_AUTH_MC_SUBCATS"
                || item.ColumnName == "pbp_d_inn_deduct_nm_cats" || item.ColumnName == "pbp_d_ann_deduct_oon_nmc_cats" ||item.ColumnName== "pbp_b14c_copay_ehc"
                || item.ColumnName == "pbp_d_opt_deduct_cats"||item.ColumnName== "pbp_c_pos_outpt_mc_bencats"||item.ColumnName== "pbp_d_oon_max_enr_nm_cat_ex"
                || item.ColumnName == "pbp_b14c_maxenr_ehc" || item.ColumnName == "pbp_d_comb_deduct_inn_m_cats" || item.ColumnName== "pbp_d_comb_deduct_inn_nm_cats" 
                ||item.ColumnName== "pbp_d_comb_deduct_oon_m_cats" || item.ColumnName== "pbp_d_comb_deduct_oon_nm_cats" || item.ColumnName == "pbp_d_maxenr_oopc_nm_cats" || item.ColumnName == "pbp_d_oon_deduct_m_cats")
            {
                prop.Value = targetValue == "" ? new JArray() : JArray.Parse(targetValue);
            }
            else
            {
                JArray arr = new JArray();
                arr.Add(targetValue);
                prop.Value = arr;
            }
        }

        private string GenerateTargetValue(JToken sourceToken, MigrationFieldItem item)
        {
            string sourceValue = sourceToken.ToString();
            string targetValue = "";
            if (!String.IsNullOrEmpty(sourceValue))
            {
                string[] codes = sourceValue.Split(';');
                JArray jarr = new JArray();
                if (codes != null && codes.Length > 0)
                {
                    foreach (string code in codes)
                    {
                        if (code.Trim() != "")
                        {
                            if (item.ColumnName == "pbp_c_oon_out_mc_bendesc_cats" || item.ColumnName == "pbp_c_oon_out_nmc_bendesc_cats"
                                || item.ColumnName == "pbp_a_tier_mc_bendesc_cats" || item.ColumnName == "pbp_a_tier_nmc_bendesc_cats" ||item.ColumnName == "PBP_C_POS_AUTH_MC_SUBCATS"
                                || item.ColumnName == "pbp_a_ffs_bid_b_ref_cats" || item.ColumnName == "pbp_a_ffs_bid_b_auth_cats" || item.ColumnName == "pbp_b14c_bendesc_ehc"
                                || item.ColumnName == "pbp_d_opt_other_benefits" || item.ColumnName == "pbp_c_oon_nmc_bendesc_cats" || item.ColumnName == "pbp_c_oon_mc_bendesc_cats"
                                || item.ColumnName == "pbp_c_pos_mc_bendesc_subcats" || item.ColumnName == "pbp_c_pos_nmc_bendesc_subcats" || item.ColumnName == "pbp_c_pos_maxplan_mc_subcats" || item.ColumnName == "pbp_c_pos_maxplan_nmc_subcats"
                                || item.ColumnName == "pbp_b14c_maxplan_ehc" || item.ColumnName == "pbp_d_inn_maxplan_nm_cats" || item.ColumnName == "pbp_d_oon_maxplan_nm_cats"
                                || item.ColumnName == "pbp_d_inn_max_enr_nm_cat_ex" || item.ColumnName == "pbp_d_comb_max_enr_inn_nmcatex" || item.ColumnName == "pbp_d_opt_secb_cats"
                                || item.ColumnName == "pbp_d_mmp_plancov_bendesc_cats" || item.ColumnName == "pbp_d_mmp_medcaid_bendesc_cats"
                                || item.ColumnName == "pbp_d_inn_deduct_nm_cats" || item.ColumnName == "pbp_d_ann_deduct_oon_nmc_cats" || item.ColumnName == "pbp_b14c_copay_ehc"
                                || item.ColumnName == "pbp_d_opt_deduct_cats" || item.ColumnName == "pbp_c_pos_outpt_mc_bencats" || item.ColumnName == "pbp_d_oon_max_enr_nm_cat_ex" 
                                || item.ColumnName == "pbp_b14c_maxenr_ehc" || item.ColumnName == "pbp_d_comb_deduct_inn_m_cats" || item.ColumnName == "pbp_d_comb_deduct_inn_nm_cats"
                                || item.ColumnName == "pbp_d_comb_deduct_oon_m_cats" || item.ColumnName == "pbp_d_comb_deduct_oon_nm_cats" || item.ColumnName == "pbp_d_maxenr_oopc_nm_cats" || item.ColumnName == "pbp_d_oon_deduct_m_cats")
                            {
                                var desc = from rec in item.Dictionaryitems
                                           where rec.Codes == code
                                           select rec.Codes;
                                if (desc != null && desc.Count() > 0)
                                {
                                    string codeDesc = desc.First().ToString().Trim();
                                    jarr.Add(codeDesc);
                                }
                            }
                            else
                            {
                                var desc = from rec in item.Dictionaryitems
                                           where rec.Codes == code
                                           select rec.CODE_VALUES;
                                if (desc != null && desc.Count() > 0)
                                {
                                    string codeDesc = desc.First();
                                    targetValue = targetValue + "," + codeDesc.ToString().Trim();
                                }

                            }
                        }
                    }
                }
                if (item.ColumnName == "pbp_c_oon_out_mc_bendesc_cats" || item.ColumnName == "pbp_c_oon_out_nmc_bendesc_cats" || item.ColumnName == "pbp_b14c_bendesc_ehc"
                    || item.ColumnName == "pbp_a_tier_mc_bendesc_cats" || item.ColumnName == "pbp_a_ffs_bid_b_ref_cats" || item.ColumnName == "pbp_a_tier_nmc_bendesc_cats"
                    || item.ColumnName == "pbp_a_ffs_bid_b_auth_cats" || item.ColumnName == "pbp_d_opt_other_benefits" || item.ColumnName == "pbp_c_oon_nmc_bendesc_cats" || item.ColumnName == "pbp_c_oon_mc_bendesc_cats"
                    || item.ColumnName == "pbp_c_pos_mc_bendesc_subcats" || item.ColumnName == "pbp_c_pos_nmc_bendesc_subcats" || item.ColumnName == "pbp_c_pos_maxplan_mc_subcats" || item.ColumnName == "pbp_c_pos_maxplan_nmc_subcats"
                    || item.ColumnName == "pbp_b14c_maxplan_ehc" || item.ColumnName == "pbp_d_inn_maxplan_nm_cats" || item.ColumnName == "pbp_d_oon_maxplan_nm_cats"
                    || item.ColumnName == "pbp_d_inn_max_enr_nm_cat_ex" || item.ColumnName == "pbp_d_comb_max_enr_inn_nmcatex" || item.ColumnName == "pbp_d_opt_secb_cats"
                    || item.ColumnName == "pbp_d_mmp_plancov_bendesc_cats" || item.ColumnName == "pbp_d_mmp_medcaid_bendesc_cats" || item.ColumnName == "pbp_b14c_copay_ehc"
                    || item.ColumnName == "pbp_d_inn_deduct_nm_cats" || item.ColumnName == "pbp_d_ann_deduct_oon_nmc_cats" || item.ColumnName == "PBP_C_POS_AUTH_MC_SUBCATS"
                    || item.ColumnName == "pbp_d_opt_deduct_cats" || item.ColumnName == "pbp_c_pos_outpt_mc_bencats" || item.ColumnName == "pbp_d_oon_max_enr_nm_cat_ex" 
                    || item.ColumnName == "pbp_b14c_maxenr_ehc" || item.ColumnName == "pbp_d_comb_deduct_inn_m_cats" || item.ColumnName == "pbp_d_comb_deduct_inn_nm_cats" 
                    || item.ColumnName == "pbp_d_comb_deduct_oon_m_cats" || item.ColumnName == "pbp_d_comb_deduct_oon_nm_cats" || item.ColumnName == "pbp_d_maxenr_oopc_nm_cats" || item.ColumnName == "pbp_d_oon_deduct_m_cats")
                    return jarr.ToString();
            }
            return targetValue.TrimStart(',');
        }

    }
}
