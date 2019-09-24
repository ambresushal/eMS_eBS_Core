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
    public class CCPPartDBMLsCCPPartDBMLsFINALExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 8;
        }

        //Default starting col position in excel here.
        public override int SetInitialColPostion()
        {
            return 2;
        }

        

        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            string sheetName = "Sheet 1";
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[1];
            if (tableIndex==0)
            {
                if (rowNo == 1 && row.Key == "SheetName")
                {
                    sheetName = String.Format("{0}", row.Value.ToString());
                    excelPackage.Workbook.Worksheets[1].Name = sheetName;
                }
                if(rowNo==1 && row.Key == "PlanNameCodeYear")
                {
                    excelPackage.Workbook.Worksheets[1].Cells[1, 2].Value = row.Value.ToString();
                }
                ws = excelPackage.Workbook.Worksheets[1];
            }
            if (tableIndex > 0 && excelPackage.Workbook.Worksheets.Count == tableIndex)  //check if sheets exits = datatables.
            {
                sheetName = String.Format("Sheet{0}", tableIndex + 1);
                excelPackage.Workbook.Worksheets.Add(sheetName, excelPackage.Workbook.Worksheets[1]);              
                ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            }

            if (rowNo == 1 && row.Key == "SheetName")
            {
                sheetName = String.Format("{0}", row.Value.ToString());
                excelPackage.Workbook.Worksheets[tableIndex + 1].Name = sheetName;
            }

            if (rowNo == 1 && row.Key == "PlanNameCodeYear")
            {
                excelPackage.Workbook.Worksheets[tableIndex + 1].Cells[1, 2].Value = row.Value.ToString();
            }


            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;

            if (ColumnNo <= 20)
            {
                ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            }

        }


    }
}
