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

namespace tmg.equinox.reporting.Reports
{
    public class CCP_PlanByCountyFinalExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        public override int SetInitialRowPostion()
        {
            return 3;
        }

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

            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            var cellFont = ws.Cells[RowNo, ColumnNo].Style.Font;
            cellFont.SetFromFont(new Font("Calibri", 11));
            ws.Cells[RowNo, ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var Range = ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo];
            EPPlus epplus = new EPPlus();
            epplus.AssignBorders(Range);

        }

        
    }
}
