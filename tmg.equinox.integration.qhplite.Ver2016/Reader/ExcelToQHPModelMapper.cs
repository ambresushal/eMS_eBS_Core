using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;
using NPOI.HSSF.UserModel;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    internal class ExcelToQHPModelMapper
    {
        private IWorkbook _workbook;
        private ISheet _benefitPackageSheet;
        private ISheet _costShareVarianceSheet;
        private IFormulaEvaluator _formulaEvaluator;
        private HSSFWorkbook _hssfWorkbook;
        internal ExcelToQHPModelMapper(IWorkbook workbook, ISheet benefitPackageSheet, ISheet costShareVarianceSheet) 
        {
            this._workbook = workbook;
            this._benefitPackageSheet = benefitPackageSheet;
            this._costShareVarianceSheet = costShareVarianceSheet;
            this._formulaEvaluator = new XSSFFormulaEvaluator(workbook);
            this._hssfWorkbook = new HSSFWorkbook();
        }

        internal PlanBenefitPackage GetBenefitPackage() 
        {
            PlanBenefitPackage package = new PlanBenefitPackage();
            package.PackageName = _benefitPackageSheet.SheetName;
            SetProperties(package, null,0);
            return package;
        }

        private void SetProperties(object objectToMap, QHPSetting parentQHPSetting, int columnIncrement) 
        {
            
            Type objectType = objectToMap.GetType();
            foreach (var prop in objectType.GetProperties()) 
            {
                object[] attr = prop.GetCustomAttributes(typeof(QHPSetting),false);
                if (attr.Length > 0) 
                {
                    QHPSetting setting = (QHPSetting)attr[0];
                    if (setting.SheetType.ToString() == "CostShareVariance")
                    {
                        var d = "f";
                    }
                    if((prop.PropertyType.IsPrimitive == true) || (prop.PropertyType.FullName == "System.String"))
                    {
                        int row = setting.Row == 0 ? parentQHPSetting.Row : setting.Row;
                        string column = setting.Column == "" ? parentQHPSetting.Column : setting.Column;                        
                        if (columnIncrement > 0)
                        {
                            int columnIndex = ConvertColumnAlphaToIndex(column);
                            column = ConvertColumnIndexToName(columnIndex + columnIncrement);
                        }
                        string cellValue = GetStringValueFromExcelCell(setting.SheetType, row , column);
                        prop.SetValue(objectToMap, cellValue, null); 
                    }
                    else if (prop.PropertyType.IsGenericType == true && prop.PropertyType.FullName.Contains("System.Collections.Generic.List"))
                    {
                        Type listObjectType = prop.PropertyType.GetGenericArguments()[0];
                        Type genericConstruct = prop.PropertyType.GetGenericTypeDefinition().MakeGenericType(listObjectType);
                        object obj = Activator.CreateInstance(genericConstruct);
                        prop.SetValue(objectToMap,obj, null);
                        SetPropertiesForList(obj, listObjectType, setting, parentQHPSetting);
                    }
                    else
                    {
                        object obj = Activator.CreateInstance(prop.PropertyType);
                        prop.SetValue(objectToMap, obj, null);
                        SetProperties(obj, parentQHPSetting,columnIncrement);
                    }
                }
            }
        }

        private void SetPropertiesForList(object objectToMap, Type listObjectType, QHPSetting setting, QHPSetting parentQHPSetting)
        {
            switch (setting.IncrementDirection) 
            {
                case IncrementDirection.Row:
                    SetPropertiesForRowList(objectToMap, listObjectType, setting);
                    break;
                case IncrementDirection.Column:
                    SetPropertiesForColumnList(objectToMap, listObjectType, setting, parentQHPSetting);
                    break;
            }
        }
        /// <summary>
        /// Get List with each object represented as a row of attributes
        /// </summary>
        /// <param name="objectToMap"></param>
        /// <param name="listObjectType"></param>
        /// <param name="setting"></param>
        private void SetPropertiesForRowList(object objectToMap, Type listObjectType, QHPSetting setting)
        {
            int rowIndex = setting.Row;
            string cellValue = GetStringValueFromExcelCell(setting.SheetType, rowIndex, setting.Column);
            MethodInfo  method = objectToMap.GetType().GetMethod("Add");
            while (cellValue != "") 
            {
                object obj = Activator.CreateInstance(listObjectType);
                setting.Row = rowIndex;
                SetProperties(obj, setting, 0);
                method.Invoke(objectToMap, new object[] { obj});
                rowIndex++;
                cellValue = GetStringValueFromExcelCell(setting.SheetType, rowIndex , setting.Column);
            }
        }

        /// <summary>
        /// Get List with each object represented as a row of columns
        /// Inherit row from parent as this may be within a parent List
        /// Covers specific scenarios for QHP template only
        /// </summary>
        /// <param name="objectToMap"></param>
        /// <param name="listObjectType"></param>
        /// <param name="setting"></param>
        private void SetPropertiesForColumnList(object objectToMap, Type listObjectType, QHPSetting setting,QHPSetting parentQHPSetting)
        {
            int columnIndex = ConvertColumnAlphaToIndex(setting.Column);
            setting.Row = parentQHPSetting.Row;
            int increment = 0;
            int rowToCheckForMandatoryCell = setting.Row;
            string cellValue = GetStringValueFromExcelCell(setting.SheetType, setting.Row, setting.Column);
            MethodInfo method = objectToMap.GetType().GetMethod("Add");
            while (cellValue != "")
            {
                object obj = Activator.CreateInstance(listObjectType);
                //inherit parent row
                setting.Row = parentQHPSetting.Row;
                SetProperties(obj, setting, increment);
                method.Invoke(objectToMap, new object[] { obj });
                increment = increment + setting.IncrementStep;
                //increment column
                columnIndex = columnIndex + setting.IncrementStep;
                string columnName = ConvertColumnIndexToName(columnIndex);
                setting.Column = columnName;
                cellValue = GetStringValueFromExcelCell(setting.SheetType, rowToCheckForMandatoryCell, setting.Column);
            }            
        }

        private string GetStringValueFromExcelCell(QHPSheetType sheetType, int rowIndex, string column)
        { 
            string cellValue = "";
            int columnIndex = ConvertColumnAlphaToIndex(column);
            ISheet sheet = null;
            if (sheetType == QHPSheetType.BenefitPackage)
            {
                sheet = _benefitPackageSheet;
            }
            else 
            {
                sheet = _costShareVarianceSheet;
            }
            if (sheet != null)
            {
                IRow row = sheet.GetRow(rowIndex - 1);
                if (row != null)
                {
                    ICell cell = row.GetCell(columnIndex);

                    if (cell != null)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.String:
                                cellValue = cell.StringCellValue;
                                break;
                            case CellType.Boolean:
                                bool value = cell.BooleanCellValue;
                                cellValue = value == true ? "Yes" : "No";
                                break;
                            case CellType.Numeric:

                                if (DateUtil.IsCellDateFormatted(cell))
                                {
                                    cellValue = cell.DateCellValue.ToShortDateString();
                                }
                                else
                                {
                                    var format = _workbook.CreateDataFormat().GetFormat("0%");
                                    var decimalFormat = _workbook.CreateDataFormat().GetFormat("0.00%");
                                    if (cell.CellStyle.DataFormat == format || cell.CellStyle.DataFormat == decimalFormat)
                                    {
                                        cellValue = (cell.NumericCellValue * 100).ToString();
                                    }
                                    else
                                    {
                                        cellValue = cell.NumericCellValue.ToString();
                                    }
                                }
                                break;
                            case CellType.Formula:
                                CellValue formulaValue = _formulaEvaluator.Evaluate(cell);
                                cellValue = GetFormulaCellValue(formulaValue);
                                break;
                        }
                    }
                }
            }
            return cellValue;
        }

        private string GetFormulaCellValue(CellValue cell)
        {
            string cellValue = "";
            switch (cell.CellType)
            {
                case CellType.String:
                    cellValue = cell.StringValue;
                    break;
                case CellType.Boolean:
                    bool value = cell.BooleanValue;
                    cellValue = value == true ? "Yes" : "No";
                    break;
                case CellType.Numeric:
                    cellValue = cell.NumberValue.ToString();
                    break;
            }
            return cellValue;
        }


        private string GetIncrementedColumnName(string columnName, int increment)
        {
            int columnIndex = ConvertColumnAlphaToIndex(columnName);
            columnIndex = columnIndex + increment;
            string incrementedColumnName = ConvertColumnIndexToName(columnIndex);
            return incrementedColumnName;
        }

        private int ConvertColumnAlphaToIndex(string columnName)
        {
            int columnIndex = -1;
            int index = columnName.Length - 1;
            foreach (char c in columnName.ToUpper()) 
            {
                if(Char.IsLetter(c) == true)
                {
                    byte[]  bytes = Encoding.ASCII.GetBytes(new char[] {c});
                    int currentIndex = bytes[0];
                    int alphaNumber = currentIndex - 64;
                    columnIndex = columnIndex + (alphaNumber + (25 * index * alphaNumber));
                    index--;
                }
            }
            return columnIndex;
        }

        private string ConvertColumnIndexToName(int columnIndex) 
        {
            //only considering 2 characters max
            string columnName = "";
            int first = columnIndex / 26 - 1;
            int second = columnIndex % 26;
            if(first > 0)
            {
                
                columnName = new String(Encoding.ASCII.GetChars(new byte[]{ (byte)(first + 65), (byte)(second + 65)}));
            }
            else
            {
                columnName = new String(Encoding.ASCII.GetChars(new byte[] { (byte)(second + 65) }));
            }
            return columnName;
        }

    }
}
