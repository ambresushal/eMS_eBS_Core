using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.web.FormDesignManager.ConfigView
{
    public class ConfigViewExcelGenerator
    {
        private List<string> _elementTypes = new List<string>() { "Section", "Rich TextBox", "Label", "RadioButton", "Checkbox", "Textbox", "Multiline TextBox", "DropDown list", "DropDown TextBox", "Calender", "Repeater" };
        private List<string> _dataTypes = new List<string>() { "Integer", "String", "Float" };
        private List<string> _libraryRegex = new List<string>() { "Date", "Zip Code", "Phone", "Url", "Email", "Decimal", "Comma Separated", "Social Security Number", "Hexa Decimal", "Numeric With 2 Precision", "Numeric With 2 Precision Or Percent", "Numeric Only", "Alphabets Only", "Time Format" };
        private List<string> _required = new List<string>() { "Yes", "No" };
        private Dictionary<string, List<string>> _colTypeMap = new Dictionary<string, List<string>>();
        private List<RuleRowModel> _ruleList;
        private List<DocumentRuleModel> _expRuleList;
        public ConfigViewExcelGenerator(List<RuleRowModel> rules, List<DocumentRuleModel> expRules)
        {
            _colTypeMap.Add("ElementType", _elementTypes);
            _colTypeMap.Add("DataType", _dataTypes);
            _colTypeMap.Add("Formats", _libraryRegex);
            _colTypeMap.Add("Required", _required);
            this._ruleList = rules;
            this._expRuleList = expRules;
        }

        public byte[] GenerateConfigExcelReport(List<UIElementRowViewModel> elements, List<ExcelConfiguration> config, string formDesignName, string formDesignVersion, List<Comments> comments, string extendedProperties)
        {
            int startRowIdx = 1; int startColIdx = 1;
            MemoryStream fileStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                if (config == null || config.Count == 0)
                {
                    config = GetDefaultConfiguration();
                }
                int rowIdx = startRowIdx; int colIdx = startColIdx;
                string sheetName = formDesignName + "_" + formDesignVersion;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                GenerateHeader(config, worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, extendedProperties);
                GenerateRow(elements, config, worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, comments, extendedProperties);
                SetBorderForDataRange(worksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                FillExcelDropDown(worksheet, "ElementType", 3);
                FillExcelDropDown(worksheet, "DataType", 5);
                FillExcelDropDown(worksheet, "Required", 7);
                FillExcelDropDown(worksheet, "Formats", 8);
                package.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            byte[] byteArray = new byte[fileStream.Length];
            fileStream.Position = 0;
            fileStream.Read(byteArray, 0, (int)fileStream.Length);
            return byteArray;
        }
        private void GenerateHeader(List<ExcelConfiguration> config, ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, string extendedProperties)
        {
            try
            {
                sheet.Row(rowIdx).Height = 15;
                ExcelRange cell;

                foreach (var column in config)
                {
                    if (!column.Include) { continue; }
                    cell = sheet.Cells[rowIdx, colIdx];
                    cell.Value = column.ColumnHeader;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 10;
                    sheet.Column(colIdx).Width = 20;
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
        private void GenerateRow(List<UIElementRowViewModel> elements, List<ExcelConfiguration> config, ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, List<Comments> comments, string extendedProperties)
        {
            try
            {
                if (elements != null && elements.Count > 0)
                {
                    foreach (var ele in elements)
                    {
                        JToken row = JToken.FromObject(ele);
                        ExcelRange cell;
                        foreach (var col in config)
                        {
                            if (!col.Include) { continue; }
                            cell = sheet.Cells[rowIdx, colIdx];
                            if (col.ColumnIndex < 0)
                            {
                                cell.Value = rowIdx - 1;
                            }
                            else if (col.ColumnName == "MaxLength")
                            {
                                int value;
                                int.TryParse(Convert.ToString(row[col.ColumnName]), out value);
                                if (value > 0)
                                {
                                    cell.Value = value;
                                }
                            }
                            else if (col.ColumnName == "Required" || col.ColumnName == "IsKey")
                            {
                                if (Convert.ToString(row["ElementType"]) != "Section" && Convert.ToString(row["ElementType"]) != "Repeater")
                                {
                                    cell.Value = string.Equals(Convert.ToString(row[col.ColumnName]), "True", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";
                                }
                            }
                            else if (col.ColumnName == "IsVisible" || col.ColumnName == "IsEnable")
                            {
                                cell.Value = string.Equals(Convert.ToString(row[col.ColumnName]), "True", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";
                            }
                            else if (col.ColumnName == "AllowBulkUpdate")
                            {
                                if (Convert.ToString(row["ElementType"]) == "Repeater")
                                {
                                    cell.Value = string.Equals(Convert.ToString(row[col.ColumnName]), "True", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";
                                }
                            }
                            else if (col.ColumnName == "HasOptions")
                            {
                                var options = row["Items"].Select(s => s["Value"]).ToArray();
                                string value = string.Empty;
                                foreach (var item in options)
                                {
                                    value += Convert.ToString(item) + System.Environment.NewLine;
                                }
                                cell.Value = value;
                            }
                            else if (col.ColumnName.Contains("ExtProp."))
                            {
                                cell.Value = Convert.ToString(row.SelectToken(col.ColumnName));
                            }
                            else if (col.ColumnName == "HasRules")
                            {
                                cell.Value = this.GetRuleDescription(ele);
                            }
                            else if (col.ColumnName == "HasExpRules")
                            {
                                cell.Value = GetExpressionRuleHash(ele);
                            }
                            else
                            {
                                cell.Value = Convert.ToString(row[col.ColumnName]);
                            }
                            cell.Style.Font.Size = 10;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            int indx = rowIdx - 2;
                            var text = comments.Where(s => s.row == Convert.ToString(indx) && s.col == Convert.ToString(col.ColumnIndex)).FirstOrDefault();
                            if (text != null)
                            {
                                cell.AddComment(text.comment.value, "");
                            }
                            colIdx++;
                        }

                        if (Convert.ToString(row["ElementType"]) == "Section" || Convert.ToString(row["ElementType"]) == "Repeater")
                        {
                            FormatContainer(sheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, Convert.ToString(row["ElementType"]));
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
        private void FillExcelDropDown(ExcelWorksheet sheet, string column, int colIndex)
        {
            try
            {
                if (!String.IsNullOrEmpty(column))
                {
                    int startRowIdx = sheet.Dimension.Start.Row;
                    int startColumnIdx = sheet.Dimension.Start.Column;
                    int endRowIdx = sheet.Dimension.End.Row;
                    int endColumnIdx = sheet.Dimension.End.Column;

                    ExcelRange elementType = sheet.Cells[startRowIdx, colIndex, endRowIdx, colIndex];
                    string addr = elementType.Address;
                    var val = sheet.DataValidations.AddListValidation(addr);
                    val.ShowErrorMessage = true;
                    val.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    val.ErrorTitle = "";
                    val.Error = "";

                    foreach (string item in _colTypeMap[column])
                    {
                        val.Formula.Values.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
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
        private void FormatContainer(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, string fieldType)
        {
            try
            {
                int endColumnIdx = sheet.Dimension.End.Column;
                sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.Font.Bold = true;
                sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;
                if (fieldType == "Section")
                {
                    sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                }
                else if (fieldType == "Repeater")
                {
                    sheet.Cells[rowIdx, startColIdx, rowIdx, endColumnIdx].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }

        private List<ExcelConfiguration> GetDefaultConfiguration()
        {
            List<ExcelConfiguration> config = new List<ExcelConfiguration>();
            config.Add(new ExcelConfiguration() { ColumnHeader = "Field Label", ColumnName = "Label", ColumnIndex = 0, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Field Type", ColumnName = "ElementType", ColumnIndex = 3, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Options", ColumnName = "HasOptions", ColumnIndex = 4, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Data Type", ColumnName = "DataType", ColumnIndex = 6, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Length", ColumnName = "MaxLength", ColumnIndex = 9, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Is Required", ColumnName = "Required", ColumnIndex = 10, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Format/Mask", ColumnName = "Formats", ColumnIndex = 11, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Help Text", ColumnName = "HelpText", ColumnIndex = 18, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Parent", ColumnName = "Parent", ColumnIndex = 13, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Rules", ColumnName = "HasRules", ColumnIndex = 14, Include = true, OutputColumnName = string.Empty });
            config.Add(new ExcelConfiguration() { ColumnHeader = "Expression", ColumnName = "HasExpRules", ColumnIndex = 15, Include = true, OutputColumnName = string.Empty });
            return config;
        }

        private string GetRuleDescription(UIElementRowViewModel element)
        {
            string ruleDesc = string.Empty;
            var rules = _ruleList.Where(s => s.UIElementID == element.UIElementID).ToList();

            if (rules != null && rules.Count > 0)
            {
                foreach (var rule in rules)
                {
                    ruleDesc = ruleDesc + rule.RuleDescription + Environment.NewLine;
                }
            }

            return ruleDesc;
        }

        private string GetExpressionRuleHash(UIElementRowViewModel element)
        {
            string expHash = string.Empty;
            var rules = _expRuleList.Where(s => s.TargetUIElementID == element.UIElementID).ToList();

            if (rules != null && rules.Count > 0)
            {
                expHash = ComputeSha256Hash(Regex.Replace(rules[0].RuleJSON, @"\s", ""));
            }

            return expHash;
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
