using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
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
            using(ExcelPackage package = new ExcelPackage())
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
            foreach (CompareResult result in documentCompareResult.Results) 
            {
                if (result is RepeaterCompareResult) 
                {
                    RepeaterCompareResult compareResult = (RepeaterCompareResult)result;
                    if (compareResult.IsRepeaterMissingInSource == false && compareResult.IsRepeaterMissingInTarget == false) 
                    {
                    ExcelWorksheet repeaterWorksheet = package.Workbook.Worksheets.Add(compareResult.RepeaterName + System.Guid.NewGuid().ToString());
                    RepeaterExcelReportGenerator generator = new RepeaterExcelReportGenerator(compareResult, repeaterWorksheet);
                    generator.GenerateReport();
                    }
                }
            }
        }


    }
}
