using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base.SQLDataAccess;
using tmg.equinox.reporting.Interface;
using System.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using tmg.equinox.reporting.Extensions;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Reports
{
    public class ANOCCampaignSOTDataForPlanInformationTransferExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        public override int SetInitialRowPostion()
        {
            return 2;
        }

        //Default Excel Copy and Excel Open Activity
        public override IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            var result = (DownloadFileInfo)CreateFile(reportSetting, dt);

            string templateFilePath = reportSetting.ReportTemplatePath;

            string fileName = result.FileName;

            ICollection<IDictionary<string, dynamic>> ClassicCollection = dt.Where(r => r.index == 0).FirstOrDefault().Data;

            string year = "";

            if (ClassicCollection.Count > 0)
                year = ClassicCollection.ElementAt(0)["year"];

            //* copy templae in to new output path and rename the file name" example d:\download\Guid\reportName_date.xls"            

            lock (thislock)
            {
                if (!Directory.Exists(reportSetting.OutputPath))
                {
                    Directory.CreateDirectory(reportSetting.OutputPath);
                }
                File.Copy(templateFilePath, fileName, true);
            }


            var fileinfo = new FileInfo(result.FileName);
            if (fileinfo.Exists)
            {
                excelPackage = new ExcelPackage(fileinfo);

            }

            UpdateYearsInExcelCoulmns(year);

            return result;
        }

        public void UpdateYearsInExcelCoulmns(string year)
        {
            ExcelWorksheet Sheet1 = excelPackage.Workbook.Worksheets[1];
            string CurrentYear = ""; string NextYear = "";
            if (string.IsNullOrEmpty(year) == true)
            {
                CurrentYear = ((DateTime.Now.Year) - 1).ToString();
                NextYear = DateTime.Now.Year.ToString();
            }
            else
            {
                NextYear = year;
                CurrentYear = (Convert.ToInt32(year) - 1).ToString();
            }

            Sheet1.Cells["B1"].Value = CurrentYear + " Plan Code";
            Sheet1.Cells["C1"].Value = NextYear + " Plan Code";
            Sheet1.Cells["D1"].Value = CurrentYear + " Plan Name";
            Sheet1.Cells["E1"].Value = NextYear + " Plan Name";
            Sheet1.Cells["W1"].Value = "Dental - " + CurrentYear;
            Sheet1.Cells["X1"].Value = "Dental - " + NextYear;
            Sheet1.Cells["Y1"].Value = "Dental Message - " + NextYear;
            Sheet1.Cells["Z1"].Value = "Dental Message - Plus " + NextYear + "(value YES / NO) ";
            Sheet1.Cells["AA1"].Value = "Vision - " + CurrentYear;
            Sheet1.Cells["AB1"].Value = "Vision - " + NextYear;
            Sheet1.Cells["AD1"].Value = "Hearing - " + CurrentYear;
            Sheet1.Cells["AE1"].Value = "Hearing - " + NextYear;
            Sheet1.Cells["AG1"].Value = "OTC - " + CurrentYear;
            Sheet1.Cells["AH1"].Value = "OTC - " + NextYear;
            Sheet1.Cells["AI1"].Value = "OTC Message-" + NextYear;
            Sheet1.Cells["AJ1"].Value = "OTC Message -Plus " + NextYear + "(Value = Yes / No)";
            Sheet1.Cells["AK1"].Value = "POS " + CurrentYear;
            Sheet1.Cells["AL1"].Value = "POS " + NextYear;
            Sheet1.Cells["AN1"].Value = "PART-B Deductible Amount - " + CurrentYear;
            Sheet1.Cells["AO1"].Value = "PART-B Deductible - " + CurrentYear;
            Sheet1.Cells["AP1"].Value = "PART-B Deductible Amount - " + NextYear;
            Sheet1.Cells["AQ1"].Value = "PART-B Deductible - " + NextYear;
            Sheet1.Cells["AU1"].Value = "Fitness " + CurrentYear;
            Sheet1.Cells["AV1"].Value = "Fitness " + NextYear;
            Sheet1.Cells["AX1"].Value = "Transportation " + CurrentYear;
            Sheet1.Cells["AY1"].Value = "Transportation " + NextYear;
        }

        public string AppendZeroToCents(string input)
        {
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            if (input != "0")
            {
                decimal n; bool isNumeric;
                if (input.Length > 0)
                {
                    isNumeric = decimal.TryParse(input, out n);
                    if (isNumeric)
                    {
                        if (n > 0 && n < 10)
                            output = input + "0";
                        else
                            output = input;
                    }
                    else
                    {
                        return input;
                    }
                }
            }
            else
            {
                return "0";
            }
            return output;
        }

        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            if (tableIndex > 0 && excelPackage.Workbook.Worksheets.Count == tableIndex)  //check if sheets exits = datatables.
            {
                string sheetName = String.Format("Sheet{0}", tableIndex + 1);
                excelPackage.Workbook.Worksheets.Add(sheetName);
            }
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;
            bool nflag = false;

            ReportHelper reportHelper = new ReportHelper();
            if (ColumnNo < 53)
            {
                if (reportHelper.CheckType(row.Value.ToString()) == "numeric")
                {
                    ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                    nflag = true;
                }

                string heading = row.Key.ToString().Trim();
                string Val = row.Value.ToString();
                if (heading == "SubsidyLevel0PremiumCentsMEMBER" || heading == "SubsidyLevel25PremiumCentsMEMBER" || heading == "SubsidyLevel50PremiumCents"
                    || heading == "SubsidyLevel75PremiumCents" || heading == "SubsidyLevel100PremiumCents")
                {
                    Val = AppendZeroToCents(Val);
                    if (nflag)
                        ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
                    else
                        ws.Cells[RowNo, ColumnNo].Value = Val;
                }
                else if (heading == "PlanCode2018" || heading == "PlanCode2019" || heading == "PlanNameSpeakonCallSCRIPTMAP")
                {
                    if (Val.StartsWith("'"))
                    {
                        ws.Cells[RowNo, ColumnNo].Style.QuotePrefix = true;
                    }
                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
                else if (heading.Contains("PARTBDeductibleAmount")) //PART-B Deductible Amount - 2018
                {
                    Val = Val.Replace(".00", "");
                    ws.Cells[RowNo, ColumnNo].Value = Val;
                }
                else
                {
                    if (ColumnNo >= 19 && ColumnNo <= 21)
                    {
                        Val = Val.Replace("$", "");
                        Val = Val.Replace("%", "");
                        Val = Val.Replace(".00", "");
                        if (reportHelper.CheckType(Val) == "numeric")
                        {
                            nflag = true;
                        }
                        if (nflag)
                        {
                            ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
                            ws.Cells[RowNo, ColumnNo].Style.Numberformat.Format = "0";
                        }
                        else
                            ws.Cells[RowNo, ColumnNo].Value = Val;
                    }
                    else
                    {
                        if (nflag)
                            ws.Cells[RowNo, ColumnNo].Value = Convert.ToInt32(Val);
                        else
                            ws.Cells[RowNo, ColumnNo].Value = Val;
                    }
                }

                if (ColumnNo >= 9)
                {
                    ws.Cells[RowNo, ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                var Range = ws.Cells[RowNo, ColumnNo, RowNo, ColumnNo];
                EPPlus epplus = new EPPlus();
                epplus.AssignBorders(Range);
            }
        }



    }
}
