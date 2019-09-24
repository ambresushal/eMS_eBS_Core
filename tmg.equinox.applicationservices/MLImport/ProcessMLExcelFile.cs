using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Configuration;
using System.IO;
//using OfficeOpenXml;

namespace tmg.equinox.applicationservices.MLImport
{
    public class ProcessMLExcelFile
    {
        public DataTable ConvertExceltoDatatable(string filePath, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string conString = string.Empty;
            DataTable dt = new DataTable();
            bool hasHeader = true;
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    dt.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                return dt;
            }
        }

        public DataTable MSConvertExceltoDatatable(string filePath, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string conString = string.Empty;
            DataTable dt = new DataTable();

            switch (extension)
            {
                case ".xls": //Excel 97-03.
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx": //Excel 07 and above.
                    conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;
            }

            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        int SheetsCount = dtExcelSchema.Rows.Count;
                        string sheetName = dtExcelSchema.Rows[SheetsCount - 1]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }
            return dt;
        }


        public DataTable ConvertExceltoDatatableUsingEPPlus(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                        if (cell.Value != null && cell.Text != null && cell.Value.ToString() != cell.Text.ToString() && !cell.Text.ToString().Contains('%') && !cell.Text.ToString().Contains('$'))
                        {
                            DateTime d = DateTime.Now;
                            if (DateTime.TryParse(cell.Value.ToString(), out d))
                                row[cell.Start.Column - 1] = d.ToShortDateString().ToString();
                        }
                    }
                }
                return tbl;
            }
        }
    }
}
