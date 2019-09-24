using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.Report;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using System.Web;
using tmg.equinox.domain.entities;

namespace tmg.equinox.applicationservices
{
    public class ReportService : IReportService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService;

        public ReportService(IUnitOfWorkAsync unitofwork, IFolderVersionServices folderVersionService)
        {
            _unitOfWork = unitofwork;
            _folderVersionService = folderVersionService;
        }

        #region constants
        public const string auditChecklist = "AuditChecklist";
        public const string generalAuditInfo = "GeneralAuditInfo";
        public const string checklistDetails = "CheckListDetails";
        public const string generalComments = "GeneralComments";
        public const string approvalDetails = "ApprovalDetails";
        #endregion constants

        /// <summary>
        /// Gets the PlaceHolders and their DataPath in JSON object which is used to parse PlaceHolder values from JSON object.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="reportID"></param>
        /// <param name="reportVersionID"></param>
        /// <returns></returns>
        public List<ReportServiceViewModel> GetFormMappingData(int tenantId, int formInstanceID, string reportName)
        {
            List<ReportServiceViewModel> mappingData = new List<ReportServiceViewModel>();

            int latestReportVersionID = (from f1 in this._unitOfWork.Repository<FormReport>().Get()
                                         join f2 in this._unitOfWork.Repository<FormReportVersion>().Get()
                                         on f1.ReportID equals f2.ReportID
                                         where f1.ReportName == reportName.ToLower()
                                         select f2.ReportVersionID
                                         ).OrderByDescending(i => i).FirstOrDefault();

            mappingData = (from frv in this._unitOfWork.RepositoryAsync<FormReportVersion>().Get()
                           join frvm in this._unitOfWork.RepositoryAsync<FormReportVersionMap>().Get()
                             on frv.ReportVersionID equals frvm.ReportVersionID
                           where (frv.ReportVersionID == latestReportVersionID
                            && frv.TenantID == tenantId && frv.TenantID == tenantId)

                           select new ReportServiceViewModel
                           {
                               ReportPlaceHolderID = frvm.PlaceHolderID,
                               JSONPath = frvm.FormDataPath,
                               JSONExpression = frvm.ValueExpression,
                               FilterExpression = frvm.FilterExpression,
                               FilterExpressionValue = frvm.FilterExpressionValue,
                               FormDesignID = frvm.FormDesignID.ToString(),
                               MapType = frvm.MapType,
                               ValueFormat = frvm.ValueFormat
                           }).ToList();

            return mappingData;
        }

