using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Reporting;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.web.FormInstance
{
    public class ChangeSummaryReportBuilder
    {
        #region Private Members
        private IFolderVersionReportService _folderVersionReportService;
        private IFolderVersionServices _folderVersionServices;
        public ExpandoObject sourceDataObj;
        public ExpandoObject targetDatatObj;
        public ExpandoObjectConverter converter;
        #endregion Private Members

        #region Constructor
        public ChangeSummaryReportBuilder(IFolderVersionServices _folderVersionServices, IFolderVersionReportService _folderVersionReportService)
        {
            this._folderVersionServices = _folderVersionServices;
            this._folderVersionReportService = _folderVersionReportService;
        }
        #endregion Constructor

        #region Public Methods
        public Byte[] ExportToExcel(int tenantId, List<string> coverPageDataList, string jsonString)
        {
            //get json object from json string
            var formList = JsonConvert.DeserializeObject<List<ReportingViewModel>>(jsonString);

            List<FormInstanceCompareResult> compareResultList = new List<FormInstanceCompareResult>();
            converter = new ExpandoObjectConverter();
            IEnumerable<ReportingViewModel> sourceRepeaterNameList = null;
            IEnumerable<ReportingViewModel> targetRepeaterNameList = null;
            IEnumerable<ReportingViewModel> sourceUIElementList = null;
            IEnumerable<ReportingViewModel> targetUIElementList = null;
            for (int i = 0; i < formList.Count; i++)
            {
                FormInstanceCompareResult compareResult = new FormInstanceCompareResult();
                string sourceID = formList[i].SourceInstanceId;
                int sourceInstanceId;
                string targetJsonData = null;
                string sourceJsonData = null;

                bool result = int.TryParse(sourceID, out sourceInstanceId);
                if (result == true)
                {
                    sourceJsonData = _folderVersionServices.GetFormInstanceData(tenantId, sourceInstanceId);
                    sourceRepeaterNameList = _folderVersionReportService.GetDataSourceList(tenantId, sourceInstanceId);
                    sourceUIElementList = _folderVersionReportService.GetUIElementList(tenantId, sourceInstanceId);
                }
                int targetInnstanceId;
                string targetId = formList[i].TargetInstanceId;
                result = int.TryParse(targetId, out targetInnstanceId);
                if (result == true)
                {
                    targetJsonData = _folderVersionServices.GetFormInstanceData(tenantId, targetInnstanceId);
                    targetRepeaterNameList = _folderVersionReportService.GetDataSourceList(tenantId, targetInnstanceId);
                    targetUIElementList = _folderVersionReportService.GetUIElementList(tenantId, targetInnstanceId);
                }

                if (string.IsNullOrWhiteSpace(sourceJsonData) == false && string.IsNullOrWhiteSpace(targetJsonData) == true || string.IsNullOrWhiteSpace(sourceJsonData) == true & string.IsNullOrWhiteSpace(targetJsonData) == false || string.IsNullOrWhiteSpace(sourceJsonData) == true & string.IsNullOrWhiteSpace(targetJsonData) == true)
                {
                    compareResult.SourceName = formList[i].SourceInstanceName;
                    compareResult.TargetName = formList[i].TargetInstanceName;
                    compareResultList.Add(compareResult);
                }
                else
                {
                    sourceDataObj = JsonConvert.DeserializeObject<ExpandoObject>(sourceJsonData, converter);
                    targetDatatObj = JsonConvert.DeserializeObject<ExpandoObject>(targetJsonData, converter);

                    var sourceFormData = sourceDataObj as IDictionary<string, object>;
                    var targetFormData = targetDatatObj as IDictionary<string, object>;

                    compareResult.SourceName = formList[i].SourceInstanceName;
                    compareResult.TargetName = formList[i].TargetInstanceName;
                    FormInstanceComparer comparer = new FormInstanceComparer();

                    compareResult.Result = comparer.ProcessCompare(sourceFormData, targetFormData, sourceRepeaterNameList, targetRepeaterNameList, sourceUIElementList, targetUIElementList);
                    compareResultList.Add(compareResult);
                }
            }

            MemoryStream reportStream = GenrateExcelReport(tenantId, coverPageDataList, compareResultList, jsonString);
            byte[] byteArray = new byte[reportStream.Length];
            reportStream.Position = 0;
            reportStream.Read(byteArray, 0, (int)reportStream.Length);
            return byteArray;
        }

        public MemoryStream GenrateExcelReport(int tenantId, List<string> coverPageDataList, List<FormInstanceCompareResult> changeSummaryData, string jsonString)
        {
            var formList = JsonConvert.DeserializeObject<List<ReportingViewModel>>(jsonString);

            //get cover page data
            DataTable coverPageData = GetCoverPageData(coverPageDataList, jsonString);
            using (ExcelPackage excelPkg = new ExcelPackage())
            {
                //Create cover page 
                ExcelWorksheet worksheet = excelPkg.Workbook.Worksheets.Add(ChangeSummaryConstants.ChangeSummaryReportExcelSheetName);
                int lastAddress = coverPageData.Rows.Count + 3;
                worksheet.Cells["B4"].LoadFromDataTable(coverPageData, false);
                //worksheet.Cells["B4"].LoadFromCollection(changeSummaryData[0].Comparision, false);
                worksheet.Cells["B4:D" + lastAddress + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B4:D" + lastAddress + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B4:D" + lastAddress + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B4:D" + lastAddress + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.AutoFitColumns();
                worksheet.InsertRow(1, 1);
                worksheet.Cells["B5:" + "B8"].Style.Font.Bold = true;
                worksheet.Cells["B2"].Value = ChangeSummaryConstants.ChangeSummaryReportName;
                worksheet.Cells["B2"].Style.Font.Bold = true;
                worksheet.Cells["B2"].Style.Font.Size = 14;
                worksheet.Cells["B3"].Value = "Report Generation Date";
                worksheet.Cells["B3"].Style.Font.Bold = true;
                worksheet.Cells["B3"].Style.Font.Size = 12;
                worksheet.Cells["C3"].Value = DateTime.Now.ToShortDateString();
                worksheet.Cells.AutoFitColumns();

                //create no of Sheets based on no of document
                for (int i = 0; i < changeSummaryData.Count(); i++)
                {
                    string sheetName = GenerateSheetName(formList[i].SourceInstanceName, formList[i].TargetInstanceName);

                    lastAddress = 0;

                    string sourceId = formList[i].SourceInstanceId;
                    foreach (ExcelWorksheet sheet in excelPkg.Workbook.Worksheets)
                    {
                        if (sheet.Name == sheetName)
                        {
                            sheetName = sheetName + "(" + (Convert.ToInt32(sheet.Index) - 1) + ")";
                        }
                    }
                    ExcelWorksheet worksheetDoc = excelPkg.Workbook.Worksheets.Add(sheetName);

                    if (changeSummaryData[i].Result != null && changeSummaryData[i].Result.compareList.Count > 0)
                    {
                        List<CompareFieldResult> changeSummaryDatas = changeSummaryData[i].Result.compareList;

                        int startAddress = 5;
                        lastAddress = 5;
                        for (int k = 0; k < changeSummaryDatas.Count(); k++)
                        {
                            if (string.IsNullOrEmpty(changeSummaryDatas[k].SectionName) == false)
                            {
                                worksheetDoc.Cells["B" + startAddress + ""].Value = changeSummaryDatas[k].SectionName.ToString();
                                worksheetDoc.Cells["B" + startAddress + ":F" + startAddress + ""].Merge = true;
                                worksheetDoc.Cells["B" + startAddress + ":F" + startAddress + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheetDoc.Cells["B" + startAddress + ":F" + startAddress + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheetDoc.Cells["B" + startAddress + ":F" + startAddress + ""].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheetDoc.Cells["B" + startAddress + ":F" + startAddress + ""].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#82CAFF"));
                                startAddress++;
                            }
                            if (string.IsNullOrEmpty(changeSummaryDatas[k].SubSectionName) == false)
                            {
                                worksheetDoc.Cells["C" + startAddress + ""].Value = changeSummaryDatas[k].SubSectionName.ToString();
                                worksheetDoc.Cells["C" + startAddress + ":F" + startAddress + ""].Merge = true;
                                worksheetDoc.Cells["C" + startAddress + ":F" + startAddress + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheetDoc.Cells["C" + startAddress + ":F" + startAddress + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                //worksheetDoc.Cells["C" + startAddress + ":F" + startAddress + ""].Style.Font.Bold= true;
                                worksheetDoc.Cells["C" + startAddress + ":F" + startAddress + ""].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#1E90FF"));
                                startAddress++;
                            }
                            if (string.IsNullOrEmpty(changeSummaryDatas[k].FieldType) == false)
                            {
                                worksheetDoc.Cells["D" + startAddress + ""].Value = changeSummaryDatas[k].FieldType.ToString();

                                if (changeSummaryDatas[k].SourceValue == null && changeSummaryDatas[k].TargetValue == null)
                                {
                                    int lenght = changeSummaryDatas[k].FieldType.Length;
                                    if (lenght > 3 && changeSummaryDatas[k].FieldType.ToString().Substring(0, 3) == "Row")
                                    {
                                        worksheetDoc.Cells["D" + startAddress + ""].Style.Font.Bold = true;
                                    }
                                    else
                                    {
                                        worksheetDoc.Cells["D" + startAddress + ""].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#1E90FF"));
                                    }
                                    startAddress++;
                                }

                            }
                            if (string.IsNullOrEmpty(changeSummaryDatas[k].SourceValue) == false)
                            {
                                worksheetDoc.Cells["E" + startAddress + ""].Value = changeSummaryDatas[k].SourceValue.ToString();
                                if (changeSummaryDatas[k].SourceValue == "No Changes" || changeSummaryDatas[k].SourceValue == "<blank>")
                                {
                                    worksheetDoc.Cells["E" + startAddress + ""].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                }
                                if (string.IsNullOrEmpty(changeSummaryDatas[k].TargetValue) == true)
                                {
                                    startAddress++;
                                }
                            }
                            if (string.IsNullOrEmpty(changeSummaryDatas[k].TargetValue) == false)
                            {
                                worksheetDoc.Cells["F" + startAddress + ""].Value = changeSummaryDatas[k].TargetValue.ToString();
                                if (changeSummaryDatas[k].TargetValue == "No Changes" || changeSummaryDatas[k].TargetValue == "<blank>")
                                {
                                    worksheetDoc.Cells["F" + startAddress + ""].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#808080"));
                                }
                                startAddress++;
                            }
                        }

                        lastAddress = changeSummaryDatas.Count() + 4;
                        worksheetDoc.Cells["B5:F5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheetDoc.Cells["B5:B" + lastAddress + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheetDoc.Cells["D5:D" + lastAddress + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheetDoc.Cells["C5:C" + lastAddress + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheetDoc.Cells["B" + lastAddress + ":F" + lastAddress + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheetDoc.Cells["F5:F" + lastAddress + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        worksheetDoc.Cells["E5"].Value = ChangeSummaryConstants.NotApplicable;
                        worksheetDoc.Cells["F5"].Value = ChangeSummaryConstants.NotApplicable;
                    }
                    worksheetDoc.Cells["B4"].Value = "Documents";
                    worksheetDoc.Cells["B4"].Style.Font.Bold = true;
                    worksheetDoc.Cells["E4"].Value = formList[i].SourceInstanceName.ToString();
                    worksheetDoc.Cells["E4"].Style.Font.Bold = true;
                    worksheetDoc.Cells["E4"].Style.Font.Size = 11;

                    worksheetDoc.Cells["F4"].Value = formList[i].TargetInstanceName.ToString();
                    worksheetDoc.Cells["F4"].Style.Font.Bold = true;
                    worksheetDoc.Cells["F4"].Style.Font.Size = 11;

                    worksheetDoc.Cells["B4:F4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetDoc.Cells["B4:F4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#3BB9FF"));

                    worksheetDoc.Cells["B2"].Value = "Change Summary Fields";
                    worksheetDoc.Cells["B2"].Style.Font.Bold = true;
                    worksheetDoc.Cells["B2"].Style.Font.Size = 12;
                    worksheetDoc.Cells["B5:B10000"].Style.Font.Bold = true;
                    worksheetDoc.Cells["C1:E10000"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheetDoc.Cells["C1:C10000"].Style.WrapText = true;
                    worksheetDoc.Cells["D1:D10000"].Style.WrapText = true;
                    worksheetDoc.Cells["E1:E10000"].Style.WrapText = true;
                    worksheetDoc.Cells["E1:E10000"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheetDoc.Cells["E1:E10000"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheetDoc.Cells["F1:F10000"].Style.WrapText = true;
                    worksheetDoc.Cells["F1:F10000"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheetDoc.Cells["F1:F10000"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheetDoc.Column(2).Width = 25;
                    worksheetDoc.Column(3).Width = 20;
                    worksheetDoc.Column(4).Width = 30;
                    worksheetDoc.Column(5).Width = 25;
                    worksheetDoc.Column(6).Width = 25;
                }
                var fileStream = new MemoryStream();
                excelPkg.SaveAs(fileStream);
                fileStream.Position = 0;                
                return fileStream;                
            }
        }
        #endregion Public Methods

        #region Private Methods
        private DataTable GetCoverPageData(List<string> coverPageDataList, string jsonString)
        {
            var jsonObject = JsonConvert.DeserializeObject<List<ReportingViewModel>>(jsonString);
            int count = jsonObject.Count();

            string sourceFolderAccountName = coverPageDataList[0].ToString();
            string targetFolderAccountName = coverPageDataList[1].ToString();
            string sourceFolderName = coverPageDataList[2].ToString();
            string targetFolderName = coverPageDataList[3].ToString();
            string sourceFolderVersion = coverPageDataList[4].ToString();
            string targetFolderVersion = coverPageDataList[5].ToString();

            if (string.IsNullOrWhiteSpace(sourceFolderAccountName) == true)
            {
                sourceFolderAccountName = sourceFolderName;
            }
            if (string.IsNullOrWhiteSpace(targetFolderAccountName) == true)
            {
                targetFolderAccountName = targetFolderName;
            }

            DataTable coverPageTable = new DataTable();
            coverPageTable.Columns.Add("Types", typeof(string));
            coverPageTable.Columns.Add("SourceForm", typeof(string));
            coverPageTable.Columns.Add("TargetForm", typeof(string));

            coverPageTable.Rows.Add("Selected Accounts", sourceFolderAccountName, targetFolderAccountName);
            coverPageTable.Rows.Add("Selected Folders", sourceFolderName, targetFolderName);
            coverPageTable.Rows.Add("Selected Folder Versions", sourceFolderVersion, targetFolderVersion);

            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    coverPageTable.Rows.Add("Selected Documents compared", jsonObject[i].SourceInstanceName, jsonObject[i].TargetInstanceName);
                }
                else
                {
                    coverPageTable.Rows.Add("", jsonObject[i].SourceInstanceName, jsonObject[i].TargetInstanceName);
                }
            }

            return coverPageTable;
        }
        private string GenerateSheetName(string SourceInstanceName, string TargetInstanceName)
        {
            int lenght = SourceInstanceName.Length + TargetInstanceName.Length;
            string sheetName = String.Empty;

            try
            {
                if (SourceInstanceName.Length >= 13 && TargetInstanceName.Length >= 13)
                {
                    sheetName = SourceInstanceName.ToString().Substring(0, 12) + " Vs " + TargetInstanceName.ToString().Substring(0, 12);
                }
                else if (SourceInstanceName.Length >= 13 && TargetInstanceName.Length <= 13)
                {
                    if (lenght > 26)
                    {
                        sheetName = SourceInstanceName.ToString().Substring(0, (SourceInstanceName.Length - (lenght - 26))) + " Vs " + TargetInstanceName.ToString();
                    }
                    else
                    {
                        sheetName = SourceInstanceName.ToString() + " Vs " + TargetInstanceName.ToString();
                    }
                }
                else if (SourceInstanceName.Length <= 13 && TargetInstanceName.Length >= 13)
                {                    
                    if (lenght > 26)
                    {
                        sheetName = SourceInstanceName.ToString() + " Vs " + TargetInstanceName.ToString().Substring(0, (TargetInstanceName.Length - (lenght - 26)));
                    }
                    else
                    {
                        sheetName = SourceInstanceName.ToString() + " Vs " + TargetInstanceName.ToString();
                    }
                }
                else
                {
                    sheetName = SourceInstanceName.ToString() + " Vs " + TargetInstanceName.ToString();
                }
            }
            catch (Exception ex)
            {
                sheetName = SourceInstanceName.ToString() + " Vs " + TargetInstanceName.ToString();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);                
            }
            return sheetName;
        }
        #endregion Private Methods
    }

    public class FormInstanceCompareResult
    {
        public CompareResult Result { get; set; }
        public string SourceName { get; set; }
        public string TargetName { get; set; }
    }

    public class CompareResult
    {
        public List<CompareFieldResult> compareList { get; set; }
    }
    public class CompareFieldResult
    {
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string FieldType { get; set; }
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
    }
    internal class ChangeSummaryConstants
    {
        public const string ChangeSummaryReportExcelSheetName = "ChangeSummary";
        public const string ChangeSummaryReportName = "Change Summary Report";
        public const string ChangeLabel = "Change_Label";
        public const string NotApplicable = "Not Applicable";
        public const string NoChanges = "No Changes";

    }
}