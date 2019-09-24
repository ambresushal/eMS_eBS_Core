using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class RepeaterExcelReportGenerator
    {
        private RepeaterCompareResult repeaterCompareResult;
        private ExcelWorksheet repeaterSheet;
        public RepeaterExcelReportGenerator(RepeaterCompareResult repeaterCompareResult, ExcelWorksheet repeaterSheet)
        {
            this.repeaterCompareResult = repeaterCompareResult;
            this.repeaterSheet = repeaterSheet;
        }

        public void GenerateReport()
        {
            int row = 0;
            GenerateHeader(ref row);
            GenerateRepeaterMatchResults(ref row);
            repeaterSheet.Cells.AutoFitColumns();
        }

        private void GenerateHeader(ref int row)
        {
            ExcelRange cell = repeaterSheet.Cells["B2"];
            cell.Style.Font.Bold = true;
            cell.Value = "Compare Criteria";
            cell = repeaterSheet.Cells["C2"];
            cell.Style.Font.Bold = true;
            cell.Value = "Source";
            cell = repeaterSheet.Cells["D2"];
            cell.Style.Font.Bold = true;
            cell.Value = "Target";
            row = 3;
            foreach (RepeaterCompareKey key in repeaterCompareResult.Keys) 
            {
                cell = repeaterSheet.Cells["B" + row];
                cell.Value = key.KeyLabel;
                cell = repeaterSheet.Cells["C" + row];
                cell.Value = key.SourceKey;
                cell = repeaterSheet.Cells["D" + row];
                cell.Value = key.TargetKey;
                row = row + 1;
            }

            row = row + 2;
            cell = repeaterSheet.Cells["B" + row];
            cell.Style.Font.Bold = true;
            cell.Value = "Fields Compared";
            row = row + 1;
            if (repeaterCompareResult.Fields != null)
            {
                foreach (RepeaterCompareField field in repeaterCompareResult.Fields)
                {
                    cell = repeaterSheet.Cells["B" + row];
                    cell.Value = field.FieldName;
                    row = row + 1;
                }
            }
        }

        private void GenerateRepeaterMatchResults(ref int row)
        {
            row = row + 2;
            Dictionary<string, int> columns = GenerateRepeaterMatchHeader(ref row);
            GenerateRepeaterReport(columns, ref row);
        }

        private Dictionary<string, int> GenerateRepeaterMatchHeader(ref int row) 
        {
            int column = 2;
            int endColumn = column + repeaterCompareResult.Keys.Count + (repeaterCompareResult.Fields == null ? 0 : repeaterCompareResult.Fields.Count - 1);
            ExcelRange cell = repeaterSheet.Cells[row, column, row, endColumn];
            cell.Merge = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Font.Bold = true;
            cell.Value = "Results";
            Dictionary<string, int> columnKeyFieldMatch = new Dictionary<string, int>();
            int numberOfColumns = repeaterCompareResult.Keys.Count + (repeaterCompareResult.Fields == null ? 0 : repeaterCompareResult.Fields.Count - 1);
            row = row + 1;
            //prepare header cells
            int rowFilter = row;
            int colFilterStart = column;
            int colFilterEnd = column;
            foreach (RepeaterCompareKey key in repeaterCompareResult.Keys) 
            {
                cell = repeaterSheet.Cells[row, column];
                cell.Value = key.KeyLabel;
                cell.Style.Font.Bold = true;
                columnKeyFieldMatch.Add(key.KeyName, column);
                column = column + 1;
            }
            cell = repeaterSheet.Cells[row, column];
            cell.Value = "[Document]";
            cell.Style.Font.Bold = true;
            cell.AutoFilter = true;
            columnKeyFieldMatch.Add("[Document]", column);
            column = column + 1;
            cell = repeaterSheet.Cells[row, column];
            cell.Value = "[Sync]";
            cell.Style.Font.Bold = true;
            cell.AutoFilter = true;
            columnKeyFieldMatch.Add("[Sync]", column);
            column = column + 1;
            cell = repeaterSheet.Cells[row, column];
            cell.Value = "[Match]";
            cell.Style.Font.Bold = true;
            cell.AutoFilter = true;
            columnKeyFieldMatch.Add("[Match]", column);
            column = column + 1;
            if (repeaterCompareResult.Fields != null)
            {
                foreach (RepeaterCompareField field in repeaterCompareResult.Fields)
                {
                    if (!columnKeyFieldMatch.Keys.Contains(field.FieldName))
                    {
                        cell = repeaterSheet.Cells[row, column];
                        cell.Value = field.FieldName;
                        cell.Style.Font.Bold = true;
                        cell.AutoFilter = true;
                        columnKeyFieldMatch.Add(field.FieldName, column);
                        column = column + 1;
                    }
                }
            }
            colFilterEnd = column - 1;
            cell = repeaterSheet.Cells[rowFilter, colFilterStart, rowFilter, colFilterEnd];
            cell.AutoFilter = true;
            return columnKeyFieldMatch;
        }

        private void GenerateRepeaterReport(Dictionary<string, int> columns, ref int row)
        {
            if (repeaterCompareResult.Rows.Count > 0)
            {
                ExcelRange cell = null;
                RepeaterCompareRow repeaterRow = repeaterCompareResult.Rows.First();
                int rowCount = 2;
                //if any source/target keys are not matching, means that it is a mismatch type of compare
                //therefore, 2 rows per result row
                row = row + 1;
                foreach (RepeaterCompareRow resultRow in repeaterCompareResult.Rows)
                {
                    if (resultRow.ChildRows != null)
                    {
                        foreach (RepeaterCompareRow childRow in resultRow.ChildRows)
                        {
                            foreach (RepeaterCompareKey key in resultRow.Keys)
                            {
                                int column = columns[key.KeyName];
                                if (key.SourceTargetKeyMatch == true)
                                {
                                    cell = repeaterSheet.Cells[row, column, row + 1, column];
                                    cell.Merge = true;
                                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    cell.Value = key.SourceKey;
                                }
                                else
                                {
                                    cell = repeaterSheet.Cells[row, column];
                                    cell.Value = key.SourceKey;
                                    cell = repeaterSheet.Cells[row + 1, column];
                                    cell.Value = key.TargetKey;
                                }
                            }
                            foreach (RepeaterCompareKey key in childRow.Keys)
                            {
                                int column = columns[key.KeyName];
                                if (key.SourceTargetKeyMatch == true)
                                {
                                    cell = repeaterSheet.Cells[row, column, row + 1, column];
                                    cell.Merge = true;
                                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    cell.Value = key.SourceKey;
                                }
                                else
                                {
                                    cell = repeaterSheet.Cells[row, column];
                                    cell.Value = key.SourceKey;
                                    if (key.IsMissingInSource == true) 
                                    {
                                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                    }
                                    cell = repeaterSheet.Cells[row + 1, column];
                                    cell.Value = key.TargetKey;
                                    if (key.IsMissingInTarget == true)
                                    {
                                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                    }
                                }
                            }

                            int documentCol = columns["[Document]"];
                            cell = repeaterSheet.Cells[row, documentCol];
                            cell.Value = "Source";
                            cell = repeaterSheet.Cells[row + 1, documentCol];
                            cell.Value = "Target";

                            int matchCol = columns["[Match]"];
                            cell = repeaterSheet.Cells[row, matchCol, row + 1, matchCol];
                            cell.Merge = true;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            cell.Value = "No";
                            if (childRow.IsMatch == true)
                            {
                                cell.Value = "Yes";
                            }

                            int syncCol = columns["[Sync]"];
                            cell = repeaterSheet.Cells[row, syncCol, row + 1, syncCol];
                            cell.Merge = true;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            cell.Value = "No";
                            if (childRow.CanSync == true) 
                            {
                                cell.Value = "Yes";
                            }
                            
                            
                            foreach (RepeaterCompareField field in resultRow.Fields)
                            {
                                int column = columns[field.FieldName];
                                cell = repeaterSheet.Cells[row, column];
                                cell.Value = field.SourceValue;
                                if (field.SourceValue != field.TargetValue)
                                {
                                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                                cell = repeaterSheet.Cells[row + 1, column];
                                cell.Value = field.TargetValue;
                                if (field.SourceValue != field.TargetValue)
                                {
                                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                            }
                            foreach (RepeaterCompareField field in childRow.Fields)
                            {
                                int column = columns[field.FieldName];
                                cell = repeaterSheet.Cells[row, column];
                                cell.Value = field.SourceValue;
                                if (field.SourceValue != field.TargetValue)
                                {
                                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                                cell = repeaterSheet.Cells[row + 1, column];
                                cell.Value = field.TargetValue;
                                if (field.SourceValue != field.TargetValue)
                                {
                                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                            }
                            row = row + rowCount;
                        }
                    }
                    else 
                    {
                        foreach (RepeaterCompareKey key in resultRow.Keys)
                        {
                            int column = columns[key.KeyName];
                            if (key.SourceTargetKeyMatch == true)
                            {
                                cell = repeaterSheet.Cells[row, column, row + 1, column];
                                cell.Merge = true;
                                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                cell.Value = key.SourceKey;
                            }
                            else
                            {
                                cell = repeaterSheet.Cells[row, column];
                                cell.Value = key.SourceKey;
                                if (key.IsMissingInSource == true)
                                {
                                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                                cell = repeaterSheet.Cells[row + 1, column];
                                cell.Value = key.TargetKey;
                                if (key.IsMissingInTarget == true)
                                {
                                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }

                            }
                        }
                        int documentCol = columns["[Document]"];
                        cell = repeaterSheet.Cells[row, documentCol];
                        cell.Value = "Source";
                        cell = repeaterSheet.Cells[row + 1, documentCol];
                        cell.Value = "Target";

                        int matchCol = columns["[Match]"];
                        cell = repeaterSheet.Cells[row, matchCol, row + 1, matchCol];
                        cell.Merge = true;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        cell.Value = "No";
                        if (resultRow.IsMatch == true)
                        {
                            cell.Value = "Yes";
                            }

                        int syncCol = columns["[Sync]"];
                        cell = repeaterSheet.Cells[row, syncCol, row + 1, syncCol];
                        cell.Merge = true;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        cell.Value = "No";
                        if (resultRow.CanSync == true)
                        {
                            cell.Value = "Yes";
                        }
                        foreach (RepeaterCompareField field in resultRow.Fields)
                        {
                            int column = columns[field.FieldName];
                            cell = repeaterSheet.Cells[row, column];
                            cell.Value = field.SourceValue;
                            if (field.SourceValue != field.TargetValue)
                            {
                                cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            }
                            cell = repeaterSheet.Cells[row + 1, column];
                            cell.Value = field.TargetValue;
                            if (field.SourceValue != field.TargetValue)
                            {
                                cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            }
                        }
                        row = row + rowCount;
                    }
                }
            }
        }
    }
}
