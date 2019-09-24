using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class URRTModelToExcelMapper2016URRTTemplate
    {
        #region Private Members
        private PlanURRTemplate _planURRTTemplate;
        private string _saveFilePath;
        private XSSFWorkbook _workbook;
        private ISheet _marketExperienceSheet;
        private ISheet _planProductInfoSheet;
        private Guid _emptyWorkbookGuid;
        private int? rowCount = null;
        private string _finalWorkbookFilePath;
        private XSSFFormulaEvaluator _formulaEvaluator = null;
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public URRTModelToExcelMapper2016URRTTemplate(PlanURRTemplate _planURRTTemplate, string _saveFilePath)
        {
            this._planURRTTemplate = _planURRTTemplate;
            this._saveFilePath = _saveFilePath;

            string emptyTemplatePath = GetEmptyTemplatePath(_saveFilePath);

            IExcelFileReader fileReader = new ExcelFileReader();
            MemoryStream qhpStream = fileReader.GetExcelFile(emptyTemplatePath);
            this._workbook = new XSSFWorkbook(qhpStream);

            _emptyWorkbookGuid = Guid.NewGuid();
            _finalWorkbookFilePath = (_saveFilePath + _emptyWorkbookGuid.ToString() + ".xlsm");
            _formulaEvaluator = _workbook.GetCreationHelper().CreateFormulaEvaluator() as XSSFFormulaEvaluator;
        }
        #endregion Constructor

        #region Public Methods
        public Guid GetExcel()
        {
            try
            {
                ISheet marketExperienceSheet = _workbook.GetSheet("Wksh 1 - Market Experience");
                if (marketExperienceSheet != null)
                {
                    _marketExperienceSheet = marketExperienceSheet;
                }
                else
                {
                    _marketExperienceSheet = _workbook.CreateSheet("Wksh 1 - Market Experience");
                }

                ISheet planProductInfoSheet = _workbook.GetSheet("Wksh 2 - Plan Product Info");
                if (planProductInfoSheet != null)
                {
                    _planProductInfoSheet = planProductInfoSheet;
                }
                else
                {
                    _planProductInfoSheet = _workbook.CreateSheet("Wksh 2 - Plan Product Info");
                }

                MapURRTData();
                _workbook.Add(_marketExperienceSheet);
                _workbook.Add(_planProductInfoSheet);

                SaveExcelFile();
            }
            catch (Exception ex)
            {
                throw;
            }
            return _emptyWorkbookGuid;
        }
        #endregion Public Methods

        #region Private Members
        private string GetEmptyTemplatePath(string folderPath)
        {
            string fileName = folderPath + "\\";

            fileName += "\\Unified_Rate_Review_Template.xlsm";

            return fileName;
        }

        private void SaveExcelFile()
        {
            using (FileStream targetFileStream = new FileStream(_finalWorkbookFilePath, FileMode.OpenOrCreate))
            {
                _workbook.Write(targetFileStream);
            }

            string workBookFile = _saveFilePath + "//workbook.xml";

            using (ZipArchive zipFile = ZipFile.Open(_finalWorkbookFilePath, ZipArchiveMode.Update))
            {
                RemoveCDataFromSheet(zipFile, "xl/worksheets/sheet1.xml");
                RemoveCDataFromSheet(zipFile, "xl/worksheets/sheet2.xml");
                RemoveAmpersandWithSymbol(zipFile, "xl/worksheets/sheet1.xml");
                RemoveAmpersandWithSymbol(zipFile, "xl/worksheets/sheet2.xml");

                var workbookEntry = from ent in zipFile.Entries where ent.FullName == "xl/workbook.xml" select ent;
                if (workbookEntry != null && workbookEntry.Count() > 0)
                {
                    workbookEntry.First().Delete();
                    zipFile.CreateEntryFromFile(_saveFilePath + "//workbook.xml", "xl/workbook.xml");
                }
                var calcChainEntry = from ent in zipFile.Entries where ent.FullName == "xl/calcChain.xml" select ent;
                if (calcChainEntry != null && calcChainEntry.Count() > 0)
                {
                    calcChainEntry.First().Delete();
                    //zipFile.CreateEntryFromFile(_saveFilePath + "//calcChain.xml", "xl/calcChain.xml");
                }
                var sheet3Entry = from ent in zipFile.Entries where ent.FullName == "xl/worksheets/sheet3.xml" select ent;
                if (sheet3Entry != null && sheet3Entry.Count() > 0)
                {
                    sheet3Entry.First().Delete();
                    zipFile.CreateEntryFromFile(_saveFilePath + "//sheet3.xml", "xl/worksheets/sheet3.xml");
                }
                var sheet4Entry = from ent in zipFile.Entries where ent.FullName == "xl/worksheets/sheet4.xml" select ent;
                if (sheet4Entry != null && sheet4Entry.Count() > 0)
                {
                    sheet4Entry.First().Delete();
                    zipFile.CreateEntryFromFile(_saveFilePath + "//sheet4.xml", "xl/worksheets/sheet4.xml");
                }
            }
        }

        private void RemoveAmpersandWithSymbol(ZipArchive zipFile, string sheetName)
        {
            var entry = from ent in zipFile.Entries where ent.FullName == sheetName select ent;
            if (entry != null && entry.Count() > 0)
            {
                string textContent = "";
                using (StreamReader reader = new StreamReader(entry.First().Open()))
                {
                    textContent = reader.ReadToEnd();
                }
                textContent = textContent.Replace("&", "&amp;");
                entry.First().Delete();
                ZipArchiveEntry newEntry = zipFile.CreateEntry(sheetName);
                using (StreamWriter writer = new StreamWriter(newEntry.Open()))
                {
                    writer.Write(textContent);
                }
            }
        }

        private void RemoveCDataFromSheet(ZipArchive zipFile, string sheetName)
        {
            var entry = from ent in zipFile.Entries where ent.FullName == sheetName select ent;
            if (entry != null && entry.Count() > 0)
            {
                string textContent = "";
                using (StreamReader reader = new StreamReader(entry.First().Open()))
                {
                    textContent = reader.ReadToEnd();
                }
                textContent = textContent.Replace("<![CDATA[", "");
                textContent = textContent.Replace("]]>", "");
                entry.First().Delete();
                ZipArchiveEntry newEntry = zipFile.CreateEntry(sheetName);
                using (StreamWriter writer = new StreamWriter(newEntry.Open()))
                {
                    writer.Write(textContent);
                }
            }
        }

        private void MapURRTData()
        {
            SetProperties(_planURRTTemplate, null, 0);
            EvaluateExcelWorkBookForumla();
        }

        private void SetProperties(object objectToMap, URRTSetting parentQHPSetting, int columnIncrement)
        {
            if (objectToMap != null)
            {
                Type objectType = objectToMap.GetType();

                foreach (var prop in objectType.GetProperties())
                {
                    object[] attr = prop.GetCustomAttributes(typeof(URRTSetting), false);
                    if (attr.Length > 0)
                    {
                        URRTSetting setting = (URRTSetting)attr[0];
                        if ((prop.PropertyType.IsPrimitive == true) || (prop.PropertyType.FullName == "System.String"))
                        {
                            string cellValue = (string)prop.GetValue(objectToMap);
                            string propertyName = prop.Name;
                            int rowNumber = setting.Row == 0 ? parentQHPSetting.Row : setting.Row;
                            string column = setting.Column == "" ? parentQHPSetting.Column : setting.Column;
                            if (columnIncrement > 0)
                            {
                                int columnIndex = ConvertColumnAlphaToIndex(column);
                                setting.Column = ConvertColumnIndexToName(columnIndex + columnIncrement);
                            }
                            SetStringValueToExcelCell(setting, rowNumber, cellValue);
                        }
                        else if (prop.PropertyType.IsGenericType == true && prop.PropertyType.FullName.Contains("System.Collections.Generic.List"))
                        {
                            Type listObjectType = prop.PropertyType.GetGenericArguments()[0];
                            Type genericConstruct = prop.PropertyType.GetGenericTypeDefinition().MakeGenericType(listObjectType);
                            object value = prop.GetValue(objectToMap, null);
                            int listCount = (int)value.GetType().GetMethod("get_Count").Invoke(value, null);
                            // Loop though generic list
                            for (int index = 0; index < listCount; index++)
                            {
                                object item = value.GetType().GetMethod("get_Item").Invoke(value, new object[] { index });
                                if (item != null)
                                    if (index > 0 && setting.SheetType == URRTSheetType.PlanProductInfo)
                                        columnIncrement = columnIncrement + 1;
                                SetProperties(item, setting, columnIncrement);
                            }
                        }
                        else
                        {
                            object obj = prop.GetValue(objectToMap);
                            SetProperties(obj, parentQHPSetting, columnIncrement);
                        }
                    }
                }
            }
        }

        private void SetStringValueToExcelCell(URRTSetting map, int rowIndex, string cellValue, int columnIncreament = 0)
        {
            var value = !string.IsNullOrEmpty(cellValue) || !string.IsNullOrWhiteSpace(cellValue) ? cellValue as object : DBNull.Value;

            int columnIndex = (ConvertColumnAlphaToIndex(map.Column) + columnIncreament);

            ISheet sheet = null;
            if (map.SheetType == URRTSheetType.MarketExperience)
            {
                sheet = _marketExperienceSheet;
            }
            else if (map.SheetType == URRTSheetType.PlanProductInfo)
            {
                sheet = _planProductInfoSheet;
            }

            if (sheet != null)
            {
                if (sheet.Workbook.GetSheetAt(sheet.Workbook.ActiveSheetIndex).IsColumnHidden(columnIndex))
                {
                    sheet.Workbook.GetSheetAt(sheet.Workbook.ActiveSheetIndex).SetColumnHidden(columnIndex, false);
                }
                IRow row = sheet.GetRow(rowIndex - 1);
                if (row == null)
                {
                    row = sheet.CreateRow(rowIndex - 1);
                }
                if (row != null)
                {
                    XSSFCell cell = row.GetCell(columnIndex) as XSSFCell;

                    if (cell == null)
                        cell = row.GetCell(columnIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK) as XSSFCell;

                    //cell.CellStyle.IsLocked = false;
                    if (!cell.CellStyle.IsLocked && !cell.CellStyle.IsHidden)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Numeric:
                            case CellType.String:
                            case CellType.Blank:
                                SetValueToCell(cell, cellValue);
                                break;
                            case CellType.Formula:
                                //evaluate cell formula here
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (cell.CellType == CellType.Formula && cell.CellFormula != null)
                                _formulaEvaluator.EvaluateFormulaCell(cell);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                //Force formulas to update with new data we added
                //sheet.ForceFormulaRecalculation = true;
            }
        }

        private void SetValueToCell(XSSFCell cell, string cellValue)
        {
            if (DateUtil.IsCellDateFormatted(cell))
            {
                DateTime date;

                if (DateTime.TryParse(cellValue, out date))
                    cell.SetCellValue(date.ToShortDateString());
            }
            else
            {
                double numericValue;
                if (double.TryParse(cellValue, out numericValue))
                {
                    var format = _workbook.CreateDataFormat().GetFormat("0%");
                    var decimalFormat = _workbook.CreateDataFormat().GetFormat("0.00%");
                    if (cell.CellStyle.DataFormat == format || cell.CellStyle.DataFormat == decimalFormat)
                    {
                        cell.SetCellValue((numericValue * 100));
                    }
                    else
                    {
                        cell.SetCellValue(numericValue);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(cellValue) && !string.IsNullOrWhiteSpace(cellValue))
                        cell.SetCellValue(cellValue);
                }
            }
        }

        private int ConvertColumnAlphaToIndex(string columnName)
        {
            return CellReference.ConvertColStringToIndex(columnName);
        }

        private string ConvertColumnIndexToName(int columnIndex)
        {
            //only considering 2 characters max
            string columnName = "";
            int first = columnIndex / 26 - 1;
            int second = columnIndex % 26;
            if (first > 0)
            {

                columnName = new String(Encoding.ASCII.GetChars(new byte[] { (byte)(first + 65), (byte)(second + 65) }));
            }
            else
            {
                columnName = new String(Encoding.ASCII.GetChars(new byte[] { (byte)(second + 65) }));
            }
            return columnName;
        }

        private void EvaluateExcelWorkBookForumla()
        {
            if (_formulaEvaluator != null)
            {
                try
                {
                    _formulaEvaluator.EvaluateAll();
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion Private Members
    }
}

