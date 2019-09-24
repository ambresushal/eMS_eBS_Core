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
    public class ServiceAreaExpansionsAndExits475<QueueItem> : BaseExcelReportExecuter<QueueItem>
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
            ExcelWorksheet sourceTemplateSheetCopy = excelPackage.Workbook.Worksheets[1];
            string RunDate, MemberShip, cellA1Value = "";

            RunDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            RunDate = "Report Run: " + RunDate;
            MemberShip = "Membership: " + DateTime.Now.AddMonths(1).ToString("MMMM",
                  System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + " " + DateTime.Now.Year + " Membership Bridge";
            sourceTemplateSheetCopy.Cells["A2"].Value = RunDate + "\n" + MemberShip;

            cellA1Value = DateTime.Now.Year + " - Service Area Expansions & Exits";
            sourceTemplateSheetCopy.Cells["A1"].Value = cellA1Value;
        }

        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo + 2;
            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
        }
    }
}
