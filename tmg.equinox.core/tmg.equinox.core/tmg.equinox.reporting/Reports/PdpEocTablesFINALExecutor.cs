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
    public class PdpEocTablesFinalExecutor<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {
        public int ClassicTable1Count = 0;
        public int ExtraTable1Count = 0;
        public int valueTable1Count = 0;
        //Set data starting row position in excel here.
        public override int SetInitialRowPostion()
        {
            return 5;
        }

        //Set data starting col position in excel here.
        public override int SetInitialColPostion()
        {
            return 2;
        }

        //Default Excel Copy and Excel Open Activity
        public override IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            var result = (DownloadFileInfo)CreateFile(reportSetting, dt);

            string templateFilePath = reportSetting.ReportTemplatePath;

            string fileName = result.FileName;

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

            //Classic
            PrepareTables(dt);
          

            return result;
        }

        public void AssignBorders(ExcelRange modelTable)
        {
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Medium;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Medium;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        }

        public void PrepareTables(ICollection<DataHolder> dt)
        {
            ExcelWorksheet Sheet1 = excelPackage.Workbook.Worksheets[1];
            int ClassicCount = 1, ExtraCount = 1, ValueCount = 1;
            EPPlus epplus = new EPPlus();
            ICollection<IDictionary<string, dynamic>> ClassicCollection = dt.Where(r => r.index == 0).FirstOrDefault().Data;
            ClassicCount = ClassicCollection.Count();
            ClassicTable1Count = ClassicCount / 2;
            int ClassicTable2Count = ClassicCount - ClassicTable1Count;
            if (ClassicTable1Count == 0) ClassicTable1Count = 1;
            ExcelRange ClassicTable1Range = Sheet1.Cells[5, 2, 5 + ClassicTable1Count - 1, 6];
            epplus.AssignBorders(ClassicTable1Range);
            if (ClassicTable2Count == 0) ClassicTable2Count = 1;
            ExcelRange ClassicTable2Range = Sheet1.Cells[5, 8, 5 + ClassicTable2Count - 1, 12];
            epplus.AssignBorders(ClassicTable2Range);


            ICollection<IDictionary<string, dynamic>> ExtraCollection = dt.Where(r => r.index == 1).FirstOrDefault().Data;
            ExtraCount = ExtraCollection.Count();
            ExtraTable1Count = ExtraCount / 2;
            int ExtraTable2Count = ExtraCount - ExtraTable1Count;
            if (ExtraTable1Count == 0) ExtraTable1Count = 1;
            ExcelRange ExtraTable1Range = Sheet1.Cells[5, 14, 5 + ExtraTable1Count - 1, 18];
            epplus.AssignBorders(ExtraTable1Range);
            if (ExtraTable2Count == 0) ExtraTable2Count = 1;
            ExcelRange ExtraTable2Range = Sheet1.Cells[5, 20, 5 + ExtraTable2Count - 1, 24];
            epplus.AssignBorders(ExtraTable2Range);


            ICollection<IDictionary<string, dynamic>> ValueCollection = dt.Where(r => r.index == 2).FirstOrDefault().Data;
            ValueCount = ValueCollection.Count();

            valueTable1Count = ExtraCount / 2;
            int valueTable2Count = ExtraCount - valueTable1Count;
            if (valueTable1Count == 0) valueTable1Count = 1;
            ExcelRange valueTable1Range = Sheet1.Cells[5, 26, 5 + valueTable1Count - 1, 30];
            epplus.AssignBorders(valueTable1Range);
            if (valueTable2Count == 0) valueTable2Count = 1;
            ExcelRange valueTable2Range = Sheet1.Cells[5, 32, 5 + valueTable2Count - 1, 36];
            epplus.AssignBorders(valueTable2Range);

            //if (ValueCount == 0) ValueCount = 1;
            //ExcelRange ValueRange = Sheet1.Cells[5, 26, 5 + ValueCount - 1, 30];
            //epplus.AssignBorders(ValueRange);
                    
        }



        //Default Excel Sheets data writing activity
        public override void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
        {
            int ColumnNo = 1;
            int RowNo = 1;
            if (tableIndex == 0)
            {
                ColumnNo = SetInitialColPostion();
                if (rowNo > ClassicTable1Count)
                {
                    ColumnNo = 8;
                    rowNo = rowNo - ClassicTable1Count;
                }
            }
            else if (tableIndex == 1)  //Resultset 2
            {
                ColumnNo = 14;
                if (rowNo > ExtraTable1Count)
                {
                    ColumnNo = 20;
                    rowNo = rowNo - ExtraTable1Count;
                }
            }
            else if (tableIndex == 2) //Resultset 3
            {
                ColumnNo = 26;

                if (rowNo > valueTable1Count) {
                    ColumnNo = 32;
                    rowNo = rowNo - valueTable1Count;
                }
            }
            RowNo = SetInitialRowPostion();


            ExcelWorksheet ws = excelPackage.Workbook.Worksheets[1];

            ColumnNo = ColumnNo + colNo;
            ColumnNo = ColumnNo - 1;

            RowNo = RowNo + rowNo;
            RowNo = RowNo - 1;
            ReportHelper reportHelper = new ReportHelper();
            string heading = row.Key.ToString();
            if (heading == "ClassicMonthlyPremium" || heading == "ClassicDeductible" || heading == "ExtraMonthlyPremium"
                || heading == "ExtraDeductible" || heading == "ValueMonthlyPremium" || heading == "ValueDeductible")
            {
                string Val = row.Value.ToString();
                Val = reportHelper.FormatNumberWithDecimals(Val);
                if (heading == "ClassicDeductible" || heading == "ExtraDeductible" || heading == "ValueDeductible")
                    Val = Val.Replace(".00", "");
                ws.Cells[RowNo, ColumnNo].Value = Val;
            }
            else
            {
                ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
            }

        }
    }
}
