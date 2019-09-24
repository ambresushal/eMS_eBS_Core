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
using System.Drawing;
using OfficeOpenXml.Style;

namespace tmg.equinox.reporting.Reports
{
    public class BenefitsMemberFacingPostBenchmarkExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 2;
        }

        //Set data starting col position in excel here.
        public override int SetInitialColPostion()
        {
            return 10;
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

            //Color Formatting starts here
            //Year Formatting
            string CurrentYear = DateTime.Now.Year.ToString();
            string NextYear = ((DateTime.Now.Year) + 1).ToString();
            if (RowNo == 4)
            {
                if (row.Value.ToString().Contains("NEW"))
                {
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.Green);
                }
                if (row.Value.ToString() == NextYear)
                {
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                }
                if (row.Value.ToString() == CurrentYear)
                {
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.MediumPurple);
                }
                if (row.Value.ToString() == "TERM")
                {
                    ws.Cells[RowNo + 1, ColumnNo, 457, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo + 1, ColumnNo, 457, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    //ws.Cells[RowNo + 1, ColumnNo, 457, ColumnNo].Clear(); //Clear Range columns                   
                }
            }
            //IN/OON/Tier Formatting
            if (RowNo == 7)
            {
                if (row.Value.ToString() == "IN")
                {
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
                }
                if (row.Value.ToString() == "OON")
                {
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                }
            }


            //Section A values
            ReportHelper reportHelper = new ReportHelper();
            if (RowNo > 13 && RowNo < 20)
            {
                ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatNumberWithDecimals(row.Value.ToString());
            }
            //else if (RowNo != 47)
            //{
            //    ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatNumberWithoutDecimals(row.Value.ToString().Replace(".00", ""));
            //}
            else
            {
                if (RowNo == 36 || RowNo == 49 || RowNo == 63) //AE, AR, BD
                {
                    string Val = row.Value.ToString();
                    Val = reportHelper.FormatStringWithDecimalsSkipDays(Val);
                    Val = Val.Replace("<cr> ", "<cr>");
                    Val = Val.Replace(".00", "");
                    if (RowNo == 63)
                    {
                        if (ws.Cells[59, ColumnNo].Value.ToString().Contains(".00"))
                        {
                            string row59Val = ws.Cells[59, ColumnNo].Value.ToString().Split('.')[0];
                            Val = Val.Replace(row59Val, row59Val + ".00");
                        }
                    }

                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
                else if (RowNo == 59)
                {
                    ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
                }
                else 
                {
                    ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatNumberWithoutDecimals(row.Value.ToString().Replace(".00", ""));
                }
            }
            ws.Cells[RowNo, ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[RowNo, ColumnNo].Style.WrapText = true;


        }

        private int ConvertToNumber(string input)
        {
            if (string.IsNullOrEmpty(input) == true) { return 0; }
            decimal n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    return Convert.ToInt32(input);
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        int GetLastUsedRow(ExcelWorksheet sheet)
        {
            var row = sheet.Dimension.End.Row;
            while (row >= 1)
            {
                var range = sheet.Cells[row, 1, row, sheet.Dimension.End.Column];
                if (range.Any(c => !string.IsNullOrEmpty(c.Text)))
                {
                    break;
                }
                row--;
            }
            return row;
        }

        int GetLastUsedColumn(ExcelWorksheet sheet)
        {
            var column = sheet.Dimension.End.Column;
            while (column >= 1)
            {
                if (sheet.Cells[2, column].Text != "")
                {
                    break;
                }
                column--;
            }
            return column;
        }

        private void ApplyColors()
        {
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[1];
            Color colFromHexA = System.Drawing.ColorTranslator.FromHtml("#ADE0E6"); //light blue
            Color colFromHexB = System.Drawing.ColorTranslator.FromHtml("#FFC0CB"); //Pink
            int lastColumn = GetLastUsedColumn(ws);
            int lastRow = GetLastUsedRow(ws);

            int start2018col = 0;
            int start2019col = 0;
            for (int i = 10; i <= lastColumn; i++)
            {
                if (ws.Cells[4, i].Text != "2019")
                {
                    continue;
                }
                if (ws.Cells[4, i + 1].Text == "2020")
                {
                    int length2018 = i - start2018col;
                    for (int j = i + 1; j <= lastColumn; j++)
                    {
                        if (ws.Cells[4, j].Text != "NEW 2020")
                        {
                            i = j;
                        }

                        if (ws.Cells[4, j].Text != "2020")
                        {
                            i = j - 1;
                            break;
                        }
                        for (int k = 7; k <= lastRow; k++)
                        {
                            if (k == 36 || k == 49 || k == 63) { continue; }
                            if (ws.Cells[k, length2018].Text.ToUpper().Replace(" ", "") != ws.Cells[k, j].Text.ToUpper().Replace(" ", ""))
                            {
                                if (string.IsNullOrEmpty(ws.Cells[k, j].Text) == false)
                                {
                                    ws.Cells[k, j, k, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[k, j, k, j].Style.Fill.BackgroundColor.SetColor(colFromHexB);
                                }
                            }
                        }
                        length2018++;
                    }
                    start2018col = 0;
                }
                else
                {
                    if (ws.Cells[4, i].Text == "2019")
                    {
                        start2018col++;
                    }
                    continue;
                }
            }
        }


        //Save excel once data wrirting is finished & Update Queue .
        public override void UpdateResultInQueue(IResult result, QueueItem item)
        {
            excelPackage.Save();
            ApplyColors();
            //update the path and result
            MergeCells();
            excelPackage.Save();
            Base.Model.DownloadFileInfo fileInfo = (Base.Model.DownloadFileInfo)result;
            var customQueue = new CustomQueueMangement();
            var baseInfo = SetJobInfo(fileInfo, item);
            //baseInfo.Status = "Succeeded";
            var instanceCustomQueue = customQueue.CreateInstanceCustomQueue(baseInfo);
            customQueue.ExecuteCustomQueueMethod(instanceCustomQueue, baseInfo, "UpdateQueue");
        }

        private void MergeCells()
        {
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[1];
            int lastColumn = ws.Dimension.End.Column;
            string[] cellValues = new string[10];
            int counter = 0;
            bool areEqual = false;
            int startPosition = 430;
            int endPosition = 439;

            for (int i = 10; i <= lastColumn; i++)
            {
                if (ws.Cells[9, i].Text.Trim() == "DSNP")
                {
                    ExcelRange cells = ws.Cells[startPosition, i, endPosition, i];//get 10 rows in column
                    counter = 0;
                    for (int j = startPosition; j <= endPosition; j++)
                    {
                        cellValues[counter] = ws.Cells[j, i].Text;
                        counter++;

                    }
                    //areEqual = cellValues.Select(s => s.ToLower()).Distinct().Count() == 1;
                    //if (areEqual)
                    //{
                    cells.Merge = true;
                    //}
                    //else
                    //{
                    //    cells.Merge = false;
                    //}
                    Array.Clear(cellValues, 0, cellValues.Length);
                    areEqual = false;
                }
            }
        }

    }
}
