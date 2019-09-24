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
    public class LISGridsForWebsiteFormulasExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {

        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 4;
        }

        public int GetRowsCount(ICollection<IDictionary<string, dynamic>> collection, string StateValue)
        {
            ICollection<IDictionary<string, dynamic>> Collection = new Collection<IDictionary<string, dynamic>>();
            foreach (var item in collection)
            {
                if (item["State"] == StateValue)
                {
                    Collection.Add(item);
                }
            }
            int rowCount = Collection.Count;
            return rowCount;
        }

        public ICollection<IDictionary<string, dynamic>> GetStateRows(ICollection<IDictionary<string, dynamic>> collection, string StateValue)
        {
            ICollection<IDictionary<string, dynamic>> Collection = new Collection<IDictionary<string, dynamic>>();
            foreach (var item in collection)
            {
                if (item["State"] == StateValue)
                {
                    Collection.Add(item);
                }
            }
            return Collection;
        }


        public void AssignBorders(ExcelRange modelTable)
        {
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ExcelWorksheet Sheet1 = excelPackage.Workbook.Worksheets[1];
            ExcelRange PlanRange = Sheet1.Cells[modelTable.Start.Row, modelTable.Start.Column + 2, modelTable.End.Row - 1, modelTable.End.Column];
            PlanRange.Style.Border.BorderAround(ExcelBorderStyle.Medium);
        }

        public ExcelRange GetStateTemplate()
        {
            ExcelRange StateTemplate = excelPackage.Workbook.Worksheets[1].Cells[1, 1, 3, 8];
            return StateTemplate;
        }

        public void PrepareSheet1(ICollection<DataHolder> dt)
        {
            ExcelWorksheet Sheet1 = excelPackage.Workbook.Worksheets[1];
            List<string> States = new List<string>();
            ICollection<IDictionary<string, dynamic>> CCPCollection = dt.Where(r => r.index == 0).FirstOrDefault().Data;
            var distinctCCPCollection = CCPCollection.GroupBy(r => r["State"]).ToList();
            //Get Unique States from collection
            foreach (var item in distinctCCPCollection.Select(s => s.Key))
            {
                States.Add(Convert.ToString(item));
            }

            int StateCounter = 0;
            int StateStartPosition = 4;
            int rowNo = 0;
            int colNo = 0;
            int tableNo = 0;
            bool isNewRow = false;
            foreach (string state in States)
            {
                Sheet1.Cells[StateStartPosition - 3, 3].Value = "State: " + state;
                int StateRecordsCount = GetRowsCount(CCPCollection, state);
                Sheet1.InsertRow(StateStartPosition, StateRecordsCount + 1);
                var StateRange = Sheet1.Cells[StateStartPosition, 1, StateStartPosition + StateRecordsCount, 8];
                Sheet1.Cells[StateStartPosition, 1, StateStartPosition + StateRecordsCount, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                Sheet1.Cells[StateStartPosition, 1, StateStartPosition + StateRecordsCount, 8].Style.WrapText = true;
                Sheet1.Cells[StateStartPosition, 1, StateStartPosition + StateRecordsCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                Sheet1.Cells[StateStartPosition, 5, StateStartPosition + StateRecordsCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                AssignBorders(StateRange);

                ICollection<IDictionary<string, dynamic>> Collection = GetStateRows(CCPCollection, state);
                rowNo = StateStartPosition;
                foreach (var row in Collection)
                {
                    foreach (var col in row)
                    {
                        colNo++;

                        WriteInContainer(col, null, isNewRow, rowNo, colNo, tableNo);

                        isNewRow = false;
                    }
                    colNo = 0;
                    isNewRow = true;
                    rowNo++;
                }

                StateCounter++;
                if (StateCounter > 0 && StateCounter < States.Count)
                {
                    //Copy State Template
                    Sheet1.Cells[1, 1, 3, 8].Copy(Sheet1.Cells[StateStartPosition + StateRecordsCount + 1, 1, StateStartPosition + StateRecordsCount + 3, 8]);
                    StateStartPosition = StateStartPosition + StateRecordsCount + 3 + 1;
                }

            }

        }

        public void PrepareSheet2(ICollection<DataHolder> dt)
        {
            ICollection<DataHolder> PDPCollection = dt.Where(r => r.index > 0).ToList();
            ExcelWorksheet Sheet2 = excelPackage.Workbook.Worksheets[2];
            List<int> Indexes = new List<int>();

            foreach (var item in PDPCollection)
            {
                Indexes.Add(item.index);
            }

            int Counter = 0;
            int StartPosition = 3;
            int rowNo = 0;
            int colNo = 2;
            int tableNo = 1;
            bool isNewRow = false;
            string IndexValue = "";
            bool Key = false;
            foreach (int index in Indexes)
            {
                if (index < 10)
                    IndexValue = "0" + index.ToString();
                else
                    IndexValue = index.ToString();

                Sheet2.Cells[StartPosition, 1].Value = IndexValue;

                ICollection<IDictionary<string, dynamic>> Collection = PDPCollection.Where(r => r.index == index).FirstOrDefault().Data;
                rowNo = StartPosition;
                foreach (var row in Collection)
                {
                    if (rowNo == StartPosition)//key row //Print Header row
                        Key = true;

                    foreach (var col in row)
                    {
                        WriteInExcel(col, Key, isNewRow, rowNo, colNo, tableNo);
                        colNo++;
                        isNewRow = false;
                    }

                    //if (Key) { rowNo++; }

                    colNo = 2;
                    isNewRow = true;
                    rowNo++;
                    Key = false;
                }

                Counter++;
                if (Counter > 0 && Counter < Indexes.Count)
                {
                    //Copy  Template
                    Sheet2.Cells[2, 1, 7, 5].Copy(Sheet2.Cells[StartPosition + 6, 1, StartPosition + 7, 5]);
                    //Sheet2.Cells[StartPosition + 6, 1, StartPosition + 7, 5].Clear();
                    if (Counter != 1)
                    {
                        Sheet2.DeleteRow(StartPosition - 1, 1, true);
                        //Sheet2.Cells[StartPosition - 1, 1, StartPosition - 1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Sheet2.Cells[StartPosition - 1, 1, StartPosition - 1, 5].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
                    }
                    if (Counter == 1)
                    {
                        StartPosition = StartPosition + 4 + 2 + 1;
                    }
                    else
                    {
                        StartPosition = StartPosition + 4 + 2 + 1 - 1;
                    }
                }
                if(Counter == Indexes.Count)
                {
                    //Sheet2.Cells[StartPosition - 1, 1, StartPosition - 1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //Sheet2.Cells[StartPosition - 1, 1, StartPosition - 1, 5].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
                    Sheet2.DeleteRow(StartPosition - 1, 1, true);
                }
                
            }
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

            //CCP Sheet
            PrepareSheet1(dt);

            //PDP Sheet   
            PrepareSheet2(dt);

            return result;
        }

        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            //if (tableIndex > 0 && excelPackage.Workbook.Worksheets.Count == tableIndex)  //check if sheets exits = datatables.
            //{
            //    string sheetName = String.Format("Sheet{0}", tableIndex + 1);
            //    excelPackage.Workbook.Worksheets.Add(sheetName);
            //}
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            ReportHelper reportHelper = new ReportHelper();
            if (colNo > 3 && colNo < 9)
            {
                ws.Cells[rowNo, colNo].Value = reportHelper.FormatNumberWithDecimals(row.Value.ToString());
            }
            else
            {
                ws.Cells[rowNo, colNo].Value = row.Value.ToString();
            }


        }

        //Default Excel Sheets data writing activity
        public void WriteInExcel(KeyValuePair<string, object> row, bool key, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            //if (tableIndex > 0 && excelPackage.Workbook.Worksheets.Count == tableIndex)  //check if sheets exits = datatables.
            //{
            //    string sheetName = String.Format("Sheet{0}", tableIndex + 1);
            //    excelPackage.Workbook.Worksheets.Add(sheetName);
            //}

            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            char[] array2 = new char[] { 'A', 'B', 'C', 'D', 'E', '_' };
            string HeaderPart1 = "";
            string HeaderPart2 = "";
            ReportHelper reportHelper = new ReportHelper();
            string cellValue = row.Value.ToString();
            if (colNo == 2)
                cellValue = cellValue.TrimStart(array2).TrimStart(array2);

            if (key)
            {
                //Header Formatting
                using (ExcelRange cellRange = ws.Cells[rowNo, colNo])
                {
                    string heading = cellValue;
                    string[] headingArr = heading.Split(new string[] { "Region" }, StringSplitOptions.None);
                    if (headingArr.Length == 2)
                    {
                        HeaderPart1 = headingArr[0];
                        HeaderPart2 = "Region " + headingArr[1];
                    }

                    if (colNo == 3 || colNo == 4 || colNo == 5)
                    {
                        if (cellRange.RichText.Count == 2)
                        {
                            cellRange.RichText.RemoveAt(0);
                            cellRange.RichText.RemoveAt(0);
                        }
                        cellRange.RichText.Add(HeaderPart1);
                        cellRange.RichText.Add(HeaderPart2, bold: true);

                    }                    
                    else
                    {
                        ws.Cells[rowNo, colNo].Value = row.Key.ToString();
                    }

                }
                ws.Cells[rowNo + 1, colNo].Value = reportHelper.FormatNumberWithDecimals(cellValue); 
            }
            else
            {
                if (colNo == 3 || colNo == 4 || colNo == 5)
                {
                    ws.Cells[rowNo, colNo].Value = reportHelper.FormatNumberWithDecimals(cellValue);
                }
                else
                {
                    ws.Cells[rowNo, colNo].Value = cellValue;
                }
            }
        }

    }
}
