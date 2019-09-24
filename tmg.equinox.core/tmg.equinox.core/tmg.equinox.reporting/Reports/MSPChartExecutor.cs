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
    public class MSPChartExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 3;
        }

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

            if (heading == "Year")
            {
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                    nflag = true;
                }
                if (nflag)
                    ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
                else
                    ws.Cells[RowNo, ColumnNo].Value = Val;
            }
            else
            {
                ws.Cells[RowNo, ColumnNo].Value = Val;
            }


        }
    }
}
