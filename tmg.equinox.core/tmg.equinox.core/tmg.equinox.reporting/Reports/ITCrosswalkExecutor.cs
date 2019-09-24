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
    public class ITCrosswalkExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        int LastRow = 0;
        //Default Excel Copy and Excel Open Activity
        public override IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            var result = (DownloadFileInfo)CreateFile(reportSetting, dt);

            string templateFilePath = reportSetting.ReportTemplatePath;

            string fileName = result.FileName;
            
            ICollection<IDictionary<string, dynamic>> ClassicCollection = dt.Where(r => r.index == 0).FirstOrDefault().Data;
            string year = "";
            LastRow = ClassicCollection.Count();
            if (ClassicCollection.Count>0)
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
                CurrentYear = DateTime.Now.Year.ToString();
                NextYear = ((DateTime.Now.Year) + 1).ToString();
            }
            else
            {
                NextYear = year;
                CurrentYear = (Convert.ToInt32(year) - 1).ToString();
            }

            Sheet1.Cells["A1"].Value = "Contract Number " + CurrentYear;
            Sheet1.Cells["B1"].Value = "Contract Number " + NextYear;
            Sheet1.Cells["F1"].Value = "Group Number " + CurrentYear;
            Sheet1.Cells["G1"].Value = "Group Number " + NextYear;
            Sheet1.Cells["H1"].Value = "H# " + CurrentYear;
            Sheet1.Cells["I1"].Value = "H# " + NextYear;
            Sheet1.Cells["J1"].Value = "Plan Code " + CurrentYear;
            Sheet1.Cells["K1"].Value = "Plan Code " + NextYear;
            Sheet1.Cells["L1"].Value = "PBP# " + CurrentYear;
            Sheet1.Cells["M1"].Value = "PBP# " + NextYear;
            Sheet1.Cells["N1"].Value = "County " + CurrentYear;
            Sheet1.Cells["O1"].Value = "County " + NextYear;
            Sheet1.Cells["P1"].Value = "Plan Name " + CurrentYear;
            Sheet1.Cells["Q1"].Value = "Plan Name " + NextYear;
            Sheet1.Cells["R1"].Value = "Rollover to in " + NextYear + ": (this column should have IT instructions in it and it's based on the Plan County level) Do not crosswalk to the HPMS report";
            Sheet1.Cells["S1"].Value = "Rollover to in " + NextYear + ": Product Comments at the Plan Level";
            Sheet1.Cells["U1"].Value = NextYear + " Name changes / MOOP changes* UPDATED";
            Sheet1.Cells["V1"].Value = "Rx Cvg "+ CurrentYear;
            Sheet1.Cells["W1"].Value = "Rx Cvg " + NextYear;
            Sheet1.Cells["X1"].Value = "RC1 " + CurrentYear;
            Sheet1.Cells["Y1"].Value = "RC1 " + NextYear;
            Sheet1.Cells["Z1"].Value = "RIDER 2 MTLC " + CurrentYear;
            Sheet1.Cells["AA1"].Value = "RIDER 2 MTLC " + NextYear;
            Sheet1.Cells["AB1"].Value = "RIDER 3 SNP " + CurrentYear;
            Sheet1.Cells["AC1"].Value = "RIDER 3 SNP " + NextYear;
            Sheet1.Cells["AE1"].Value = CurrentYear + " PBP Squish";
            Sheet1.Cells["AF1"].Value = "Crosswalk_" + CurrentYear + " County / States";
            Sheet1.Cells["AG1"].Value = "Crosswalk_" + NextYear + " County / States";
            Sheet1.Cells["AH1"].Value = NextYear + " PBP Squish";
            Sheet1.Cells["AI1"].Value = CurrentYear + " Plan Type / POS";
            Sheet1.Cells["AJ1"].Value =  CurrentYear + " MOOP";
            Sheet1.Cells["AK1"].Value = NextYear + " Plan Type / POS";
            Sheet1.Cells["AL1"].Value = NextYear + " MOOP INN";
            Sheet1.Cells["AM1"].Value = CurrentYear + " HMO/HMOPOS";
            Sheet1.Cells["AN1"].Value = NextYear + " HMO/HMOPOS";
            Sheet1.Cells["AO1"].Value = NextYear + " Renewal/Non-Renewal Option";
            Sheet1.Cells["AP1"].Value = NextYear + " OTC";
            Sheet1.Cells["AQ1"].Value = NextYear + " OTC Freq.";
            Sheet1.Cells["AR1"].Value = NextYear + " Card / Catalog";
            Sheet1.Cells["AS1"].Value = NextYear + " Postacute Meals";
            Sheet1.Cells["AT1"].Value = NextYear + " Chronic Meals";
            Sheet1.Cells["AU1"].Value = NextYear + " FIPS Code";
            Sheet1.Cells["AV1"].Value = CurrentYear + " LOB";
            Sheet1.Cells["AW1"].Value = NextYear + " LOB";
            Sheet1.Cells["AX1"].Value = NextYear + " SNP";
            Sheet1.Cells["AY1"].Value = NextYear + " State_1";
            Sheet1.Cells["AZ1"].Value = CurrentYear + " MSP";
            Sheet1.Cells["BA1"].Value = NextYear + " MSP";

            Sheet1.Row(1).Height = 60;
        }

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
            EPPlus epplus = new EPPlus();
            ReportHelper reportHelper = new ReportHelper();
            string heading = row.Key.ToString();
            if (ColumnNo < 61)
            {
                if (heading == "PreviousYearMOOP" || heading == "UpcomingYearMOOP")
                {
                    string MOOPValue = reportHelper.FormatStringWithDecimals(row.Value.ToString());
                    MOOPValue = reportHelper.CheckDecimalAndFormat(MOOPValue);
                    MOOPValue = MOOPValue.Replace("'", "");
                    MOOPValue = MOOPValue.Replace(" ", "");
                    ws.Cells[RowNo, ColumnNo].Value = MOOPValue;
                }
                else if (heading == "upcomingyearpostacutemeals" || heading == "upcomingyearchronicmeals")
                {
                    string mealValue = row.Value.ToString();
                    if (mealValue == "N/A" || mealValue == "")
                    {
                        mealValue = "NA";
                    }
                    else
                    {
                        mealValue = reportHelper.FormatNumberWithoutDecimals(mealValue);
                        mealValue = mealValue.Replace(".00", "");
                        mealValue = mealValue.Replace("$", "");
                    }
                    ws.Cells[RowNo, ColumnNo].Value = mealValue;
                }
                else if(heading == "upcomingyearOTCAmount")
                {
                    string OTCAmount = row.Value.ToString();
                    OTCAmount = reportHelper.FormatNumberWithoutDecimals(OTCAmount);
                    ws.Cells[RowNo, ColumnNo].Value = OTCAmount;
                }
                else
                {
                    ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
                }

                if (heading == "Rider2MTLCPrevYear" || heading == "Rider2MTLCUpcomingYear" || heading == "Rider3MTLCPrevYear" || heading == "Rider3MTLCUpcomingYear")
                {
                    ws.Cells[RowNo, ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[RowNo, ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                
                ws.Row(RowNo).Height = 30;
            }
            if (RowNo == LastRow)
            {
                ws.Cells[LastRow+2, ColumnNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[LastRow+2, ColumnNo].Style.Fill.BackgroundColor.SetColor(Color.Pink);
            }
            ExcelRange ExtraTable2Range = ws.Cells[rowNo+1, ColumnNo];
            epplus.AssignBorders(ExtraTable2Range);

        }

    }
}
