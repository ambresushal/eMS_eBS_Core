using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.util.extensions;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base.SQLDataAccess;
using tmg.equinox.reporting.Interface;
//using tmg.equinox.repository.extensions;

namespace tmg.equinox.reporting.Reports
{
    
    public class PDPSOTPostBenchmarkExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {

        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            if (tableIndex > 0 && excelPackage.Workbook.Worksheets.Count == tableIndex)  //check if sheets exits = datatables.
            {
                string sheetName = String.Format("Sheet{0}", tableIndex + 1);
                excelPackage.Workbook.Worksheets.Add(sheetName);
            }
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;

            ReportHelper reportHelper = new ReportHelper();
            string heading = row.Key.ToString();

            
            
            if (heading == "Basic_Premium" || heading == "Supplemental_Premium" || heading== "PartD_Premium"
                || heading == "PartC_ Premium" || heading == "Total_Plan_Premium" || heading == "Giveback_Amount" || heading == "Gap_Tier1_Daily_Std" || heading == "Gap_Tier2_Daily_Pref"
                || heading == "PartB_Pct (Member pays)" || heading== "Tier3_Daily_Pref" || heading == "Tier1_Daily_Pref" || heading== "Tier1_Daily_Std" || heading== "Tier2_Daily_Std"
                || heading == "Tier3_Daily_Std" || heading == "Gap_Tier1_1_Mo_Std" || heading == "Tier2_Daily_Pref" || heading == "CMS_ PREMIUM_ PAID" || heading == "Tier1_Daily_LTC_copay" || heading== "Gap_Tier1_3_Mo_Std"
                || heading == "Tier1_1_Mo_Pref" || heading == "Tier1_3_Mo_Pref" || heading == "Tier1_3_Mo_Pref_Mail" || heading == "Tier1_3_Mo_Std" || heading == "Tier2_1_Mo_Pref" || heading == "Tier2_3_Mo_Pref"
                || heading == "Tier2_3_Mo_Pref_Mail" || heading == "Tier2_3_Mo_Std" || heading == "Tier3_1_Mo_Pref" || heading == "Tier3_2_Mo_Pref" || heading == "Tier3_3_Mo_Pref_Mail" || heading == "Tier3_1_Mo_Std"
                || heading == "Tier3_3_Mo_Std" || heading == "Tier4_1_Mo_Pref" || heading == "Tier4_3_Mo_Pref" || heading == "Tier4_3_Mo_Pref_Mail" || heading == "Tier4_Daily_Std" || heading == "Tier4_1_Mo_Std"
                || heading == "Tier4_3_Mo_Std" || heading == "Tier5_1_Mo_Pref" || heading == "Tier5_1_Mo_Std" || heading == "Gap_Tier1_3_Mo_Pref_Mail" || heading == "Cat_Gen_copay" || heading == "Cat_Brand_copay"
                || heading == "Tier1_1_Mo_LTC_copay" || heading == "Tier2_Daily_LTC_copay" || heading == "Tier2_1_Mo_LTC_copay" || heading == "Tier3_Daily_LTC_copay" || heading == "Tier3_1_Mo_LTC_copay" || heading == "Tier4_Daily_LTC_copay"
                || heading == "Tier4_1_Mo_LTC_copay" || heading == "Tier5_1_Mo_LTC_copay" || heading == "Tier1_1_Mo_Std" || heading == "Tier2_1_Mo_Std" || heading == "Gap_Tier1_Daily_Std")
            {
                ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatNumberWithDecimals(row.Value.ToString());
            }
            else if(heading == "MOOP" || heading == "PartD_Ded_Amt" || heading== "Ded_Gen_copay" || heading== "Ded_Brand_copay" || heading== "ICL_Amount" || heading== "TrOOP")
            {
                ws.Cells[RowNo, ColumnNo].Value = reportHelper.CheckDecimalAndFormat(row.Value.ToString());
            }
            else if (heading == "Deductible_Description" || heading == "OTC_Medical")
            {
                ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatStringWithDecimalsInFirstPlace(row.Value.ToString()).Replace(".00","");
            }
            else if(heading == "PartB_Pct (as filed with CMS)" || heading == "Diabetes_Supplies (as filed with CMS)" || heading == "Diabetes_Supplies (Member Pays)")
            {
                ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatStringWithDecimals(row.Value.ToString());
            }
            else if (heading == "Excluded_Drugs" || heading== "Gap_Coverage" || heading == "SNP" || heading== "OTC_Step_Therapy_PartD" || heading == "FTP")
            {
                if (row.Value.ToString().ToUpper() != "NA")
                {
                    ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString().ToUpperFirstLetter();
                }
            }
            else
            {
                ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            }


        }

    }
}
