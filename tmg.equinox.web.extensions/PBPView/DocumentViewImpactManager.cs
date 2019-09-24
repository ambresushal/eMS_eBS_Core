using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.web.PBPView
{
    public class DocumentViewImpactConstant
    {
        public static readonly string SOURCES = "documentrule.ruleconditions.sources";
        public static readonly string TARGETELEMENT = "documentrule.targetelement";
        public static readonly string UPDATETYPE = "Cascade Updated";
        public static readonly string ACTION = "Verify Changes";
        public static readonly string SOURCELEMENT = "sourceelement";
        public static readonly string ANCHORNAME = "Medicare";
        public static readonly string FINDTEXT = "is changed from ";
        public static readonly string ENABLEDISABLETYPE = "Cascade Enable Disable";
        public static readonly string RUNVALIDATIONTYPE = "Cascade Run Validation";
        public static readonly string VALUETYPE = "Cascade Value Rule";
        public static readonly string VISIBILITYTYPE = "Cascade Visibility";
        public static readonly string ISREQUIREDTYPE = "Cascade Is Required";
        public static readonly string ERRORTYPE = "Cascade Error";
        public static readonly string EXPRESSIONVALUETYPE = "Cascade Expression Value";
        public static readonly string HIGHLIGHTTYPE = "Cascade Highlight";
        public static readonly string DIALOGTYPE = "Cascade Dialog";
        public static readonly string CUSTOMRULETYPE = "Cascade CustomRule";
        public static readonly string POTENTIALIMPACT = "Potential Impact";
    }
    public class DocumentViewImpactManager
    {
        public List<SourceElement> GetImpactedFields(List<ActivityLogModel> activityLog, string compiledList, List<ActivityLogModel> pbpViewActivityLog, int viewFormInstanceId, int viewFormDesignId, int pbpViewFormDesignID, IFormInstanceViewImpactLogService impactLogService)
        {
            int elementCnt = 0;
            List<SourceElement> impactedFields = new List<SourceElement>();
            List<DropDownElementItem> dropDownItems = impactLogService.GetDDLDropDownItems(pbpViewFormDesignID);
            if (!string.IsNullOrEmpty(compiledList))
            {
                List<SourceElement> allImpactedList = JsonConvert.DeserializeObject<List<SourceElement>>(compiledList);
                impactedFields = (from source in allImpactedList
                                  join curr in activityLog on new { source.ElementLabel, source.ElementPathName }
                                  equals new { ElementLabel = curr.Field, ElementPathName = curr.ElementPath }
                                  select new SourceElement()
                                  {
                                      ID = elementCnt++,
                                      ElementID = source.ElementID,
                                      ElementName = source.ElementName,
                                      ElementLabel = source.ElementLabel,
                                      ElementPath = source.ElementPath,
                                      ElementPathName = source.ElementPathName,
                                      Description = curr.Description.Substring(curr.Description.IndexOf(DocumentViewImpactConstant.FINDTEXT) + DocumentViewImpactConstant.FINDTEXT.Length),
                                      Keys = curr.RowNum,
                                      UpdatedBy = curr.UpdatedBy,
                                      UpdatedDate = curr.UpdatedLast,
                                      // ImpactedElements = source.ImpactedElements
                                      ImpactedElements = GetImpactedElements(source.ImpactedElements)
                                  }).ToList();


                impactedFields.ForEach(e => e.ImpactedElements.ForEach(
                    i =>
                    {
                        i.FormInstanceID = viewFormInstanceId; i.FormDesignVersionID = viewFormDesignId;
                        i.Description = (i.UpdateType == DocumentViewImpactConstant.UPDATETYPE ? GetActivityDescription(i.ElementName, i.ElementLabel, i.ElementPathLabel, i.ElementID, pbpViewActivityLog, dropDownItems)
                                        : DocumentViewImpactConstant.POTENTIALIMPACT);
                    }));
            }
            return impactedFields;

        }
        public byte[] GeneratePBPViewImpactExcelReport(List<SourceElement> impactJsonObject, string formInstanceName)
        {
            int startRowIdx = 2; int startColIdx = 2; int fontSize = 10;
            MemoryStream fileStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                int rowIdx = startRowIdx; int colIdx = startColIdx;
                //Generate report sheet
                ExcelWorksheet sotWorksheet = package.Workbook.Worksheets.Add(formInstanceName);
                GenerateImpactFields(sotWorksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, fontSize, impactJsonObject);
                SetExcelBorders(sotWorksheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx);
                package.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            byte[] byteArray = new byte[fileStream.Length];
            fileStream.Position = 0;
            fileStream.Read(byteArray, 0, (int)fileStream.Length);
            return byteArray;
        }
        private string GetActivityDescription(string elementName, string elementLabel, string elementPathLabel, int elementID, List<ActivityLogModel> pbpViewActivityLog, List<DropDownElementItem> dropDownItems)
        {
            string description = String.Empty;
            try
            {
                if (pbpViewActivityLog != null && pbpViewActivityLog.Count > 0)
                {
                    if (!String.IsNullOrEmpty(elementLabel) && !String.IsNullOrEmpty(elementName) && !String.IsNullOrEmpty(elementPathLabel))
                    {
                        if (elementName.IndexOf("Repeater") != -1)
                        {
                            description = elementLabel + " Repeater Was Modified";
                        }
                        else
                        {
                            var row = pbpViewActivityLog.Where(x => x.ElementPath == elementPathLabel && x.Field == elementLabel).FirstOrDefault();
                            if (row != null)
                            {
                                description = row.Description;
                                if (!String.IsNullOrEmpty(description))
                                {
                                    var isExist = dropDownItems.Exists(x => x.UIElementID == elementID);
                                    if (isExist)
                                    {
                                        var elements = dropDownItems.Where(y => y.UIElementID == elementID).ToList();
                                        if (elements != null && elements.Count > 0)
                                        {
                                            var text = description.Substring(description.IndexOf(DocumentViewImpactConstant.FINDTEXT) + DocumentViewImpactConstant.FINDTEXT.Length);
                                            if (!String.IsNullOrEmpty(text))
                                            {
                                                string from = text.Substring(0, text.IndexOf("to") - 1);
                                                string to = text.Substring((text.IndexOf("to") + 3), ((text.Length - 1) - (text.IndexOf("to") + 3)));
                                                string msg = "Value of " + elementLabel + " is changed from {0} to {1}.";
                                                // Chec from and To is multiselect
                                                if (from.StartsWith("[") && from.EndsWith("]") && from != "[Select One]")
                                                {
                                                    from = GetMultiSelectValue(from, elements);
                                                }
                                                else
                                                {
                                                    from = (!String.IsNullOrEmpty(from)) ? (elements.Where(x => x.Value == from.Trim()).FirstOrDefault() == null ? String.Empty : (elements.Where(x => x.Value == from.Trim()).FirstOrDefault().DisplayText))
                                                            : String.Empty;
                                                }
                                                if (to.StartsWith("[") && to.EndsWith("]") && to != "[Select One]")
                                                {
                                                    to = GetMultiSelectValue(to, elements);
                                                }
                                                else
                                                {
                                                    to = (!String.IsNullOrEmpty(to)) ? (elements.Where(x => x.Value == to.Trim()).FirstOrDefault() == null ? String.Empty : (elements.Where(x => x.Value == to.Trim()).FirstOrDefault().DisplayText))
                                                   : String.Empty;
                                                }
                                                msg = String.Format(msg, from, to);
                                                description = msg;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return description;
        }
        private List<ImpactedElement> GetImpactedElements(List<ImpactedElement> impactedElements)
        {
            try
            {
                if (impactedElements != null && impactedElements.Count > 0)
                {
                    int cnt = 0;
                    foreach (var item in impactedElements)
                    {
                        item.ID = cnt++;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return impactedElements;
        }
        private string GetMultiSelectValue(string srcValue, List<DropDownElementItem> elements)
        {
            string val = string.Empty;
            try
            {
                JArray srcArray = JArray.Parse(srcValue);
                if (srcArray != null && srcArray.Count > 0)
                {
                    int cnt = srcArray.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        val += (!String.IsNullOrEmpty(Convert.ToString(srcArray[i]))) ? (elements.Where(z => z.Value == Convert.ToString(srcArray[i]).Trim()).FirstOrDefault() == null ? String.Empty : (elements.Where(z => z.Value == Convert.ToString(srcArray[i]).Trim()).FirstOrDefault().DisplayText))
                        : String.Empty;
                        if (i != cnt - 1)
                            val += ",";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return val;
        }
        private void GenerateParentHeader(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, int fontSize)
        {
            try
            {
                ExcelRange cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "SOT/Medicare Field Path";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Column(colIdx).Width = 45;
                sheet.Row(rowIdx).Height = 15;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                //Field Name Header 
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Field";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Column(colIdx).Width = 50;
                colIdx++;

                //Field Change
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Field Change";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Column(colIdx).Width = 50;
                colIdx++;

                //Updated By
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Updated By";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Column(colIdx).Width = 25;

                colIdx++;
                //Last Updated
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Last Updated";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Column(colIdx).Width = 50;

                sheet.Cells[rowIdx, startColIdx, rowIdx, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[rowIdx, startColIdx, rowIdx, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[rowIdx, startColIdx, rowIdx, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[rowIdx, startColIdx, rowIdx, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }
            catch (Exception ex)
            {
            }
        }
        private void GenerateChildHeader(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, int fontSize)
        {
            try
            {
                rowIdx++;
                colIdx = startColIdx + 1;
                ExcelRange cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "PBP View Field Path";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Column(colIdx).Width = 50;
                sheet.Row(rowIdx).Height = 15;
                sheet.Cells.Style.WrapText = true;
                colIdx++;

                //Field Name Header 
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Field";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Column(colIdx).Width = 50;
                colIdx++;

                //Field Change
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Update Type";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Column(colIdx).Width = 25;
                colIdx++;

                //Updated By
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "Field Value Update";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Column(colIdx).Width = 50;

                colIdx++;
                //Last Updated
                cell = sheet.Cells[rowIdx, colIdx];
                cell.Value = "PBP View Action";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = fontSize;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Column(colIdx).Width = 25;

                sheet.Cells[rowIdx, startColIdx + 1, rowIdx, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[rowIdx, startColIdx + 1, rowIdx, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[rowIdx, startColIdx + 1, rowIdx, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[rowIdx, startColIdx + 1, rowIdx, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }
            catch (Exception ex)
            {
            }
        }
        private void GenerateImpactFields(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, int fontSize, List<SourceElement> impactJsonObject)
        {
            try
            {
                if (impactJsonObject != null && impactJsonObject.Count > 0)
                {
                    foreach (var field in impactJsonObject)
                    {
                        colIdx = startColIdx;
                        // Create Parent Repeater Header
                        GenerateParentHeader(sheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, fontSize);

                        rowIdx++;
                        colIdx = startColIdx;

                        ExcelRange cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = field.ElementPathName;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = field.ElementLabel;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = field.Description.Replace("<b>", "").Replace("</b>", "");
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = field.UpdatedBy.Replace("<b>", "").Replace("</b>", "");
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = field.UpdatedDate;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.Numberformat.Format = "mm/dd/yyyy hh:mm AM/PM";
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        // Add Child Rows
                        GenerateChildHeader(sheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, fontSize);
                        GenerateChildImpactFields(sheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, fontSize, field.ImpactedElements);
                        rowIdx = rowIdx++;
                    }
                }
                else
                {
                    // if no impact history generate blank only header
                    GenerateParentHeader(sheet, ref rowIdx, ref colIdx, startRowIdx, startColIdx, fontSize);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void GenerateChildImpactFields(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx, int fontSize, List<ImpactedElement> impactElements)
        {
            try
            {
                rowIdx++;
                colIdx = startColIdx + 1;
                if (impactElements != null && impactElements.Count > 0)
                {
                    foreach (var row in impactElements)
                    {
                        ExcelRange cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = row.ElementPathLabel;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = row.ElementLabel;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = row.UpdateType;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = row.Description.Replace("<b>", "").Replace("</b>", "");
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        colIdx++;

                        cell = sheet.Cells[rowIdx, colIdx];
                        cell.Value = row.Action;
                        cell.Style.Font.Size = fontSize;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        rowIdx++;
                        colIdx = startColIdx + 1;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void SetExcelBorders(ExcelWorksheet sheet, ref int rowIdx, ref int colIdx, int startRowIdx, int startColIdx)
        {
            try
            {
                int endRowIdx = sheet.Dimension.End.Row;
                int endColumnIdx = sheet.Dimension.End.Column;
                var sheetRange = sheet.Cells[startRowIdx, startColIdx, endRowIdx, endColumnIdx];
                sheetRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheetRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheetRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheetRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
            catch (Exception ex) { }
        }

    }
}
