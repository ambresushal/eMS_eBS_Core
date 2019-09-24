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
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base.SQLDataAccess;
using tmg.equinox.reporting.Interface;
using System.Collections.ObjectModel;
using System.Drawing;

namespace tmg.equinox.reporting.Reports
{

    public class TIDCardExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
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

            string Val = row.Value.ToString();

            ReportHelper reportHelper = new ReportHelper();
            string heading = row.Key.ToString();
            if (heading == "OVCP" || heading == "ERCP"
                || heading == "HPCP" || heading == "SPCP" || heading == "AMBCP" || heading == "OVCP2"
                || heading == "SURGCP" || heading == "OTCCP" || heading == "MHCP" || heading == "ERCP2"
                || heading == "OVCP2" || heading == "ERCP3" || heading == "SPCP2" || heading == "DME"
                || heading == "URCP" || heading == "TIER_5_COPAY" || heading == "OVCP_OON"
                || heading == "HPCP_OON" || heading == "SPCP_OON" || heading == "AMBCP_OON"
                || heading == "SURGCP_OON" || heading == "MHCP_OON" || heading == "DME_OON"
                || heading == "OVCP_TIER2" || heading == "HPCP_TIER2" || heading == "SPCP_TIER2"
                || heading == "SURGCP_TIER2" || heading == "MHCP_TIER2" || heading == "DME_TIER2" || heading == "COINSURANCE")
            {
                Val = reportHelper.CheckDecimalAndFormat(Val);
                if (!(Val.Contains("$") && Val.Contains("%")))
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
            else if (heading == "DEDUCTIBLE" || heading == "RXCP" || heading == "BDCP2" || heading == "BDCP")
            {
                Val = reportHelper.FormatStringWithDecimalsInFirstPlace(Val);
                ws.Cells[RowNo, ColumnNo].Value = Val.Replace(".00", "");
            }
            else if (heading == "GNCP" || heading == "BDCP" || heading == "RXCP" || heading == "BDCP2")
            {

                Val = reportHelper.FormatStringWithDecimals(Val);
                Val = Val.Replace("%", "");
                //Val = Val.Replace(".00", "");
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
            else if (heading == "OLD_SUBSIDY_LEVEL" || heading == "SUBSIDY_LEVEL")
            {
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                    ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
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
            //else
            //{
            //    if (heading == "EFFECTIVE_DATE" || heading == "TERM_DATE")
            //    {
            //        if (row.Value.ToString().Length > 0)
            //            ws.Cells[RowNo, ColumnNo].Value = Val + " 00:00:00";
            //        else
            //            ws.Cells[RowNo, ColumnNo].Value = Val;
            //        //ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
            //        //ws.Cells[RowNo, ColumnNo].Formula = row.Value.ToString();
            //    }
            //    else
            //    {
            //        ws.Cells[RowNo, ColumnNo].Value = Val;
            //    }
            //}

            var cellFont = ws.Cells[RowNo, ColumnNo].Style.Font;
            cellFont.SetFromFont(new Font("Calibri", 10));

        }
    }
}