        /// <summary>
        /// Formats Value using the specified Format for the value expression.
        /// </summary>
        /// <param name="Exp"></param>
        /// <param name="ExpVal"></param>
        /// <param name="KelVal"></param>
        /// <returns></returns>
        public string GetFormattedValue(string Exp, string ExpVal, Dictionary<string, string> KelVal)
        {
            Regex valueRegex = new Regex(@"[[][^][]*[]]");
            Regex ifelsevalueRegex = new Regex(@"((?<=(\[{)|(,( )*,( )*{))((.)(?!,( )*,))+(?=(}( )*,( )*,)|(}\])))");
            Regex valueKeysRegex = new Regex(@"[{][^}]+[}]");
            Regex expRegex = new Regex(@"(?<=\[)[ ]*[(]((.)(?!(\]( )*\[)))*[)]");
            Regex expRegex2 = new Regex(@"[{][^)]*");                               //  [({{Key1}={Value1}} AND {{Key2}<>{Value2}}
            Regex ANDorORregex = new Regex(@"[)][\w ]*[(]");
            string ANDorORValue = "";
            ExpVal = ExpVal.Replace("] [", "][");
            MatchCollection m1 = expRegex.Matches(Exp);

            if (m1.Count > 0)
            {
                int temp = -1;
                bool IsTrue1 = false;

                MatchCollection values = valueRegex.Matches(ExpVal);

                ExpVal = "";
                var dfg = values[0];
                string valuestring = "";

                foreach (Match m in m1)
                {
                    try
                    {
                        temp++;
                        MatchCollection m2 = expRegex2.Matches(m.Value);
                        ANDorORValue = ANDorORregex.Match(m.Value).Value.Replace(")", "").Replace("(", "").Replace(" ", "").ToUpper();
                        IsTrue1 = true;
                        foreach (Match mm in m2)
                        {
                            if (ANDorORValue == "OR" || ANDorORValue == "")
                            {
                                IsTrue1 = MatchFound(mm.Value, KelVal);
                                if (IsTrue1) break;
                            }
                            else if (ANDorORValue == "AND")
                            {
                                IsTrue1 = IsTrue1 && MatchFound(mm.Value, KelVal);
                                if (!IsTrue1) break;
                            }
                        }
                        //[{statictexttrue_1 {Key1} ,{Key2}, statictexttrue_2} ,, { statictextfalse_1 {Key1} ,{Key2},statictextfalse_1 }]
                        MatchCollection IfElsevalues = ifelsevalueRegex.Matches(values[temp].Value);
                        if (IsTrue1 || IfElsevalues.Count > 1)
                        {
                            valuestring = IsTrue1 ? IfElsevalues[0].Value : IfElsevalues[1].Value;

                            MatchCollection valls = valueKeysRegex.Matches(valuestring);
                            foreach (Match mv in valls)
                            {
                                valuestring = valuestring.Replace(mv.Value, KelVal[mv.Value.Replace("{", "").Replace("}", "")]);
                            }
                            ExpVal += valuestring;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                MatchCollection valls = valueKeysRegex.Matches(ExpVal);
                foreach (Match mv in valls)
                {
                    ExpVal = ExpVal.Replace(mv.Value, KelVal[mv.Value.Replace("{", "").Replace("}", "")]);
                }
                ExpVal = ExpVal.Replace("[", "").Replace("]", "");
            }
            if (!String.IsNullOrEmpty(ExpVal))
            {
                if (ExpVal.Contains("Chr(254)"))
                    ExpVal = ExpVal.Replace("Chr(254)", ((Char)254).ToString());
                if (ExpVal.Contains("Chr(168)"))
                    ExpVal = ExpVal.Replace("Chr(168)", ((Char)168).ToString());
                if (ExpVal.IndexOf(',') == 0 && m1.Count > 2)
                    ExpVal = ExpVal.Substring(1, ExpVal.Length - 1);
            }
            ExpVal = ExpVal.Replace("\\n", "\n").Replace("\\t", "    ");
            //ExpVal = ExpVal.Contains("<li>") ? "<ul>" + ExpVal + "</ul>" : ExpVal;
            return ExpVal;
        }

        /// <summary>
        /// Operates on Set of Unit Expressions aggregated using either a Logical AND/OR.
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="KelVal"></param>
        /// <returns></returns>
        public static bool MatchFound(string exp, Dictionary<string, string> KelVal)
        {
            // {{IsthereaVisionVendor}={true}} AND {{VisionVendorDetails.VisionVendor}={}} AND {{VisionVendorDetails.VisionCustomerServicePhoneNumber}<>{}}
            string LOperand = string.Empty, ROperand = string.Empty, Operator = string.Empty;
            Regex regex = new Regex(@"[{][^{}]*[}][=<>]*[{][^}]*[}]");
            MatchCollection m1 = regex.Matches(exp);
            bool IsTrue2 = true;
            string ANDorORValue = "";
            Regex ANDorORregex = new Regex(@"[}][}][ \w]*[{][{]"); //  {{Key1}={Val1}} OR {{Key2}<>{Val2}}
            Regex operators = new Regex(@"[}][=<> ]*[{]");
            Regex operandRegex = new Regex(@"[{][^}]*[}]");    //[{][\w ]*[}]        // mm = {{Key1}={Value1}} AND {{Key2}<>{Value2}} AND {{Key3}<>{Value3}}                                                                   
            ANDorORValue = ANDorORregex.Match(exp).Value.Replace("{", "").Replace("}", "").Replace(" ", "").ToUpper();
            foreach (Match mmm in m1)                         //  {Key2}<>{Value2}
            {
                try
                {
                    MatchCollection operands = operandRegex.Matches(mmm.Value);

                    Operator = operators.Match(mmm.Value).Value.Replace("{", "").Replace("}", "").Replace(" ", "");

                    LOperand = KelVal[operands[0].Value.Replace("{", "").Replace("}", "")];
                    LOperand = LOperand == null ? "" : LOperand.ToLower().Trim();
                    ROperand = operands[1].Value.Replace("{", "").Replace("}", "");
                    ROperand = ROperand == null ? "" : ROperand.ToLower().Trim();

                    if (Operator == "=")
                    {
                        if (ANDorORValue == "OR" || ANDorORValue == "")
                        {
                            IsTrue2 = (LOperand == ROperand);
                            if (IsTrue2) break;
                        }

                        if (ANDorORValue == "AND")
                        {
                            IsTrue2 = IsTrue2 && (LOperand == ROperand);
                            if (!IsTrue2) break;
                        }
                    }
                    else if (Operator == "<>")
                    {
                        if (ANDorORValue == "OR" || ANDorORValue == "")
                        {
                            IsTrue2 = (LOperand != ROperand);
                            if (IsTrue2) break;
                        }

                        if (ANDorORValue == "AND")
                        {
                            IsTrue2 = IsTrue2 && (LOperand != ROperand);
                            if (!IsTrue2) break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return IsTrue2;
        }

        /// <summary>
        /// Parses the Values from the JSON data using the Datapath and filters retrieved from the DB.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="ReportName"></param>
        /// <param name="reportVersionID"></param>
        /// <returns></returns>
        public List<ReportServiceViewModel> ParseMappingDataFromJSON(IFolderVersionServices service, int tenantId, int formInstanceID, out int TierCount, int formDesignID, int folderVersionID, out string formData, out string adminFormData, out string NetworkTiers, string ReportName = "FaxBack")
        {
            #region Declare
            List<ReportServiceViewModel> mappings = new List<ReportServiceViewModel>();
            string NetworkTierPath = "";
            string CostShareTier = "";
            string CoverageLevels = "";
            bool IsDataFromCollection = false;
            List<int> ApplicableDocumentIDsForReport = new List<int>();
            Dictionary<string, string> KeyVal = new Dictionary<string, string>();
            Dictionary<int, JObject> source = new Dictionary<int, JObject>();
            Dictionary<string, string> CoverageLevelInfo = new Dictionary<string, string>();
            List<JToken> pathData = null;
            Regex regex = new Regex(@"(((?<={( )*{)[^}{]+)|((?<!(=( )*{|<>( )*{))(?<={)[\w][^{}]+))");
            Regex VisionDollarValues = new Regex(@"[0-9]*");
            Regex VisionOtherThanOnlyInValues = new Regex(@"[A-Za-z0-9_@.()^\/%#&$+-]*");
            string json = string.Empty;
            int CurrentFormDesignID = 0;
            string s = "BM:Cmgmt:CertOthrProOutp";  // "BM:DE:NotCOV";                     //"BM:DE:COV";//"BM:BO:Dental1";   // "BM:Cmgmt:PreCeti:Dtl";
            bool IsFilterconditionAND = true;
            int temp = 0;
            formData = string.Empty;
            adminFormData = string.Empty;
            NetworkTiers = string.Empty;
            Dictionary<int, string> JSONs = new Dictionary<int, string>();
            var countOfVendors = 0;
            TierCount = 0;
            #endregion

            #region Initialize

            /* Get the Mappings for the PlaceHolders. */
            mappings = GetFormMappingData(tenantId, formInstanceID, ReportName ?? "");
            if (mappings.Any())
            {
                ApplicableDocumentIDsForReport.AddRange(mappings.Select(i => Convert.ToInt32(i.FormDesignID ?? "0")).Distinct().ToList<int>());
                JSONs = service.GetFormInstanceReportData(tenantId, formInstanceID, folderVersionID, ApplicableDocumentIDsForReport, ref CurrentFormDesignID, ReportName);

                /* Getting the JSON of the Documents needed for Report */
                if (JSONs != null && JSONs.Count > 0)
                {
                    foreach (var KeyValuePair in JSONs)
                    {
                        source.Add(KeyValuePair.Key, JObject.Parse(KeyValuePair.Value));
                        if (KeyValuePair.Key == CurrentFormDesignID)
                            formData = KeyValuePair.Value;
                        if (KeyValuePair.Key == formDesignID)
                            adminFormData = KeyValuePair.Value;
                    }
                }


                if (ReportName == GlobalVariables.DentalMatrix || ReportName == GlobalVariables.DentalFaxBack)
                {
                    NetworkTierPath = "Networks.NetworkInformation.SelectthePlansCostShareTiers";
                    CostShareTier = "NetworkTier";
                    //CovInfo = source[CurrentFormDesignID].SelectToken("CostShare.Deductibles.DentalDeductible");//CostShare.Deductibles.DentalDeductible[0].DentalDeductible
                }
                else if (ReportName == GlobalVariables.BenefitMatrix || ReportName == GlobalVariables.FaxBack || ReportName == GlobalVariables.SPDMatrix)
                {
                    NetworkTierPath = "Network.NetworkInformation.SelectthePlansCostShareTiers";
                    CostShareTier = "CostShareTier";
                }


                string NtwTiers = "";
                //Networks.NetworkInformation.SelectthePlansCostShareTiers
                /* Getting the Network Tiers specified for the Current Form */
                if (!string.IsNullOrEmpty(CostShareTier))
                {
                    if (!string.IsNullOrEmpty(CostShareTier))
                    {
                        var networkTiers = source[CurrentFormDesignID].SelectToken(NetworkTierPath).ToList();
                        if (networkTiers != null && networkTiers.Any())
                        {
                            networkTiers.Where(i => !string.IsNullOrEmpty((string)i.SelectToken(CostShareTier))).Select(i => NtwTiers += ((string)i.SelectToken(CostShareTier) ?? "") + ";");
                            foreach (var NT in networkTiers)
                            {
                                NtwTiers += ((string)NT.SelectToken(CostShareTier) ?? "") + ";";
                            }
                            TierCount = networkTiers.Where(i => !string.IsNullOrEmpty((string)i.SelectToken(CostShareTier))).Count();
                        }
                        NetworkTiers = NtwTiers;
                    }
                }



                var CoverageInformation = source[CurrentFormDesignID].SelectToken("CostShare.CoverageLevel.CoverageLevelList");

                if (CoverageInformation != null && CoverageInformation.Any())
                    CoverageLevelInfo = CoverageInformation
                                            .Where(i => !string.IsNullOrEmpty(i.SelectToken("CoverageType").ToString()))
                                            .Select(i => new { CoverageType = (string)i.SelectToken("CoverageType"), CoverageName = (string)i.SelectToken("CoverageName") })
                                            .ToDictionary(i => i.CoverageType, i => i.CoverageName);

            }




            //json = service.GetFormInstanceReportData(tenantId, formInstanceID, folderVersionID, ref formDesignID);
            //if (!source.Keys.Contains(formDesignID.ToString()))
            //{
            //    source.Add(formDesignID.ToString(), JObject.Parse(json));           /* Adding related Document JSON to Dictionary */
            //    adminFormData = json;
            //}

            List<JToken> data = null;
            #endregion



            foreach (ReportServiceViewModel field in mappings)
            {
                try
                {
                    if (field.ReportPlaceHolderID.Contains("BM:Onco:Value1"))
                    {
                    }



                    IsFilterconditionAND = true;
                    MatchCollection values = regex.Matches(field.JSONExpression);
                    string valueExpression = field.JSONExpression;

                    KeyVal = new Dictionary<string, string>();
                    data = new List<JToken>();
                    var temppathdata = source[Convert.ToInt32(field.FormDesignID)].SelectToken(field.JSONPath);
                    countOfVendors = temppathdata.Count();
                    if (temppathdata.Type == JTokenType.Array) //IsDataFromCollection
                    {
                        pathData = pathData = (temppathdata.ToList().Where(i => i.ToList().Count > 0)).ToList();

                        //if ((field.ReportPlaceHolderID.Contains("BAM:BP:AnnualFundingAmounts")) || (field.ReportPlaceHolderID.Contains("BAM:HRA:AnnualFundingAmounts")))
                        //{
                        //    pathData = pathData = (temppathdata.ToList().Where(i => i.ToList().Count > 0)).ToList();
                        //}
                        //else
                        //{
                        //    pathData = (temppathdata.ToList().Where(i => i.ToList().Count > 0).OrderBy(i => ((string)(i.ToList()[0])) ?? "")).ToList();
                        //}
                        IsDataFromCollection = true;
                    }
                    else
                        pathData = (new List<JToken>() { temppathdata });

                    //pathData = temppathdata.Type == JTokenType.Array ? (temppathdata.ToList()) : (new List<JToken>() { temppathdata });

                    IsFilterconditionAND = (field.FilterExpression ?? "").Replace(" ", "").ToUpper().Contains("}AND{");

                    if (pathData.Count() > 0)
                    {
                        /* Process fields, with Array Filter condition specified. */
                        if (field.MapType == "Array" && field.JSONExpression != null && field.FilterExpression != null && field.FilterExpressionValue != null)
                        {
                            string filterExpression = field.FilterExpression;
                            MatchCollection filters = regex.Matches(field.FilterExpression);
                            MatchCollection filterValues = regex.Matches(field.FilterExpressionValue);


                            #region Processing of Filter based Value Expressions
                            if (IsFilterconditionAND) data = pathData;
                            temp = 0;

                            /* Filter out the record from the Array using the Filter and FilterValue Pair defined for the PPlaceHolder. */
                            foreach (Match filter in filters)
                            {
                                string filterKey = filter.Value;
                                string filtervalue = filterValues[temp].Value;

                                if (IsFilterconditionAND && data != null)                       //  AND Condition                           
                                {
                                    if (filterKey.Contains("[") && filterKey.Contains("]"))                                            //  IQMedicalPlanNetwork[CostShareTier]
                                    {
                                        data = data.Select(i => i.SelectToken(filterKey.Split('[')[0]))
                                                   .Children()
                                                   .Where(p => p.SelectToken(filterKey.Split('[')[1].Replace("]", "")).ToString() == filtervalue).ToList();
                                    }
                                    else
                                    {
                                        /* For Deductibles and Out of Pocket get the Individual, Family or Other coverage level Amounts based on CoverageType since CoverageName is user defined and hence not static. */
                                        if (filterKey.Contains("CoverageLevel") && field.JSONPath.Contains("CostShare"))
                                        {
                                            if (!KeyVal.Keys.Contains(filtervalue))
                                                KeyVal.Add(filtervalue, CoverageLevelInfo[filtervalue]);
                                            if (CoverageLevelInfo.Keys.Contains(filtervalue))
                                                data = data.Where(x => (x == null ? "" : (string)x.SelectToken(filterKey)) == CoverageLevelInfo[filtervalue]).ToList();
                                            else
                                                data = null;
                                        }
                                        else
                                            data = data.Where(x => (x == null ? "" : (string)x.SelectToken(filterKey)) == filtervalue).ToList();
                                    }
                                    if (data.Count() == 0) break;
                                }
                                else                                                            //  OR Condition
                                {
                                    var tempData = pathData.Where(x => (x == null ? "" : (string)x.SelectToken(filterKey)) == filtervalue);
                                    if (tempData.Any())
                                        data.AddRange(tempData);
                                }
                                temp++;
                            }

                            /* Computing the Patient-Pays Amount in case of Coinsurance */
                            if (field.JSONPath.Contains("CostShare.Coinsurance.CoinsuranceList") && KeyVal.Keys.Count > 1)
                                KeyVal.Add("PatientAmount", (100 - Convert.ToInt32(data.FirstOrDefault().SelectToken("Amount").ToString().Replace("%", ""))).ToString() + "%");
                            #endregion
                        }
                        else
                            data = pathData;

                    }

                    #region Process Data retreived from the JSON Path.
                    if (data.Count() > 0)
                    {
                        /* Move in for Processing if Display Value needs not be Formatted, move into else block otherwise. */
                        if (string.IsNullOrEmpty(field.ValueFormat))
                        {
                            foreach (Match m in values)
                            {
                                valueExpression = valueExpression
                                                    .Replace(m.Value, data.Select(p => (string)p.SelectToken(m.Value))
                                                    .FirstOrDefault()).Replace("{", "").Replace("}", "");
                            }
                        }
                        else
                        {
                            string dataValue = string.Empty;

                            if (field.MapType == "OthDep")
                                dataValue = source[Convert.ToInt32(field.FormDesignID)].SelectToken(field.FilterExpression).Where(e => (string)e.SelectToken("BenefitCategory1") == field.FilterExpressionValue).Count() > 0 ? "" : "false";

                            if (string.IsNullOrEmpty(dataValue))
                            {
                                foreach (JToken Keys in data)
                                {
                                    KeyVal.Clear();
                                    foreach (Match m in values)
                                    {
                                        var Keytemp = (m.Value ?? "").Replace("[", "_").Replace("]", "_");
                                        if (!KeyVal.Keys.Contains(Keytemp))
                                        {
                                            string Value = string.Empty;
                                            try
                                            {  // $.DentalPlansCostShareTiers[?(@.NetworkTier == 'Network')][0]['NetworkTier']

                                                var rtyty = Keys.SelectToken("$.DentalPlansCostShareTiers[?(@.NetworkTier == 'Out of Network')]['Amount']");

                                                /* Selects the Token Value present within the FormDataPath specified.*/
                                                Value = (string)(Keys.SelectToken(m.Value));
                                            }
                                            catch (Exception)
                                            {
                                            }

                                            /* Selects the Token Value present at any Location in any of the JSON used for Report.*/
                                            if (Value == null)
                                            {
                                                foreach (int FormDesignId in ApplicableDocumentIDsForReport)
                                                {
                                                    if (source.Keys.Contains(FormDesignId))
                                                    {
                                                        Value = (string)(source[FormDesignId].SelectToken(m.Value));
                                                        if (Value != null) break;
                                                    }
                                                }
                                            }

                                            KeyVal.Add(Keytemp, Value);        // Storing all the KeyValue Pair to be used to format the Value Displayed in Report.
                                        }
                                    }
                                    /* Format the Display Value using the specified ValueFormat for the PlaceHolder */
                                    dataValue += GetFormattedValue(valueExpression, field.ValueFormat, KeyVal);
                                }
                                //var custoCareNumber = dataValue.Length - dataValue.Replace(",", "").Length;
                                var vendorsWithoutustoCareNumber = dataValue.Length - dataValue.Replace(";", "").Length;
                                if (field.ReportPlaceHolderID.Contains("BM:Onco:Value1") && vendorsWithoutustoCareNumber > 0)// || countOfVendors == 1)
                                    dataValue = dataValue.TrimEnd(' ').TrimEnd(';');
                                if (((field.FilterExpressionValue != null && !IsFilterconditionAND) || IsDataFromCollection || field.ReportPlaceHolderID.Contains("BAM:BP:PlanType")) && !field.ReportPlaceHolderID.Contains("BM:Onco:Value1") && !field.ReportPlaceHolderID.Contains("BM:Subro:Value1") && !field.ReportPlaceHolderID.Contains("BM:Subro:Value2:1") && !field.ReportPlaceHolderID.Contains("BM:Subro:Value2:2"))
                                    dataValue = Regex.Replace(dataValue, "([,][ ]*$)|([;][ ]*$)", "");

                                if (field.ReportPlaceHolderID.Contains("BAM:HRA:HealthyRewardsInformation"))
                                    dataValue = Regex.Replace(dataValue, "\n:", ":");

                                valueExpression = dataValue;
                            }
                            else
                                valueExpression = field.JSONExpression.Contains("OlogyBenefit") ? "Not a Benefit" : "";

                            if (field.ReportPlaceHolderID.Contains(":FSA"))
                                valueExpression = string.IsNullOrEmpty(valueExpression) ? "Not Applicable" : valueExpression;

                            valueExpression = valueExpression.Trim(new char[] { '\n' });
                            if (((!string.IsNullOrEmpty(field.FilterExpression) && !IsFilterconditionAND) || data.Count > 1) && !field.ReportPlaceHolderID.Contains("BM:Onco:Value1") && !field.ReportPlaceHolderID.Contains("BM:Subro:Value2:2"))
                                valueExpression = valueExpression.Trim(new char[] { ',', ' ' });

                        }

                        if (valueExpression.Contains("<li>") && !valueExpression.Contains("<ul>"))
                            valueExpression = "<ul>" + valueExpression + "</ul>";
                        valueExpression = valueExpression.Replace("$$", "$");
                        valueExpression = valueExpression.Replace("%%", "%");

                        field.Value = valueExpression;
                        field.ReportPlaceHolderID = field.ReportPlaceHolderID.Replace(':', '_');
                        if (field.ReportPlaceHolderID.Contains("BM_NtwInfo_UCRval"))
                            field.Value = field.Value.Replace("%", "");


                        #region Remove Blank <ul><li></li></ul>

                        if (field.Value.Contains("<ul><li></li></ul>"))
                        {
                            field.Value = field.Value.Replace("<ul><li></li></ul>", "");
                        }
                        #endregion
                        #region Handle General Cost share for Vision Matrix
                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            if (field.ReportPlaceHolderID.StartsWith("VN_T") && (field.ReportPlaceHolderID.EndsWith("CvrgLvls") || field.ReportPlaceHolderID.EndsWith("Copays") || field.ReportPlaceHolderID.EndsWith("Coins")))
                            {
                                //if (!VisionDollarValues.Match(field.Value).Success)
                                //    field.Value = field.Value.Split('\n')[0];   //field.Value = field.Value.Replace("$","");

                                string[] strings = field.Value.Split('\n');
                                var len = strings.Length;
                                var backupVal = field.Value;
                                field.Value = "";
                                for (int i = 0; i < len; i++)
                                {
                                    var result = "";
                                    if ((VisionOtherThanOnlyInValues.Match(strings[i]).Length > 0))
                                    {
                                        if (field.ReportPlaceHolderID.EndsWith("Coins"))
                                        {
                                            result = strings[i].Replace("% by Plan", "").Replace("%", "");   //field.Value = field.Value.Replace("$","");
                                            var tite = result.Split(':')[0];
                                            var onlyVal = result.Split(':')[1].Trim();
                                            if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                            {
                                                onlyVal = onlyVal + '%';
                                                result = tite + ":" + onlyVal;
                                            }
                                        }

                                        else
                                        {
                                            result = strings[i].Replace("$", "");   //field.Value = field.Value.Replace("$","");
                                            var tite = result.Split(':')[0];
                                            var onlyVal = result.Split(':')[1].Trim();
                                            if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                            {
                                                onlyVal = '$' + onlyVal;
                                                //result.Split(':')[1] = onlyVal;
                                                result = tite + ":" + onlyVal;
                                            }
                                        }

                                        field.Value = field.Value + result + "\n";
                                    }
                                    else
                                    {
                                        field.Value = field.Value + strings[i] + "\n";
                                    }

                                }
                                field.Value = field.Value.Replace("$Not Applicable", "Not Applicable").Replace("$Not Covered", "Not Covered").Replace("$No Copay", "No Copay").Replace("$Ded / Coins", "Ded / Coins");
                                field.Value = field.Value.Replace("Not Applicable%", "Not Applicable").Replace("Not Covered%", "Not Covered").Replace("No Copay%", "No Copay").Replace("Ded / Coins%", "Ded / Coins");
                            }
                            if (!string.IsNullOrEmpty(field.Value))
                            {
                                if (field.ReportPlaceHolderID.StartsWith("VN_OvrLftm") || field.ReportPlaceHolderID.StartsWith("VN_OvrAnnl") || field.ReportPlaceHolderID.StartsWith("VN_RestBen") || field.ReportPlaceHolderID.StartsWith("VFB_Max"))
                                {
                                    //if (!VisionDollarValues.Match(field.Value).Success)
                                    //    field.Value = field.Value.Split('\n')[0];   //field.Value = field.Value.Replace("$","");

                                    string[] strings = field.Value.Split('\n');
                                    var len = strings.Length;
                                    var backupVal = field.Value;
                                    field.Value = "";
                                    for (int i = 0; i < len; i++)
                                    {
                                        var result = "";
                                        if ((VisionOtherThanOnlyInValues.Match(strings[i]).Length > 0))
                                        {
                                            if (field.ReportPlaceHolderID.EndsWith("Coins"))
                                            {
                                                result = strings[i].Replace("% by Plan", "").Replace("%", "");   //field.Value = field.Value.Replace("$","");
                                                var tite = result.Split(':')[0];
                                                var onlyVal = result.Split(':')[1].Trim();
                                                if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                                {
                                                    onlyVal = onlyVal + '%';
                                                    result = tite + ":" + onlyVal;
                                                }
                                            }

                                            else
                                            {
                                                if (ReportName.Equals("Vision FaxBack"))
                                                {
                                                    result = strings[i].Replace("$", "");   //field.Value = field.Value.Replace("$","");
                                                    var tite = result.Split(':')[0];
                                                    Regex rgx = new Regex("[a-zA-Z ]");
                                                    var onlyVal = result.Split(':')[1].Trim();
                                                    if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                                    {
                                                        onlyVal = '$' + onlyVal;
                                                        //result.Split(':')[1] = onlyVal;
                                                        result = tite + ":" + onlyVal;
                                                    }
                                                }
                                                else
                                                {
                                                    Regex moneyL = new Regex(@"\$\d+");
                                                    Regex moneyR = new Regex(@"\d+\$");
                                                    field.Value = (moneyL.IsMatch(strings[0]) || moneyR.IsMatch(strings[0])) ? field.Value = field.Value + strings[0] + "\n" : field.Value + strings[0].Replace('$', ' ') + "\n";
                                                }

                                            }

                                            field.Value = field.Value + result + "\n";
                                        }
                                        else
                                        {
                                            field.Value = field.Value + strings[i] + "\n";
                                        }

                                    }
                                }
                            }
                        }
                        #endregion


                        #region Handle General Cost share for Dental Matrix

                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            if (field.ReportPlaceHolderID.StartsWith("DM_T") && (field.ReportPlaceHolderID.EndsWith("CvrgLvls") || field.ReportPlaceHolderID.EndsWith("Copays") || field.ReportPlaceHolderID.EndsWith("Coins")))
                            {
                                //if (!VisionDollarValues.Match(field.Value).Success)
                                //    field.Value = field.Value.Split('\n')[0];   //field.Value = field.Value.Replace("$","");
                                string[] strings = field.Value.Split('\n');
                                var len = strings.Length;
                                var backupVal = field.Value;
                                field.Value = "";
                                for (int i = 0; i < len; i++)
                                {
                                    var result = "";
                                    //strings[i].Replace("$", "");
                                    if ((VisionOtherThanOnlyInValues.Match(strings[i]).Length > 0))
                                    {
                                        if (field.ReportPlaceHolderID.EndsWith("Coins"))
                                        {
                                            result = strings[i].Replace("% by Plan", "").Replace("%", "");   //field.Value = field.Value.Replace("$","");
                                            var tite = result.Split(':')[0];
                                            var onlyVal = result.Split(':')[1].Trim();
                                            if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                            {
                                                onlyVal = onlyVal + '%';
                                                result = tite + ":" + onlyVal;
                                            }
                                        }

                                        else
                                        {
                                            result = strings[i].Replace("$", "");   //field.Value = field.Value.Replace("$","");
                                            var tite = result.Split(':')[0];
                                            var onlyVal = result.Split(':')[1].Trim();
                                            if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                            {
                                                onlyVal = '$' + onlyVal;
                                                //result.Split(':')[1] = onlyVal;
                                                result = tite + ":" + onlyVal;
                                            }
                                        }

                                        field.Value = field.Value + result + "\n";
                                    }
                                    else
                                    {
                                        field.Value = field.Value + strings[i] + "\n";
                                    }

                                }
                                if (field.ReportPlaceHolderID.EndsWith("Coins"))
                                {
                                    field.Value = field.Value.Replace("Services:", ":");
                                }
                                field.Value = field.Value.Replace("$Not Applicable", "Not Applicable").Replace("$Not Covered", "Not Covered").Replace("$No Copay", "No Copay").Replace("$Ded / Coins", "Ded / Coins");
                                field.Value = field.Value.Replace("Not Applicable%", "Not Applicable").Replace("Not Covered%", "Not Covered").Replace("No Copay%", "No Copay").Replace("Ded / Coins%", "Ded / Coins");
                            }
                        }

                        if (!string.IsNullOrEmpty(field.Value))
                        {
                            if (field.ReportPlaceHolderID.StartsWith("DM_DplMax") || field.ReportPlaceHolderID.StartsWith("DM_SpecBenfitMax"))
                            {
                                string[] strings = field.Value.Split('\n');
                                var len = strings.Length;
                                var backupVal = field.Value;
                                field.Value = "";
                                for (int i = 0; i < len; i++)
                                {
                                    var result = "";
                                    if ((VisionOtherThanOnlyInValues.Match(strings[i]).Length > 0))
                                    {
                                        if (field.ReportPlaceHolderID.EndsWith("Coins"))
                                        {
                                            result = strings[i].Replace("% by Plan", "").Replace("%", "");   //field.Value = field.Value.Replace("$","");
                                            var tite = result.Split(':')[0];
                                            var onlyVal = result.Split(':')[1].Trim();
                                            if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                            {
                                                onlyVal = onlyVal + '%';
                                                result = tite + ":" + onlyVal;
                                            }
                                        }

                                        else
                                        {
                                            result = strings[i].Replace("$", "");   //field.Value = field.Value.Replace("$","");
                                            var tite = result.Split(':')[0];
                                            var onlyVal = result.Split(':')[1].Trim();
                                            if (VisionDollarValues.Match(result.Split(':')[1].Trim()).Length > 0)
                                            {
                                                onlyVal = '$' + onlyVal;
                                                //result.Split(':')[1] = onlyVal;
                                                result = tite + ":" + onlyVal;
                                            }
                                        }

                                        field.Value = field.Value + result + "\n";
                                    }
                                    else
                                    {
                                        field.Value = field.Value + strings[i] + "\n";
                                    }

                                }
                                field.Value = field.Value.Replace("Overall Annual Maximum:", " ");
                                field.Value = field.Value.Replace("Overall Lifetime Maximum:", " ");
                                field.Value = field.Value.Replace("$Not Applicable", "Not Applicable").Replace("$Not Covered", "Not Covered").Replace("$No Copay", "No Copay").Replace("$Ded / Coins", "Ded / Coins");
                                field.Value = field.Value.Replace("Not Applicable%", "Not Applicable").Replace("Not Covered%", "Not Covered").Replace("No Copay%", "No Copay").Replace("Ded / Coins%", "Ded / Coins");
                            }
                        }

                        #endregion

                        if (field.ReportPlaceHolderID.Replace(":", "_").Contains("BM_NtwInfo_Ntwrk"))
                        {
                            field.Value = Regex.Replace((field.Value ?? "").Trim(), @"[Hh][Tt][Tt][Pp][:][/][/][ ]*[Hh][Tt][Tt][Pp][:][/][/]", "Http://");
                        }

                    }
                    else
                    {
                        valueExpression = field.FilterExpression.Contains("BenefitCategory") ? "Not Covered" : "";          // For Covered services Display "Not Covered" otherwise Blank, if value is not Found.
                    }
                    #endregion

                    if (field.JSONPath == "BenefitReview.BenefitReviewGrid" && (field.FilterExpression.Contains("VisionService") || field.FilterExpression.Contains("DentalService")) && string.IsNullOrEmpty(valueExpression) && !field.ReportPlaceHolderID.Contains("_BenefitClass"))
                        field.Value = "Not Covered";

                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            return mappings;
        }

        public string GetDataPath(string propertyName, int formInstanceID)
        {
            string path = string.Empty;
            try
            {
                path = (from c in this._unitOfWork.RepositoryAsync<FormDesignDataPath>().Get()
                        join fins in this._unitOfWork.Repository<FormInstance>().Get()
                        on c.FormDesignVersionID equals fins.FormDesignVersionID
                        where (c.Name == propertyName && fins.FormInstanceID == formInstanceID)
                        select c.Path).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return path;
        }

        public List<SBCReportServiceViewModel> GetSBCReportServiceMasterDataList(int tenantID)
        {

            List<SBCReportServiceViewModel> data = null;

            try
            {
                data = (from c in this._unitOfWork.RepositoryAsync<SBCReportServiceMaster>().Get()
                        where c.TenantID == tenantID
                        select new SBCReportServiceViewModel
                        {

                            ReportPlaceHolderID = c.ReportPlaceHolderID,
                            Category1Name = c.Category1Name,
                            Category2Name = c.Category2Name,
                            Category3Name = c.Category3Name,
                            POSName = c.POSName,
                            NetworkName = c.NetworkName,
                            TenantID = c.TenantID
                        }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }


            return data;
        }

        public SBCReportHeaderViewModel GetSBCReportHeader(int tenantID, int forminstanceID, int accountID)
        {
            SBCReportHeaderViewModel data = null;

            try
            {
                data = new SBCReportHeaderViewModel();
                data.TenantID = tenantID;
                data.FormInstanceId = forminstanceID;
                data.AccountID = accountID;
                data.AccountName = this._unitOfWork.Repository<Account>()
                                                    .Query()
                                                    .Filter(c => c.AccountID == accountID)
                                                    .Get()
                                                    .Select(c => c.AccountName)
                                                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return data;
        }

        #region Audit Checklist
     
        #endregion Audit Checklist

        private string FormatArrayData(JToken source, MatchCollection mc, string valueExpression)
        {
            string result = valueExpression;

            foreach (Match m in mc)
            {
                result = result.Replace(m.Value, (string)source.SelectToken(m.Value)).Replace("{", "").Replace("}", "");
            }

            result = result.Replace("\\n", Environment.NewLine);

            return result;
        }
    }
}
