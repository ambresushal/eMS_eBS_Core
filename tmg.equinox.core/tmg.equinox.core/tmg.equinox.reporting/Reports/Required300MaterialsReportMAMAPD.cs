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
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Reports
{
    public class Required300MaterialsReportMAMAPD<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 6;
        }

        public void AssignBorders(ExcelRange modelTable)
        {
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            modelTable.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        public int GetTotalMembershipCount(ICollection<IDictionary<string, dynamic>> collection, string ColumnName)
        {
            ICollection<IDictionary<string, dynamic>> Collection = new Collection<IDictionary<string, dynamic>>();
            foreach (var item in collection)
            {
                if(item.ContainsKey("Membership") && item["Membership"] != null && !string.IsNullOrEmpty(item["Membership"].ToString())
                && item["Membership"] == ColumnName)
                {
                    Collection.Add(item);
                }
            }
            int CellValueCount = Collection.Count;
            if (CellValueCount == 0)
            {
                CellValueCount = 0;
            }
            return CellValueCount;
        }

        public void PrepareSheetANOCTemplate(ICollection<DataHolder> dt, int index)
        {
            ICollection<IDictionary<string, dynamic>> ExtraCollection = dt.Where(r => r.index == index - 1).FirstOrDefault().Data;
            ExcelWorksheet SheetANOCTemplate = excelPackage.Workbook.Worksheets[index];
            string RunDate = "";
            RunDate = DateTime.Now.ToString("MM/dd/yyyy");
            SheetANOCTemplate.Cells["A2"].Value = "Last Updated: " + RunDate;
            SheetANOCTemplate.Cells["A3"].Value = "Membership as of: March 2019 Membership Bridge";
            //SheetANOCTemplate.Cells["L3"].Value = GetTotalMembershipCount(ExtraCollection, "Membership");
        }

        public void PrepareSheetEOCTemplate(ICollection<DataHolder> dt, int index)
        {
            ICollection<IDictionary<string, dynamic>> ExtraCollection = dt.Where(r => r.index == index - 1).FirstOrDefault().Data;
            ExcelWorksheet SheetEOCTemplate = excelPackage.Workbook.Worksheets[index];
            string RunDate = "";
            RunDate = DateTime.Now.ToString("MM/dd/yyyy");
            SheetEOCTemplate.Cells["A2"].Value = "Last Updated: " + RunDate;
            SheetEOCTemplate.Cells["A3"].Value = "Membership as of: March 2019 Membership Bridge";
            //SheetEOCTemplate.Cells["H3"].Value = GetTotalMembershipCount(ExtraCollection, "Membership");
        }

        public void PrepareSheetSARTemplate(ICollection<DataHolder> dt, int index)
        {
            ICollection<IDictionary<string, dynamic>> ExtraCollection = dt.Where(r => r.index == index - 1).FirstOrDefault().Data;
            ExcelWorksheet SARTemplate = excelPackage.Workbook.Worksheets[index];
            string RunDate = "";
            RunDate = DateTime.Now.ToString("MM/dd/yyyy");
            SARTemplate.Cells["A2"].Value = "Last Updated: " + RunDate;
            SARTemplate.Cells["A3"].Value = "Membership as of: March 2019 Membership Bridge";
            //SARTemplate.Cells["N3"].Value = GetTotalMembershipCount(ExtraCollection, "Membership");
        }

        public override IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            var result = (DownloadFileInfo)CreateFile(reportSetting, dt);
            string templateFilePath = reportSetting.ReportTemplatePath;
            string fileName = result.FileName;

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
            foreach(var item in excelPackage.Workbook.Worksheets)
            {
                switch(item.Name)
                {
                    case "ANOC Template":
                        PrepareSheetANOCTemplate(dt, item.Index);
                        break;
                    case "EOC Template":
                        PrepareSheetEOCTemplate(dt, item.Index);
                        break;
                    case "SAR Template":
                        PrepareSheetSARTemplate(dt, item.Index);
                        break;
                }
            }
            return result;
        }

        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;

            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            EPPlus epplus = new EPPlus();
            ExcelRange ExtraTable2Range = ws.Cells[RowNo, ColumnNo];
            epplus.AssignBorders(ExtraTable2Range);
        }

        private void ApplyColours(ExcelWorksheet ws, int RecordsCount, int StartPostion)
        {
            int ColorParts = RecordsCount / 4;
            int ModColor = RecordsCount % 4;
            int LastColorPart = ColorParts + ModColor;

            int Color1StartPossition = StartPostion;
            int Color1EndPossition = Color1StartPossition - 1 + ColorParts;
            if (Color1EndPossition < Color1StartPossition) Color1EndPossition = Color1StartPossition;

            int Color2StartPossition = Color1EndPossition + 1;
            int Color2EndPossition = Color1EndPossition + ColorParts;
            if (Color2EndPossition < Color2StartPossition) Color2EndPossition = Color2StartPossition;

            int Color3StartPossition = Color2EndPossition + 1;
            int Color3EndPossition = Color2EndPossition + ColorParts;
            if (Color3EndPossition < Color3StartPossition) Color3EndPossition = Color3StartPossition;

            int Color4StartPossition = Color3EndPossition + 1;
            int Color4EndPossition = Color3EndPossition + LastColorPart;
            if (Color4EndPossition < Color4StartPossition) Color4EndPossition = Color4StartPossition;

            switch (RecordsCount)
            {
                case 1:
                    {
                        ws.Cells[Color1StartPossition, 1, Color1EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color1StartPossition, 1, Color1EndPossition, 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                        break;
                    }
                case 2:
                    {
                        ws.Cells[Color1StartPossition, 1, Color1EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color1StartPossition, 1, Color1EndPossition, 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        ws.Cells[Color2StartPossition, 1, Color2EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color2StartPossition, 1, Color2EndPossition, 1].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);//#FCD5B4    //#E6B8B7
                        break;
                    }
                case 3:
                    {
                        Color colFromHexA = System.Drawing.ColorTranslator.FromHtml("#FCD5B4");
                        ws.Cells[Color3StartPossition, 1, Color3EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color3StartPossition, 1, Color3EndPossition, 1].Style.Fill.BackgroundColor.SetColor(colFromHexA);

                        break;
                    }
                default:
                    {
                        ws.Cells[Color1StartPossition, 1, Color1EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color1StartPossition, 1, Color1EndPossition, 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        ws.Cells[Color2StartPossition, 1, Color2EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color2StartPossition, 1, Color2EndPossition, 1].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);//#FCD5B4    //#E6B8B7

                        Color colFromHexA = System.Drawing.ColorTranslator.FromHtml("#FCD5B4");
                        ws.Cells[Color3StartPossition, 1, Color3EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color3StartPossition, 1, Color3EndPossition, 1].Style.Fill.BackgroundColor.SetColor(colFromHexA);

                        Color colFromHexB = System.Drawing.ColorTranslator.FromHtml("#E6B8B7");
                        ws.Cells[Color4StartPossition, 1, Color4EndPossition, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[Color4StartPossition, 1, Color4EndPossition, 1].Style.Fill.BackgroundColor.SetColor(colFromHexB);
                        break;
                    }

            }


        }


        private int DeriveColumnNumber(int tierNo, int columnNo)
        {
            int derivedColumnNo = columnNo;
            // tableNo 3 and 4 are for Tier 4 and 5
            if (tierNo == 4)
            {
                //if (columnNo == 6)  //"Standard Retail and Mail Servicecost - sharing(in-network)" SQL Column No
                //{
                //    derivedColumnNo = 5;    //Excel column no
                //}
                if (columnNo == 7)
                {
                    derivedColumnNo = 5;
                }
                if (columnNo == 8)
                {
                    derivedColumnNo = 7;
                }
                if (columnNo == 9)
                {
                    derivedColumnNo = 8;
                }

                //if (columnNo > 5 && columnNo < 10)
                //{
                //    derivedColumnNo = columnNo - 1;
                //}
                if (columnNo == 11)
                {
                    derivedColumnNo = 10;
                }
                if (columnNo == 12)
                {
                    derivedColumnNo = 9;
                }
            
  
            }

            if (tierNo == 5)
            {
                if (columnNo == 5 || columnNo > 9)
                {
                    derivedColumnNo = 0;
                }
               

                if (columnNo > 5 && columnNo < 10)
                {
                    derivedColumnNo = columnNo - 1;
                }
            }

            //return derived column no
            return derivedColumnNo;
        }
    }
}
