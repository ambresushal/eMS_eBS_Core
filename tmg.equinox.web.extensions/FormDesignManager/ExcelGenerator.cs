using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.RulesManager;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.web.FormDesignManager
{
    public class ExcelGenerator
    {
        private List<RuleRowViewModel> _ruleList;
        private string _viewType;
        public ExcelGenerator(List<RuleRowViewModel> ruleList, string viewType)
        {
            this._ruleList = ruleList;
            this._viewType = viewType;
        }

        public byte[] GenerateExcelReport()
        {
            int startRowIdx = 1; int startColIdx = 1;
            MemoryStream fileStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                int rowIdx = startRowIdx; int colIdx = startColIdx;
                string sheetName = "Rule Master List";
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                GenerateHeader(worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                GenerateRow(worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                SetBorderForDataRange(worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                package.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            byte[] byteArray = new byte[fileStream.Length];
            fileStream.Position = 0;
            fileStream.Read(byteArray, 0, (int)fileStream.Length);
            return byteArray;
        }

        private List<string> GetExcelConfiguration()
        {
            List<string> columns = new List<string>() { "Rule Name", "Rule Description", "Rule Type", "Sources", "Targets" };

            if (string.Equals(_viewType, "target", StringComparison.OrdinalIgnoreCase))
            {
                columns = new List<string>() { "Target Section", "Target Element", "Target Keys", "Rule Name", "Rule Description", "Rule Type", "Sources" };
            }

            if (string.Equals(_viewType, "source", StringComparison.OrdinalIgnoreCase))
            {
                columns = new List<string>() { "Source Section", "Source Element", "Source Keys", "Rule Name", "Rule Description", "Rule Type", "Targets" };
            }

            return columns;
        }
        private void GenerateHeader(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx)
        {
            try
            {
                List<string> columns = GetExcelConfiguration();
                sheet.Row(rowIdx).Height = 15;
                ExcelRange cell;

                foreach (var column in columns)
                {
                    cell = sheet.Cells[rowIdx, colIdx];
                    cell.Value = column;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 10;
                    sheet.Column(colIdx).Width = 50;
                    sheet.Cells.Style.WrapText = true;
                    colIdx++;
                }

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
        private void GenerateRow(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx)
        {
            try
            {
                List<string> columns = GetExcelConfiguration();
                if (_ruleList != null && _ruleList.Count > 0)
                {
                    foreach (var rule in _ruleList)
                    {
                        ExcelRange cell;
                        foreach (var column in columns)
                        {
                            cell = sheet.Cells[rowIdx, colIdx];
                            if (string.Equals(column, "Target Section", StringComparison.OrdinalIgnoreCase) || string.Equals(column, "Source Section", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.Section;
                            }

                            if (string.Equals(column, "Target Element", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.TargetElement;
                            }

                            if (string.Equals(column, "Source Element", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.SourceElement;
                            }

                            if (string.Equals(column, "Target Keys", StringComparison.OrdinalIgnoreCase) || string.Equals(column, "Source Keys", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.KeyFilter;
                            }

                            if (string.Equals(column, "Rule Name", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.RuleName;
                            }

                            if (string.Equals(column, "Rule Description", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.Description;
                            }

                            if (string.Equals(column, "Rule Type", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.Value = rule.Type;
                            }

                            if (string.Equals(column, "Sources", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrEmpty(rule.SourceElement))
                                {
                                    string[] sources = rule.SourceElement.Split(',');
                                    string sourceList = string.Join("," + System.Environment.NewLine, sources.Distinct().ToArray());
                                    cell.Value = sourceList;
                                }
                            }

                            if (string.Equals(column, "Targets", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrEmpty(rule.TargetElement))
                                {
                                    string[] targets = rule.TargetElement.Split(',');
                                    string targetsList = string.Join("," + System.Environment.NewLine, targets.Distinct().ToArray());
                                    cell.Value = targetsList;
                                }
                            }

                            cell.Style.Font.Size = 10;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            int indx = rowIdx - 2;
                            colIdx++;
                        }

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
