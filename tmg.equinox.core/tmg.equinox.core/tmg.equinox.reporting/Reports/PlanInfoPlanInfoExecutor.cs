using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.util.extensions;
//using tmg.equinox.infrastructure.util.extensions;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base.SQLDataAccess;
using tmg.equinox.reporting.Interface;
//using tmg.equinox.repository.extensions;

namespace tmg.equinox.reporting.Reports
{
    public class PlanInfoPlanInfoExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
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

            if (!ws.Name.ToString().Contains("20"))
            {
                //string name = ((tmg.equinox.reporting.Base.Model.DownloadFileInfo)result).FileName.ToString();

                //string[] namelist = name.Split('-');

                //string[] namelist1 = namelist[1].Split(' ');

                //ws.Name = namelist1[1] + ' ' + ws.Name.ToString();

                ws.Name =  ws.Name.ToString();
            }

            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;
            bool nflag = false;

            ReportHelper reportHelper = new ReportHelper();
            string Val = row.Value.ToString();
            string heading = row.Key.ToString();
            if (heading == "PartCPremium" || heading == "PartDPremium" || heading == "PartDPremiumLess100%LIS"
                || heading == "PartDPremiumLess75%LIS" || heading == "PartDPremiumLess50%LIS" || heading == "PartDPremiumLess25%LIS")
            {
                ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatNumberWithDecimals(row.Value.ToString());
            }
            else if (heading == "PCPRequired")
            {
                ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString().ToUpperFirstLetter();
            }
            else if (heading == "BenefitYear" || heading == "PBPCode" || heading == "SegNo" || heading == "RxBin" || heading== "RxGroup") //Ints
            {
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    nflag = true;
                }
                if (nflag)
                {
                    ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                }
                else
                    ws.Cells[RowNo, ColumnNo].Value = Val;
            }
            else
            {
                ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            }

            if (heading != "PBPName")
            {
                ws.Cells[RowNo, ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

        }
    }
}
