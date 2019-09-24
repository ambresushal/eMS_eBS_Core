using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;

namespace tmg.equinox.pbpimportservices
{
    public static class HelperUtility
    {
        public static string GetIntervalNumber(string interval)
        {
            string intervalno = string.Empty;
            if (!string.IsNullOrEmpty(interval))
            {
                switch (interval)
                {
                    case "1": intervalno = "0";
                        break;
                    case "2": intervalno = "1";
                        break;
                    case "3": intervalno = "2";
                        break;
                    case "4": intervalno = "3";
                        break;
                }
            }

            return intervalno;
        }

        public static string GetBenefitperiod(string bencode)
        {
            string code = string.Empty;
            if (!string.IsNullOrEmpty(bencode))
            {
                switch (bencode)
                {
                    case "1": code = "original Medicare";
                        break;
                    case "2": code = "annual";
                        break;
                    case "3": code = "per admission";
                        break;
                }
            }

            return code;
        }

        public static bool CheckIfServiceCodeExists(string servicecodelist, string servicecode, char seperator)
        {
            string[] codes = servicecodelist.Split(seperator);

            foreach (string s in codes)
                if (s == servicecode)
                    return true;

            return false;
                
        }

        public static string GetDollerValue(string value)
        {
            int i = 0;
            if (int.TryParse(value, out i))
                return string.Format("${0}", i);

            return value;
        }

        public static string GetPercentageValue(string value)
        {
            int i = 0;
            if (int.TryParse(value, out i))
                return string.Format("{0}%", i);

            return value;
        }

        public static string GetMaximumPlanBenefitCoveragePeriodicity(string value)
        {
            string intervalno = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "1": intervalno = "every three years";
                        break;
                    case "2": intervalno = "every two years";
                        break;
                    case "3": intervalno = "every year";
                        break;
                    case "4": intervalno = "every six months";
                        break;
                    case "5": intervalno = "every three months ";
                        break;
                    case "6": intervalno = "every month";
                        break;
                }
            }

            return intervalno;
        }

        #region private methods

        public static string ApplyCustomRuleGetData(string pbpTableName, string pbpFieldName, int customRuleTypeId, int index, List<PBPDataMapViewModel> pbpDataModelList, string pbpQId, int pbpBatchId, List<PlanTypeViewModel> planTypeList)
        {
            string pbpActualData = string.Empty;
            string pbpData = string.Empty;

            if (string.IsNullOrEmpty(pbpTableName) || string.IsNullOrEmpty(pbpFieldName))
                return pbpData;

            pbpTableName = pbpTableName.Trim();
            pbpFieldName = pbpFieldName.Trim();

            PBPDataMapViewModel datamodel = pbpDataModelList.Where(d => d.QID == pbpQId && d.TableName == pbpTableName && d.PBPImportBatchID == pbpBatchId).FirstOrDefault();

            if (datamodel != null)
            {
                JObject jobj = JObject.Parse(datamodel.JsonData);
                if (jobj[pbpFieldName] != null && !string.IsNullOrEmpty(jobj[pbpFieldName].ToString()))
                    pbpActualData = jobj[pbpFieldName].ToString();
            }

            if (!string.IsNullOrEmpty(pbpActualData))
            {
                switch (customRuleTypeId)
                {
                    //Get the values for Plan Type
                    case 1:
                        pbpData = GetValueFromCode(pbpActualData, planTypeList);
                        break;
                    //Get the values for check box/radio button 
                    case 2:
                        pbpData = GetValueForCheckBox(pbpActualData);
                        break;
                    //Get true/false depending on position of 1 in the string
                    case 3: pbpData = GetValueAgainstIndex(pbpActualData, index);
                        break;
                }
            }

            return pbpData;
        }

        public static string GetValueAgainstIndex(string pbpActualData, int index)
        {
            char[] a = pbpActualData.ToCharArray();

            if (a.Length != 0 && index != 0)
            {
                if (a.Length < index - 1)
                    return false.ToString();

                if (a[index - 1] == '1')
                    return true.ToString();
                else
                    return false.ToString();
            }
            else
                return false.ToString();
        }

        public static string GetValueForCheckBox(string pbpActualData)
        {
            string value = false.ToString();

            if (pbpActualData == "1")
                value = true.ToString();

            return value;
        }

        public static string GetValueFromCode(string pbpActualData, List<PlanTypeViewModel> planTypeList)
        {
            string value = string.Empty;

            int code = 0;
            if (int.TryParse(pbpActualData, out code))
                value = planTypeList.Where(p => p.Code == code).Select(p => p.Description).First();

            return value;
        }

        public static string GetDataFromPBP(string pbpTableName, string pbpFieldName, List<PBPDataMapViewModel> pbpDataModelList, string pbpQId, int pbpBatchId)
        {
            string pbpData = string.Empty;

            if (string.IsNullOrEmpty(pbpTableName) || string.IsNullOrEmpty(pbpFieldName))
                return pbpData;
            pbpTableName = pbpTableName.Trim();
            pbpFieldName = pbpFieldName.Trim();

            List<PBPDataMapViewModel> dataModelList = pbpDataModelList.Where(d => d.QID == pbpQId && d.TableName == pbpTableName && d.PBPImportBatchID == pbpBatchId).ToList();

            foreach (PBPDataMapViewModel datamodel in dataModelList)
            {
                JObject jobj = JObject.Parse(datamodel.JsonData);
                if (jobj[pbpFieldName] != null && !string.IsNullOrEmpty(jobj[pbpFieldName].ToString()))
                    pbpData += jobj[pbpFieldName].ToString();
            }

            return pbpData;
        }

        public static string GetDataFromPBP(string pbpTableName, string pbpFieldName, PBPDataMapViewModel dataModel)
        {
            string pbpData = string.Empty;

            if (string.IsNullOrEmpty(pbpTableName) || string.IsNullOrEmpty(pbpFieldName))
                return pbpData;
            pbpTableName = pbpTableName.Trim();
            pbpFieldName = pbpFieldName.Trim();

            if (dataModel == null)
                return pbpData;
            else
            {
                JObject jobj = JObject.Parse(dataModel.JsonData);
                if (jobj[pbpFieldName] != null && !string.IsNullOrEmpty(jobj[pbpFieldName].ToString()))
                    pbpData = jobj[pbpFieldName].ToString();
            }

            return pbpData;
        }

        public static PBPImportActivityLogViewModel GetLogEntity(string message, string userName, int queueId, int pbpBatchId)
        {
            PBPImportActivityLogViewModel log = new PBPImportActivityLogViewModel();
            log.CreatedBy = userName;
            log.CreatedDate = DateTime.Now;
            log.PBPImportQueueID = queueId;
            log.PBPImportBatchID = pbpBatchId;
            log.Message = message;
            return log;
        }

        #endregion private methods
    }
}
