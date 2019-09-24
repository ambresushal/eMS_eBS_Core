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
    public class CtsPlanMasterFinalExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;

            if(ColumnNo>1 && ColumnNo<=7)
            {
                ws.Cells[RowNo, ColumnNo].Style.Font.Color.SetColor(Color.Red); 
            }
            else
            {
                ws.Cells[RowNo, ColumnNo].Style.Font.Color.SetColor(Color.Black);
            }

            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();


        }
    }
}
