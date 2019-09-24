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
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Reports
{
    public class Ancillary100VendorReport<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        int LastRow = 0;
        //Default Excel Copy and Excel Open Activity
        public override IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            var result = (DownloadFileInfo)CreateFile(reportSetting, dt);
            string templateFilePath = reportSetting.ReportTemplatePath;
            string fileName = result.FileName;
           
            //* copy templae in to new output path and rename the file name" example d:\download\Guid\reportName_date.xls"            
            lock (thislock)
            {
                if (!Directory.Exists(reportSetting.OutputPath))
                {
                    Directory.CreateDirectory(reportSetting.OutputPath);
                }
                File.Copy(templateFilePath, fileName, true);
            }
            var fileinfo = new FileInfo(result.FileName);
            if (fileinfo.Exists)
            {
                excelPackage = new ExcelPackage(fileinfo);
            }
            UpdateRunDateInExcel();
            return result;
        }
        public void UpdateRunDateInExcel()
        {
            ExcelWorksheet Sheet1 = excelPackage.Workbook.Worksheets[1];
            string RunDate = "";
            RunDate = DateTime.Now.ToString("MM/dd/yyyy");
            Sheet1.Cells["A2"].Value = "Run Date: " + RunDate;
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
            RowNo = RowNo + 2;
            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();

            //EPPlus epplus = new EPPlus();
            //ReportHelper reportHelper = new ReportHelper();
            string heading = row.Key.ToString();
            if (heading == "CurrentBidYear" || heading == "NextBidYear")
            {
                ws.Cells[RowNo, ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            //ExcelRange ExtraTable2Range = ws.Cells[rowNo+1, ColumnNo];
            //epplus.AssignBorders(ExtraTable2Range);
        }
    }
}
