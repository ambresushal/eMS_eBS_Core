using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FindnReplace;

namespace tmg.equinox.web.FindnReplace
{
    public class AnnotationExcelGenerator
    {

        public AnnotationExcelGenerator()
        {

        }

        public byte[] GenerateExcelReport(List<AnnotationViewModel> annotations)
        {
            int startRowIdx = 1; int startColIdx = 1;
            MemoryStream fileStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                int rowIdx = startRowIdx; int colIdx = startColIdx;
                string sheetName = "Annotations";
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                GenerateHeader(worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                GenerateRow(annotations, worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                SetBorderForDataRange(worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                package.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            byte[] byteArray = new byte[fileStream.Length];
            fileStream.Position = 0;
            fileStream.Read(byteArray, 0, (int)fileStream.Length);
            return byteArray;
        }
        private void GenerateHeader(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx)
        {
            try
            {
                sheet.Row(rowIdx).Height = 15;

                ExcelRange cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "ElementPath";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 10;
                sheet.Column(colIdx).Width = 20;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Field";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 10;
                sheet.Column(colIdx).Width = 20;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "AnnotationFor";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 10;
                sheet.Column(colIdx).Width = 20;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Annotation";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 10;
                sheet.Column(colIdx).Width = 20;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "AuthorName";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 10;
                sheet.Column(colIdx).Width = 20;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                int endColumnIdx = sheet.Dimension.End.Column;
                sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                rowIdx++;
                colIdx = startColIdx;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
        }
        private void GenerateRow(List<AnnotationViewModel> elements, ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx)
        {
            try
            {
                if (elements != null && elements.Count > 0)
                {
                    foreach (var ele in elements)
                    {
                        ExcelRange cell;
                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = ele.ElementPath;
                        cell.Style.Font.Size = 10;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = ele.Field;
                        cell.Style.Font.Size = 10;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = ele.AnnotationFor;
                        cell.Style.Font.Size = 10;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = ele.Annotation;
                        cell.Style.Font.Size = 10;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = ele.AuthorName;
                        cell.Style.Font.Size = 10;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        rowIdx++;
                        colIdx = startColIdx;
                    }

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
        }
        private void SetBorderForDataRange(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx)
        {
            try
            {
                int endRowIdx = sheet.Dimension.End.Row;
                int endColumnIdx = sheet.Dimension.End.Column;
                var sheetRange = sheet.Cells[startColIdx, startColIdx, endRowIdx, endColumnIdx];
                sheetRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheetRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheetRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheetRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }

    }
}
