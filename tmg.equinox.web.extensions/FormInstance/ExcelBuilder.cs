using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.PlanContactReport;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.MDBComparer;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.FormInstance
{
    public class ExcelBuilder
    {
        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="isGroupHeader">if set to <c>true</c> [is group header].</param>
        /// <param name="noOfColInGroup">The number of column in group.</param>
        /// <param name="isChildGrid">if set to <c>true</c> [is child grid].</param>
        /// <returns></returns>
        public MemoryStream ExportToExcel(string csv, bool isGroupHeader, int noOfColInGroup, bool isChildGrid, string sheetHeader)
        {
            DataTable dt = ConvertCsvData(csv, isChildGrid);

            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add("Sheet1");
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

                //worksheet.Cells.AutoFitColumns();

                worksheet.InsertRow(1, 1);

                int totalColumn = worksheet.Dimension.End.Column;
                string lastColAddress = worksheet.Cells[1, totalColumn].Address;
                worksheet.Cells[1, 1].Value = sheetHeader;
                worksheet.Cells["A1:" + lastColAddress].Merge = true;
                worksheet.Cells["A1:" + lastColAddress].Style.WrapText = true;

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.Size = 14;
                worksheet.Row(1).Height = 100;
                if (totalColumn == 1)
                {
                    worksheet.Column(1).Width = 70;
                    worksheet.Cells.Style.WrapText = true;
                }
                else if (totalColumn == 2)
                {
                    worksheet.Column(1).Width = 50;
                    worksheet.Column(2).Width = 50;
                    worksheet.Cells.Style.WrapText = true;
                }
                else
                {
                    try
                    {
                        worksheet.Cells.AutoFitColumns();
                    }
                    catch (Exception ex)
                    {
                        string customMsg = "Content lenght is greater than 32676, so the AutoFitColumn is not working";
                        Exception customException = new Exception(customMsg, ex);
                        ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                    }
                }

                var fileStream = new MemoryStream();
                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;

                return fileStream;
            }

        }

        public MemoryStream ExportToExcelVendorList(string[] csv, bool isGroupHeader, int noOfColInGroup, bool isChildGrid, string[] sheetHeader)
        {

            DataTable dt1 = ConvertCsvData(csv[0], isChildGrid);
            DataTable dt2 = ConvertCsvData(csv[1], isChildGrid);
            DataTable dt3 = ConvertCsvData(csv[2], isChildGrid);
            DataTable dt4 = ConvertCsvData(csv[3], isChildGrid);
            List<DataTable> dt = new List<DataTable>();
            dt.Add(dt1);
            dt.Add(dt2);
            dt.Add(dt3);
            dt.Add(dt4);

            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                for (int i = 0; i < dt.Count; i++)
                {
                    if (dt[i].Rows.Count != 0)
                    {
                        ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add(sheetHeader[i]);

                        worksheet.Cells.LoadFromDataTable(dt[i], false);

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
                        int totalColumn = worksheet.Dimension.End.Column;
                        string lastColAddress = worksheet.Cells[1, totalColumn].Address;
                        worksheet.Cells[1, 1].Value = "Vendor Feed Report";
                        worksheet.Cells["A1:" + lastColAddress].Merge = true;
                        worksheet.Cells["A1:" + lastColAddress].Style.WrapText = true;

                        worksheet.Row(1).Style.Font.Bold = true;
                        worksheet.Row(1).Style.Font.Size = 14;
                        worksheet.Row(1).Height = 100;

                        if (totalColumn == 1)
                        {
                            worksheet.Column(1).Width = 70;
                            worksheet.Cells.Style.WrapText = true;
                        }
                        else if (totalColumn == 2)
                        {
                            worksheet.Column(1).Width = 50;
                            worksheet.Column(2).Width = 50;
                            worksheet.Cells.Style.WrapText = true;
                        }
                        else
                        {
                            worksheet.Cells.AutoFitColumns();
                        }
                    }
                }

                var fileStream = new MemoryStream();
                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;

                return fileStream;
            }

        }

        public string ExportToExcelPlanContactReport(List<GroupContactReportViewModel> groupContactList, List<BroakerContactReportViewModel> broakerContactList, List<HSBContactReportViewModel> hSBContactList)
        {
            int columNumber = 1;
            var fileName = HttpContext.Current.Server.MapPath("~/App_Data/PlanContactReport" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
            var file = new FileInfo(fileName);
            var package = new ExcelPackage(file);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Group Contact");
            int Row = 2;
            WriteplanContactHeader(worksheet, Row, columNumber, "Group Contact");
            WritegroupContactList(worksheet, groupContactList, Row, columNumber);
            ExcelWorksheet broakerContactListWorksheet = package.Workbook.Worksheets.Add("Broker Contact");
            Row = 2; columNumber = 1;
            WriteplanContactHeader(broakerContactListWorksheet, Row, columNumber, "Broker Contact");
            WritebroakerContactList(broakerContactListWorksheet, broakerContactList, Row, columNumber);

            ExcelWorksheet hsbContactListWorksheet = package.Workbook.Worksheets.Add("HSB Contact");
            Row = 2; columNumber = 1;
            WriteHSBContactHeader(hsbContactListWorksheet, Row, columNumber);
            WriteHSBContactList(hsbContactListWorksheet, hSBContactList, Row, columNumber);

            package.Save();
            return fileName;
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

                string[] Lines = CSVdata.Split(new char[] { '\r' });

                if (Lines == null)
                    return dt;
                if (Lines.GetLength(0) == 0)
                    return dt;

                string[] HeaderText = Lines[0].Split('\t');

                int numOfColumns = HeaderText.Count();


                foreach (string header in HeaderText)
                    dt.Columns.Add(header, typeof(string));

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



        private DataTable GetDataTableFromCSV(string CSVdata, bool isChildGrid)
        {
            //  Convert a tab-separated set of data into a DataTable, ready for our C# CreateExcelFile libraries
            //  to turn into an Excel file.
            //
            DataTable dt = new DataTable();
            try
            {
                System.Diagnostics.Trace.WriteLine(CSVdata);

                string[] Lines = CSVdata.Split(new char[] { '\n' });

                if (Lines == null)
                    return dt;
                if (Lines.GetLength(0) == 0)
                    return dt;

                string[] HeaderText = Lines[0].Split('\t');

                int numOfColumns = HeaderText.Count();


                foreach (string header in HeaderText)
                    dt.Columns.Add(header, typeof(string));

                int colCount = dt.Columns.Count;
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
                        {
                            if (f < colCount)
                                Row[f] = Fields[f];
                        }
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

        private DataTable GetAuditChecklistDataTableFromCSV(string CSVdata, bool isChildGrid)
        {
            //  Convert a tab-separated set of data into a DataTable, ready for our C# CreateExcelFile libraries
            //  to turn into an Excel file.
            //
            DataTable dt = new DataTable();
            try
            {
                System.Diagnostics.Trace.WriteLine(CSVdata);

                string[] Lines = CSVdata.Split(new char[] { '\n' });

                if (Lines == null)
                    return dt;
                if (Lines.GetLength(0) == 0)
                    return dt;

                string[] HeaderText = Lines[6].Split('\t');

                int numOfColumns = HeaderText.Count();

                foreach (string header in HeaderText)
                    dt.Columns.Add(header, typeof(string));

                int colCount = dt.Columns.Count;
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
                        {
                            if (f < colCount)
                                Row[f] = Fields[f];
                        }
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

        public void WriteplanContactHeader(ExcelWorksheet worksheet, int Row, int columNumber, string Header)
        {
            string[] ColumHeader = new string[] { "CompanyName", "Name", "Title", "Email", "OfficePhone", "CellPhone", "AddressLine1", "AddressLine2", "City", "State", "Zip", "FaxNumber" };
            int totalColumn = ColumHeader.Count() + 1;
            string lastColAddress = worksheet.Cells[1, totalColumn].Address;
            worksheet.Cells[1, 1].Value = Header;
            worksheet.Cells["A1:" + lastColAddress].Merge = true;
            worksheet.Cells["A1:" + lastColAddress].Style.WrapText = true;
            if (Header.Equals("Broker Contact"))
            {
                ColumHeader[0] = "Name";
                ColumHeader[1] = "CompanyName";
            }
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.Font.Size = 14;
            worksheet.Row(1).Height = 100;
            if (totalColumn == 1)
            {
                worksheet.Column(1).Width = 70;
                worksheet.Cells.Style.WrapText = true;
            }
            else if (totalColumn == 2)
            {
                worksheet.Column(1).Width = 50;
                worksheet.Column(2).Width = 50;
                worksheet.Cells.Style.WrapText = true;
            }
            else
            {
                worksheet.Cells.AutoFitColumns();
            }
            for (int r = 0; r < ColumHeader.Count(); r++)
            {
                SetValueInCell(worksheet, Row, columNumber, ColumHeader[r]);//Write value in Cell
                SetValueInCellWithBold(worksheet, Row, columNumber, ColumHeader[r]);
                columNumber = columNumber + 1;
            }
        }

        public void SetValueInCell(ExcelWorksheet worksheet, int Row, int Colum, string Value)
        {
            if (Value != "")
            {
                worksheet.Cells[Row, Colum].Value = Value;
            }
            else
            {
                worksheet.Cells[Row, Colum].Value = 0;
            }
        }

        public void SetValueInCellWithBold(ExcelWorksheet worksheet, int Row, int Colum, string Value)
        {
            SetValueInCell(worksheet, Row, Colum, Value);
            worksheet.Cells[Row, Colum].Style.Font.Bold = true;
            worksheet.Column(Colum).Width = 35.50;
        }

        public void WritegroupContactList(ExcelWorksheet worksheet, List<GroupContactReportViewModel> groupContactList, int Row, int columNumber)
        {
            if (groupContactList.Count() > 0)
            {
                columNumber = 1; Row = 3;
                for (int r = 0; r < groupContactList.Count(); r++)
                {
                    SetValueInCell(worksheet, Row, columNumber, groupContactList[r].CompanyName);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].Name);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].Title);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].Email);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].OfficePhone);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].CellPhone);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].AddressLine1);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].AddressLine2);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].City);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].State);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].Zip);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, groupContactList[r].FaxNumber);//Write value in Cell
                    Row = Row + 1;
                    columNumber = 1;
                }
            }

        }

        public void WritebroakerContactList(ExcelWorksheet worksheet, List<BroakerContactReportViewModel> broakerContactList, int Row, int columNumber)
        {
            if (broakerContactList.Count() > 0)
            {
                columNumber = 1; Row = 3;
                for (int r = 0; r < broakerContactList.Count(); r++)
                {
                    SetValueInCell(worksheet, Row, columNumber, broakerContactList[r].Name);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].CompanyName);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].Title);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].Email);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].OfficePhone);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].CellPhone);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].AddressLine1);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].AddressLine2);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].City);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].State);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].Zip);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, broakerContactList[r].FaxNumber);//Write value in Cell
                    Row = Row + 1;
                    columNumber = 1;
                }
            }
        }

        public void WriteHSBContactList(ExcelWorksheet worksheet, List<HSBContactReportViewModel> hSBContactList, int Row, int columNumber)
        {
            if (hSBContactList.Count() > 0)
            {
                columNumber = 1; Row = 3;
                for (int r = 0; r < hSBContactList.Count(); r++)
                {
                    SetValueInCell(worksheet, Row, columNumber, hSBContactList[r].Name);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, hSBContactList[r].Title);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, hSBContactList[r].Department);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, hSBContactList[r].Email);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, hSBContactList[r].OfficePhone);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, hSBContactList[r].CellPhone);//Write value in Cell
                    SetValueInCell(worksheet, Row, columNumber = columNumber + 1, hSBContactList[r].Location);//Write value in Cell
                    Row = Row + 1;
                    columNumber = 1;
                }
            }
        }

        public void WriteHSBContactHeader(ExcelWorksheet worksheet, int Row, int columNumber)
        {
            string[] ColumHeader = new string[] { "Name", "Title", "Department", "Email", "OfficePhone", "CellPhone", "Location" };
            int totalColumn = ColumHeader.Count() + 1;
            string lastColAddress = worksheet.Cells[1, totalColumn].Address;
            worksheet.Cells[1, 1].Value = "HSB Contact";
            worksheet.Cells["A1:" + lastColAddress].Merge = true;
            worksheet.Cells["A1:" + lastColAddress].Style.WrapText = true;
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.Font.Size = 14;
            worksheet.Row(1).Height = 100;
            for (int r = 0; r < ColumHeader.Count(); r++)
            {
                SetValueInCell(worksheet, Row, columNumber, ColumHeader[r]);//Write value in Cell
                SetValueInCellWithBold(worksheet, Row, columNumber, ColumHeader[r]);
                columNumber = columNumber + 1;
            }
        }

        public MemoryStream DownLoadDataTableToExcel(DataTable dataTable, string sheetHeader, string sheetName)
        {
            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                var fileStream = new MemoryStream();
                try
                {
                    ExcelWorksheet worksheet1 = excelPkg.Workbook.Worksheets.Add(sheetName);
                    worksheet1.Cells.LoadFromDataTable(dataTable, true);
                    worksheet1.Row(1).Style.Font.Bold = true;
                    worksheet1.Cells.AutoFitColumns();
                    worksheet1.InsertRow(1, 1);
                    int totalColumn = dataTable.Rows.Count > 0 ? worksheet1.Dimension.End.Column : 1;
                    string lastColAddress = worksheet1.Cells[1, totalColumn].Address;
                    worksheet1.Cells[1, 1].Value = sheetHeader;
                    worksheet1.Cells["A1:" + lastColAddress].Merge = true;
                    worksheet1.Cells["A1:" + lastColAddress].Style.WrapText = true;
                    worksheet1.Row(1).Style.Font.Bold = true;
                    worksheet1.Row(1).Style.Font.Size = 18;
                    worksheet1.Row(1).Height = 30;
                    worksheet1.Cells.AutoFitColumns();

                    excelPkg.SaveAs(fileStream);
                    fileStream.Position = 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excelPkg.Dispose();
                }
                return fileStream;
            }
        }

        public MemoryStream DownLoadDataTablesToExcel(ExcelPackage excelPkg, DataTable dataTable, string sheetHeader, string sheetName)
        {
            var fileStream = new MemoryStream();
            try
            {
                ExcelWorksheet worksheet1 = excelPkg.Workbook.Worksheets.Add(sheetName);
                worksheet1.Cells.LoadFromDataTable(dataTable, true);
                worksheet1.Row(1).Style.Font.Bold = true;
                worksheet1.Cells.AutoFitColumns();
                worksheet1.InsertRow(1, 1);
                int totalColumn = dataTable.Rows.Count > 0 ? worksheet1.Dimension.End.Column : 1;
                string lastColAddress = worksheet1.Cells[1, totalColumn].Address;
                worksheet1.Cells[1, 1].Value = sheetHeader;
                worksheet1.Cells["A1:" + lastColAddress].Merge = true;
                worksheet1.Cells["A1:" + lastColAddress].Style.WrapText = true;
                worksheet1.Row(1).Style.Font.Bold = true;
                worksheet1.Row(1).Style.Font.Size = 18;
                worksheet1.Row(1).Height = 30;
                worksheet1.Cells.AutoFitColumns();

                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
               // excelPkg.Dispose();
            }
            return fileStream;

        }

        public MemoryStream DownloadPreviewGridExcel(DataTable dataTable, string sheetHeader, string sheetName)
        {
            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                var fileStream = new MemoryStream();
                try
                {
                    ExcelWorksheet worksheet1 = excelPkg.Workbook.Worksheets.Add(sheetName);
                    worksheet1.Cells.LoadFromDataTable(dataTable, true);
                    worksheet1.Row(1).Style.Font.Bold = true;
                    worksheet1.Cells.AutoFitColumns();
                    worksheet1.InsertRow(1, 1);
                    int totalColumn = worksheet1.Dimension.End.Column;
                    string lastColAddress = worksheet1.Cells[1, totalColumn].Address;
                    worksheet1.Cells[1, 1].Value = sheetHeader;
                    worksheet1.Cells["A1:" + lastColAddress].Merge = true;
                    worksheet1.Cells["A1:" + lastColAddress].Style.WrapText = true;
                    worksheet1.Row(1).Style.Font.Bold = true;
                    worksheet1.Row(1).Style.Font.Size = 18;
                    worksheet1.Row(1).Height = 30;
                    worksheet1.Cells.AutoFitColumns();

                    excelPkg.SaveAs(fileStream);
                    fileStream.Position = 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excelPkg.Dispose();
                }
                return fileStream;

            }

        }

        public MemoryStream DownloadPreviewGridExcel(DataTable dataTable, bool isGroupHeader, int noOfColInGroup, bool isChildGrid, string sheetHeader, string sheetName)
        {
            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                var fileStream = new MemoryStream();
                try
                {
                    ExcelWorksheet worksheet1 = excelPkg.Workbook.Worksheets.Add(sheetName);
                    worksheet1.Cells.LoadFromDataTable(dataTable, true);
                    if (isGroupHeader)
                    {
                        SetGroupHeaderStyle(noOfColInGroup, worksheet1);
                    }
                    else
                        worksheet1.Row(1).Style.Font.Bold = true;

                    
                    worksheet1.InsertRow(1, 1);
                    int totalColumn = worksheet1.Dimension.End.Column;
                    string lastColAddress = worksheet1.Cells[1, totalColumn].Address;
                    worksheet1.Cells[1, 1].Value = sheetHeader;
                    worksheet1.Cells["A1:" + lastColAddress].Merge = true;
                    worksheet1.Cells["A1:" + lastColAddress].Style.WrapText = true;
                    worksheet1.Row(1).Style.Font.Bold = true;
                    worksheet1.Row(1).Style.Font.Size = 18;
                    worksheet1.Row(1).Height = 30;

                    worksheet1.Row(1).Style.Font.Size = 14;
                    worksheet1.Row(1).Height = 100;
                    if (totalColumn == 1)
                    {
                        worksheet1.Column(1).Width = 70;
                        worksheet1.Cells.Style.WrapText = true;
                    }
                    else if (totalColumn == 2)
                    {
                        worksheet1.Column(1).Width = 50;
                        worksheet1.Column(2).Width = 50;
                        worksheet1.Cells.Style.WrapText = true;
                    }
                    else
                    {
                        try
                        {
                            worksheet1.Cells.AutoFitColumns();
                        }
                        catch (Exception ex)
                        {
                            string customMsg = "Content lenght is greater than 32676, so the AutoFitColumn is not working";
                            Exception customException = new Exception(customMsg, ex);
                            ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                        }
                    }


                    excelPkg.SaveAs(fileStream);
                    fileStream.Position = 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excelPkg.Dispose();
                }
                return fileStream;

            }

        }

        #region MDB Compare Report
        public MemoryStream DownLoadMDBCompareToExcel(ExcelWorksheet worksheet1, DataTable dataTable, string sheetHeader, List<MigrationFieldItem> migrationMapList, ref bool tableDifference, ref int diffCount,
            string importFileName, string importedFilePath, string exportFileName, string exportedFilePath, ref DataTable diffSummryOfTable, ref DataTable importExportMissingDT, List<DifferValueViewModel> ValuePair)
        {
            tableDifference = false;
            diffCount = 0;

            var fileStream = new MemoryStream();
            try
            {
                if (sheetHeader == "Summary")
                {
                    fileStream = SummaryReport(worksheet1, dataTable, sheetHeader, importFileName, importedFilePath, exportFileName, exportedFilePath, importExportMissingDT);
                }
                else
                {
                    worksheet1.Cells.LoadFromDataTable(dataTable, true);
                    worksheet1.Row(1).Style.Font.Bold = true;
                    worksheet1.Cells.AutoFitColumns();

                    //Comment is nothing but set full path of JSON element
                    SetCommentToCloumnHeader(worksheet1, dataTable, migrationMapList);

                    MergeQID(worksheet1);

                    SetColorToDifferentValues(worksheet1, ref tableDifference, dataTable.TableName, ref diffCount, ref diffSummryOfTable, ref importExportMissingDT, ValuePair);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return fileStream;

        }

        private MemoryStream SummaryReport(ExcelWorksheet worksheet1, DataTable dataTable, string sheetHeader, string importFileName, string importedFilePath, string exportFileName, string exportedFilePath, DataTable importExportMissingDT)
        {
            var fileStream = new MemoryStream();
            //worksheet1.Cells.LoadFromDataTable(dataTable, true);            
            //  worksheet1.Cells.AutoFitColumns();
            worksheet1.InsertRow(1, 1);
            worksheet1.InsertRow(1, 4);
            worksheet1.Cells[2, 1].Value = "Type";
            worksheet1.Cells[2, 2].Value = "File Name";
            worksheet1.Cells[2, 3].Value = "File Path";

            worksheet1.Cells[3, 1].Value = "Import";
            worksheet1.Cells[4, 1].Value = "Export";

            worksheet1.Cells[3, 2].Value = importFileName;
            worksheet1.Cells[4, 2].Value = exportFileName;

            worksheet1.Cells[3, 3].Value = importedFilePath;
            worksheet1.Cells[4, 3].Value = exportedFilePath;

            worksheet1.Cells[3, 3, 3, 4].Merge = true;
            worksheet1.Cells[4, 3, 4, 4].Merge = true;

            int totalColumn = dataTable.Rows.Count > 0 ? worksheet1.Dimension.End.Column : 1;
            string lastColAddress = worksheet1.Cells[1, totalColumn].Address;
            worksheet1.Cells[1, 1].Value = sheetHeader;
            worksheet1.Cells[1, 1, 1, 4].Merge = true;
            worksheet1.Cells[1, 1, 1, 4].Style.WrapText = true;
            worksheet1.Row(1).Style.Font.Bold = true;
            worksheet1.Row(2).Style.Font.Bold = true;
            worksheet1.Row(1).Style.Font.Size = 18;
            worksheet1.Row(1).Height = 30;

            int fromRow = 6;
            if (dataTable.Rows.Count > 0)
            {
                worksheet1.InsertRow(fromRow, importExportMissingDT.Rows.Count);
                worksheet1.Cells[fromRow, 1].Value = "Table Name";
                worksheet1.Cells[fromRow, 2].Value = "Column Name";
                worksheet1.Cells[fromRow, 3].Value = "Difference Count";
                worksheet1.Cells[fromRow, 4].Value = "Unique Count";
                worksheet1.Row(fromRow).Style.Font.Bold = true;
            }

            foreach (DataRow drRow in dataTable.Rows)
            {
                fromRow++;
                worksheet1.Cells[fromRow, 1].Value = drRow.ItemArray[0];
                worksheet1.Cells[fromRow, 2].Value = drRow.ItemArray[1];
                worksheet1.Cells[fromRow, 3].Value = drRow.ItemArray[2];
                worksheet1.Cells[fromRow, 4].Value = drRow.ItemArray[3];
                ExcelRange Rng = worksheet1.Cells[fromRow, 2];
                Rng.Hyperlink = new Uri("#'" + drRow.ItemArray[0] + "'!" + drRow.ItemArray[4] + "", UriKind.Relative);
            }



            fromRow = Convert.ToInt32(dataTable.Rows.Count) + 10;
            if (importExportMissingDT.Rows.Count > 0)
            {
                worksheet1.InsertRow(fromRow, importExportMissingDT.Rows.Count);
                worksheet1.Cells[fromRow, 1].Value = "Table Name";
                worksheet1.Cells[fromRow, 2].Value = "QID";
                worksheet1.Cells[fromRow, 3].Value = "Status";
                worksheet1.Cells[fromRow, 4].Value = "Difference Count";

                worksheet1.Row(fromRow).Style.Font.Bold = true;
            }

            foreach (DataRow drRow in importExportMissingDT.Rows)
            {
                fromRow++;
                worksheet1.Cells[fromRow, 1].Value = drRow.ItemArray[0];
                worksheet1.Cells[fromRow, 2].Value = drRow.ItemArray[1];
                worksheet1.Cells[fromRow, 3].Value = drRow.ItemArray[2];
                worksheet1.Cells[fromRow, 4].Value = drRow.ItemArray[3];
                ExcelRange Rng = worksheet1.Cells[fromRow, 2];
                Rng.Hyperlink = new Uri("#'" + drRow.ItemArray[0] + "'!" + drRow.ItemArray[4] + "", UriKind.Relative);
            }

            worksheet1.Cells.AutoFitColumns();
            worksheet1.Column(2).Width = 80;
            worksheet1.Column(2).Style.WrapText = true;


            fileStream.Position = 0;
            return fileStream;
        }

        private void SetCommentToCloumnHeader(ExcelWorksheet worksheet1, DataTable dataTable, List<MigrationFieldItem> migrationMapList)
        {
            for (int currentCell = worksheet1.Dimension.Start.Column; currentCell <= worksheet1.Dimension.End.Column; currentCell++)
            {
                string firstCellValue = worksheet1.Cells[1, currentCell].Value == null ? null : worksheet1.Cells[1, currentCell].Value.ToString();

                if (firstCellValue != "Type")
                {
                    MigrationFieldItem pbp = migrationMapList.Where(a => a.FieldTitle == firstCellValue).FirstOrDefault();
                    if (pbp != null)
                    {
                        worksheet1.Cells[1, currentCell].AddComment(pbp.DocumentPath + "|" + pbp.ColumnName, "SuperUser");
                    }
                    else
                    {
                        pbp = migrationMapList.Where(a => a.ColumnName.ToLower() == firstCellValue.ToLower()).FirstOrDefault();
                        if (pbp != null)
                        {
                            worksheet1.Cells[1, currentCell].AddComment(pbp.DocumentPath + "|" + pbp.ColumnName, "SuperUser");
                            worksheet1.Cells[1, currentCell].Value = pbp.FieldTitle;
                        }
                    }
                }
            }
        }

        private void MergeQID(ExcelWorksheet worksheet1)
        {
            for (int currentRow = worksheet1.Dimension.Start.Row + 1; currentRow < worksheet1.Dimension.End.Row; currentRow++)
            {
                int nextRow = 0;
                for (int i = currentRow + 1; i <= worksheet1.Dimension.End.Row; i++)
                {
                    string firstCellValue = worksheet1.Cells[currentRow, 1].Value == null ? null : worksheet1.Cells[currentRow, 1].Value.ToString();
                    string secondCellValue = worksheet1.Cells[i, 1].Value == null ? null : worksheet1.Cells[i, 1].Value.ToString();

                    if (firstCellValue == secondCellValue)
                    {
                        nextRow = i;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (nextRow != 0)
                {
                    worksheet1.Cells[currentRow, 1, nextRow, 1].Merge = true;
                    currentRow = nextRow;
                }
            }
        }

        private void SetColorToDifferentValues(ExcelWorksheet worksheet1, ref bool tableDifference, string tableName, ref int diffCount, ref DataTable diffSummryOfTable, ref DataTable importExportMissingDT, List<DifferValueViewModel> ValuePair)
        {
            int rowCount = diffSummryOfTable.Rows.Count;

            for (int currentCell = worksheet1.Dimension.Start.Column + 1; currentCell <= worksheet1.Dimension.End.Column; currentCell++)
            {
                List<string> cellValues = new List<string>();
                List<string> missingQID = new List<string>();
                for (int currentRow = worksheet1.Dimension.Start.Row + 1; currentRow < worksheet1.Dimension.End.Row; currentRow = currentRow + 2)
                {
                    string firstRowCellValue = worksheet1.Cells[currentRow, currentCell].Value == null ? null : worksheet1.Cells[currentRow, currentCell].Value.ToString();
                    string secondRowCellValue = worksheet1.Cells[currentRow + 1, currentCell].Value == null ? null : worksheet1.Cells[currentRow + 1, currentCell].Value.ToString();

                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                    if (currentCell == 2 && firstRowCellValue == "Import Missing")
                    {
                        worksheet1.Cells[currentRow, currentCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet1.Cells[currentRow, currentCell].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        missingQID.Add(worksheet1.Cells[currentRow, 1].Value + "+" + "Import Missing");
                        tableDifference = true;
                        diffCount++;
                        SetValueDifferComment(worksheet1, ValuePair, currentRow, currentCell);
                    }
                    else if (currentCell == 2 && secondRowCellValue == "Export Missing")
                    {
                        worksheet1.Cells[currentRow + 1, currentCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet1.Cells[currentRow + 1, currentCell].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        missingQID.Add(worksheet1.Cells[currentRow, 1].Value + "+" + "Export Missing");
                        tableDifference = true;
                        diffCount++;
                        SetValueDifferComment(worksheet1, ValuePair, currentRow, currentCell);
                    }
                    else if (currentCell != 2 && firstRowCellValue != secondRowCellValue)
                    {
                        worksheet1.Cells[currentRow, currentCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet1.Cells[currentRow, currentCell].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        worksheet1.Cells[currentRow + 1, currentCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet1.Cells[currentRow + 1, currentCell].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        cellValues.Add(firstRowCellValue + "+" + secondRowCellValue);

                        tableDifference = true;
                        diffCount++;
                        SetValueDifferComment(worksheet1, ValuePair, currentRow, currentCell);
                    }
                }

                if (cellValues.Count > 0)
                {
                    //set difference and unique count of column
                    SetDifferntAndUniqueCountOfColumn(ref diffSummryOfTable, currentCell, tableName, cellValues, (string)worksheet1.Cells[1, currentCell].Value, (string)worksheet1.Cells[1, currentCell].Address);
                }

                if (missingQID.Count > 0)
                {
                    //set missing count of import and export for QID
                    SetImportExportMissingCount(ref importExportMissingDT, currentCell, tableName, missingQID, (string)worksheet1.Cells[1, currentCell].Address);
                }
            }

        }

        private void SetDifferntAndUniqueCountOfColumn(ref DataTable diffSummryOfTable, int currentCell, string tableName, List<string> values, string columnName, string colAddress)
        {
            var query = values.GroupBy(s => s).Select(g => new { key = g.Key, Count = g.Count() });
            foreach (var item in query)
            {
                DataRow toInsert = diffSummryOfTable.NewRow();
                toInsert[0] = tableName;
                toInsert[1] = columnName;
                toInsert[2] = item.Count;
                toInsert[3] = 1;
                toInsert[4] = colAddress;
                diffSummryOfTable.Rows.InsertAt(toInsert, diffSummryOfTable.Rows.Count + 1);
            }
        }

        private void SetImportExportMissingCount(ref DataTable importExportMissingDT, int currentCell, string tableName, List<string> missingQID, string colAddress)
        {
            var query = missingQID.GroupBy(s => s).Select(g => new { key = g.Key, Count = g.Count() });
            foreach (var item in query)
            {
                DataRow toInsert = importExportMissingDT.NewRow();
                toInsert[0] = tableName;
                toInsert[1] = item.key.Split('+')[0];
                toInsert[2] = item.key.Split('+')[1];
                toInsert[3] = item.Count;
                toInsert[4] = colAddress;
                importExportMissingDT.Rows.InsertAt(toInsert, importExportMissingDT.Rows.Count + 1);
            }
        }

        private void SetValueDifferComment(ExcelWorksheet worksheet1, List<DifferValueViewModel> ValuePair, int row, int column)
        {
            string QID = string.Empty;
            if (worksheet1.Name != null)
            {
                string tab = worksheet1.Name.ToString();

                if (worksheet1.Cells[1, column].Comment != null)
                {
                    object cellValue = worksheet1.Cells[row, 1].Text;
                    QID = cellValue.ToString();

                    string columnComment = worksheet1.Cells[1, column].Comment.Text.ToString();
                    string[] JsonPath = columnComment.Split('|');

                    if (JsonPath.Count() > 1)
                    {
                        var Obj = ValuePair.ToList().Where(s => s.TableName.Equals(tab)
                                              && s.QID.Equals(QID)
                                              && s.DocumentPath.Equals(JsonPath[0])).FirstOrDefault();
                        if (Obj != null)
                        {
                            try
                            {
                                worksheet1.Cells[row, column].AddComment("Base Line value :- " + Obj.BaselineValue.Trim() + "\r\n" + "In-Progress value :- " + Obj.inPorgressValue.Trim(), "Super User");
                                if (worksheet1.Cells[row, 1].Comment == null)
                                {
                                    worksheet1.Cells[row, 1].AddComment("Base Line FromInstanceId : -" + Obj.BaseLineFormInstanceId + "\r\n" + "In-Progress FormInstanceId : - " + Obj.inProgressFormInstanceId, "Super User");
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
