using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class CompareExcelReportGenerator
    {
        private DocumentCompareResult documentCompareResult;
        public CompareExcelReportGenerator(DocumentCompareResult compareResult)
        {
            this.documentCompareResult = compareResult;
        }

        public byte[] GenerateExcelReport()
        {
            MemoryStream fileStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                //generate report summary sheet
                ExcelWorksheet summaryWorksheet = package.Workbook.Worksheets.Add("Report Summary");
                SummaryExcelReportGenerator summaryGenerator = new SummaryExcelReportGenerator(documentCompareResult, summaryWorksheet);
                GenerateRepeaterReports(package);
                summaryGenerator.GenerateReport();
                package.SaveAs(fileStream);
                fileStream.Position = 0;
            }
            byte[] byteArray = new byte[fileStream.Length];
            fileStream.Position = 0;
            fileStream.Read(byteArray, 0, (int)fileStream.Length);
            return byteArray;
        }

        private void GenerateRepeaterReports(ExcelPackage package)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (CompareResult result in documentCompareResult.Results)
            {
                if (result is RepeaterCompareResult)
                {
                    RepeaterCompareResult compareResult = (RepeaterCompareResult)result;
                    if (compareResult.IsRepeaterMissingInSource == false && compareResult.IsRepeaterMissingInTarget == false)
                    {
                        //ExcelWorksheet repeaterWorksheet = package.Workbook.Worksheets.Add(compareResult.RepeaterName + System.Guid.NewGuid().ToString());
                        ExcelWorksheet repeaterWorksheet = null;
                        if (package.Workbook.Worksheets.Where(s => s.Name.Equals(compareResult.RepeaterName)).Any())
                        {
                            int value = 0;
                            if (!dict.ContainsKey(compareResult.RepeaterName))
                            {
                                dict.Add(compareResult.RepeaterName, 1);
                               int dictvalue = dict[compareResult.RepeaterName];
                                if (dictvalue == 1)
                                {
                                    value = dictvalue;
                                }
                            }
                            else
                            {
                                value = dict[compareResult.RepeaterName] + 1;
                                dict[compareResult.RepeaterName] = value;
                            }
                            string  tempRepeaterName = compareResult.RepeaterName + "(" + value + ")";
                            compareResult.RepeaterName = tempRepeaterName;
                            repeaterWorksheet = package.Workbook.Worksheets.Add(tempRepeaterName);
                        }
                        else
                        {
                            repeaterWorksheet = package.Workbook.Worksheets.Add(compareResult.RepeaterName);
                        }
                        RepeaterExcelReportGenerator generator = new RepeaterExcelReportGenerator(compareResult, repeaterWorksheet);
                        generator.GenerateReport();
                    }
                }
            }
        }


    }
}
