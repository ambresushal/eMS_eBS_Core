using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.reporting;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.MLImport
{
    public class MLImportHelperService : IMLImportHelperService
    {
        private static readonly ILog _logger = LogProvider.For<MLImportHelperService>();
        public IUnitOfWorkAsync _unitOfWork { get; set; }

        public MLImportHelperService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }        

        #region "High Level JSON creation with Section Name & Repeater Name"
        public string PremiumMLJsonFormatting(DataTable table)
        {
            table = RemoveEmptyRows(table);
            table = RemoveEmptyColumns(table);
            string MLSectionName = "PlanPremium";
            string MLSectionNameList = "PlanPremiumAmounts";
            string JSONString = "{\"" + MLSectionName + "\":{\"" + MLSectionNameList + "\":";
            JSONString += PremiumMLCreateJson(table);
            JSONString += "}}";
            return JSONString;
        }

        public string FormularyInfoMLJsonFormatting(DataTable table)
        {
            table = RemoveEmptyRows(table);
            table = RemoveEmptyColumns(table);
            string MLSectionName = "FormularyInformations";
            string MLSectionNameList = "FormularyInformationList";
            string JSONString = "{\"" + MLSectionName + "\":{\"" + MLSectionNameList + "\":";
            JSONString += FormularyInfoMLCreateJson(table);
            JSONString += "}}";
            return JSONString;
        }

        public string FIPSMLJsonFormatting(DataTable table)
        {
            table = RemoveEmptyRows(table);
            table = RemoveEmptyColumns(table);
            string MLSectionName = "FIPS";
            string MLSectionNameList = "FIPSCodeList";
            string JSONString = "{\"" + MLSectionName + "\":{\"" + MLSectionNameList + "\":";
            JSONString += FIPSCreateJson(table);
            JSONString += "}}";
            return JSONString;
        }

        public string BenchmarkInfoMLJsonFormatting(DataTable table)
        {
            table = RemoveEmptyRows(table);
            table = RemoveEmptyColumns(table);
            string MLSectionName = "BenchmarkInformations";
            string MLSectionNameList = "BenchmarkInformationList";
            string JSONString = "{\"" + MLSectionName + "\":{\"" + MLSectionNameList + "\":";
            JSONString += BenchmarkInfoMLCreateJson(table);
            JSONString += "}}";
            return JSONString;
        }

        public string PrescriptionMLJsonFormatting(DataTable table, DateTime effectiveDate)
        {
            table = RemoveEmptyRows(table);
            table = RemoveEmptyColumns(table);
            string MLSectionName = "PartDBenefitGrid";
            string MLSectionNameList = "PartDBenefitDetails";
            string JSONString = "{\"" + MLSectionName + "\":{\"" + MLSectionNameList + "\":";
            if(effectiveDate.Year==2018)
                JSONString += PrescriptionMLCreateJson(table);
            else
                JSONString += PartDMLCreateJson(table);
            JSONString += "}}";
            return JSONString;
        }

        #endregion "High Level JSON creation with Section Name & Repeater Name"

        #region "Inner Data Level JSON Creation"
        public string PremiumMLCreateJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            string ColumnName = "";
            string ColumnValue = "";
            int ColumnCount = table.Columns.Count;
            try
            {
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("[");
                    for (int tblRowNo = 0; tblRowNo < table.Rows.Count; tblRowNo++)
                    {
                        JSONString.Append("{");
                        JSONString.Append("\"RowIDProperty\":" + "\"" + tblRowNo + "\",");
                        for (int tblColumnNo = 0; tblColumnNo < ColumnCount; tblColumnNo++)
                        {
                            ColumnName = JsonColumnNameFormatting(table.Columns[tblColumnNo].ColumnName.ToString());
                            ColumnValue = table.Rows[tblRowNo][tblColumnNo].ToString();                            
                            if (ColumnValue == "") ColumnValue = "NA";
                            if (tblColumnNo < ColumnCount - 1)
                            {
                                switch (ColumnName)
                                {
                                    case "EffectiveDate":
                                    case "TermDate":
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(ColumnValue) + "\",");
                                        break;
                                    case "Contractnumber":
                                        ColumnName = "WellCarePlanCode";
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "PartBPremiumGiveback":
                                        ColumnName = "PartBPremiumReduction";
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "PartDPremiumCMSPays":
                                        ColumnName = "PartDPremium100CMSPays";
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "SubsidyLevels":
                                        ColumnName = "WCSubsidyLevels";
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "PlanCode":
                                        if (ColumnValue != null && ColumnValue.StartsWith("S") == true)
                                        {
                                            ColumnValue = ColumnValue.PadRight(11, '0');
                                        }
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    //case "PremiumSubsidy":
                                    //    JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "%" + "\",");
                                    //    break;
                                    //case "MemberDeductibleifapplicable":
                                    //case "MemberDeductibleRx":
                                    //case "ICL":
                                    //case "DeductibleInitialCoverageCoverageGapGeneric":
                                    //case "DeductibleInitialCoverageCoverageGapBrand":
                                    //case "TROOP":
                                    //case "CatastrophicStageGeneric":
                                    //case "CatastrophicStageBrand":
                                    //    if (ColumnValue.Length > 0)
                                    //        JSONString.Append("\"" + ColumnName + "\":" + "\"" + FormatCurrency(Convert.ToDecimal(ColumnValue)) + "\",");
                                    //    else
                                    //        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                    //    break;
                                    default:
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                }

                            }
                            else if (tblColumnNo == ColumnCount - 1)
                            {
                                JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\"");
                            }
                        }
                        if (tblRowNo == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                    JSONString.Append("]");
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml PremiumMLCreateJson" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return JSONString.ToString();
        }

        public string FIPSCreateJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            string ColumnName = "";
            string ColumnValue = "";
            int ColumnCount = table.Columns.Count;
            try
            {
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("[");
                    for (int tblRowNo = 0; tblRowNo < table.Rows.Count; tblRowNo++)
                    {
                        JSONString.Append("{");
                        JSONString.Append("\"RowIDProperty\":" + "\"" + tblRowNo + "\",");
                        for (int tblColumnNo = 0; tblColumnNo < ColumnCount; tblColumnNo++)
                        {
                            ColumnName = JsonColumnNameFormatting(table.Columns[tblColumnNo].ColumnName.ToString());
                            ColumnValue = table.Rows[tblRowNo][tblColumnNo].ToString();
                            if (ColumnValue == "") ColumnValue = "NA";
                            if (tblColumnNo < ColumnCount - 1)
                            {
                                switch (ColumnName)
                                {
                                    case "Effectivedate":
                                    case "TermDate":
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(ColumnValue) + "\",");
                                        break;
                                    default:
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                }

                            }
                            else if (tblColumnNo == ColumnCount - 1)
                            {
                                JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\"");
                            }
                        }
                        if (tblRowNo == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                    JSONString.Append("]");
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml FIPSMLCreateJson" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return JSONString.ToString();
        }

        public string FormularyInfoMLCreateJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            string ColumnName = "";
            string ColumnValue = "";
            int ColumnCount = table.Columns.Count;
            try
            {
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("[");
                    for (int tblRowNo = 0; tblRowNo < table.Rows.Count; tblRowNo++)
                    {
                        JSONString.Append("{");
                        JSONString.Append("\"RowIDProperty\":" + "\"" + tblRowNo + "\",");
                        for (int tblColumnNo = 0; tblColumnNo < ColumnCount; tblColumnNo++)
                        {
                            ColumnName = JsonColumnNameFormatting(table.Columns[tblColumnNo].ColumnName.ToString());
                            ColumnValue = table.Rows[tblRowNo][tblColumnNo].ToString();
                            if (ColumnValue == "") ColumnValue = "NA";
                            if (ColumnName == "CMSDescription") ColumnName = "Description";
                            if (tblColumnNo < ColumnCount - 1)
                            {
                                switch (ColumnName)
                                {
                                    case "EffectiveDate":
                                    case "TermDate":
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(ColumnValue) + "\",");
                                        break;
                                    case "PlanCode":
                                        if (ColumnValue != null && ColumnValue.StartsWith("S") == true)
                                        {
                                            ColumnValue = ColumnValue.PadRight(11, '0');
                                        }
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    default:
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                }

                            }
                            else if (tblColumnNo == ColumnCount - 1)
                            {
                                JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\"");
                            }
                        }
                        if (tblRowNo == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                    JSONString.Append("]");
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml FormularyInfoMLCreateJson" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return JSONString.ToString();
        }

        public string BenchmarkInfoMLCreateJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            string ColumnName = "";
            string ColumnValue = "";
            int ColumnCount = table.Columns.Count;
            try
            {
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("[");
                    ReportHelper reportHelper = new ReportHelper();
                    for (int tblRowNo = 0; tblRowNo < table.Rows.Count; tblRowNo++)
                    {
                        JSONString.Append("{");
                        JSONString.Append("\"RowIDProperty\":" + "\"" + tblRowNo + "\",");
                        for (int tblColumnNo = 0; tblColumnNo < ColumnCount; tblColumnNo++)
                        {
                            ColumnName = JsonColumnNameFormatting(table.Columns[tblColumnNo].ColumnName.ToString());
                            ColumnValue = table.Rows[tblRowNo][tblColumnNo].ToString();
                            if (ColumnValue == "") ColumnValue = "NA";
                            if (tblColumnNo < ColumnCount - 1)
                            {
                                switch (ColumnName)
                                {
                                    case "Effectivedate":
                                    case "TermDate":
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(ColumnValue) + "\",");
                                        break;
                                    case "PDRegionalPlanBenchmarkAmount":
                                    case "WellCareFinalBidAmount":
                                        if (ColumnValue.Length > 0)
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + reportHelper.FormatNumberWithDecimals(ColumnValue) + "\",");
                                        else
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    default:
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                }

                            }
                            else if (tblColumnNo == ColumnCount - 1)
                            {
                                JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\"");
                            }
                        }
                        if (tblRowNo == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                    JSONString.Append("]");
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml BenchmarkInfoMLCreateJson" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return JSONString.ToString();
        }

        public string PrescriptionMLCreateJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            string ColumnName = "";
            string ColumnValue = "";

            try
            {
                #region "Tier Array Declarations"
                //Tier Columns
                string[] PreferredNetworkNominalCopay = new string[5];
                string[] NonPreferredNetworkNominalCopay = new string[5];
                string[] Coverageinthegap = new string[5];
                string[] GapPreferredRxCostShareAverage = new string[5];
                string[] GapNonPreferredRxCostShareAverage = new string[5];
                string[] PreferredRxCostShareAverage = new string[5];
                string[] NonPreferredRxCostShareAverage = new string[5];

                //NonTier Columns
                string[] EffectiveDate = new string[5];
                string[] TermDate = new string[5];
                string[] StateRegion = new string[5];
                string[] Plan = new string[5];
                string[] PlanName = new string[5];
                string[] DeductibleTiers = new string[5];
                string[] ValueDeductible = new string[5];
                string[] CoverageYN = new string[5];
                string[] PreICLDrugCoveragethroughGap = new string[5];
                string[] MailOrderMultiplier = new string[5];
                string[] SupplementaldrugsCoverageYN = new string[5];
                string[] SupplementaldrugNames = new string[5];
                string[] SupplementaldrugTiers = new string[5];
                string[] FormularyName = new string[5];
                string[] Type = new string[5];
                string[] OTCSteptherapy = new string[5];
                string[] ExcludedDrugsEnhancedAlternativeONLY = new string[5];
                string[] PreferredRetailCostSharing1Month = new string[5];
                string[] PreferredRetailCostSharing3Month = new string[5];
                string[] StandardRetailCostSharing1Month = new string[5];
                string[] StandardRetailCostSharing3Month = new string[5];
                string[] StandardMailOrderCostSharing1Month = new string[5];
                string[] StandardMailOrderCostSharing3Month = new string[5];
                string[] PreferredMailOrderCostSharing1Month = new string[5];
                string[] PreferredMailOrderCostSharing3Month = new string[5];
                string[] OutofNetworkPharmacy = new string[5];
                string[] LongTermCarePharmacy = new string[5];
                string[] GapCoveragePreferredRetailCostSharing1Month = new string[5];
                string[] GapCoveragePreferredRetailCostSharing3Month = new string[5];
                string[] GapCoverageStandardRetailCostSharing1Month = new string[5];
                string[] GapCoverageStandardRetailCostSharing3Month = new string[5];
                string[] GapCoverageStandardMailOrderCostSharing1Month = new string[5];
                string[] GapCoverageStandardMailOrderCostSharing3Month = new string[5];
                string[] GapCoveragePreferredMailOrderCostSharing1Month = new string[5];
                string[] GapCoveragePreferredMailOrderCostSharing3Month = new string[5];
                string[] GapCoverageOutofNetworkPharmacy = new string[5];
                string[] GapCoverageLongTermCarePharmacy = new string[5];
                string[] TypeofDrugs = new string[5];
                string[] PrescriptionTier = new string[5];
                string[] TierDescription = new string[5];
                #endregion "Tier Array Declarations"

                int ColumnCount = table.Columns.Count;
                //Rule variables//
                string RPlan = "";
                int rowIDProperty = 0;
                #region "Tierwise array filling and processing"                
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("["); //Start of JSon string            
                    for (int tblRowNo = 1; tblRowNo < table.Rows.Count; tblRowNo++)
                    {
                        //collect tiers data for current row 
                        for (int tblColumnNo = 0; tblColumnNo < ColumnCount; tblColumnNo++)
                        {
                            ColumnValue = table.Rows[tblRowNo][tblColumnNo].ToString();
                            if (ColumnValue == "") ColumnValue = "NA";
                            if (ColumnValue == "0") ColumnValue = "$0.00";
                            //Tier Columns
                            #region "Filling arrays from excel"
                            if (tblColumnNo.Equals((int)Tiers.PreferredNetworkNominalCopayTier1)) PreferredNetworkNominalCopay[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredNetworkNominalCopayTier2)) PreferredNetworkNominalCopay[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredNetworkNominalCopayTier3)) PreferredNetworkNominalCopay[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredNetworkNominalCopayTier4)) PreferredNetworkNominalCopay[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredNetworkNominalCopayTier5)) PreferredNetworkNominalCopay[4] = ConvertDecimaltoInt(ColumnValue);

                            if (tblColumnNo.Equals((int)Tiers.NonPreferredNetworkNominalCopayTier1)) NonPreferredNetworkNominalCopay[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredNetworkNominalCopayTier2)) NonPreferredNetworkNominalCopay[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredNetworkNominalCopayTier3)) NonPreferredNetworkNominalCopay[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredNetworkNominalCopayTier4)) NonPreferredNetworkNominalCopay[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredNetworkNominalCopayTier5)) NonPreferredNetworkNominalCopay[4] = ConvertDecimaltoInt(ColumnValue);

                            if (tblColumnNo.Equals((int)Tiers.CoverageinthegapTier1)) Coverageinthegap[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.CoverageinthegapTier2)) Coverageinthegap[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.CoverageinthegapTier3)) Coverageinthegap[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.CoverageinthegapTier4)) Coverageinthegap[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.CoverageinthegapTier5)) Coverageinthegap[4] = ConvertDecimaltoInt(ColumnValue);

                            if (tblColumnNo.Equals((int)Tiers.GapPreferredRxCostShareAverageTier1)) GapPreferredRxCostShareAverage[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapPreferredRxCostShareAverageTier2)) GapPreferredRxCostShareAverage[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapPreferredRxCostShareAverageTier3)) GapPreferredRxCostShareAverage[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapPreferredRxCostShareAverageTier4)) GapPreferredRxCostShareAverage[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapPreferredRxCostShareAverageTier5)) GapPreferredRxCostShareAverage[4] = ConvertDecimaltoInt(ColumnValue);

                            if (tblColumnNo.Equals((int)Tiers.GapNonPreferredRxCostShareAverageTier1)) GapNonPreferredRxCostShareAverage[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapNonPreferredRxCostShareAverageTier2)) GapNonPreferredRxCostShareAverage[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapNonPreferredRxCostShareAverageTier3)) GapNonPreferredRxCostShareAverage[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapNonPreferredRxCostShareAverageTier4)) GapNonPreferredRxCostShareAverage[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.GapNonPreferredRxCostShareAverageTier5)) GapNonPreferredRxCostShareAverage[4] = ConvertDecimaltoInt(ColumnValue);

                            if (tblColumnNo.Equals((int)Tiers.PreferredRxCostShareAverageTier1)) PreferredRxCostShareAverage[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredRxCostShareAverageTier2)) PreferredRxCostShareAverage[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredRxCostShareAverageTier3)) PreferredRxCostShareAverage[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredRxCostShareAverageTier4)) PreferredRxCostShareAverage[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.PreferredRxCostShareAverageTier5)) PreferredRxCostShareAverage[4] = ConvertDecimaltoInt(ColumnValue);

                            if (tblColumnNo.Equals((int)Tiers.NonPreferredRxCostShareAverageTier1)) NonPreferredRxCostShareAverage[0] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredRxCostShareAverageTier2)) NonPreferredRxCostShareAverage[1] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredRxCostShareAverageTier3)) NonPreferredRxCostShareAverage[2] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredRxCostShareAverageTier4)) NonPreferredRxCostShareAverage[3] = ConvertDecimaltoInt(ColumnValue);
                            if (tblColumnNo.Equals((int)Tiers.NonPreferredRxCostShareAverageTier5)) NonPreferredRxCostShareAverage[4] = ConvertDecimaltoInt(ColumnValue);

                            //NonTier Columns
                            if (tblColumnNo.Equals((int)Tiers.EffectiveDate)) EffectiveDate[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.EffectiveDate)) EffectiveDate[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.EffectiveDate)) EffectiveDate[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.EffectiveDate)) EffectiveDate[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.EffectiveDate)) EffectiveDate[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.TermDate)) TermDate[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.TermDate)) TermDate[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.TermDate)) TermDate[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.TermDate)) TermDate[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.TermDate)) TermDate[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.StateRegion)) StateRegion[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.StateRegion)) StateRegion[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.StateRegion)) StateRegion[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.StateRegion)) StateRegion[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.StateRegion)) StateRegion[4] = ColumnValue;


                            if (tblColumnNo.Equals((int)Tiers.Plan))
                            {
                                if(ColumnValue != null && ColumnValue.StartsWith("S") == true)
                                {
                                    ColumnValue = ColumnValue.PadRight(11, '0');
                                }
                                Plan[0] = ColumnValue;
                                Plan[1] = ColumnValue;
                                Plan[2] = ColumnValue;
                                Plan[3] = ColumnValue;
                                Plan[4] = ColumnValue;
                            }

                            if (tblColumnNo.Equals((int)Tiers.PlanName)) PlanName[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PlanName)) PlanName[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PlanName)) PlanName[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PlanName)) PlanName[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PlanName)) PlanName[4] = ColumnValue;


                            if (tblColumnNo.Equals((int)Tiers.DeductibleTiers)) DeductibleTiers[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.DeductibleTiers)) DeductibleTiers[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.DeductibleTiers)) DeductibleTiers[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.DeductibleTiers)) DeductibleTiers[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.DeductibleTiers)) DeductibleTiers[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.ValueDeductible)) ValueDeductible[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.ValueDeductible)) ValueDeductible[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.ValueDeductible)) ValueDeductible[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.ValueDeductible)) ValueDeductible[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.ValueDeductible)) ValueDeductible[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.CoverageYN))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    CoverageYN[0] = "Yes";
                                }
                                else
                                {
                                    CoverageYN[0] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.CoverageYN))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    CoverageYN[1] = "Yes";
                                }
                                else
                                {
                                    CoverageYN[1] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.CoverageYN))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    CoverageYN[2] = "Yes";
                                }
                                else
                                {
                                    CoverageYN[2] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.CoverageYN))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    CoverageYN[3] = "Yes";
                                }
                                else
                                {
                                    CoverageYN[3] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.CoverageYN))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    CoverageYN[4] = "Yes";
                                }
                                else
                                {
                                    CoverageYN[4] = "No";
                                }
                            }

                            if (tblColumnNo.Equals((int)Tiers.PreICLDrugCoveragethroughGap)) PreICLDrugCoveragethroughGap[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PreICLDrugCoveragethroughGap)) PreICLDrugCoveragethroughGap[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PreICLDrugCoveragethroughGap)) PreICLDrugCoveragethroughGap[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PreICLDrugCoveragethroughGap)) PreICLDrugCoveragethroughGap[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.PreICLDrugCoveragethroughGap)) PreICLDrugCoveragethroughGap[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.MailOrderMultiplier)) MailOrderMultiplier[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.MailOrderMultiplier)) MailOrderMultiplier[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.MailOrderMultiplier)) MailOrderMultiplier[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.MailOrderMultiplier)) MailOrderMultiplier[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.MailOrderMultiplier)) MailOrderMultiplier[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugsCoverageYN)) SupplementaldrugsCoverageYN[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugsCoverageYN)) SupplementaldrugsCoverageYN[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugsCoverageYN)) SupplementaldrugsCoverageYN[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugsCoverageYN)) SupplementaldrugsCoverageYN[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugsCoverageYN)) SupplementaldrugsCoverageYN[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugNames)) SupplementaldrugNames[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugNames)) SupplementaldrugNames[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugNames)) SupplementaldrugNames[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugNames)) SupplementaldrugNames[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugNames)) SupplementaldrugNames[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugTiers)) SupplementaldrugTiers[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugTiers)) SupplementaldrugTiers[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugTiers)) SupplementaldrugTiers[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugTiers)) SupplementaldrugTiers[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.SupplementaldrugTiers)) SupplementaldrugTiers[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.FormularyName)) FormularyName[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.FormularyName)) FormularyName[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.FormularyName)) FormularyName[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.FormularyName)) FormularyName[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.FormularyName)) FormularyName[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.Type)) Type[0] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.Type)) Type[1] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.Type)) Type[2] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.Type)) Type[3] = ColumnValue;
                            if (tblColumnNo.Equals((int)Tiers.Type)) Type[4] = ColumnValue;

                            if (tblColumnNo.Equals((int)Tiers.OTCSteptherapy))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    OTCSteptherapy[0] = "Yes";
                                }
                                else
                                {
                                    OTCSteptherapy[0] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.OTCSteptherapy))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    OTCSteptherapy[1] = "Yes";
                                }
                                else
                                {
                                    OTCSteptherapy[1] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.OTCSteptherapy))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    OTCSteptherapy[2] = "Yes";
                                }
                                else
                                {
                                    OTCSteptherapy[2] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.OTCSteptherapy))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    OTCSteptherapy[3] = "Yes";
                                }
                                else
                                {
                                    OTCSteptherapy[3] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.OTCSteptherapy))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    OTCSteptherapy[4] = "Yes";
                                }
                                else
                                {
                                    OTCSteptherapy[4] = "No";
                                }
                            }

                            if (tblColumnNo.Equals((int)Tiers.ExcludedDrugsEnhancedAlternativeONLY))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[0] = "Yes";
                                }
                                else
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[0] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.ExcludedDrugsEnhancedAlternativeONLY))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[1] = "Yes";
                                }
                                else
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[1] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.ExcludedDrugsEnhancedAlternativeONLY))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[2] = "Yes";
                                }
                                else
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[2] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.ExcludedDrugsEnhancedAlternativeONLY))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[3] = "Yes";
                                }
                                else
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[3] = "No";
                                }
                            }
                            if (tblColumnNo.Equals((int)Tiers.ExcludedDrugsEnhancedAlternativeONLY))
                            {
                                if (!String.IsNullOrEmpty(ColumnValue) && ColumnValue == "Y")
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[4] = "Yes";
                                }
                                else
                                {
                                    ExcludedDrugsEnhancedAlternativeONLY[4] = "No";
                                }
                            }

                            //Columns Not in Excel but Present in Design
                            TypeofDrugs[0] = "Generic";
                            TypeofDrugs[1] = "Generic";
                            TypeofDrugs[2] = "Generic & brand";
                            TypeofDrugs[3] = "Generic & brand";
                            TypeofDrugs[4] = "Generic & brand";

                            PrescriptionTier[0] = "Tier 1";
                            PrescriptionTier[1] = "Tier 2";
                            PrescriptionTier[2] = "Tier 3";
                            PrescriptionTier[3] = "Tier 4";
                            PrescriptionTier[4] = "Tier 5";

                            TierDescription[0] = "Preferred Generic";
                            TierDescription[1] = "Generic";
                            TierDescription[2] = "Preferred Brand";
                            TierDescription[3] = "Non-Preferred Drug";
                            TierDescription[4] = "Specialty";

                            #endregion "Filling arrays from excel"
                        }

                        //Convert 1-5 Tiers to rows [process tiers data for current row]
                        #region "Process Tier Arrays"
                        for (int TierNo = 0; TierNo < 5; TierNo++)
                        {
                            JSONString.Append("{");
                            JSONString.Append("\"RowIDProperty\":" + "\"" + rowIDProperty + "\",");
                            rowIDProperty++;
                            #region RULES LOGIC GOES HERE
                            RPlan = Plan[0].ToString();

                            //Pre ICL Tier wise logic
                            string PreICL = "";
                            string[] PreICLArr = new string[5];
                            string[] PreICLArrResult = new string[5];
                            if (Coverageinthegap[TierNo] != "NA")
                            {
                                
                                if (ConvertToNumber(Coverageinthegap[TierNo]) >= 0)
                                {
                                    CoverageYN[TierNo] = "Yes";
                                    PreICL = PreICLDrugCoveragethroughGap[TierNo];
                                    if (!String.IsNullOrEmpty(PreICL))
                                    {
                                        if (PreICL.Contains(","))
                                        {
                                            PreICLArr = PreICL.Split(',');
                                            for (int i = 0; i < PreICLArr.Length; i++)
                                            {
                                                if (PreICLArr[i].Trim(' ') == "Full Tier Coverage")
                                                {
                                                    PreICLArrResult[i] = "Full";
                                                }
                                                else if (PreICLArr[i].Trim(' ') == "Partial Tier Coverage")
                                                {
                                                    PreICLArrResult[i] = "Partial";
                                                }
                                                else
                                                {
                                                    PreICLArrResult[i] = "NA";
                                                }
                                            }
                                            for (int i = PreICLArr.Length; i < 5; i++)
                                            {
                                                PreICLArrResult[i] = "NA";
                                            }

                                        }
                                        else
                                        {
                                            if (PreICL == "Full Tier Coverage")
                                            {
                                                PreICLArrResult[0] = "Full";
                                                PreICLArrResult[1] = "Full";
                                                PreICLArrResult[2] = "Full";
                                                PreICLArrResult[3] = "Full";
                                                PreICLArrResult[4] = "Full";
                                            }
                                            else if (PreICL == "Partial Tier Coverage")
                                            {
                                                PreICLArrResult[0] = "Partial";
                                                PreICLArrResult[1] = "Partial";
                                                PreICLArrResult[2] = "Partial";
                                                PreICLArrResult[3] = "Partial";
                                                PreICLArrResult[4] = "Partial";
                                            }
                                            else
                                            {
                                                PreICLArrResult[0] = "NA";
                                                PreICLArrResult[1] = "NA";
                                                PreICLArrResult[2] = "NA";
                                                PreICLArrResult[3] = "NA";
                                                PreICLArrResult[4] = "NA";
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    CoverageYN[TierNo] = "No";
                                    PreICLArrResult[TierNo] = "NA";
                                }

                            }
                            else
                            {
                                CoverageYN[TierNo] = "No";
                                PreICLArrResult[TierNo] = "NA";
                            }

                            //Tierwise Calculations
                            switch (TierNo)
                            {
                                case 0:
                                    //PreferredRetailCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = PreferredNetworkNominalCopay[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    //PreferredRetailCostSharing3Month
                                    if (RPlan[0] == 'S')
                                    {
                                        if (PreferredRetailCostSharing1Month[TierNo] != "NA")
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = CheckAppender(PreferredRetailCostSharing1Month[TierNo], (ConvertToDecimal(PreferredRetailCostSharing1Month[TierNo]) * 3).ToString());
                                        }
                                        else
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = "NA";
                                        }

                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing3Month[TierNo] = "NA";
                                    }
                                    //StandardRetailCostSharing1Month
                                    StandardRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo].ToString();
                                    //StandardRetailCostSharing3Month
                                    StandardRetailCostSharing3Month[TierNo] = CheckAppender(StandardRetailCostSharing1Month[TierNo], (ConvertToDecimal(StandardRetailCostSharing1Month[TierNo]) * 3).ToString());
                                    //StandardMailOrderCostSharing1Month
                                    StandardMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                                    //StandardMailOrderCostSharing3Month
                                    StandardMailOrderCostSharing3Month[TierNo] = StandardRetailCostSharing3Month[TierNo];
                                    //PreferredMailOrderCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = PreferredRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    PreferredMailOrderCostSharing3Month[TierNo] = "$0.00";
                                    //PreferredMailOrderCostSharing3Month                                    
                                    break;
                                case 1:
                                    //PreferredRetailCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = PreferredNetworkNominalCopay[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    //PreferredRetailCostSharing3Month
                                    if (RPlan[0] == 'S')
                                    {
                                        if (PreferredRetailCostSharing1Month[TierNo] != "NA")
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = CheckAppender(PreferredRetailCostSharing1Month[TierNo], (ConvertToDecimal(PreferredRetailCostSharing1Month[TierNo]) * 3).ToString());
                                        }
                                        else
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = "NA";
                                        }

                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing3Month[TierNo] = "NA";
                                    }
                                    //StandardRetailCostSharing1Month
                                    StandardRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo].ToString();
                                    //StandardRetailCostSharing3Month
                                    StandardRetailCostSharing3Month[TierNo] = CheckAppender(StandardRetailCostSharing1Month[TierNo], (ConvertToDecimal(StandardRetailCostSharing1Month[TierNo]) * 3).ToString());
                                    //StandardMailOrderCostSharing1Month
                                    StandardMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                                    //StandardMailOrderCostSharing3Month
                                    StandardMailOrderCostSharing3Month[TierNo] = StandardRetailCostSharing3Month[TierNo].ToString();
                                    //PreferredMailOrderCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = PreferredRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    //PreferredMailOrderCostSharing3Month
                                    //PreferredMailOrderCostSharing3Month[TierNo] = (ConvertToDecimal(PreferredMailOrderCostSharing1Month[TierNo]) * ConvertToDecimal(MailOrderMultiplier[TierNo])).ToString();
                                    PreferredMailOrderCostSharing3Month[TierNo] = CheckAppender(PreferredMailOrderCostSharing1Month[TierNo], (ConvertToDecimal(PreferredMailOrderCostSharing1Month[TierNo]) * ConvertToDecimal(MailOrderMultiplier[TierNo])).ToString());
                                    break;
                                case 2:
                                    //PreferredRetailCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = PreferredNetworkNominalCopay[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    //PreferredRetailCostSharing3Month
                                    if (RPlan[0] == 'S')
                                    {
                                        if (PreferredRetailCostSharing1Month[TierNo] != "NA")
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = CheckAppender(PreferredRetailCostSharing1Month[TierNo], (ConvertToDecimal(PreferredRetailCostSharing1Month[TierNo]) * 3).ToString());
                                        }
                                        else
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = "NA";
                                        }

                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing3Month[TierNo] = "NA";
                                    }
                                    //StandardRetailCostSharing1Month
                                    StandardRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo].ToString();
                                    //StandardRetailCostSharing3Month
                                    StandardRetailCostSharing3Month[TierNo] = CheckAppender(StandardRetailCostSharing1Month[TierNo], (ConvertToDecimal(StandardRetailCostSharing1Month[TierNo]) * 3).ToString());
                                    //StandardMailOrderCostSharing1Month
                                    StandardMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo].ToString();
                                    //StandardMailOrderCostSharing3Month
                                    StandardMailOrderCostSharing3Month[TierNo] = StandardRetailCostSharing3Month[TierNo].ToString();
                                    //PreferredMailOrderCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = PreferredRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    //PreferredMailOrderCostSharing3Month
                                    //PreferredMailOrderCostSharing3Month[TierNo] = (ConvertToDecimal(PreferredMailOrderCostSharing1Month[TierNo]) * ConvertToDecimal(MailOrderMultiplier[TierNo])).ToString();
                                    PreferredMailOrderCostSharing3Month[TierNo] = CheckAppender(PreferredMailOrderCostSharing1Month[TierNo], (ConvertToDecimal(PreferredMailOrderCostSharing1Month[TierNo]) * ConvertToDecimal(MailOrderMultiplier[TierNo])).ToString());
                                    break;
                                case 3:
                                    //PreferredRetailCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        if (PreferredNetworkNominalCopay[TierNo].Contains("%"))
                                        {
                                            PreferredRetailCostSharing1Month[TierNo] = PreferredNetworkNominalCopay[TierNo];
                                        }
                                        else if (PreferredNetworkNominalCopay[TierNo] == "NA")
                                        {
                                            PreferredRetailCostSharing1Month[TierNo] = "NA";
                                        }
                                        else
                                        {
                                            decimal preferredNominalCopay = ConvertToDecimal(PreferredNetworkNominalCopay[TierNo]);
                                            if (preferredNominalCopay > 1)
                                            {
                                                PreferredRetailCostSharing1Month[TierNo] = PreferredNetworkNominalCopay[TierNo];
                                            }
                                            else
                                            {
                                                PreferredRetailCostSharing1Month[TierNo] = (preferredNominalCopay * 100).ToString() + "%";
                                            }
                                        }
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    //PreferredRetailCostSharing3Month
                                    if (RPlan[0] == 'S')
                                    {
                                        if (PreferredRetailCostSharing1Month[TierNo] != "NA")
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = PreferredRetailCostSharing1Month[TierNo];
                                        }
                                        else
                                        {
                                            PreferredRetailCostSharing3Month[TierNo] = "NA";
                                        }
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing3Month[TierNo] = "NA";
                                    }
                                    //StandardRetailCostSharing1Month
                                    StandardRetailCostSharing1Month[TierNo] = (ConvertToDecimal(NonPreferredNetworkNominalCopay[TierNo]) * 3).ToString();
                                    if (NonPreferredNetworkNominalCopay[TierNo].Contains("%"))
                                    {
                                        StandardRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo];
                                    }
                                    else if (NonPreferredNetworkNominalCopay[TierNo] == "NA")
                                    {
                                        StandardRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    else
                                    {
                                        decimal nonPreferredNominalCopay = ConvertToDecimal(NonPreferredNetworkNominalCopay[TierNo]);
                                        if (nonPreferredNominalCopay > 1)
                                        {
                                            StandardRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo];
                                        }
                                        else
                                        {
                                            StandardRetailCostSharing1Month[TierNo] = (nonPreferredNominalCopay * 100).ToString() + "%";
                                        }
                                    }
                                    //StandardRetailCostSharing3Month
                                    StandardRetailCostSharing3Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                                    //StandardMailOrderCostSharing1Month
                                    StandardMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                                    //StandardMailOrderCostSharing3Month
                                    StandardMailOrderCostSharing3Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                                    //PreferredMailOrderCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = PreferredRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    //PreferredMailOrderCostSharing3Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredMailOrderCostSharing3Month[TierNo] = PreferredMailOrderCostSharing1Month[TierNo];
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredMailOrderCostSharing3Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                                    }
                                    break;
                                case 4:
                                    //PreferredRetailCostSharing1Month
                                    if (NonPreferredNetworkNominalCopay[TierNo].Contains("%"))
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo];
                                    }
                                    else if(NonPreferredNetworkNominalCopay[TierNo] == "NA")
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    else
                                    {
                                        decimal nonPreferredNominalCopay = ConvertToDecimal(NonPreferredNetworkNominalCopay[TierNo]);
                                        if (nonPreferredNominalCopay > 1)
                                        {
                                            PreferredRetailCostSharing1Month[TierNo] = NonPreferredNetworkNominalCopay[TierNo]; //+ "%";
                                        }
                                        else
                                        {
                                            PreferredRetailCostSharing1Month[TierNo] = (nonPreferredNominalCopay * 100).ToString() + "%";
                                        }
                                    }
                                    string nonPreferredNominalCopayStr = PreferredRetailCostSharing1Month[TierNo];
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredRetailCostSharing1Month[TierNo] = "NA";
                                    }
                                    //PreferredRetailCostSharing3Month
                                    PreferredRetailCostSharing3Month[TierNo] = "NA";
                                    //StandardRetailCostSharing1Month
                                    StandardRetailCostSharing1Month[TierNo] = nonPreferredNominalCopayStr;
                                    //StandardRetailCostSharing3Month
                                    StandardRetailCostSharing3Month[TierNo] = "NA";
                                    //StandardMailOrderCostSharing1Month
                                    StandardMailOrderCostSharing1Month[TierNo] = nonPreferredNominalCopayStr;
                                    //StandardMailOrderCostSharing3Month
                                    StandardMailOrderCostSharing3Month[TierNo] = "NA";
                                    //PreferredMailOrderCostSharing1Month
                                    if (RPlan[0] == 'S')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = nonPreferredNominalCopayStr;
                                    }
                                    if (RPlan[0] == 'H')
                                    {
                                        PreferredMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo].ToString();
                                    }
                                    //PreferredMailOrderCostSharing3Month
                                    PreferredMailOrderCostSharing3Month[TierNo] = "NA";
                                    break;
                            }

                            //PreferredMailOrderCostSharing1Month -- Same values for all tiers
                            if (RPlan[0] == 'S')
                            {
                                PreferredMailOrderCostSharing1Month[TierNo] = PreferredRetailCostSharing1Month[TierNo];
                            }
                            if (RPlan[0] == 'H')
                            {
                                PreferredMailOrderCostSharing1Month[TierNo] = StandardRetailCostSharing1Month[TierNo];
                            }

                            OutofNetworkPharmacy[TierNo] = StandardRetailCostSharing1Month[TierNo];
                            LongTermCarePharmacy[TierNo] = StandardRetailCostSharing1Month[TierNo];
                            GapCoveragePreferredRetailCostSharing1Month[TierNo] = "NA";
                            GapCoveragePreferredRetailCostSharing3Month[TierNo] = "NA";
                            GapCoverageStandardRetailCostSharing1Month[TierNo] = Coverageinthegap[TierNo].ToString();
                            if (Coverageinthegap[TierNo].ToString() == "NA")
                            {
                                GapCoverageStandardRetailCostSharing3Month[TierNo] = "NA";
                            }
                            else
                            {
                                GapCoverageStandardRetailCostSharing3Month[TierNo] = CheckAppender(Coverageinthegap[TierNo], (ConvertToDecimal(Coverageinthegap[TierNo].ToString()) * ConvertToDecimal(MailOrderMultiplier[TierNo].ToString())).ToString());
                            }
                            GapCoverageStandardMailOrderCostSharing1Month[TierNo] = Coverageinthegap[TierNo].ToString();
                            if (Coverageinthegap[TierNo].ToString() == "NA")
                            {
                                GapCoverageStandardMailOrderCostSharing3Month[TierNo] = "NA";
                            }
                            else
                            {
                                GapCoverageStandardMailOrderCostSharing3Month[TierNo] = CheckAppender(Coverageinthegap[TierNo], (ConvertToDecimal(Coverageinthegap[TierNo].ToString()) * ConvertToDecimal(MailOrderMultiplier[TierNo].ToString())).ToString());
                            }
                            GapCoveragePreferredMailOrderCostSharing1Month[TierNo] = Coverageinthegap[TierNo].ToString();
                            if(TierNo == 0)
                            {
                                if (CoverageYN[TierNo].ToString() == "Yes")
                                {
                                    GapCoveragePreferredMailOrderCostSharing3Month[TierNo] = "$0.00";
                                }
                                else
                                {
                                    GapCoveragePreferredMailOrderCostSharing3Month[TierNo] = "NA";
                                }
                            }
                            else
                            {
                                if(GapCoveragePreferredMailOrderCostSharing1Month[TierNo] == "NA")
                                {
                                    GapCoveragePreferredMailOrderCostSharing3Month[TierNo] = "NA";
                                }
                                else
                                {
                                    GapCoveragePreferredMailOrderCostSharing3Month[TierNo] = CheckAppender(Coverageinthegap[TierNo], (ConvertToDecimal(Coverageinthegap[TierNo].ToString()) * ConvertToDecimal(MailOrderMultiplier[TierNo].ToString())).ToString());
                                }
                            }

                            GapCoverageOutofNetworkPharmacy[TierNo] = Coverageinthegap[TierNo].ToString();
                            GapCoverageLongTermCarePharmacy[TierNo] = Coverageinthegap[TierNo].ToString();

                            #endregion RULES LOGIC GOES HERE

                            for (int tblColNo = 0; tblColNo < ColumnCount; tblColNo++)
                            {
                                ColumnName = JsonColumnNameFormatting(table.Columns[tblColNo].ColumnName.ToString());
                                ColumnValue = table.Rows[tblRowNo][tblColNo].ToString();
                                //Columns Not in Header row SubHeader
                                //Coverageinthegap => CoverageYN
                                //F19 => PreICLDrugCoveragethroughGap
                                //F20 => Coverageinthegap
                                //Supplementaldrugs => SupplementaldrugsCoverageYN
                                //F37 => SupplementaldrugNames
                                //F38 => SupplementaldrugTiers
                                if (ColumnName == "Coverageinthegap") ColumnName = "CoverageYN";
                                if (ColumnName == "Column19") ColumnName = "PreICLDrugCoveragethroughGap";
                                if (ColumnName == "Column20") ColumnName = "Coverageinthegap";
                                if (ColumnName == "Supplementaldrugs") ColumnName = "SupplementaldrugsCoverageYN";
                                if (ColumnName == "Column23") ColumnName = "SupplementaldrugNames";
                                if (ColumnName == "Column24") ColumnName = "SupplementaldrugTiers";

                                if (tblColNo < ColumnCount) //Loop through all columns, except last
                                {
                                    switch (ColumnName)
                                    {
                                        case "EffectiveDate":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(EffectiveDate[TierNo]) + "\",");
                                            break;
                                        case "TermDate":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(TermDate[TierNo]) + "\",");
                                            break;
                                        case "StateRegion":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(StateRegion[TierNo]) + "\",");
                                            break;
                                        case "Plan":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(Plan[TierNo]) + "\",");
                                            break;
                                        case "PlanName":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + PlanName[TierNo] + "\",");
                                            break;
                                        case "PreferredNetworkNominalCopay":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + PreferredNetworkNominalCopay[TierNo] + "\",");
                                            break;
                                        case "NonPreferredNetworkNominalCopay":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + NonPreferredNetworkNominalCopay[TierNo] + "\",");
                                            break;
                                        case "DeductibleTiers":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + DeductibleTiers[TierNo] + "\",");
                                            break;
                                        case "ValueDeductible":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + ValueDeductible[TierNo] + "\",");
                                            break;
                                        case "CoverageYN":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + CoverageYN[TierNo] + "\",");
                                            break;
                                        case "PreICLDrugCoveragethroughGap":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + PreICLArrResult[TierNo] + "\",");
                                            break;
                                        case "Coverageinthegap":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + Coverageinthegap[TierNo] + "\",");
                                            break;
                                        case "GapPreferredRxCostShareAverage":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + GapPreferredRxCostShareAverage[TierNo] + "\",");
                                            break;
                                        case "GapNonPreferredRxCostShareAverage":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + GapNonPreferredRxCostShareAverage[TierNo] + "\",");
                                            break;
                                        case "MailOrderMultiplier":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + MailOrderMultiplier[TierNo] + "\",");
                                            break;
                                        case "SupplementaldrugsCoverageYN":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + SupplementaldrugsCoverageYN[TierNo] + "\",");
                                            break;
                                        case "SupplementaldrugNames":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + SupplementaldrugNames[TierNo] + "\",");
                                            break;
                                        case "SupplementaldrugTiers":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + SupplementaldrugTiers[TierNo] + "\",");
                                            break;
                                        case "FormularyName":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + FormularyName[TierNo] + "\",");
                                            break;
                                        case "Type":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + Type[TierNo] + "\",");
                                            break;
                                        case "PreferredRxCostShareAverage":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + PreferredRxCostShareAverage[TierNo] + "\",");
                                            break;
                                        case "NonPreferredRxCostShareAverage":
                                            JSONString.Append("\"" + ColumnName + "\":" + "\"" + NonPreferredRxCostShareAverage[TierNo] + "\",");
                                            break;
                                        case "OTCStepTherapyYN":
                                            //JSONString.Append("\"" + ColumnName + "\":" + "\"" + OTCSteptherapy[TierNo] + "\",");
                                            JSONString.Append("\"OTCSteptherapy\":" + "\"" + OTCSteptherapy[TierNo] + "\",");
                                            break;
                                        case "ExcludedDrugsEnhancedAlternativeonlyYN":                                            
                                            JSONString.Append("\"ExcludedDrugsEnhancedAlternativeONLY\":" + "\"" + ExcludedDrugsEnhancedAlternativeONLY[TierNo] + "\",");
                                            //JSONString.Append("\"" + ColumnName + "\":" + "\"" + ExcludedDrugsEnhancedAlternativeONLY[TierNo] + "\",");
                                            break;

                                    }//end switch                               
                                }//end if table columns count except last column
                                if (tblColNo+1 == ColumnCount) //last column
                                {
                                    //Columns Not in Excel but Present in Design
                                    JSONString.Append("\"TypeofDrugs\":" + "\"" + TypeofDrugs[TierNo] + "\",");
                                    JSONString.Append("\"PrescriptionTier\":" + "\"" + PrescriptionTier[TierNo] + "\",");
                                    JSONString.Append("\"TierDescription\":" + "\"" + TierDescription[TierNo] + "\",");
                                    JSONString.Append("\"PreferredRetailCostSharing1Month\":" + "\"" + PreferredRetailCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"PreferredRetailCostSharing3Month\":" + "\"" + PreferredRetailCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"StandardRetailCostSharing1Month\":" + "\"" + StandardRetailCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"StandardRetailCostSharing3Month\":" + "\"" + StandardRetailCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"StandardMailOrderCostSharing1Month\":" + "\"" + StandardMailOrderCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"StandardMailOrderCostSharing3Month\":" + "\"" + StandardMailOrderCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"PreferredMailOrderCostSharing1Month\":" + "\"" + PreferredMailOrderCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"PreferredMailOrderCostSharing3Month\":" + "\"" + PreferredMailOrderCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"OutofNetworkPharmacy\":" + "\"" + OutofNetworkPharmacy[TierNo] + "\",");
                                    JSONString.Append("\"LongTermCarePharmacy\":" + "\"" + LongTermCarePharmacy[TierNo] + "\",");
                                    JSONString.Append("\"GapCoveragePreferredRetailCostSharing1Month\":" + "\"" + GapCoveragePreferredRetailCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoveragePreferredRetailCostSharing3Month\":" + "\"" + GapCoveragePreferredRetailCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoverageStandardRetailCostSharing1Month\":" + "\"" + GapCoverageStandardRetailCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoverageStandardRetailCostSharing3Month\":" + "\"" + GapCoverageStandardRetailCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoverageStandardMailOrderCostSharing1Month\":" + "\"" + GapCoverageStandardMailOrderCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoverageStandardMailOrderCostSharing3Month\":" + "\"" + GapCoverageStandardMailOrderCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoveragePreferredMailOrderCostSharing1Month\":" + "\"" + GapCoveragePreferredMailOrderCostSharing1Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoveragePreferredMailOrderCostSharing3Month\":" + "\"" + GapCoveragePreferredMailOrderCostSharing3Month[TierNo] + "\",");
                                    JSONString.Append("\"GapCoverageOutofNetworkPharmacy\":" + "\"" + GapCoverageOutofNetworkPharmacy[TierNo] + "\",");
                                    JSONString.Append("\"GapCoverageLongTermCarePharmacy\":" + "\"" + GapCoverageLongTermCarePharmacy[TierNo] + "\"");
                                }
                            }
                            JSONString = JSONString.Replace(".000", ".00");
                            if (TierNo == 4 && tblRowNo == table.Rows.Count - 1) // last row
                            {
                                JSONString.Append("}");
                            }
                            else
                            {
                                JSONString.Append("},");
                            }

                        }// end for table columns
                        #endregion "Process Tier Arrays"

                    }// end rows for loop
                    JSONString.Append("]");//End of JSon string
                }
                #endregion "Tierwise array filling  and processing"
            }
            catch (Exception ex)
            {
                _logger.Debug("ml PrescriptionMLCreateJson" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return JSONString.ToString();
        }

        public string PartDMLCreateJson(DataTable table)
        {
            var JSONString = new StringBuilder();
            string ColumnName = "";
            string ColumnValue = "";
            int ColumnCount = table.Columns.Count;
            try
            {
                if (table.Rows.Count > 0)
                {
                    JSONString.Append("[");
                    for (int tblRowNo = 0; tblRowNo < table.Rows.Count; tblRowNo++)
                    {
                        JSONString.Append("{");
                        for (int tblColumnNo = 0; tblColumnNo < ColumnCount; tblColumnNo++)
                        {
                            ColumnName = JsonColumnNameFormatting(table.Columns[tblColumnNo].ColumnName.ToString());
                            ColumnValue = table.Rows[tblRowNo][tblColumnNo].ToString();
                            if (ColumnValue == "") ColumnValue = "NA";
                            if (tblColumnNo < ColumnCount - 1)
                            {
                                switch (ColumnName)
                                {
                                    //case "Effectivedate":
                                    //case "TermDate":
                                    //    JSONString.Append("\"" + ColumnName + "\":" + "\"" + JsonValueFormatting(ColumnValue) + "\",");
                                    //    break;
                                    case "Contractnumber":
                                        JSONString.Append("\"Plan\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "DeductibleValue":
                                        JSONString.Append("\"ValueDeductible\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "DrugBenefitType":
                                        JSONString.Append("\"Type\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "AvgExpectedCoinsDollarAmtPreferredRetail1Month":
                                        JSONString.Append("\"PreferredRxCostShareAverage\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "AvgExpectedCoinsDollarAmtStandardRetail":
                                        JSONString.Append("\"NonPreferredRxCostShareAverage\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "GapCoverageYN":
                                        JSONString.Append("\"CoverageYN\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "OutofNetworkPharmacy1M":
                                        JSONString.Append("\"OutofNetworkPharmacy\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "GapCoverageOutofNetworkPharmacy1Month":
                                        JSONString.Append("\"GapCoverageOutofNetworkPharmacy\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "GapCoverageAvgExpectedCoinsDollarAmtPreferredRetail":
                                        JSONString.Append("\"GapNonPreferredRxCostShareAverage\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    case "GapCoverageAvgExpectedCoinsDollarAmtStandardRetail":
                                        JSONString.Append("\"GapPreferredRxCostShareAverage\":" + "\"" + ColumnValue + "\",");
                                        break;
                                    default:
                                        JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\",");
                                        break;
                                }

                            }
                            else if (tblColumnNo == ColumnCount - 1)
                            {
                                JSONString.Append("\"" + ColumnName + "\":" + "\"" + ColumnValue + "\"");
                            }
                        }
                        if (tblRowNo == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                    JSONString.Append("]");
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml PartDMLCreateJson" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return JSONString.ToString();
        }

        #endregion "Inner Data Level JSON Creation"

        #region "Helper functions for ML import"

        public string FormatCurrency(decimal Value)
        {
            string FormattedValue = Value.ToString("C", Cultures.UnitedStates);
            return FormattedValue;
        }

        public string JsonColumnNameFormatting(string JColumn)
        {
            JColumn = JColumn.Replace(" ", "");
            JColumn = JColumn.Replace("?", "");
            JColumn = JColumn.Replace("-", "");
            JColumn = JColumn.Replace("_", "");
            JColumn = JColumn.Replace("(", "");
            JColumn = JColumn.Replace(")", "");
            JColumn = JColumn.Replace("%", "");
            JColumn = JColumn.Replace("/", "");
            JColumn = JColumn.Replace("\n", "");
            JColumn = JColumn.Replace("\t", "");
            return JColumn;
        }

        public string JsonValueFormatting(string JValue)
        {
            JValue = JValue.Replace(" 00:00:00", "");
            JValue = JValue.Replace(" 12:00:00 AM", "");
            return JValue;
        }

        public decimal ConvertToNumber(string input)
        {
            if (input == null)
            {
                return -1;
            }
            input = input.Replace("%", "");
            input = input.Replace("$", "");

            decimal n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = decimal.TryParse(input, out n);
                if (!isNumeric)
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
            return n;
        }

        public decimal ConvertToDecimal(string input)
        {
            if (input == null)
            {
                return 0;
            }
            input = input.Replace("%", "");
            input = input.Replace("$", "");

            decimal n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = decimal.TryParse(input, out n);
            }
            else
            {
                n = 0;
            }
            return n;
        }

        public string CheckAppender(string input, string multipliedValue)
        {
            if (input.Contains("%") == true && !multipliedValue.Contains("%"))
            {
                multipliedValue = multipliedValue + "%";
            }
            if (input.Contains("$") == true && !multipliedValue.Contains("$"))
            {
                multipliedValue = "$" + multipliedValue;
            }
            return multipliedValue;
        }

        public string ConvertDecimaltoInt(string input)
        {
            if (input == null) return "NA";
            string Output = "";
            Decimal n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = Decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    if (n < 1)
                    {
                        n = n * 100;
                        n = Math.Round(n, 0);
                        Output = n.ToString() + "%";
                    }
                    else
                    {
                        Output = "$" + n.ToString();
                    }
                }
                else
                {
                    return input;
                }
            }
            else
            {
                Output = "$0";
            }
            return Output;
        }

        public DataTable RemoveEmptyRows(DataTable dt)
        {
            try
            {
                List<int> rowIndexesToBeDeleted = new List<int>();
                int indexCount = 0;
                foreach (var row in dt.Rows)
                {
                    var r = (DataRow)row;
                    int emptyCount = 0;
                    int itemArrayCount = r.ItemArray.Length;
                    foreach (var i in r.ItemArray) if (string.IsNullOrWhiteSpace(i.ToString())) emptyCount++;

                    if (emptyCount == itemArrayCount) rowIndexesToBeDeleted.Add(indexCount);

                    indexCount++;
                }

                int count = 0;
                foreach (var i in rowIndexesToBeDeleted)
                {
                    dt.Rows.RemoveAt(i - count);
                    count++;
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml RemoveEmptyRows" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return dt;
        }

        public DataTable RemoveEmptyColumns(DataTable table)
        {
            try
            {
                foreach (var column in table.Columns.Cast<DataColumn>().ToArray())
                {
                    if (table.AsEnumerable().All(dr => dr.IsNull(column)))
                        table.Columns.Remove(column);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml RemoveEmptyColumns" + ex.Message);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return table;
        }
        #endregion "Helper functions for ML import"

        #region "Template Validation Logic"

        public bool ProcessMasterListImportTemplateValidation(string MLSectionName, DataTable SourceTable,string filePath)
        {
            bool IsValidTemplete = false;
            try
            {
                IsValidTemplete = IsMasterListImportTemplateValid(MLSectionName, SourceTable, filePath);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                IsValidTemplete = false;
            }
            return IsValidTemplete;
        }

        private bool IsMasterListImportTemplateValid(string MLSectionName, DataTable targetTable,string FilePath)
        {
            bool IsValidTemplete = false;
            bool IsColumnSequanceValid = false;
            try
            {
                MasterListTemplateViewModel ViewModel = GetTemplateDetails(MLSectionName);
                if (ViewModel != null)
                {
                    string TemplatePath = FilePath + ViewModel.FilePath;
                    string FileName = Path.GetFileName(ViewModel.FilePath);
                    DataTable SourceTable = GetTemplateColumnList(TemplatePath, FileName);
                    IsValidTemplete = IsDataTableSame(SourceTable, targetTable);
                    IsColumnSequanceValid = IsColumnnSequanceValid(SourceTable, targetTable);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                IsValidTemplete = false;
            }
            return IsValidTemplete && IsColumnSequanceValid;
        }
        
        private MasterListTemplateViewModel GetTemplateDetails(string MLSectionName)
        {
            MasterListTemplateViewModel ViewModelList = new MasterListTemplateViewModel();
            try
            {
                ViewModelList = (from Template in this._unitOfWork.Repository<MasterListTemplate>().Get()
                             .Where(s => s.IsActive == true
                                    && s.MLSectionName.Equals(MLSectionName)
                                    )
                                 select new MasterListTemplateViewModel
                                 {
                                     MasterListTemplate1Up = Template.MasterListTemplate1Up,
                                     MLSectionName = Template.MLSectionName,
                                     FilePath = Template.FilePath,
                                     IsActive = Template.IsActive,
                                 }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ViewModelList;
        }
        
        private DataTable GetTemplateColumnList(string filePath, string FileName)
        {
            DataTable SourceDataTable = new DataTable();
            ProcessMLExcelFile processMLExcelFile = new ProcessMLExcelFile();
            //SourceDataTable = processMLExcelFile.ConvertExceltoDatatable(filePath, FileName);
            SourceDataTable = processMLExcelFile.ConvertExceltoDatatableUsingEPPlus(filePath);
            return SourceDataTable;
        }        

        private bool IsDataTableSame(DataTable SourceTable, DataTable TargetTable)
        {
           
            if (SourceTable.Columns.Count != TargetTable.Columns.Count)
                return false;
            try
            {
                for (int i = 0; i < SourceTable.Columns.Count; i++)
                {
                    bool IsValid = false;
                    for (int c = 0; c < TargetTable.Columns.Count; c++)
                    {
                        if (SourceTable.Columns[i].ToString().ToUpper().Equals(TargetTable.Columns[c].ToString().ToUpper()))
                        {
                            IsValid = true;
                            break;
                        }

                    }
                    if (IsValid == false)
                    {
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        private bool IsColumnnSequanceValid(DataTable SourceTable, DataTable TargetTable)
        {
            try
            {
                for (int i = 0; i < SourceTable.Columns.Count; i++)
                {
                    if (!SourceTable.Columns[i].ToString().ToUpper().Equals(TargetTable.Columns[i].ToString().ToUpper()))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return true;
        }


        #endregion "Template Validation Logic"
    }
}
