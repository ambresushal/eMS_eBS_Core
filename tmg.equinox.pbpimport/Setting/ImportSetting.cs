using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using Newtonsoft.Json.Linq;
//using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices;
using System.Configuration;
//using tmg.equinox.infrastructure.exceptionhandling;
//using tmg.equinox.applicationservices.PBPImport;

namespace tmg.equinox.pbpimport
{
    public class ImportSetting
    {

    }

    public class ValueManipulator
    {
        public string Value = string.Empty;
        public List<string> PBPFileColumNameList = new List<string>()
        {
            "PBP_A_ORG_TYPE","PBP_A_PLAN_TYPE","PBP_A_CONTRACT_PARTD_FLAG","PBP_A_DSNP_ZERODOLLAR","MRX_DRUG_BEN_YN",
            "PBP_A_TIER_MC_BENDESC_CATS","PBP_A_SPECIAL_NEED_PLAN_TYPE","PBP_A_SNP_INSTITUTIONAL_TYPE","PBP_A_SNP_COND"
        };

        public bool IsNeedToManipulatorValue(string pbpFileColumName)
        {
            return PBPFileColumNameList.Where(s => s.Contains(pbpFileColumName)).Any();
        }

        public string ValueManipulatorProcessor(string pbpFileColumName, string inCommingValue, string MappingType)
        {
            switch (pbpFileColumName)
            {
                case "PBP_A_ORG_TYPE":
                    Value = PBP_A_ORG_TYPE_ValueManipulator(inCommingValue);
                    break;
                case "PBP_A_PLAN_TYPE":
                    Value = PBP_A_PLAN_TYPE_ValueManipulator(inCommingValue, MappingType);
                    break;
                case "PBP_A_CONTRACT_PARTD_FLAG":
                    Value = PBP_A_CONTRACT_PARTD_FLAG_ValueManipulator(inCommingValue, MappingType);
                    break;
                case "PBP_A_DSNP_ZERODOLLAR":
                    Value = PBP_A_DSNP_ZERODOLLAR_ValueManipulator(inCommingValue, MappingType);
                    break;
                case "MRX_DRUG_BEN_YN":
                    Value = MRX_DRUG_BEN_YN_Medicare_ValueManipulator(inCommingValue, MappingType);
                    break;
                case "PBP_A_TIER_MC_BENDESC_CATS":
                    Value = PBP_A_TIER_MC_BENDESC_CATS_ValueManipulator(inCommingValue, MappingType);
                    break;
                case "PBP_A_SPECIAL_NEED_PLAN_TYPE":
                    Value = (MappingType.Equals(DocumentName.MEDICARE)) ? PBP_A_SPECIAL_NEED_PLAN_TYPE_ValueManipulator(inCommingValue) : inCommingValue;
                    break;
                case "PBP_A_SNP_INSTITUTIONAL_TYPE":
                    Value = PBP_A_SNP_INSTITUTIONAL_TYPE_ValueManipulator(inCommingValue);
                    break;
                case "PBP_A_SNP_COND":
                    Value = PBP_A_SNP_COND_ValueManipulator(inCommingValue, MappingType);
                    break;
            }
            return Value;
        }
        private string PBP_A_SPECIAL_NEED_PLAN_TYPE_ValueManipulator(string inCommingValue)
        {
            string PBP_A_SPECIAL_NEED_PLAN_TYPE_Value = string.Empty;
            try
            {
                switch (inCommingValue)
                {
                    case "1":
                        PBP_A_SPECIAL_NEED_PLAN_TYPE_Value = "Institutional";
                        break;
                    case "3":
                        PBP_A_SPECIAL_NEED_PLAN_TYPE_Value = "Dual-Eligible";
                        break;
                    case "4":
                        PBP_A_SPECIAL_NEED_PLAN_TYPE_Value = "Chronic Or Disabling Condition";
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return PBP_A_SPECIAL_NEED_PLAN_TYPE_Value;
        }
        private string PBP_A_SNP_INSTITUTIONAL_TYPE_ValueManipulator(string inCommingValue)
        {
            string PBP_A_SNP_INSTITUTIONAL_TYPE_Value = string.Empty;
            try
            {
                switch (inCommingValue)
                {
                    case "1":
                        PBP_A_SNP_INSTITUTIONAL_TYPE_Value = "Institutional (Facility)";
                        break;
                    case "2":
                        PBP_A_SNP_INSTITUTIONAL_TYPE_Value = "Institutional Equivalent (Living in the Community)";
                        break;
                    case "3":
                        PBP_A_SNP_INSTITUTIONAL_TYPE_Value = "Institutional (Facility) and Institutional Equivalent (Living in the Community)";
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return PBP_A_SNP_INSTITUTIONAL_TYPE_Value;
        }
        private string PBP_A_SNP_COND_ValueManipulator(string inCommingValue, string MappingType)
        {
            string binaryValue = "00000000000000000000";
            string PBP_A_SNP_COND_Value = "[";

            string[] stringValues = new string[] { "Chronic alcohol and other drug dependence", "Autoimmune Disorders", "Cancer Excluding Pre-Cancer Conditions Or In-Situ status", "Cardiovascular Disorders","Chronic Heart Failure",
                              "Dementia","Diabetes Mellitus","End-Stage Liver Disease","Dialysis Services Requiring Dialysis (Any Mode Of Dialysis)","Severe Hematologic Disorders",
                              "HIV/AIDS","Chronic Lung Disorders","Chronic And Disabling Mental Health Conditions","Neurologic Disorders","Stroke",
                              "Cardiovascular Disaorders And/Or Chronic Heart Failure","Cardiovascular Disorders And/Or Diabetes","Chronic Heart Failure And/Or Diabetes","Cardiovascular Disorders, Chronic Heart Failure, and/or Diabetes","Cardiovascular Disorders and/or Stroke"};

            try
            {
                for (int i = 0; i < inCommingValue.Length; i++)
                {
                    if (inCommingValue.Substring(i, 1).ToString() == "1")
                    {
                        if (MappingType.Equals(DocumentName.MEDICARE))
                        {
                            PBP_A_SNP_COND_Value += "'" + stringValues[i] + "',";
                        }
                        else
                        {
                            string firstPart = binaryValue.Substring(0, i);
                            string secondPart = binaryValue.Substring(i + 1, (binaryValue.Length - (i + 1)));
                            PBP_A_SNP_COND_Value += "'" + firstPart + "1" + secondPart + "',";
                        }
                    }
                }

                PBP_A_SNP_COND_Value = PBP_A_SNP_COND_Value.Substring(0, PBP_A_SNP_COND_Value.Length - 1);
                PBP_A_SNP_COND_Value += "]";
            }
            catch (Exception ex)
            {

            }
            return PBP_A_SNP_COND_Value;
        }
        private string PBP_A_ORG_TYPE_ValueManipulator(string inCommingValue)
        {
            string PBP_A_ORG_TYPE_Value = string.Empty;
            try
            {
                PBP_A_ORG_TYPE_Value = Convert.ToInt32(inCommingValue).ToString();
            }
            catch (Exception ex)
            {

            }
            return PBP_A_ORG_TYPE_Value;
        }

        private string PBP_A_PLAN_TYPE_ValueManipulator(string inCommingValue, string MappingType)
        {
            string PBP_A_PLAN_TYPE_Value = string.Empty;
            Dictionary<string, string> PlanTypeDict = GetPlanType();
            try
            {
                if (MappingType.Equals(DocumentName.MEDICARE))
                {
                    PBP_A_PLAN_TYPE_Value = PlanTypeDict.FirstOrDefault(x => x.Value.Equals(inCommingValue)).Key.ToString();
                }
                else
                {
                    PBP_A_PLAN_TYPE_Value = Convert.ToInt32(inCommingValue).ToString();
                }

            }
            catch (Exception ex)
            {

            }
            return PBP_A_PLAN_TYPE_Value;
        }

        private string PBP_A_CONTRACT_PARTD_FLAG_ValueManipulator(string inCommingValue, string MappingType)
        {
            string PBP_A_CONTRACT_PARTD_FLAG_Value = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(MappingType) && MappingType == DocumentName.MEDICARE)
                {
                    if (inCommingValue == "1")
                    {
                        PBP_A_CONTRACT_PARTD_FLAG_Value = "YES";
                    }
                    else if (inCommingValue == "2")
                    {
                        PBP_A_CONTRACT_PARTD_FLAG_Value = "NO";
                    }
                }
                else
                {
                    PBP_A_CONTRACT_PARTD_FLAG_Value = inCommingValue;
                }
            }
            catch (Exception ex)
            {

            }
            return PBP_A_CONTRACT_PARTD_FLAG_Value;
        }



        private string PBP_A_DSNP_ZERODOLLAR_ValueManipulator(string inCommingValue, string MappingType)
        {
            string PBP_A_DSNP_ZERODOLLAR_Value = string.Empty;
            try
            {
                if (MappingType.Equals(DocumentName.MEDICARE))
                {
                    if (inCommingValue == "1")
                    {
                        PBP_A_DSNP_ZERODOLLAR_Value = "YES";
                    }
                    else if (inCommingValue == "2")
                    {
                        PBP_A_DSNP_ZERODOLLAR_Value = "NO";
                    }
                }
                else
                {
                    PBP_A_DSNP_ZERODOLLAR_Value = inCommingValue;
                }
            }
            catch (Exception ex)
            {

            }
            return PBP_A_DSNP_ZERODOLLAR_Value;
        }

        private string MRX_DRUG_BEN_YN_Medicare_ValueManipulator(string inCommingValue, string MappingType)
        {
            string MRX_DRUG_BEN_YN_Value = string.Empty;
            try
            {
                if (MappingType.Equals(DocumentName.MEDICARE))
                {
                    if (inCommingValue == "1")
                    {
                        MRX_DRUG_BEN_YN_Value = "YES";
                    }
                    else if (inCommingValue == "2")
                    {
                        MRX_DRUG_BEN_YN_Value = "NO";
                    }
                }
                else if (MappingType.Equals(DocumentName.PBPVIEW))
                {
                    MRX_DRUG_BEN_YN_Value = inCommingValue;
                }
            }
            catch (Exception ex)
            {

            }
            return MRX_DRUG_BEN_YN_Value;
        }

        private string PBP_A_TIER_MC_BENDESC_CATS_ValueManipulator(string inCommingValue, string MappingType)
        {
            try
            {
                if (!String.IsNullOrEmpty(inCommingValue))
                {
                    string[] ItemArray = inCommingValue.Split(';');
                    string OutPutStr = string.Empty;
                    OutPutStr = "[";
                    foreach (var item in ItemArray)
                    {
                        if (!item.Equals(";") && !String.IsNullOrEmpty(item))
                        {
                            OutPutStr += "'" + item.ToString() + "',";
                        }
                    }
                    OutPutStr = OutPutStr.TrimEnd(',') + "]";
                    JToken Val = JToken.Parse(OutPutStr);
                    return Val.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        public string ConvertValue(int ValueType, string inCommingValue, string MappingType)
        {
            string Value = string.Empty;
            switch (ValueType)
            {
                case 1:
                    //Convert 1 to YES and 2 to NO
                    Value = YESNOValueConvertor(inCommingValue, MappingType);
                    break;
            }
            return Value;
        }

        private string YESNOValueConvertor(string inCommingValue, string MappingType)
        {
            string Value = string.Empty;

            if (MappingType.Equals(DocumentName.MEDICARE))
            {
                if (inCommingValue == "1")
                {
                    Value = "YES";
                }
                else if (inCommingValue == "2")
                {
                    Value = "NO";
                }
            }
            else if (MappingType.Equals(DocumentName.PBPVIEW))
            {
                Value = inCommingValue;
            }

            return Value;
        }

        private Dictionary<string, string> GetPlanType()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("HMO", "01"); dict.Add("HMOPOS", "02"); dict.Add("Local PPO", "04"); dict.Add("PSO (State License)", "05");

            dict.Add("MSA", "07"); dict.Add("RFB PFFS", "08"); dict.Add("PFFS", "09"); dict.Add("1876 Cost", "18");

            dict.Add("HCPP - 1833 Cost", "19"); dict.Add("National Pace", "20"); dict.Add("Medicare Prescription Drug Plan", "29"); dict.Add("Employer/Union Only Direct Contract PDP", "30");

            dict.Add("Regional PPO", "31"); dict.Add("Fallback", "32"); dict.Add("Employer/Union Only Direct Contract PFFS", "40"); dict.Add("RFB HMO", "42");

            dict.Add("RFB HMOPOS", "43"); dict.Add("RFB Local PPO", "44"); dict.Add("RFB PSO (State License)", "45"); dict.Add("Employer Direct PPO", "47");

            dict.Add("MMP HMO", "48"); dict.Add("MMP HMOPOS", "49");
            return dict;
        }
    }
}
