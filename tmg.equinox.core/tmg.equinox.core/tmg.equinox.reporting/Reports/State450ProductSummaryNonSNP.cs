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
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Reports
{
    public class State450ProductSummaryNonSNP<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 9;
        }

        public override IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            var result = (DownloadFileInfo)CreateFile(reportSetting, dt);
            string templateFilePath = reportSetting.ReportTemplatePath;
            string fileName = result.FileName;

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
            PrepareStateTemplate(dt);
            AddStateSheet(dt);
            return result;
        }
        public void PrepareStateTemplate(ICollection<DataHolder> dt)
        {
            ExcelWorksheet sourceTemplateSheetCopy = excelPackage.Workbook.Worksheets[1];
            string RunDate, MemberShip = "";
            RunDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            RunDate = "Report Run: " + RunDate;
            MemberShip = "Membership: " + DateTime.Now.AddMonths(1).ToString("MMMM",
                  System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + " " + DateTime.Now.Year + " Membership Bridge";
            sourceTemplateSheetCopy.Cells["A2"].Value = RunDate +"\n"+ MemberShip;
        }
        public void AddStateSheet(ICollection<DataHolder> dt)
        {
            //Multiple sheets added as per number of states. For each state there is one sheet added below.
            ExcelWorksheet sourceTemplateSheetCopy = excelPackage.Workbook.Worksheets[1];//{Template} Sheet
            int i = 0;
            foreach (var items in dt)
            {
                if (i % 2 != 0 && dt.Count() != i)
                {
                    string sheetName = Convert.ToString(items.Data.ToList()[5].Values.ToList()[0]) + " Non-SNP";
                    string cellA1Value = sheetName + " " + DateTime.Now.Year + " Products";
                    if (i == 1)
                    {
                        sourceTemplateSheetCopy.Cells["A1"].Value = cellA1Value;
                        sourceTemplateSheetCopy.Name = sheetName;
                    }
                    else
                    {
                        excelPackage.Workbook.Worksheets.Add(sheetName, sourceTemplateSheetCopy);
                        ExcelWorksheet updateHeader = excelPackage.Workbook.Worksheets[sheetName];
                        updateHeader.Cells["A1"].Value = cellA1Value;
                    }
                }
                i++;
            }
        }
        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            int setSheetcount, ColumnNo, RowNo;
            //For select state sheet from entired result set need to gropping. 
            //like 1 & 2 resultset for sheet 1 
            //like 3 & 4 resultset for sheet 2
            //like 5 & 6 resultset for sheet 3
            setSheetcount = (tableIndex / 2) + 1;
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[setSheetcount];
            if (tableIndex % 2 == 0)
            {
                ColumnNo = SetInitialColPostion();
                ColumnNo = ColumnNo + colNo;
                ColumnNo = ColumnNo - 1;
                RowNo = SetInitialRowPostion();
                RowNo = RowNo + rowNo;
                RowNo = RowNo - 1;
            }
            else
            {
                ColumnNo = 5;
                ColumnNo = ColumnNo + colNo;
                ColumnNo = ColumnNo - 1;
                RowNo = 3;
                RowNo = RowNo + rowNo;
                RowNo = RowNo - 1;
            }
            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            ExcelRange ExtraTable2Range = ws.Cells[RowNo, ColumnNo];
            AssignBorders(ExtraTable2Range);
            ApplyBackgroundCellColours(ws,RowNo, ColumnNo, row);
        }
        public void AssignBorders(ExcelRange modelTable)
        {
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
        public void ApplyBackgroundCellColours(ExcelWorksheet ws, int RowNo, int ColumnNo, KeyValuePair<string, object> row)
        {
            if (row.Key == "SalesMarket" || row.Key == "BusinessMarket" || row.Key == "Counties" || row.Key == "SCC")
            {
                ws.Cells[RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(255, 0, 61, 161);//DarkBlue colour
                ws.Cells[RowNo, ColumnNo].Style.Font.Name = "Arial";
                ws.Cells[RowNo, ColumnNo].Style.Font.Size = 8;
                ws.Cells[RowNo, ColumnNo].Style.Font.Color.SetColor(Color.White);
                //ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            else if ((RowNo == 3 || RowNo == 4 || RowNo == 5 || RowNo == 6) && ColumnNo >= 5)
            {
                ws.Cells[RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(255, 0, 61, 161);//DarkBlue colour
                ws.Cells[RowNo, ColumnNo].Style.Font.Name = "Arial";
                ws.Cells[RowNo, ColumnNo].Style.Font.Size = 8;
            }
            else if ((RowNo == 7 || RowNo == 8) && ColumnNo >= 5)
            {
                ws.Cells[RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(255, 255, 192, 0);//Golden colour
                ws.Cells[RowNo, ColumnNo].Style.Font.Name = "Arial";
                ws.Cells[RowNo, ColumnNo].Style.Font.Size = 9;
                if (RowNo == 8)
                {
                    ExcelRange StartRange = ws.Cells[RowNo + 1, ColumnNo];
                    ExcelRange EndRange = ws.Cells[RowNo + 65536, ColumnNo];
                    ws.Cells[RowNo, ColumnNo].Formula = "=SUM(" + StartRange + ":" + EndRange + ")";
                }
            }
            else if(RowNo >= 9 && ColumnNo >= 5)
            {
                if(row.Value.ToString() != "NA") { 
                    ws.Cells[RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(255,192,233,255);//Light Blue
                }
            }
        }
    }
}
