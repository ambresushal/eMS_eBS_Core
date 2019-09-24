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
    public class HighLevelBenefitsbyPlanPostBenchmarkExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        public override int SetInitialRowPostion()
        {
            return 3;
        }

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

            UpdateYearsInExcelCoulmns();

            return result;
        }

        public void UpdateYearsInExcelCoulmns()
        {
            ExcelWorksheet Sheet1 = excelPackage.Workbook.Worksheets[1];
            string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.Month.ToString();
            string CurrentDay = DateTime.Now.Day.ToString();
            string NextYear = ((DateTime.Now.Year) + 1).ToString();

            Sheet1.Cells["A1"].Value = NextYear + " High Level Benefits(" + CurrentMonth + " / " + CurrentDay + " / " + CurrentYear + ")";
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
            if (heading == "EyeExamRoutine")
            {
                Val= reportHelper.FormatNumberWithDecimalsYESNO(Val);
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
            else if(heading == "PremiumFieldwithCMS")
            {
                Val = reportHelper.FormatNumberWithDecimalsYESNO(Val);
                string CurrencyCellFormat = "$###,###,##0.00";
                if (reportHelper.CheckType(Val) == "numeric")
                {
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = CurrencyCellFormat;
                    nflag = true;
                }
                if (nflag)
                    ws.Cells[RowNo, ColumnNo].Value = Convert.ToDecimal(Val);
                else
                    ws.Cells[RowNo, ColumnNo].Value = Val;
            }
            else if (heading == "No")
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

            var Range = ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo];
            EPPlus epplus = new EPPlus();
            epplus.AssignBorders(Range);

        }
    }
}
