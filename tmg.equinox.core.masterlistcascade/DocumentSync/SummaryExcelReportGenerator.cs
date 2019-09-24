using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.mlcascade.documentcomparer
{
    public class SummaryExcelReportGenerator
    {
        private DocumentCompareResult documentCompareResult;
        private ExcelWorksheet summarySheet;
        public SummaryExcelReportGenerator(DocumentCompareResult compareResult, ExcelWorksheet summarySheet)
        {
            this.documentCompareResult = compareResult;
            this.summarySheet = summarySheet;
        }

        public void GenerateReport()
        {
            GenerateSummarySheet();
        }

        private void GenerateSummarySheet()
        {
            GenerateSummaryHeader(summarySheet, documentCompareResult.SourceDocument, documentCompareResult.TargetDocument);
            GenerateSummaryReport(summarySheet);
            summarySheet.Cells.AutoFitColumns();
        }

        private void GenerateSummaryHeader(ExcelWorksheet sheet, CompareDocument sourceDocument, CompareDocument targetDocument)
        {
            //set header
            ExcelRange cell = sheet.Cells["B2"];
            cell.Value = "Document Compare Report";
            cell.Style.Font.Bold = true;

            //source and target document
            cell = sheet.Cells["C4"];
            cell.Value = "Source Document";
            cell.Style.Font.Bold = true;

            cell = sheet.Cells["D4"];
            cell.Value = "Target Document";
            cell.Style.Font.Bold = true;

            cell = sheet.Cells["B5:B9"];
            cell.Style.Font.Bold = true;
            cell = sheet.Cells["B5"];
            cell.Value = "Document Name";
            cell = sheet.Cells["B6"];
            cell.Value = "Folder Version Number";
            cell = sheet.Cells["B7"];
            cell.Value = "Effective Date";
            cell = sheet.Cells["B8"];
            cell.Value = "Folder Name";
            cell = sheet.Cells["B9"];
            cell.Value = "Account Name";

            //source document
            cell = sheet.Cells["C5:C9"];
            cell = sheet.Cells["C5"];
            cell.Value = sourceDocument.DocumentName;
            cell = sheet.Cells["C6"];
            cell.Value = sourceDocument.FolderVerionNumber;
            cell = sheet.Cells["C7"];
            cell.Value = sourceDocument.EffectiveDate;
            cell = sheet.Cells["C8"];
            cell.Value = sourceDocument.FolderName;
            cell = sheet.Cells["C9"];
            cell.Value = sourceDocument.AccountName;

            //target document
            cell = sheet.Cells["D5:D9"];
            cell = sheet.Cells["D5"];
            cell.Value = targetDocument.DocumentName;
            cell = sheet.Cells["D6"];
            cell.Value = targetDocument.FolderVerionNumber;
            cell = sheet.Cells["D7"];
            cell.Value = targetDocument.EffectiveDate;
            cell = sheet.Cells["D8"];
            cell.Value = targetDocument.FolderName;
            cell = sheet.Cells["D9"];
            cell.Value = targetDocument.AccountName;
        }

        private void GenerateSummaryReport(ExcelWorksheet sheet)
        {
            //set header
            ExcelRange cell = sheet.Cells["B12"];
            cell.Value = "Results";
            cell.Style.Font.Bold = true;

            cell = sheet.Cells["B13:G13"];
            cell.Style.Font.Bold = true;
            cell = sheet.Cells["B13"];
            cell.Value = "Main Section";
            cell = sheet.Cells["C13"];
            cell.Value = "Sub Sections";
            cell = sheet.Cells["D13"];
            cell.Value = "Field";
            cell = sheet.Cells["E13"];
            cell.Value = "Field Type";
            cell = sheet.Cells["F13"];
            cell.Value = "Source Document";
            cell = sheet.Cells["G13"];
            cell.Value = "Target Document";

            GenerateSummmaryResults(sheet);
        }

        private void GenerateSummmaryResults(ExcelWorksheet sheet)
        {
            int startRow = 14;
            foreach (CompareResult compareResult in documentCompareResult.Results)
            {
                GenerateSummaryResult(sheet, compareResult, ref startRow);
            }
        }

        private void GenerateSummaryResult(ExcelWorksheet sheet, CompareResult result, ref int row)
        {
            if (result is SectionCompareResult)
            {
                GenerateSectionResult(sheet, (SectionCompareResult)result, ref row);
            }
            else 
            {
                GenerateRepeaterResult(sheet, (RepeaterCompareResult)result, ref row);
            }
        }

        private void GenerateSectionResult(ExcelWorksheet sheet, SectionCompareResult sectionResult, ref int row)
        {
            ExcelRange cell = sheet.Cells["B" + row];
            cell.Value = sectionResult.RootSectionName;
            cell = sheet.Cells["C" + row];
            string subSections = "";
            for (int index = 0; index < sectionResult.ParentSections.Count; index++)
            {
                subSections = subSections + " " + sectionResult.ParentSections[index];
                if (index < sectionResult.ParentSections.Count - 1)
                {
                    subSections = subSections + " > ";
                }
            }
            cell.Value = subSections;
            if (sectionResult.IsSectionMissingInSource == true)
            {
                cell = sheet.Cells["F" + row];
                cell.Value = "Section Missing";
            }
            if (sectionResult.IsSectionMissingInTarget == true)
            {
                cell = sheet.Cells["G" + row];
                cell.Value = "Section Missing";
            }
            if (sectionResult.IsSectionMissingInSource == true && sectionResult.IsSectionMissingInTarget == true)
            {
                row = row + 1;
            }
            else
            {
                foreach (SectionCompareField field in sectionResult.Fields)
                {
                    cell = sheet.Cells["D" + row];
                    cell.Value = field.FieldName;
                    cell = sheet.Cells["F" + row];
                    if (field.IsMissingInSource || !field.IsMatch)
                    {
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    cell.Value = field.SourceValue;
                    cell = sheet.Cells["G" + row];

                    if (field.IsMissingInSource || !field.IsMatch)
                    {
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    cell.Value = field.TargetValue;
                    row = row + 1;
                }
            }
        }

        private void GenerateRepeaterResult(ExcelWorksheet sheet, RepeaterCompareResult repeaterResult, ref int row)
        {
            ExcelRange cell = sheet.Cells["B" + row];
            cell.Value = repeaterResult.RootSectionName;
            cell = sheet.Cells["C" + row];
            string subSections = "";
            for (int index = 0; index < repeaterResult.ParentSections.Count; index++)
            {
                subSections = subSections + " " + repeaterResult.ParentSections[index];
                if (index < repeaterResult.ParentSections.Count - 1)
                {
                    subSections = subSections + " > ";
                }
            }
            cell.Value = subSections;
            if (repeaterResult.IsRepeaterMissingInSource == true)
            {
                cell = sheet.Cells["F" + row];
                cell.Value = "Repeater Missing in Source";
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            if (repeaterResult.IsRepeaterMissingInTarget == true)
            {
                cell = sheet.Cells["G" + row];
                cell.Value = "Repeater Missing in Target";
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            if (repeaterResult.IsRepeaterMissingInSource == true || repeaterResult.IsRepeaterMissingInTarget == true)
            {
                row = row + 1;
            }
            else
            {
                cell = sheet.Cells["D" + row];
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                cell.Value = "Result in " + repeaterResult.RepeaterName + " sheet";
                row = row + 1;
            }
        }
    }
}
