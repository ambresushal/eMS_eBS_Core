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
    public class PdpEocSBCopayGridFINALExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        public int ClassicTier1RecordsCount = 0;
        public int ClassicTier2RecordsCount = 0;
        public int ClassicTier3RecordsCount = 0;
        public int ClassicTier4RecordsCount = 0;
        public int ClassicTier5RecordsCount = 0;

        public int ExtraTier1RecordsCount = 0;
        public int ExtraTier2RecordsCount = 0;
        public int ExtraTier3RecordsCount = 0;
        public int ExtraTier4RecordsCount = 0;
        public int ExtraTier5RecordsCount = 0;

        public int ValueTier1RecordsCount = 0;
        public int ValueTier2RecordsCount = 0;
        public int ValueTier3RecordsCount = 0;
        public int ValueTier4RecordsCount = 0;
        public int ValueTier5RecordsCount = 0;

        public const int TierHeaderRows = 8;
        public const int Tier1StartPosition = 7;
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 7;
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

        public int GetRowsCount(ICollection<IDictionary<string, dynamic>> collection, string TierValue)
        {
            ICollection<IDictionary<string, dynamic>> Collection = new Collection<IDictionary<string, dynamic>>();
            foreach (var item in collection)
            {
                if (item["Tier"] == TierValue)
                {
                    Collection.Add(item);
                }
            }
            int rowCount = Collection.Count;
            if (rowCount == 0)
            {
                rowCount = 1;
            }
            return rowCount;
        }

        public void PrepareSheet1(ICollection<DataHolder> dt)
        {
            int InsertRowPosition = 0;
            ICollection<IDictionary<string, dynamic>> ClassicCollection = dt.Where(r => r.index == 0).FirstOrDefault().Data;

            ClassicTier1RecordsCount = GetRowsCount(ClassicCollection, "Tier 1");
            excelPackage.Workbook.Worksheets[1].InsertRow(Tier1StartPosition, ClassicTier1RecordsCount-1);
            var ClassicRange1 = excelPackage.Workbook.Worksheets[1].Cells[7, 1, ClassicTier1RecordsCount + 7-1, 12];
            AssignBorders(ClassicRange1);
            ApplyColours(excelPackage.Workbook.Worksheets[1], ClassicTier1RecordsCount, Tier1StartPosition);


            ClassicTier2RecordsCount = GetRowsCount(ClassicCollection, "Tier 2");
            InsertRowPosition = 15 + ClassicTier1RecordsCount-1;
            excelPackage.Workbook.Worksheets[1].InsertRow(InsertRowPosition, ClassicTier2RecordsCount);
            var ClassicRange2 = excelPackage.Workbook.Worksheets[1].Cells[InsertRowPosition, 1, InsertRowPosition + ClassicTier2RecordsCount-1, 12];
            AssignBorders(ClassicRange2);
            ApplyColours(excelPackage.Workbook.Worksheets[1], ClassicTier2RecordsCount, InsertRowPosition);

            ClassicTier3RecordsCount = GetRowsCount(ClassicCollection, "Tier 3");
            InsertRowPosition = 22 + ClassicTier1RecordsCount + ClassicTier2RecordsCount-1;
            excelPackage.Workbook.Worksheets[1].InsertRow(InsertRowPosition, ClassicTier3RecordsCount);
            var ClassicRange3 = excelPackage.Workbook.Worksheets[1].Cells[InsertRowPosition, 1, InsertRowPosition + ClassicTier3RecordsCount-1, 12];
            AssignBorders(ClassicRange3);
            ApplyColours(excelPackage.Workbook.Worksheets[1], ClassicTier3RecordsCount, InsertRowPosition);

            ClassicTier4RecordsCount = GetRowsCount(ClassicCollection, "Tier 4");
            InsertRowPosition = 29 + ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount-1;
            excelPackage.Workbook.Worksheets[1].InsertRow(InsertRowPosition, ClassicTier4RecordsCount);
            var ClassicRange4 = excelPackage.Workbook.Worksheets[1].Cells[InsertRowPosition, 1, InsertRowPosition + ClassicTier4RecordsCount-1, 10];
            AssignBorders(ClassicRange4);
            ApplyColours(excelPackage.Workbook.Worksheets[1], ClassicTier4RecordsCount, InsertRowPosition);

            ClassicTier5RecordsCount = GetRowsCount(ClassicCollection, "Tier 5");
            InsertRowPosition = 36 + ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount + ClassicTier4RecordsCount - 1;
            excelPackage.Workbook.Worksheets[1].InsertRow(InsertRowPosition, ClassicTier5RecordsCount);
            var ClassicRange5 = excelPackage.Workbook.Worksheets[1].Cells[InsertRowPosition, 1, InsertRowPosition + ClassicTier5RecordsCount-1, 8];
            AssignBorders(ClassicRange5);
            ApplyColours(excelPackage.Workbook.Worksheets[1], ClassicTier5RecordsCount, InsertRowPosition);
        }

        public void PrepareSheet2(ICollection<DataHolder> dt)
        {
            int InsertRowPosition = 0;
            ICollection<IDictionary<string, dynamic>> ExtraCollection = dt.Where(r => r.index == 1).FirstOrDefault().Data;
            ExtraTier1RecordsCount = GetRowsCount(ExtraCollection, "Tier 1");
            excelPackage.Workbook.Worksheets[2].InsertRow(Tier1StartPosition, ExtraTier1RecordsCount - 1);
            var ExtraRange1 = excelPackage.Workbook.Worksheets[2].Cells[7, 1, ExtraTier1RecordsCount + 7 - 1, 12];
            AssignBorders(ExtraRange1);
            ApplyColours(excelPackage.Workbook.Worksheets[2], ExtraTier1RecordsCount, Tier1StartPosition);

            ExtraTier2RecordsCount = GetRowsCount(ExtraCollection, "Tier 2");
            InsertRowPosition = 15 + ExtraTier1RecordsCount - 1;
            excelPackage.Workbook.Worksheets[2].InsertRow(InsertRowPosition, ExtraTier2RecordsCount);
            var ExtraRange2 = excelPackage.Workbook.Worksheets[2].Cells[InsertRowPosition, 1, InsertRowPosition + ExtraTier2RecordsCount - 1, 12];
            AssignBorders(ExtraRange2);
            ApplyColours(excelPackage.Workbook.Worksheets[2], ExtraTier2RecordsCount, InsertRowPosition);

            ExtraTier3RecordsCount = GetRowsCount(ExtraCollection, "Tier 3");
            InsertRowPosition = 22 + ExtraTier1RecordsCount + ExtraTier2RecordsCount - 1;
            excelPackage.Workbook.Worksheets[2].InsertRow(InsertRowPosition, ExtraTier3RecordsCount);
            var ExtraRange3 = excelPackage.Workbook.Worksheets[2].Cells[InsertRowPosition, 1, InsertRowPosition + ExtraTier3RecordsCount - 1, 12];
            AssignBorders(ExtraRange3);
            ApplyColours(excelPackage.Workbook.Worksheets[2], ExtraTier3RecordsCount, InsertRowPosition);

            ExtraTier4RecordsCount = GetRowsCount(ExtraCollection, "Tier 4");
            InsertRowPosition = 29 + ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount - 1;
            excelPackage.Workbook.Worksheets[2].InsertRow(InsertRowPosition, ExtraTier4RecordsCount);
            var ExtraRange4 = excelPackage.Workbook.Worksheets[2].Cells[InsertRowPosition, 1, InsertRowPosition + ExtraTier4RecordsCount - 1, 10];
            AssignBorders(ExtraRange4);
            ApplyColours(excelPackage.Workbook.Worksheets[2], ExtraTier4RecordsCount, InsertRowPosition);

            ExtraTier5RecordsCount = GetRowsCount(ExtraCollection, "Tier 5");
            InsertRowPosition = 36 + ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount + ExtraTier4RecordsCount - 1;
            excelPackage.Workbook.Worksheets[2].InsertRow(InsertRowPosition, ExtraTier5RecordsCount);
            var ExtraRange5 = excelPackage.Workbook.Worksheets[2].Cells[InsertRowPosition, 1, InsertRowPosition + ExtraTier5RecordsCount - 1, 8];
            AssignBorders(ExtraRange5);
            ApplyColours(excelPackage.Workbook.Worksheets[2], ExtraTier5RecordsCount, InsertRowPosition);
        }

        public void PrepareSheet3(ICollection<DataHolder> dt)
        {
            int InsertRowPosition = 0;
            ICollection<IDictionary<string, dynamic>> ValueScriptCollection = dt.Where(r => r.index == 2).FirstOrDefault().Data;
            ValueTier1RecordsCount = GetRowsCount(ValueScriptCollection, "Tier 1");
            excelPackage.Workbook.Worksheets[3].InsertRow(Tier1StartPosition, ValueTier1RecordsCount - 1);
            var ValueRange1 = excelPackage.Workbook.Worksheets[3].Cells[7, 1, ValueTier1RecordsCount + 7 - 1, 12];
            AssignBorders(ValueRange1);
            ApplyColours(excelPackage.Workbook.Worksheets[3], ValueTier1RecordsCount, Tier1StartPosition);

            ValueTier2RecordsCount = GetRowsCount(ValueScriptCollection, "Tier 2");
            InsertRowPosition = 15 + ValueTier1RecordsCount - 1;
            excelPackage.Workbook.Worksheets[3].InsertRow(InsertRowPosition, ValueTier2RecordsCount);
            var ValueRange2 = excelPackage.Workbook.Worksheets[3].Cells[InsertRowPosition, 1, InsertRowPosition + ValueTier2RecordsCount - 1, 12];
            AssignBorders(ValueRange2);
            ApplyColours(excelPackage.Workbook.Worksheets[3], ValueTier2RecordsCount, InsertRowPosition);

            ValueTier3RecordsCount = GetRowsCount(ValueScriptCollection, "Tier 3");
            InsertRowPosition = 22 + ValueTier1RecordsCount + ValueTier2RecordsCount - 1;
            excelPackage.Workbook.Worksheets[3].InsertRow(InsertRowPosition, ValueTier3RecordsCount);
            var ValueRange3 = excelPackage.Workbook.Worksheets[3].Cells[InsertRowPosition, 1, InsertRowPosition + ValueTier3RecordsCount - 1, 12];
            AssignBorders(ValueRange3);
            ApplyColours(excelPackage.Workbook.Worksheets[3], ValueTier3RecordsCount, InsertRowPosition);

            ValueTier4RecordsCount = GetRowsCount(ValueScriptCollection, "Tier 4");
            InsertRowPosition = 29 + ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount - 1;
            excelPackage.Workbook.Worksheets[3].InsertRow(InsertRowPosition, ValueTier4RecordsCount);
            var ValueRange4 = excelPackage.Workbook.Worksheets[3].Cells[InsertRowPosition, 1, InsertRowPosition + ValueTier4RecordsCount - 1, 10];
            AssignBorders(ValueRange4);
            ApplyColours(excelPackage.Workbook.Worksheets[3], ValueTier4RecordsCount, InsertRowPosition);

            ValueTier5RecordsCount = GetRowsCount(ValueScriptCollection, "Tier 5");
            InsertRowPosition = 36 + ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount + ValueTier4RecordsCount - 1;
            excelPackage.Workbook.Worksheets[3].InsertRow(InsertRowPosition, ValueTier5RecordsCount);
            var ValueRange5 = excelPackage.Workbook.Worksheets[3].Cells[InsertRowPosition, 1, InsertRowPosition + ValueTier5RecordsCount - 1, 8];
            AssignBorders(ValueRange5);
            ApplyColours(excelPackage.Workbook.Worksheets[3], ValueTier5RecordsCount, InsertRowPosition);
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

            //Classic Tier
            PrepareSheet1(dt);

            //Extra Sheet   
            PrepareSheet2(dt);

            //Value Script Sheet     
            PrepareSheet3(dt);

            return result;
        }

        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            //if (tableIndex > 0 && excelPackage.Workbook.Worksheets.Count == tableIndex)  //check if sheets exits = datatables.
            //{
            //    string sheetName = String.Format("Sheet{0}", tableIndex + 1);
            //    excelPackage.Workbook.Worksheets.Add(sheetName);
            //}
            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[tableIndex + 1];
            int ColumnNo = SetInitialColPostion();
            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;
            int RowNo = SetInitialRowPostion();
            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;
            int tierNo = 1;

            if (tableIndex == 0)    //Classic Sheet
            {                
                //Tier 2 Records
                if (rowNo > ClassicTier1RecordsCount && rowNo <= (ClassicTier1RecordsCount + ClassicTier2RecordsCount))
                {
                    RowNo = RowNo + 7;
                    tierNo = 2;
                }
                //Tier 3 
                else if (rowNo > (ClassicTier1RecordsCount + ClassicTier2RecordsCount) && rowNo <= (ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount))
                {
                    RowNo = RowNo + 14;
                    tierNo = 3;
                }
                //Tier 4
                else if (rowNo > (ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount) && rowNo <= (ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount + ClassicTier4RecordsCount))
                {
                    RowNo = RowNo + 21;
                    tierNo = 4;
                }
                //Tier 5
                else if (rowNo > (ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount + ClassicTier4RecordsCount) && rowNo <= (ClassicTier1RecordsCount + ClassicTier2RecordsCount + ClassicTier3RecordsCount + ClassicTier4RecordsCount + ClassicTier5RecordsCount))
                {
                    RowNo = RowNo + 28;
                    tierNo = 5;
                }

            }
            else if (tableIndex == 1) //Extra Sheet
            {
                //Tier 2 Records
                if (rowNo > ExtraTier1RecordsCount && rowNo <= (ExtraTier1RecordsCount + ExtraTier2RecordsCount))
                {
                    RowNo = RowNo + 7;
                    tierNo = 2;
                }
                //Tier 3 
                else if (rowNo > (ExtraTier1RecordsCount + ExtraTier2RecordsCount) && rowNo <= (ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount))
                {
                    RowNo = RowNo + 14;
                    tierNo = 3;
                }
                //Tier 4
                else if (rowNo > (ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount) && rowNo <= (ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount + ExtraTier4RecordsCount))
                {
                    RowNo = RowNo + 21;
                    tierNo = 4;
                }
                //Tier 5
                else if (rowNo > (ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount + ExtraTier4RecordsCount) && rowNo <= (ExtraTier1RecordsCount + ExtraTier2RecordsCount + ExtraTier3RecordsCount + ExtraTier4RecordsCount + ExtraTier5RecordsCount))
                {
                    RowNo = RowNo + 28;
                    tierNo = 5;
                }
            }
            else    //Value Script Sheet
            {
                //Tier 2 Records
                if (rowNo > ValueTier1RecordsCount && rowNo <= (ValueTier1RecordsCount + ValueTier2RecordsCount))
                {
                    RowNo = RowNo + 7;
                    tierNo = 2;
                }
                //Tier 3 
                else if (rowNo > (ValueTier1RecordsCount + ValueTier2RecordsCount) && rowNo <= (ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount))
                {
                    RowNo = RowNo + 14;
                    tierNo = 3;
                }
                //Tier 4
                else if (rowNo > (ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount) && rowNo <= (ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount + ValueTier4RecordsCount))
                {
                    RowNo = RowNo + 21;
                    tierNo = 4;
                }
                //Tier 5
                else if (rowNo > (ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount + ValueTier4RecordsCount) && rowNo <= (ValueTier1RecordsCount + ValueTier2RecordsCount + ValueTier3RecordsCount + ValueTier4RecordsCount + ValueTier5RecordsCount))
                {
                    RowNo = RowNo + 28;
                    tierNo = 5;
                }
            }          

            //Cell Value Writing
            if (ColumnNo <= 12)
            {
                ColumnNo = DeriveColumnNumber(tierNo, ColumnNo);
                if (ColumnNo > 0)
                {
                    ReportHelper reportHelper = new ReportHelper();
                    string heading = row.Key.ToString();
                    if (heading == "PreferredRetailcostsharing30daysupply" || heading == "StandardRetailandMailServicecostsharing30daysupplyinnetwork" || heading == "PreferredMailServicecostsharing30daysupply"
                        || heading == "OutofNetworkcostsharing30daysupply" || heading == "LongTermCareLTCcostsharing31daysupply" || heading == "PreferredRetailcostsharing90daysupply" || heading == "StandardRetailandMailServicecostsharinginnetwork90daysupply"
                        || heading == "PreferredMailServicecostsharing90daysupply" || heading == "Tier1PreferredGenericDrugs")
                    {
                        ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatNumberWithDecimals(row.Value.ToString());
                    }
                    else if (heading == "Deductible")
                    {
                        ws.Cells[RowNo, ColumnNo].Value = reportHelper.FormatStringWithDecimalsInFirstPlace(row.Value.ToString());
                    }
                    else
                    {
                        ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
                    }
                }
            }


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
