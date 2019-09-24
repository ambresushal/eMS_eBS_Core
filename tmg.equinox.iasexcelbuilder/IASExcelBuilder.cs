using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;

namespace tmg.equinox.iasexcelbuilder
{
    public class IASExcelBuilder
    {
        #region Private Memebers

        string _filePath = string.Empty;

        #endregion Private Members

        #region Public Properties



        #endregion Public Properties

        #region Constructor



        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="isGroupHeader">if set to <c>true</c> [is group header].</param>
        /// <param name="noOfColInGroup">The number of column in group.</param>
        /// <param name="isChildGrid">if set to <c>true</c> [is child grid].</param>
        /// <returns></returns>
        public string ExportToExcel(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo, string folderPath, List<DocumentInstanceModel> DocumentInstance)
        {
            //_filePath = folderPath + GlobalUpdateConstants.IASFolderPath;
            _filePath = folderPath;
            bool isGroupHeader = false;
            int noOfColInGroup = 0;
            bool isChildGrid = true;

            string header = string.Empty;

            //header = header + "\r\n" + GlobalUpdateID;
            header = header + "\r\nName: " + GlobalUpdateName;
            header = header + "\r\nEffective Date From: " + GlobalUpdateEffectiveDateFrom.ToShortDateString() + " To: " + GlobalUpdateEffectiveDateTo.ToShortDateString();
            header = header + "\r\nGenerated: " + DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString();

            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                foreach (var Document in DocumentInstance)
                {
                    string csv = Document.csv;
                    DataTable dt = ConvertCsvData(csv, isChildGrid);

                    //ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add(GlobalUpdateConstants.IASReportSheetName);
                    string sheetNameInExcel = string.Empty;
                    if (Document.DocumentName.Length > 25)
                    {
                        sheetNameInExcel = Convert.ToString(Document.DocumentName.Substring(0, 25));
                    }
                    else
                    {
                        sheetNameInExcel = Convert.ToString(Document.DocumentName);
                    }
                    ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add(sheetNameInExcel);
                    worksheet.Cells.LoadFromDataTable(dt, false);

                    if (isGroupHeader)
                    {
                        SetGroupHeaderStyle(noOfColInGroup, worksheet);
                    }
                    else if (isChildGrid)
                    {
                        SetChildGridHeaderStyle(worksheet);
                    }
                    else
                    {
                        worksheet.Row(1).Style.Font.Bold = true;
                    }

                    worksheet.InsertRow(1, 1);

                    int totalRow = worksheet.Dimension.End.Row;
                    int totalColumn = worksheet.Dimension.End.Column;
                    string lastColAddress = worksheet.Cells[1, 10].Address;
                    worksheet.Cells[1, 1].Value = header;
                    worksheet.Cells["A1:" + lastColAddress].Merge = true;
                    worksheet.Cells["A1:" + lastColAddress].Style.WrapText = true;

                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Row(1).Style.Font.Size = 12;
                    worksheet.Row(1).Height = 50;

                    worksheet.View.FreezePanes(4, 11);

                    //Merge Header cells of Static Fields
                    worksheet.Row(2).Height = 80;
                    for (int i = 1; i <= 10; i++)
                    {
                        string initialColAddress = worksheet.Cells[2, i].Address;
                        string endColAddress = worksheet.Cells[3, i].Address;
                        worksheet.Cells[initialColAddress + ":" + endColAddress].Merge = true;
                        worksheet.Cells[initialColAddress + ":" + endColAddress].Style.WrapText = true;
                        worksheet.Cells[initialColAddress + ":" + endColAddress].Value = worksheet.Cells[initialColAddress].Text;
                        worksheet.Cells[2, i].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#000000"));
                    }
                    worksheet.Cells[2, 1, 2, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[2, 1, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 1, 2, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[2, 1, 2, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DAECA3"));
                    
                    for (int j = 11; j < totalColumn; j++)
                    {
                        worksheet.Cells[3, j].Style.WrapText = true;
                        worksheet.Cells[3, j].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#000000"));
                        worksheet.Cells[3, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[3, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[3, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#B4E1F0"));
                    }
                    //

                    //Add dropdown with Yes/No options for Accept Changes column in excel
                    int count = 0;
                    for (int r = 4; r <= totalRow; r++)
                    {
                        for (int c = 11; c < totalColumn; c++)
                        {
                            count++;
                            if (count % 6 == 0)
                            {
                                string addr = worksheet.Cells[r, c].Address;
                                var val = worksheet.DataValidations.AddListValidation(addr);
                                val.ShowErrorMessage = true;
                                val.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                val.ErrorTitle = GlobalUpdateConstants.ExcelErrorTitle;
                                val.Error = GlobalUpdateConstants.ExcelError;
                                val.Formula.Values.Add(GlobalUpdateConstants.YES);
                                val.Formula.Values.Add(GlobalUpdateConstants.NO);
                            }
                        }
                    }
                    //

                    //Add dropdown with options for Checkbox/Radio Button/Dropdown List/Dropdown Textbox  control's New Value column in excel
                    int checkCount = 0;
                    for (int r = 4; r <= totalRow; r++)
                    {
                        for (int c = 11; c < totalColumn; c++)
                        {
                            checkCount++;
                            if (checkCount % 6 == 0)
                            {
                                int optionLabelColumn = c - 4;
                                int optionLabelNoColumn = c - 3;
                                int itemDataColumn = c - 2;
                                int newValueColumn = c - 1;
                                if (worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Text == GlobalUpdateConstants.SELECTED || worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Text == GlobalUpdateConstants.NOTSELECTED || worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Text == GlobalUpdateConstants.NONE)
                                {
                                    string addr = worksheet.Cells[r, newValueColumn].Address;
                                    var val = worksheet.DataValidations.AddListValidation(addr);
                                    val.ShowErrorMessage = true;
                                    val.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                    val.ErrorTitle = GlobalUpdateConstants.ExcelErrorTitle;
                                    val.Error = GlobalUpdateConstants.ExcelError;
                                    val.Formula.Values.Add(GlobalUpdateConstants.NONE);
                                    val.Formula.Values.Add(GlobalUpdateConstants.SELECTED);
                                    val.Formula.Values.Add(GlobalUpdateConstants.NOTSELECTED);
                                }
                                else if (worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Text == worksheet.Cells[worksheet.Cells[r, optionLabelColumn].Address].Text || worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Text == worksheet.Cells[worksheet.Cells[r, optionLabelNoColumn].Address].Text || worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Text == GlobalUpdateConstants.CHOOSE)
                                {
                                    string addr = worksheet.Cells[r, newValueColumn].Address;
                                    var val = worksheet.DataValidations.AddListValidation(addr);
                                    val.ShowErrorMessage = true;
                                    val.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                    val.ErrorTitle = GlobalUpdateConstants.ExcelErrorTitle;
                                    val.Error = GlobalUpdateConstants.ExcelError;
                                    val.Formula.Values.Add(GlobalUpdateConstants.CHOOSE);
                                    val.Formula.Values.Add(worksheet.Cells[worksheet.Cells[r, optionLabelColumn].Address].Text);
                                    val.Formula.Values.Add(worksheet.Cells[worksheet.Cells[r, optionLabelNoColumn].Address].Text);
                                }
                                
                                /*
                            else if (worksheet.Cells[worksheet.Cells[r, itemDataColumn].Address].Text != GlobalUpdateConstants.NULL)
                            {
                                string addr = worksheet.Cells[r, newValueColumn].Address;
                                var val = worksheet.DataValidations.AddListValidation(addr);
                                val.ShowErrorMessage = true;
                                val.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                val.ErrorTitle = GlobalUpdateConstants.ExcelErrorTitle;
                                val.Error = GlobalUpdateConstants.ExcelError;
                                //Get & Add options of Dropdown List
                                string itemData = worksheet.Cells[worksheet.Cells[r, itemDataColumn].Address].Text;
                                List<DropDownItemModel> items = JsonConvert.DeserializeObject<List<DropDownItemModel>>(itemData);
                                foreach (var item in items)
                                {
                                    val.Formula.Values.Add(item.Value);
                                }
                            }
                                 */
                            }
                        }
                    }
                    //

                    //Merge Header cells of Changed Fields
                    int mergeCount = 0;
                    string startColAddress = null;
                    for (int c = 11; c < totalColumn; c++)
                    {
                        mergeCount++;
                        if (startColAddress == null)
                        {
                            startColAddress = worksheet.Cells[2, c].Address;
                        }
                        else if (mergeCount % 6 == 0)
                        {
                            int headerColumn = c - 2;
                            string headerColumnText = worksheet.Cells[worksheet.Cells[2, headerColumn].Address].Text;
                            int ruleStartColumn = c - 6;
                            int ruleEndColumn = c;
                            int itemDataHeaderColumn = c - 1;
                            string rulesText = worksheet.Cells[worksheet.Cells[2, itemDataHeaderColumn].Address].Text;
                            string endColAddress = worksheet.Cells[2, c].Address;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Merge = true;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Style.WrapText = true;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Value = headerColumnText;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#000000"));
                            worksheet.Cells[startColAddress + ":" + endColAddress].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[startColAddress + ":" + endColAddress].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#CAEAF4"));
                            if (rulesText != "")
                            {
                                AddRulesCustomObject(worksheet, 1, 3, ruleStartColumn, ruleEndColumn, eShapeStyle.Rect, headerColumnText, rulesText);
                            }
                            startColAddress = null;
                        }
                    }
                    //

                    //Disable required columns
                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowDeleteColumns = false;
                    worksheet.Protection.AllowDeleteRows = false;
                    worksheet.Protection.AllowInsertColumns = false;
                    worksheet.Protection.AllowInsertRows = false;
                    worksheet.Protection.AllowFormatColumns = false;
                    worksheet.Protection.AllowFormatRows = false;
                    worksheet.Protection.AllowFormatCells = false;
                    worksheet.Protection.AllowAutoFilter = true;
                    worksheet.Protection.AllowSort = true;                    
                    worksheet.Protection.AllowEditObject = false;
                    worksheet.Protection.SetPassword(GlobalUpdateConstants.PasswordforExcelSheet);
                    worksheet.Protection.AllowSelectLockedCells = true;
                    worksheet.Protection.AllowSelectUnlockedCells = true;
                    worksheet.Protection.AllowInsertHyperlinks = false;
                    worksheet.Protection.AllowPivotTables = false;
                    
                    int disableCount = 0;
                    for (int c = 11; c < totalColumn; c++)
                    {
                        disableCount++;
                        if (disableCount % 6 == 0)
                        {
                            worksheet.Column(c).Style.Locked = false;
                            int newValueColumn = c - 1;
                            worksheet.Column(newValueColumn).Style.Locked = false;
                            worksheet.Cells[worksheet.Cells[3, c].Address].Style.Locked = true;
                            worksheet.Cells[worksheet.Cells[3, newValueColumn].Address].Style.Locked = true;
                        }
                    }
                    //
                    
                    //Disable Accept Change/ New Value cell for selected row where Current Value cell has NA value in Excel as these elements are absent in FormInstances. 
                    int disableAcceptChangeCount = 0;
                    for (int r = 4; r <= totalRow; r++)
                    {
                        for (int c = 11; c < totalColumn; c++)
                        {
                            disableAcceptChangeCount++;
                            if (disableAcceptChangeCount % 6 == 0)
                            {
                                int newValueColumn = c - 1;
                                int oldValueColumn = c - 5;
                                string oldValueText = Convert.ToString(worksheet.Cells[r, oldValueColumn].Value);
                                if (oldValueText == GlobalUpdateConstants.NA)
                                {
                                    worksheet.Cells[worksheet.Cells[r, c].Address].Style.Locked = true;
                                    worksheet.Cells[worksheet.Cells[r, newValueColumn].Address].Style.Locked = true;
                                }
                            }
                        }
                    }
                    //

                    //Set Filters for required columns in excel
                    //worksheet.Cells["A2:J2"].AutoFilter = true;
                    string filterEndColAddress = "A3:" + worksheet.Cells[3, --totalColumn].Address;
                    worksheet.Cells[filterEndColAddress].AutoFilter = true;
                    //
                    
                    /*
                    //Wrap Text of all cells in excel
                    for (int r = 4; r <= totalRow; r++)
                    {
                        for (int c = 1; c < totalColumn; c++)
                        {
                            worksheet.Cells[r, c].Style.WrapText = true;
                        }
                    }
                    //
                    */

                    //worksheet.Cells.AutoFitColumns();
                    //Set Width for all columns in excel
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(4).Width = 25;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 12;
                    worksheet.Column(9).Width = 20;
                    worksheet.Column(10).Width = 10;
                    List<string> largeWidthColumns = new List<string>() { "Current Value", "New Value" };
                    for (int i = 11; i < totalColumn; i++)
                    {
                        if (largeWidthColumns.Contains(Convert.ToString(worksheet.Cells[3, i].Value)))
                        {
                            worksheet.Column(i).Width = 20;
                        }
                    }
                    List<string> smallwidthColumns = new List<string>() { "Accept Change" };
                    for (int i = 11; i < totalColumn; i++)
                    {
                        if (smallwidthColumns.Contains(Convert.ToString(worksheet.Cells[3, i].Value)))
                        {
                            worksheet.Column(i).Width = 11;
                        }
                    }
                    //

                    //Hide not required columns in excel
                    worksheet.Column(1).Hidden = true;
                    worksheet.Column(3).Hidden = true;
                    worksheet.Column(5).Hidden = true;
                    worksheet.Column(8).Hidden = true;
                    List<string> hideColumns = new List<string>() { "OptionLabel", "OptionLabelNo", "ItemData" };
                    for (int i = 11; i < totalColumn; i++)
                    {
                        if (hideColumns.Contains(Convert.ToString(worksheet.Cells[3, i].Value)))
                        {
                            worksheet.Column(i).Hidden = true;
                        }
                    }
                    //
                }
                excelPkg.Workbook.Protection.LockStructure = true;

                MemoryStream stream = new MemoryStream();
                excelPkg.SaveAs(stream);
                stream.Position = 0;

                //return stream;
                string fileName = GlobalUpdateConstants.IASReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
                string excelFileFullName = _filePath + fileName;

                if (System.IO.File.Exists(excelFileFullName))
                {
                    File.Delete(excelFileFullName);
                }

                FileStream file = new FileStream(excelFileFullName, FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();

                return excelFileFullName;
            }

        }

        public string ExportToErrorLogExcel(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo, string folderPath, string csv)
        {
            //_filePath = folderPath + GlobalUpdateConstants.IASErrorLogFolderPath;
            _filePath = folderPath;
            bool isGroupHeader = false;
            int noOfColInGroup = 0;
            bool isChildGrid = false;

            string header = string.Empty;

            //header = header + "\r\n" + GlobalUpdateID;
            header = header + "\r\nName: " + GlobalUpdateName;
            header = header + "\r\nEffective Date From: " + GlobalUpdateEffectiveDateFrom.ToShortDateString() + " To: " + GlobalUpdateEffectiveDateTo.ToShortDateString();
            header = header + "\r\nGenerated: " + DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString();

            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                DataTable dt = ConvertCsvData(csv, isChildGrid);

                ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add(GlobalUpdateConstants.ErrorLogReportSheetName);
                worksheet.Cells.LoadFromDataTable(dt, false);

                if (isGroupHeader)
                {
                    SetGroupHeaderStyle(noOfColInGroup, worksheet);
                }
                else if (isChildGrid)
                {
                    SetChildGridHeaderStyle(worksheet);
                }
                else
                {
                    worksheet.Row(1).Style.Font.Bold = true;
                }

                worksheet.InsertRow(1, 1);

                int totalRow = worksheet.Dimension.End.Row;
                int totalColumn = worksheet.Dimension.End.Column;
                string lastColAddress = worksheet.Cells[1, 10].Address;
                worksheet.Cells[1, 1].Value = header;
                worksheet.Cells["A1:" + lastColAddress].Merge = true;
                worksheet.Cells["A1:" + lastColAddress].Style.WrapText = true;

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.Size = 12;
                worksheet.Row(1).Height = 50;
                //

                //Set Colors for Header columns in excel
                worksheet.Cells["A2:S2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:S2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A2:S2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#CAEAF4"));
                //

                //Disable required columns
                worksheet.Protection.IsProtected = true;
                worksheet.Protection.AllowDeleteColumns = false;
                worksheet.Protection.AllowDeleteRows = false;
                worksheet.Protection.AllowInsertColumns = false;
                worksheet.Protection.AllowInsertRows = false;
                worksheet.Protection.AllowFormatColumns = false;
                worksheet.Protection.AllowFormatRows = false;
                worksheet.Protection.AllowFormatCells = false;
                worksheet.Protection.AllowAutoFilter = true;
                worksheet.Protection.AllowSort = true;
                worksheet.Protection.AllowEditObject = false;
                worksheet.Protection.SetPassword(GlobalUpdateConstants.PasswordforExcelSheet);
                worksheet.Protection.AllowSelectLockedCells = true;
                worksheet.Protection.AllowSelectUnlockedCells = true;
                worksheet.Protection.AllowInsertHyperlinks = false;
                worksheet.Protection.AllowPivotTables = false;

                for (int c = 1; c < totalColumn; c++)
                {
                    worksheet.Column(c).Style.Locked = true;
                }
                //

                //Set Filters for required columns in excel
                worksheet.Cells["A2:S2"].AutoFilter = true;
                //

                //Wrap Text of all cells in excel
                for (int r = 2; r <= totalRow; r++)
                {
                    for (int c = 1; c < totalColumn; c++)
                    {
                        worksheet.Cells[r, c].Style.WrapText = true;
                    }
                }
                //
                
                //worksheet.Cells.AutoFitColumns();
                //Set Width for all columns in excel
                worksheet.Column(4).Width = 25;
                worksheet.Column(6).Width = 25;
                worksheet.Column(8).Width = 11;
                worksheet.Column(9).Width = 12;
                worksheet.Column(11).Width = 20;
                worksheet.Column(12).Width = 10;
                worksheet.Column(13).Width = 20;
                worksheet.Column(14).Width = 30;
                worksheet.Column(15).Width = 40;
                worksheet.Column(16).Width = 40;
                worksheet.Column(17).Width = 40;
                worksheet.Column(18).Width = 12;
                worksheet.Column(19).Width = 14;

                //Hide not required columns in excel
                worksheet.Column(1).Hidden = true;
                worksheet.Column(2).Hidden = true;
                worksheet.Column(3).Hidden = true;
                worksheet.Column(5).Hidden = true;
                worksheet.Column(7).Hidden = true;
                worksheet.Column(10).Hidden = true;
                //

                excelPkg.Workbook.Protection.LockStructure = true;
                
                MemoryStream stream = new MemoryStream();
                excelPkg.SaveAs(stream);
                stream.Position = 0;

                //return stream;
                string fileName = GlobalUpdateConstants.ErrorLogReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
                string excelFileFullName = _filePath + fileName;

                if (System.IO.File.Exists(excelFileFullName))
                {
                    File.Delete(excelFileFullName);
                }

                FileStream file = new FileStream(excelFileFullName, FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();
                
                return excelFileFullName;
            }
        }

        public DataTable getDataTableFromExcel(string path, string FormDesignName)
        {
            string FormDesignNameAsPerSheetInExcel = string.Empty;
            if (FormDesignName.Length > 25)
            {
                FormDesignNameAsPerSheetInExcel = Convert.ToString(FormDesignName.Substring(0, 25));
            }
            else
            {
                FormDesignNameAsPerSheetInExcel = FormDesignName;
            }
            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }            
				var ws = pck.Workbook.Worksheets[FormDesignNameAsPerSheetInExcel];
                DataTable tbl = new DataTable();
                bool hasHeader = true; // adjust it accordingly
                int hc = 0;
                foreach (var firstRowCell in ws.Cells[3, 1, 3, ws.Dimension.End.Column])//ws.Cells[2, 1, 2, ws.Dimension.End.Column]
                {
                    if (firstRowCell.Text == "OptionLabel" || firstRowCell.Text == "OptionLabelNo" || firstRowCell.Text == "ItemData" || firstRowCell.Text == "Current Value" || firstRowCell.Text == "New Value" || firstRowCell.Text == "Accept Change")
                    {
                        hc++;
                        string subHeader = firstRowCell.Text + hc;
                        tbl.Columns.Add(hasHeader ? subHeader : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    else
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                }
                var startRow = hasHeader ? 4 : 2;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    //var wsRow = ws.Cells[rowNum, 1, rowNum, 10];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        //row[cell.Start.Column - 1] = cell.Text;
                        if (cell.Start.Column >= 11)
                        {
                            int nextColumn = cell.Start.Column + 1;
                            int firstHeaderColumn = cell.Start.Column - 5;
                            int count = cell.Start.Column - 10;
                            if (count % 6 == 0)
                            {
                                row[cell.Start.Column - 1] = ws.Cells[ws.Cells[2, firstHeaderColumn].Address].Text;
                            }
                            else
                            {
                                row[cell.Start.Column - 1] = ws.Cells[ws.Cells[rowNum, nextColumn].Address].Text;
                            }
                        }
                        else
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private class ExcelFormulaData
        {
            public static string blankValChk = "=ISERR(COUNTA({0})=ROW()-2";
        }

        /// <summary>
        /// Sets the child grid header style.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        private void SetChildGridHeaderStyle(ExcelWorksheet worksheet)
        {
            worksheet.Row(1).Style.Font.Bold = true;

            var cells = worksheet.Cells.Where(c => c.Text == "dummyColumn");
            foreach (var cell in cells)
            {
                cell.Value = "";
                worksheet.Row(cell.Start.Row).Style.Font.Bold = true;
            }
        }

        /// <summary>
        /// Sets the group header style.
        /// </summary>
        /// <param name="noOfColInGroup">The no of col in group.</param>
        /// <param name="worksheet">The worksheet.</param>
        private void SetGroupHeaderStyle(int noOfColInGroup, ExcelWorksheet worksheet)
        {
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Bold = true;

            int totalColumn = worksheet.Dimension.End.Column;

            for (int i = 1; i < totalColumn; i++)
            {

                if (!string.IsNullOrEmpty(worksheet.Cells[1, i].Text))
                {
                    string firstCell = worksheet.Cells[1, i].Address;
                    string lastCell = worksheet.Cells[1, i + noOfColInGroup - 1].Address;
                    worksheet.Cells[firstCell + ":" + lastCell].Merge = true;
                    i = i + noOfColInGroup - 1;
                }
            }
        }

        /// <summary>
        /// Converts the CSV data to Datatable.
        /// </summary>
        /// <param name="CSVdata">The cs vdata.</param>
        /// <param name="isChildGrid">if set to <c>true</c> [is child grid].</param>
        /// <returns></returns>
        private DataTable ConvertCsvData(string CSVdata, bool isChildGrid)
        {
            //  Convert a tab-separated set of data into a DataTable, ready for our C# CreateExcelFile libraries
            //  to turn into an Excel file.
            //
            DataTable dt = new DataTable();
            try
            {
                System.Diagnostics.Trace.WriteLine(CSVdata);

                string[] Lines = CSVdata.Split(new char[] { '\r', '\n' });
                if (Lines == null)
                    return dt;
                if (Lines.GetLength(0) == 0)
                    return dt;

                string[] HeaderText = Lines[0].Split('\t');

                int numOfColumns = HeaderText.Count();
                int dupCount = 1;
                foreach (string header in HeaderText)
                {
                    string newHeaderText = string.Empty;
                    if (dt.Columns.Contains(header))
                    {
                        string space = " ";
                        for (int i = 0; i < dupCount; i++)
                        {
                            space += space;
                        }
                        newHeaderText = header + space;
                        dupCount++;
                    }
                    else
                    {
                        newHeaderText = header;
                    }
                    dt.Columns.Add(newHeaderText, typeof(string));
                }
                DataRow Row;
                for (int i = 0; i < Lines.GetLength(0); i++)
                {
                    string[] Fields = Lines[i].Split('\t');

                    if (isChildGrid)
                    {
                        if (Fields.GetLength(0) > dt.Columns.Count)
                        {
                            while (Fields.GetLength(0) > dt.Columns.Count)
                                dt.Columns.Add("", typeof(string));
                        }
                    }
                    if (Fields.GetLength(0) > 1 || (Fields.GetLength(0) == 1 && !string.IsNullOrEmpty(Fields[0])))
                    {
                        Row = dt.NewRow();
                        for (int f = 0; f < Fields.GetLength(0); f++)
                            Row[f] = Fields[f];
                        dt.Rows.Add(Row);
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("An exception occurred: " + ex.Message);
                return null;
            }
        }

        ///<summary>
        /// Adding custom shape or object in specifed cell of specified excel sheet
        /// <param name="oSheet" />The ExcelWorksheet object
        /// <param name="rowIndex" />The row number of the cell where the object will put
        /// <param name="colIndex" />The column number of the cell where the object will put
        /// <param name="shapeStyle" />The style of the shape of the object
        /// <param name="text" />Text inside the object
        /// </summary>
        private void AddRulesCustomObject(ExcelWorksheet oSheet, int rowIndex, int toRowIndex, int colIndex, int toColIndex, eShapeStyle shapeStyle, string CustomObjectName, string text)
        {
            ExcelShape excelShape = oSheet.Drawings.AddShape(CustomObjectName, shapeStyle);
            excelShape.Font.Bold = true;
            excelShape.Font.Italic = true;
            excelShape.From.Column = colIndex;
            excelShape.To.Column = toColIndex;
            excelShape.From.Row = rowIndex;
            excelShape.To.Row = toRowIndex;
            excelShape.SetSize(380, 100);
            // 5x5 px space for better alignment
            excelShape.From.RowOff = Pixel2MTU(30);
            excelShape.From.ColumnOff = Pixel2MTU(1);
            // Adding text into the shape
            excelShape.RichText.Add(text);
        }

        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }

        #endregion Private Methods

    }
}
