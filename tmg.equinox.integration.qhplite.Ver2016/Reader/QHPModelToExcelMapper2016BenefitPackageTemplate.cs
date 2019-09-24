using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;
using NPOI.HSSF.UserModel;
using System.IO;
using tmg.equinox.integration.qhplite.Ver2016;
using NPOI.SS.Util;
using System.Globalization;
using System.Text.RegularExpressions;
using tmg.equinox.integration.qhplite.Ver2016.DocumentExporter.Mappings;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// This class Maps the Benefit Package Template Excel for 2016 Version
    /// </summary>
    public class QHPModelToExcelMapper2016BenefitPackageTemplate
    {
        #region Private Memebers
        private XSSFWorkbook _workbook;
        private ISheet _benefitPackageSheet;
        private ISheet _costShareVarianceSheet;
        private IFormulaEvaluator _formulaEvaluator;
        private IList<PlanBenefitPackage> _benefitPackageList;
        private string _folderPath;
        private string _saveFolderPath;
        private Guid _emptyWorkbookGuid;
        private string _finalWorkbookFilePath;
        private QhpToExcelMappingBuilder _mappingBuilder;
        private QhpCellFormattingBuilder _formatterMappingBuilder;
        private int? rowCount = null;
        private string columnName;
        private int columnIncrement;
        private string medicalAndDrugDeductiblesIntegrated;
        private string medicalAndDrugOutOfPocketIntegrated;
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public QHPModelToExcelMapper2016BenefitPackageTemplate(IList<PlanBenefitPackage> benefitPackageList, string folderPath, string saveFoldePath)
        {
            this._mappingBuilder = new QhpToExcelMappingBuilder();
            this._mappingBuilder.BuildMappings();

            this._formatterMappingBuilder = new QhpCellFormattingBuilder();
            this._formatterMappingBuilder.BuildFormatMappings();

            this._benefitPackageList = benefitPackageList;
            string emptyTemplatePath = GetEmptyTemplatePath(folderPath);

            IExcelFileReader fileReader = new ExcelFileReader();
            MemoryStream qhpStream = fileReader.GetExcelFile(emptyTemplatePath);
            this._workbook = new XSSFWorkbook(qhpStream);
            this._formulaEvaluator = new XSSFFormulaEvaluator(_workbook);

            _folderPath = folderPath;
            _saveFolderPath = saveFoldePath;
            _emptyWorkbookGuid = Guid.NewGuid();
            _finalWorkbookFilePath = (_saveFolderPath + _emptyWorkbookGuid.ToString() + ".xlsm");
        }
        #endregion Constructor

        #region Public Methods
        public Guid GetExcel()
        {
            try
            {
                for (int i = 0; i < _benefitPackageList.Count(); i++)
                {
                    ISheet benefitsSheet = _workbook.GetSheet("Benefits Package " + (i + 1).ToString());
                    if (benefitsSheet != null)
                    {
                        _benefitPackageSheet = benefitsSheet;
                    }
                    else
                    {
                        _benefitPackageSheet = _workbook.CreateSheet("Benefits Package " + (i + 1).ToString());
                    }

                    ISheet costSharingSheet = _workbook.GetSheet("Cost Share Variances " + (i + 1).ToString());
                    if (costSharingSheet != null)
                    {
                        _costShareVarianceSheet = costSharingSheet;
                    }
                    else
                    {
                        _costShareVarianceSheet = _workbook.CreateSheet("Cost Share Variances " + (i + 1).ToString());
                    }

                    MapBenefitPackageData(_benefitPackageList[i]);
                    _workbook.Add(_benefitPackageSheet);
                    _workbook.Add(_costShareVarianceSheet);
                }

                this.UpdateExcelFile();
            }
            catch (Exception ex)
            {
                throw;
            }
            return _emptyWorkbookGuid;
        }
        #endregion Public Methods

        #region Private Methods
        private string GetEmptyTemplatePath(string folderPath)
        {
            string fileName = folderPath + "\\";
            switch (_benefitPackageList.Count())
            {
                case 1:
                    fileName += "\\1 Benefit Package\\QHPExcelTemplate-1package.xlsm";
                    break;
                case 2:
                    fileName += "\\2 Benefit Package\\QHPExcelTemplate-2package.xlsm";
                    break;
                case 3:
                    fileName += "\\3 Benefit Package\\QHPExcelTemplate-3package.xlsm";
                    break;
                case 4:
                    fileName += "\\4 Benefit Package\\QHPExcelTemplate-4package.xlsm";
                    break;
                case 5:
                    fileName += "\\5 Benefit Package\\QHPExcelTemplate-5package.xlsm";
                    break;
                case 6:
                    fileName += "\\6 Benefit Package\\QHPExcelTemplate-6package.xlsm";
                    break;
                case 7:
                    fileName += "\\7 Benefit Package\\QHPExcelTemplate-7package.xlsm";
                    break;
                case 8:
                    fileName += "\\8 Benefit Package\\QHPExcelTemplate-8package.xlsm";
                    break;
                case 9:
                    fileName += "\\9 Benefit Package\\QHPExcelTemplate-9package.xlsm";
                    break;
                case 10:
                    fileName += "\\10 Benefit Package\\QHPExcelTemplate-10package.xlsm";
                    break;
                default:
                    fileName += "\\1 Benefit Package\\QHPExcelTemplate-1package.xlsm";
                    break;
            }

            return fileName;
        }

        private void UpdateExcelFile()
        {
            using (FileStream targetFileStream = new FileStream(_finalWorkbookFilePath, FileMode.OpenOrCreate))
            {
                _workbook.Write(targetFileStream);
            }
        }

        private void MapBenefitPackageData(PlanBenefitPackage planBenefitPackage)
        {
            if (planBenefitPackage != null)
            {
                rowCount = null;
                columnName = null;
                medicalAndDrugDeductiblesIntegrated = string.Empty;
                medicalAndDrugOutOfPocketIntegrated = string.Empty;
                columnIncrement = 0;
                RunCustomRules(planBenefitPackage);
                EmptyBenefitServiceColumnNames();
                SetProperties(planBenefitPackage, null, null, 0);
                RunCustomRules(planBenefitPackage);
            }
        }

        private void SetProperties(object objectToMap, QHPSetting parentQHPSetting, string parentName, int rowNumber)
        {
            if (objectToMap != null)
            {
                Type objectType = objectToMap.GetType();
                /* To Calulate Row count */
                if (rowCount == null)
                {
                    rowCount = -1;
                    columnIncrement = 0;
                }

                foreach (var prop in objectType.GetProperties())
                {
                    object[] attr = prop.GetCustomAttributes(typeof(QHPSetting), false);
                    if (attr.Length > 0)
                    {
                        /* If Condition satify only if new row started, here 'columnName' value is maintained through out the context 
                         * for each row and resets to null if new row satrted*/
                        /* TODO: values need to Match with Benefit Names*/
                        if (objectToMap.GetType().Name == "PlanCostSharingAttributes" &&
                                   ((PlanCostSharingAttributes)objectToMap).HIOSPlanIDComponentAndVariant != null
                                   && prop.Name == "CostSharingBenefitServices")
                        {
                            rowCount++;
                            columnName = null;
                        }

                        QHPSetting setting = (QHPSetting)attr[0];
                        if ((prop.PropertyType.IsPrimitive == true) || (prop.PropertyType.FullName == "System.String"))
                        {
                            string cellValue = (string)prop.GetValue(objectToMap);
                            string propertyName = prop.Name;
                            QhpToExcelMap map = _mappingBuilder.GetMap(prop.Name, parentName, setting.SheetType);
                            if (map != null)
                            {
                                var mapColumnName = map.ColumnName;

                                if (parentName == "CostSharingBenefitServices")
                                {
                                    SetStringValueForPlanBenefitDetails(rowCount.Value, cellValue, map);
                                }
                                else if (parentName == "DeductibleSubGroups")
                                {
                                    SetStringValueForDeductibleSubGroups(rowCount.Value, cellValue, map);
                                }
                                else
                                {
                                    if (map.IsHeader)
                                        SetStringValueToExcelCell(map, map.RowIndex, mapColumnName, cellValue);
                                    else
                                        SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                                }

                                if (map.DomainPropertyName == "MedicalAndDrugDeductiblesIntegrated")
                                {
                                    medicalAndDrugDeductiblesIntegrated = cellValue;
                                }
                                else if (map.DomainPropertyName == "MedicalAndDrugOutOfPocketIntegrated")
                                {
                                    medicalAndDrugOutOfPocketIntegrated = cellValue;
                                }

                                SetMedicalAndDrugSheetValues(parentName, rowNumber, cellValue, map, mapColumnName);

                            }
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
                                    SetProperties(item, setting, prop.Name, index);
                            }
                        }
                        else
                        {
                            object obj = prop.GetValue(objectToMap);
                            SetProperties(obj, parentQHPSetting, prop.Name, rowNumber);
                        }
                    }
                }
            }
        }

        private void SetMedicalAndDrugSheetValues(string parentName, int rowNumber, string cellValue, QhpToExcelMap map, string mapColumnName)
        {
            if (parentName == "MedicalEHBDeductible")
            {
                if (medicalAndDrugDeductiblesIntegrated == "Yes")
                {
                    SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                }
            }
            else if (parentName == "DrugEHBDeductible")
            {
                if (medicalAndDrugDeductiblesIntegrated == "Yes")
                {
                    SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                }
            }
            else if (parentName == "CombinedMedicalEHBDeductible")
            {
                if (medicalAndDrugDeductiblesIntegrated == "No")
                {
                    SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                }
            }
            else
            {
                if (parentName == "MaximumOutOfPocketForMedicalEHBBenefits")
                {
                    if (medicalAndDrugOutOfPocketIntegrated == "Yes")
                    {
                        SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                    }
                }

                else if (parentName == "MaximumOutOfPocketForDrugEHBBenefits")
                {
                    if (medicalAndDrugOutOfPocketIntegrated == "Yes")
                    {
                        SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                    }
                }

                else if (parentName == "MaximumOutOfPocketForMedicalAndDrugEHBBenefits")
                {
                    if (medicalAndDrugOutOfPocketIntegrated == "No")
                    {
                        SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
                    }
                }
            }
        }

        private void SetStringValueForDeductibleSubGroups(int rowNumber, string cellValue, QhpToExcelMap map)
        {
            rowNumber += 1;
            if (map.IsHeader)
            {
                SetStringValueToExcelCell(map, map.RowIndex, map.ColumnName, cellValue);
            }
            else
            {
                SetStringValueToExcelCell(map, map.RowIndex + rowNumber, map.ColumnName, cellValue);
            }
        }

        private void SetStringValueForPlanBenefitDetails(int rowNumber, string cellValue, QhpToExcelMap map)
        {
            var mapColumnName = map.ColumnName;
            /* For every row when 'DJ' ColumnName found, If Condition satifies */
            /* and here on columnName handles every column for each row */
            if (map.DomainPropertyName != "ServiceName")
            {
                if (columnName == null)
                {
                    columnName = mapColumnName;
                }
                else
                {
                    int columnIndex = ConvertColumnAlphaToIndex(columnName);
                    columnIndex = columnIndex + 1;
                    columnName = ConvertColumnIndexToName(columnIndex);

                }
                mapColumnName = columnName;

                SetStringValueToExcelCell(map, map.RowIndex + rowNumber, mapColumnName, cellValue);
            }
            else if (map.RowIndex == 1 && rowNumber == 0)
            {
                int columnIndex = ConvertColumnAlphaToIndex(map.ColumnName);
                columnIndex = columnIndex + (map.IncrementStep * columnIncrement);
                mapColumnName = ConvertColumnIndexToName(columnIndex);
                columnIncrement++;

                SetStringValueToExcelCell(map, map.RowIndex, mapColumnName, cellValue);
            }
        }

        private void SetStringValueToExcelCell(QhpToExcelMap map, int rowIndex, string column, string value)
        {
            int columnIndex = ConvertColumnAlphaToIndex(column);

            ISheet sheet = null;
            if (map.QhpSheetType == QHPSheetType.BenefitPackage)
            {
                sheet = _benefitPackageSheet;
            }
            else
            {
                sheet = _costShareVarianceSheet;
            }
            if (sheet.Workbook.GetSheetAt(sheet.Workbook.ActiveSheetIndex).IsColumnHidden(columnIndex))
            {
                sheet.Workbook.GetSheetAt(sheet.Workbook.ActiveSheetIndex).SetColumnHidden(columnIndex, false);
                sheet.Workbook.GetSheetAt(sheet.Workbook.ActiveSheetIndex).SetColumnWidth(columnIndex, 100);
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

                cell.CellStyle.IsLocked = false;
                if (map.FormPropertyName == "Benefits" && map.RowIndex == 61 && map.ColumnName == "A")
                {
                    cell.CellStyle.Alignment = HorizontalAlignment.Center;
                }
                else
                {
                    cell.CellStyle.Alignment = HorizontalAlignment.General;
                }
                cell.CellStyle.VerticalAlignment = VerticalAlignment.Justify;
                QhpToExcelMap excelMap = _formatterMappingBuilder.GetMap(map.DomainPropertyName);
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        double numericValue;
                        if (Double.TryParse(value, out numericValue))
                        {
                            cell.SetCellValue(numericValue);
                        }
                        break;
                    case CellType.String:
                    case CellType.Blank:
                        if (String.Compare(value, "true", true) == 0 || String.Compare(value, "false", true) == 0)
                        {
                            if (String.Compare(value, "true", true) == 0)
                            {
                                cell.SetCellValue("Yes");
                            }
                            else
                            {
                                cell.SetCellValue("No");
                            }
                        }
                        else
                        {
                            if (excelMap != null)
                            {
                                if (excelMap.CellFormat.DataType == QhpCellDataTypes.DECIMALTWOPLACED && value != "")
                                {
                                    cell.SetCellType(CellType.Numeric);
                                    value = value.Replace("$", "");
                                    Regex regex = new Regex(@"^(\$)\d*.\d{2}$");
                                    if (!regex.IsMatch(value))
                                        cell.SetCellValue(String.Format("{0:$0.00}", Convert.ToDecimal(value)));
                                }
                                else if (excelMap.CellFormat.DataType == QhpCellDataTypes.INTWITHCOMMA && value != "")
                                {
                                    value = value.Replace("$", "");
                                    Regex regex = new Regex(@"^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9])?$");
                                    if (!regex.IsMatch(value))
                                        cell.SetCellValue(String.Format("{0:$0,0}", Convert.ToInt32(value)));
                                }
                            }
                            else
                            {
                                cell.SetCellValue(value);
                            }
                        }
                        break;
                    case CellType.Boolean:
                        if (String.Compare(value, "true", true) == 0 || String.Compare(value, "yes", true) == 0)
                        {
                            cell.SetCellValue(true);
                        }
                        else
                        {
                            cell.SetCellValue(false);
                        }
                        break;
                    default:
                        cell.SetCellValue(value);
                        break;
                }
            }

            //Force formulas to update with new data we added
            sheet.ForceFormulaRecalculation = true;
        }

        private XSSFCell SetCellStyle(QhpToExcelMap map, XSSFCell cell)
        {
            XSSFCellStyle style = (XSSFCellStyle)_workbook.CreateCellStyle();
            if (!string.IsNullOrEmpty(map.CellFormat.CellAlignment))
            {
                style = SetCellAlignment(map.CellFormat.CellAlignment, style);
                cell.CellStyle = style;
            }
            return cell;
        }

        private XSSFCellStyle SetCellAlignment(string cellAlignment, XSSFCellStyle style)
        {
            switch (cellAlignment.ToLower())
            {
                case "right":
                    style.Alignment = HorizontalAlignment.Right;
                    break;
                case "left":
                    style.Alignment = HorizontalAlignment.Left;
                    break;
                case "center":
                    style.Alignment = HorizontalAlignment.Center;
                    break;
                default:
                    style.Alignment = HorizontalAlignment.Left;
                    break;
            }

            return style;
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

        private void RunCustomRules(PlanBenefitPackage planBenefitPackage)
        {
            RunMaximumOutOfPocketRules(planBenefitPackage);
            RunDeductibleRules(planBenefitPackage);
            RunMultipleTierNetworkRules(planBenefitPackage);
            RunRemoveExtraBenefitServicesRules(planBenefitPackage);
            RunRemoveExtraDeductibleSubGroupsRules(planBenefitPackage);
        }

        private void RunRemoveExtraDeductibleSubGroupsRules(PlanBenefitPackage planBenefitPackage)
        {
            QhpToExcelMap deductibleMap = _mappingBuilder.GetDeductibleSubGroupStartingCell();

            ISheet costShareSheet = _costShareVarianceSheet;
            IRow row = costShareSheet.GetRow(0);
            if (row != null)
            {
                XSSFCell cellBV = row.GetCell(ConvertColumnAlphaToIndex("BZ")) as XSSFCell;
                if (cellBV != null)
                {
                    if (cellBV.StringCellValue.Contains("custom 1") || cellBV.StringCellValue.Contains("HSA/HRA"))
                    {
                        HideColumns(ref costShareSheet, "BZ", deductibleMap.IncrementStep, 0);
                    }
                }

                XSSFCell cellCD = row.GetCell(ConvertColumnAlphaToIndex("CL")) as XSSFCell;
                if (cellCD != null)
                {
                    if (cellCD.StringCellValue.Contains("custom 1"))
                    {
                        HideColumns(ref costShareSheet, "CL", deductibleMap.IncrementStep, 0);
                    }
                }

                XSSFCell cellCL = row.GetCell(ConvertColumnAlphaToIndex("CT")) as XSSFCell;
                if (cellCL != null)
                {
                    if (cellCL.StringCellValue.Contains("custom 2"))
                    {
                        HideColumns(ref costShareSheet, "CT", deductibleMap.IncrementStep, 0);
                    }
                }

                XSSFCell cellCT = row.GetCell(ConvertColumnAlphaToIndex("DB")) as XSSFCell;
                if (cellCT != null)
                {
                    if (cellCT.StringCellValue.Contains("custom 3"))
                    {
                        HideColumns(ref costShareSheet, "DB", deductibleMap.IncrementStep, 0);
                    }
                }

                //XSSFCell cellDB = row.GetCell(ConvertColumnAlphaToIndex("DB")) as XSSFCell;
                //if (cellDB != null)
                //{
                //    if (cellDB.StringCellValue.Contains("custom 5"))
                //    {
                //        HideColumns(ref costShareSheet, "DB", deductibleMap.IncrementStep, 0);
                //    }
                //}
            }
        }

        private void RunRemoveExtraBenefitServicesRules(PlanBenefitPackage planBenefitPackage)
        {
            var coveredBenefitList = planBenefitPackage.Benefits
                                                        .Where(c => c.BenefitInformation.GeneralInformation.IsThisBenefitCovered == "Covered")
                                                        .Select(c => c.BenefitInformation.Benefit)
                                                        .ToArray();
            ISheet costShareSheet = _costShareVarianceSheet;
            IRow row = costShareSheet.GetRow(0);
            if (row != null)
            {
                int columnIndex = ConvertColumnAlphaToIndex("DJ");
                for (int j = columnIndex; j < ConvertColumnAlphaToIndex("ZZ"); j = j + 6)
                {
                    XSSFCell cell = row.GetCell(j) as XSSFCell;
                    if (cell != null)
                    {
                        if (!coveredBenefitList.Contains(cell.StringCellValue))
                        {
                            //6 is the number of columns covered by a Benefit Service
                            //0 is the row number on which columns we are supposed to hide
                            HideColumns(ref costShareSheet, ConvertColumnIndexToName(j), 6, 0);
                        }
                        else
                        {
                            ShowColumns(ref costShareSheet, ConvertColumnIndexToName(j), 6, 0);
                        }
                    }

                }
            }
        }

        private void RunMultipleTierNetworkRules(PlanBenefitPackage planBenefitPackage)
        {
            ISheet costShareSheet = _costShareVarianceSheet;

            var productStartRowCounterInTemplate = 4;
            var productEndRowCounterInTemplate = productStartRowCounterInTemplate + planBenefitPackage.PlanCostSharingAttributes.Count();

            var benefitServicesInNetworkColumnList = new List<string>();

            IRow benefitNetworRow = costShareSheet.GetRow(2); //2 is the third row in Benefit Template
            if (benefitNetworRow != null)
            {
                int startColumn = ConvertColumnAlphaToIndex("DJ");
                int endColumn = ConvertColumnAlphaToIndex("ZZ");

                for (int i = startColumn; i < endColumn; i++)
                {
                    XSSFCell inNetworkTier2Cell = benefitNetworRow.GetCell(i, MissingCellPolicy.RETURN_NULL_AND_BLANK) as XSSFCell;

                    if (inNetworkTier2Cell != null)
                    {
                        if (inNetworkTier2Cell.StringCellValue == "In Network (Tier 2)")
                        {
                            benefitServicesInNetworkColumnList.Add(ConvertColumnIndexToName(i));
                        }
                    }
                }

            }

            //for (productStartRowCounterInTemplate = 4; productStartRowCounterInTemplate < productEndRowCounterInTemplate; productStartRowCounterInTemplate++)
            {
                for (int k = 0; k < planBenefitPackage.PlanCostSharingAttributes.Count(); k++)
                {
                    var item = planBenefitPackage.PlanCostSharingAttributes[k];

                    string hasMultipleInNetworkTiers = item.MultipleInNetworkTiers;

                    #region Lock column values for Maximum Out of Pocket Medical & Drug Deductible
                    IRow oopmRow = costShareSheet.GetRow(productStartRowCounterInTemplate - 1);
                    if (oopmRow != null)
                    {
                        string[] columnArray = new string[] { "Z", "AA", "AH", "AI", "AP", "AQ", "AY", "AZ", "BA", "BI", "BJ", "BK", "BS", "BT", "BU" };
                        for (int i = 0; i < columnArray.Length; i++)
                        {
                            //if (hasMultipleInNetworkTiers.ToLower() == "yes")
                            //{
                            //    UnLockCell(ref costShareSheet, columnArray[i], 1, ref oopmRow);
                            //}
                            //else 
                            if (hasMultipleInNetworkTiers.ToLower() == "no")
                            {
                                LockCell(ref costShareSheet, columnArray[i], 1, ref oopmRow);
                            }
                        }
                    }
                    #endregion Lock column values for Maximum Out of Pocket Max

                    #region Lock column values for Deductible SubGroups
                    IRow dedRow = costShareSheet.GetRow(productStartRowCounterInTemplate - 1);
                    if (dedRow != null)
                    {
                        string[] columnArray = new string[] { "CN", "CO", "CV", "CW", "DD", "DE" };
                        for (int i = 0; i < columnArray.Length; i++)
                        {
                            if (hasMultipleInNetworkTiers.ToLower() == "yes")
                            {
                                UnLockCell(ref costShareSheet, columnArray[i], 1, ref dedRow);
                            }
                            else
                            {
                                if (!costShareSheet.Workbook.GetSheetAt(costShareSheet.Workbook.ActiveSheetIndex).IsColumnHidden(ConvertColumnAlphaToIndex(columnArray[i])))
                                {
                                    LockCell(ref costShareSheet, columnArray[i], 1, ref dedRow);
                                }
                            }
                        }
                    }
                    #endregion Lock column values for Deductible SubGroups

                    #region Lock Columns in Benefit Services
                    IRow benefitRow = costShareSheet.GetRow(productStartRowCounterInTemplate - 1);
                    if (benefitRow != null)
                    {
                        //get the first column for services DK
                        //get the column label for 3rd ie(2 for NPOI) 
                        //check if column label is "In Network (Tier 2)"
                        //if true lock the column
                        //else unlock the column
                        foreach (var column in benefitServicesInNetworkColumnList)
                        {
                            if (hasMultipleInNetworkTiers.ToLower() == "yes")
                            {
                                UnLockCell(ref costShareSheet, column, 1, ref benefitRow);
                            }
                            else
                            {
                                if (!costShareSheet.Workbook.GetSheetAt(costShareSheet.Workbook.ActiveSheetIndex).IsColumnHidden(ConvertColumnAlphaToIndex(column)))
                                {
                                    LockCell(ref costShareSheet, column, 1, ref benefitRow);
                                }
                            }
                        }

                    }
                    #endregion Lock Columns in Benefit Services

                    #region Lock Second Tier Utilization Column
                    IRow secondTierUtilizationRow = costShareSheet.GetRow(productStartRowCounterInTemplate - 1);
                    if (secondTierUtilizationRow != null)
                    {
                        string[] columnArray = new string[] { "K" };
                        for (int i = 0; i < columnArray.Length; i++)
                        {
                            if (hasMultipleInNetworkTiers.ToLower() == "yes")
                            {
                                UnLockCell(ref costShareSheet, columnArray[i], 1, ref secondTierUtilizationRow);
                            }
                            else
                            {
                                LockCell(ref costShareSheet, columnArray[i], 1, ref secondTierUtilizationRow);
                            }
                        }
                    }
                    #endregion Lock Second Tier Utilization Column

                    productStartRowCounterInTemplate++;
                }
            }
        }

        private void RunDeductibleRules(PlanBenefitPackage planBenefitPackage)
        {
            ISheet costShareSheet = _costShareVarianceSheet;
            IRow headerRow = costShareSheet.GetRow(0);
            XSSFCell chiropracticCell = headerRow.GetCell(ConvertColumnAlphaToIndex("BZ"), MissingCellPolicy.RETURN_NULL_AND_BLANK) as XSSFCell;

            //if (chiropracticCell != null)
            //{
            //    chiropracticCell.SetCellValue("custom 1");
            //}

            var productStartRowCounterInTemplate = 4;
            var productEndRowCounterInTemplate = productStartRowCounterInTemplate + planBenefitPackage.PlanCostSharingAttributes.Count();

            for (productStartRowCounterInTemplate = 4; productStartRowCounterInTemplate < productEndRowCounterInTemplate; productStartRowCounterInTemplate++)
            {
                // Column G is for Medical & Drug Deductibles Integrated
                int columnIndex = ConvertColumnAlphaToIndex("G");
                IRow row = costShareSheet.GetRow(productStartRowCounterInTemplate - 1);
                if (row != null)
                {
                    XSSFCell cell = row.GetCell(columnIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK) as XSSFCell;

                    if (cell != null)
                    {
                        if (cell.StringCellValue.ToLower() == "yes")
                        {
                            //Hide "Maximum Out of Pocket for Medical EHB Benefits" and "Maximum Out of Pocket for Drug EHB Benefits"
                            //Column Name T for "Maximum Out of Pocket for Medical EHB Benefits" 
                            //ends after 8 columns
                            LockCell(ref costShareSheet, "AV", 10, ref row);
                            //Column Name AB for "Maximum Out of Pocket for Drug EHB Benefits" 
                            //ends after 8 columns
                            LockCell(ref costShareSheet, "BF", 10, ref row);
                            //unlock cell for Combined Medical and Drug EHB Deductible									
                            UnLockCell(ref costShareSheet, "BP", 10, ref row);
                        }
                        else if (cell.StringCellValue.ToLower() == "no")
                        {
                            //Show "Maximum Out of Pocket for Medical EHB Benefits" and "Maximum Out of Pocket for Drug EHB Benefits"
                            //Column Name T for "Maximum Out of Pocket for Medical EHB Benefits" 
                            //ends after 8 columns
                            UnLockCell(ref costShareSheet, "AV", 10, ref row);
                            //Column Name AB for "Maximum Out of Pocket for Drug EHB Benefits" 
                            //ends after 8 columns
                            UnLockCell(ref costShareSheet, "BF", 10, ref row);

                            //lock cell for Combined Medical and Drug EHB Deductible									
                            LockCell(ref costShareSheet, "BP", 10, ref row);
                        }
                    }
                }
            }
        }

        private void RunMaximumOutOfPocketRules(PlanBenefitPackage planBenefitPackage)
        {
            var productStartRowCounterInTemplate = 4;
            var productEndRowCounterInTemplate = productStartRowCounterInTemplate + planBenefitPackage.PlanCostSharingAttributes.Count();

            ISheet costShareSheet = _costShareVarianceSheet;
            for (productStartRowCounterInTemplate = 4; productStartRowCounterInTemplate < productEndRowCounterInTemplate; productStartRowCounterInTemplate++)
            {
                // Column H for Medical & Drug Maximum Out of Pocket Integrated?*
                int columnIndex = ConvertColumnAlphaToIndex("H");
                IRow row = costShareSheet.GetRow(productStartRowCounterInTemplate - 1);
                if (row != null)
                {
                    XSSFCell cell = row.GetCell(columnIndex) as XSSFCell;

                    if (cell != null)
                    {
                        if (cell.StringCellValue.ToLower() == "yes")
                        {
                            //Hide "Maximum Out of Pocket for Medical EHB Benefits" and "Maximum Out of Pocket for Drug EHB Benefits"
                            //Column Name X for "Maximum Out of Pocket for Medical EHB Benefits" 
                            //ends after 8 columns
                            LockCell(ref costShareSheet, "X", 8, ref row);
                            //Column Name AB for "Maximum Out of Pocket for Drug EHB Benefits" 
                            //ends after 8 columns
                            LockCell(ref costShareSheet, "AF", 8, ref row);
                            //Unlock cell for Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)							
                            UnLockCell(ref costShareSheet, "AN", 8, ref row);
                        }
                        else if (cell.StringCellValue.ToLower() == "no")
                        {
                            //Show "Maximum Out of Pocket for Medical EHB Benefits" and "Maximum Out of Pocket for Drug EHB Benefits"
                            //Column Name T for "Maximum Out of Pocket for Medical EHB Benefits" 
                            //ends after 8 columns
                            UnLockCell(ref costShareSheet, "X", 8, ref row);
                            //Column Name AB for "Maximum Out of Pocket for Drug EHB Benefits" 
                            //ends after 8 columns
                            UnLockCell(ref costShareSheet, "AF", 8, ref row);
                            //lock cell for Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)							
                            LockCell(ref costShareSheet, "AN", 8, ref row);
                        }
                    }
                }
            }
        }

        private void EmptyBenefitServiceColumnNames()
        {
            ISheet costShareSheet = _costShareVarianceSheet;
            IRow row = costShareSheet.GetRow(0);
            if (row != null)
            {
                int columnIndex = ConvertColumnAlphaToIndex("DJ");
                for (int j = columnIndex; j < ConvertColumnAlphaToIndex("ZZ"); j = j + 6)
                {
                    XSSFCell cell = row.GetCell(j) as XSSFCell;
                    if (cell != null)
                    {
                        cell.SetCellValue(String.Empty);
                    }

                }
            }
        }

        private void HideColumns(ref ISheet sheet, string rangeStart, int incremenetStep, int rowNumber)
        {
            int start = ConvertColumnAlphaToIndex(rangeStart);

            for (int i = start; i < start + incremenetStep; i++)
            {
                sheet.SetColumnHidden(i, true);
            }
        }

        private void ShowColumns(ref ISheet sheet, string rangeStart, int incremenetStep, int rowNumber)
        {
            int start = ConvertColumnAlphaToIndex(rangeStart);

            for (int i = start; i < start + incremenetStep; i++)
            {
                sheet.SetColumnHidden(i, false);
            }
        }

        private void LockCell(ref ISheet sheet, string rangeStart, int incremenetStep, ref IRow row)
        {
            int startColumnIndex = ConvertColumnAlphaToIndex(rangeStart);
            int endIndex = startColumnIndex + incremenetStep;
            for (startColumnIndex = ConvertColumnAlphaToIndex(rangeStart); startColumnIndex < endIndex; startColumnIndex++)
            {
                XSSFCell cell = row.GetCell(startColumnIndex) as XSSFCell;
                if (cell != null)
                {
                    XSSFCellStyle style = (XSSFCellStyle)_workbook.CreateCellStyle();
                    XSSFColor color = new XSSFColor(System.Drawing.Color.Gray);
                    style.SetFillBackgroundColor(color);
                    style.FillPattern = FillPattern.SolidForeground;

                    style.SetFillForegroundColor(color);

                    XSSFFont font = new XSSFFont();
                    font.SetColor(color);
                    style.SetFont(font);
                    if (cell.CellStyle != style)
                    {
                        cell.CellStyle = style;
                    }
                }
            }
        }

        private void UnLockCell(ref ISheet sheet, string rangeStart, int incremenetStep, ref IRow row)
        {
            int startColumnIndex = ConvertColumnAlphaToIndex(rangeStart);
            int endIndex = startColumnIndex + incremenetStep;
            for (startColumnIndex = ConvertColumnAlphaToIndex(rangeStart); startColumnIndex < endIndex; startColumnIndex++)
            {
                XSSFCell cell = row.GetCell(startColumnIndex) as XSSFCell;
                if (cell != null)
                {
                    XSSFCellStyle style = (XSSFCellStyle)_workbook.CreateCellStyle();
                    cell.CellStyle = style;
                }
            }
        }
        #endregion Private Methods
    }
}
