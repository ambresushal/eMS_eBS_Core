using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace tmg.equinox.iasbuilder
{
    public class AuditReportBuilder
    {
        public void GenerateExcel(string filePath, DataSet dataSetToExport)
        {
            using (ExcelPackage objExcelPackage = new ExcelPackage())
            {
                foreach (DataTable dtSrc in dataSetToExport.Tables)
                {
                    //Create the worksheet    
                    ExcelWorksheet objWorksheet = objExcelPackage.Workbook.Worksheets.Add(dtSrc.TableName);
                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1   
                    int columnCount = dtSrc.Columns.Count;
                    Color colour = ColorTranslator.FromHtml("#E7EFF2");
                    objWorksheet.Cells["A1"].LoadFromDataTable(dtSrc, true);
                    objWorksheet.Cells.Style.Font.SetFromFont(new Font("Calibri", 12));
                    objWorksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    objWorksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    objWorksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    objWorksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;

                    objWorksheet.Cells.AutoFitColumns();
                    //Format the header    
                    using (ExcelRange objRange = objWorksheet.Cells[1, 1, 1, columnCount])//["A1:XFD1"])
                    {
                        objRange.Style.Font.Bold = true;
                        objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        objRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                        objRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        objRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        objRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        objRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;

                    }
                }

                //Write it back to the client    
                if (File.Exists(filePath))
                    File.Delete(filePath);

                //Create excel file on physical disk    
                FileStream objFileStrm = File.Create(filePath);
                objFileStrm.Close();

                //Write content to excel file    
                File.WriteAllBytes(filePath, objExcelPackage.GetAsByteArray());
            }
        }



    }
}
