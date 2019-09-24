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
    public class PdpPlanFinalExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
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
            string Val = row.Value.ToString();
            string heading = row.Key.ToString().Trim();
            if (heading == "GENERIC" || heading == "BRAND_PREFERRED" || heading == "BRAND_NON_PREFERRED"
                || heading == "SPECIALTY_COPAY" || heading == "TIER_5_COPAY")
            {
                Val = reportHelper.FormatStringWithDecimals(row.Value.ToString());
                if (heading != "SPECIALTY_COPAY" && heading != "TIER_5_COPAY")
                {
                    Val = Val.Replace("%", "");                    
                }
                //else
                   // Val = Val.Replace("(", "\r\n(");

                string PercentCellFormat = "0%";
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    if (row.Value.ToString().Contains("%"))
                    {
                        Decimal PercentVal = Convert.ToDecimal(Val);
                        PercentVal = PercentVal / 100;
                        ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = PercentCellFormat;
                        ws.Cells[RowNo, ColumnNo].Value = PercentVal;
                    }
                    else
                    {
                        ws.Cells[RowNo, ColumnNo].Value = Val;
                    }
                }
                else
                {
                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
            }
            else if (heading == "DEDUCTIBLE")
            {
                Val = reportHelper.FormatStringWithDecimalsInFirstPlace(row.Value.ToString());
                Val = Val.Replace("%", "");
                string PercentCellFormat = "0%";
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    if (row.Value.ToString().Contains("%"))
                    {
                        Decimal PercentVal = Convert.ToDecimal(Val);
                        PercentVal = PercentVal / 100;
                        ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = PercentCellFormat;
                        ws.Cells[RowNo, ColumnNo].Value = PercentVal;
                    }
                    else
                    {
                        ws.Cells[RowNo, ColumnNo].Value = Val;
                    }
                }
                else
                {
                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
            }
            else if (heading == "SUBSIDARY_LEVEL")
            {
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                }
                else
                {
                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
            }
            else
            {
                Val = Val.Replace("%", "");
                string PercentCellFormat = "0%";
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    if (row.Value.ToString().Contains("%"))
                    {
                        Decimal PercentVal = Convert.ToDecimal(Val);
                        PercentVal = PercentVal / 100;
                        ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = PercentCellFormat;
                        ws.Cells[RowNo, ColumnNo].Value = PercentVal;
                    }
                    else
                    {
                        ws.Cells[RowNo, ColumnNo].Value = Val;
                    }
                }
                else
                {
                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
            }


        }
    }
}
