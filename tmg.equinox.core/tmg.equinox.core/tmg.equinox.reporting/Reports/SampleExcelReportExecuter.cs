using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base.SQLDataAccess;
using tmg.equinox.reporting.Interface;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting
{
    //This needs to done for each report
    public class ExcelReportExecuter<QueueItem> : BaseExcelReportExecuter<QueueItem>
    {

        private static readonly Object thislock = new Object();

        public override BaseJobInfo SetJobInfo(DownloadFileInfo fileInfo, QueueItem item)
        {
            var reportQueuInfo =  item as ReportQueueInfo;
            reportQueuInfo.FileName = fileInfo.FileName;
            reportQueuInfo.FilePath = fileInfo.FilePath;
            return reportQueuInfo;
        }

        //Report Specific Excel Sheets data writing activity - Override base method
        public override void WriteInContainer(KeyValuePair<string, object> col, IResult result, bool isNewRow, int rowNo, int colNo, int indexTable)
        {
            //and assign values in cell : sheet created in below method
            //string path = result.FileName;
            var fileObj = (DownloadFileInfo)result;
            string fileName = fileObj.FileName;
            var fileinfo = new FileInfo(fileName);
            if (fileinfo.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(fileinfo))
                {
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    if (rowNo == 1) //first row
                    {
                        //ws.Cells[rowNo, colNo].Value = ws.Cells[rowNo, colNo].Value;
                        ws.Cells[2, colNo].Value = col.Value.ToString();
                        
                    }
                    else
                    {
                        rowNo++;
                        ws.Cells[rowNo, colNo].Value = col.Value.ToString();
                    }
                        p.Save();
                }
            }
        }

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
            return result;
        }

        public override void Dispose()
        {
           
        }

    }

}
