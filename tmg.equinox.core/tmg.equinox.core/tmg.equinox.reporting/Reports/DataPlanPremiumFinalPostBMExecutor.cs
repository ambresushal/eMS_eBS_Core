using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base.SQLDataAccess;
using tmg.equinox.reporting.Interface;
using System.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using tmg.equinox.reporting.Extensions;

namespace tmg.equinox.reporting.Reports
{
    public class DataPlanPremiumFinalPostBMExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
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
            bool nflag = false;
            ReportHelper reportHelper = new ReportHelper();
            string heading = row.Key.ToString();
            string Val = row.Value.ToString();
            if (reportHelper.CheckType(row.Value.ToString()) == "numeric")
            {
                //ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                nflag = true;
            }
            ws.Cells[RowNo, ColumnNo].Style.Font.Color.SetColor(Color.Red);

            if (heading == "GIVEBACK_AMOUNT" || heading == "CMS_PREMIUM_PAID" || heading == "PART_C_PREMIUM" ||
                heading == "PART_D_PREMIUM")
            {
                Val = reportHelper.FormatNumberWithDecimals(Val);
                ws.Cells[RowNo, ColumnNo].Value = Val;
            }
            else if (heading == "TOTAL_PLAN_PREMIUM" || heading == "LOW_INCOME_SUBSIDY_LEVEL")
            {
                if (heading == "LOW_INCOME_SUBSIDY_LEVEL" && Val == "0")
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                else
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "#,##0.00";
                Val = reportHelper.FormatNumberWithDecimalsWithoutDollar(Val);
                if (nflag)
                    ws.Cells[RowNo, ColumnNo].Value = Convert.ToDouble(Val);
                else
                    ws.Cells[RowNo, ColumnNo].Value = Val;
            }
            else if (heading == "REMAINING_SPAP_FOR_LEP")
            {
                ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
            }
            else
            {
                ws.Cells[RowNo, ColumnNo].Value = Val;
            }


        }
    }
}
