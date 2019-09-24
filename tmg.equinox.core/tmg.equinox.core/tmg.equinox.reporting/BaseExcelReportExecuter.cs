using System.Collections.Generic;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Interface;
using System;
using System.Data;
using tmg.equinox.reporting.Base.SQLDataAccess;
using OfficeOpenXml;
using System.IO;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting
{

    public class BaseExcelReportExecuter<QueueItem> : IReportExecuter<QueueItem>
    {
        protected static readonly Object thislock = new Object();
        private static readonly ILog _logger = LogProvider.For<BaseExcelReportExecuter<QueueItem>>();
        public ExcelPackage excelPackage;
        public BaseExcelReportExecuter()
        {
            
        }

        //Default starting row position in excel here.
        public virtual int SetInitialRowPostion()
        {
            return 2;
        }

        //Default starting col position in excel here.
        public virtual int SetInitialColPostion()
        {
            return 1;
        }

        public virtual string PrepareSQLStatement(ReportSetting reportSetting, QueueItem item)
        {
            int ReportQueueId = 1;
            if (item != null)
            {
                var reportQueuInfo = item as ReportQueueInfo;
                ReportQueueId = reportQueuInfo.QueueId;
            }
            
            //get the appropriate SP name and paramater based on ReportSetting
            return reportSetting.SQLstatement + " "+ ReportQueueId.ToString();
        }

        //Default Excel Copy and Excel Open Activity
        public virtual IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
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

            return result;
        }

        //Default Excel Sheets data writing activity
        public virtual void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo, int tableIndex)
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

            ws.Cells[RowNo, ColumnNo].Value = row.Value.ToString();
          

        }

        //Create file for downloading
        public IResult CreateFile(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            //write a common logic to create sheet based template  and reportSetting 
            string templateFilePath = reportSetting.ReportTemplatePath;
            string CurrentYear = DateTime.Now.Year.ToString();
            string NextYear = ((DateTime.Now.Year) + 1).ToString();
            string fileName = reportSetting.ReportName + "_" + DateTime.Now.ToString("MM/dd/yyyy  HH:mm") + ".xlsx";
            fileName = fileName.Replace("/", "");
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace("2017", NextYear);
            fileName = fileName.Replace("2018", CurrentYear);
            fileName = fileName.Replace("2019", NextYear);
            //* copy templae in to new output path and rename the file name" example d:\download\Guid\reportName_date.xls"
            fileName = reportSetting.OutputPath + String.Format("{0}", fileName);                
           
            var downloadFileInfo = new DownloadFileInfo();
            downloadFileInfo.FileName = fileName;
            downloadFileInfo.FilePath = reportSetting.OutputPath;

            return downloadFileInfo; 
        }


        public void AddtechincalLogInSheet(ICollection<DataHolder> logData)
        {
            try
            {
 
   
                foreach (var row in logData)
                {
                    string sheetName = "TechValidationLog";
                    excelPackage.Workbook.Worksheets.Add(sheetName);
                    ExcelWorksheet ws = excelPackage.Workbook.Worksheets[excelPackage.Workbook.Worksheets.Count ];

                    bool colCreated = false;

                    foreach (var table in row.Data)
                    {
                        int ColumnNo = 1;
                        int RowNo = 2;

                        foreach (var col in table)
                        {
                            if (colCreated == false)
                            {
                                ws.Cells[1, ColumnNo].Value = col.Key.ToString();
                                ws.Cells[1, ColumnNo].Style.ShrinkToFit = true;
                            }
                            
                            ws.Cells[RowNo, ColumnNo].Value = col.Value.ToString();
                            ColumnNo = ColumnNo + 1;
                        }
                        colCreated = true;
                        RowNo = RowNo + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error in AddTechnicalValidationLogToSheet", ex);
            }
        }
        //Save excel once data wrirting is finished & Update Queue .
        public virtual void  UpdateResultInQueue(IResult result, QueueItem item)
        {
            excelPackage.Save();
            //update the path and result
            Base.Model.DownloadFileInfo fileInfo = (Base.Model.DownloadFileInfo)result;
            var customQueue = new CustomQueueMangement();
            var baseInfo = SetJobInfo(fileInfo, item);
            //baseInfo.Status = "Succeeded";
            var instanceCustomQueue = customQueue.CreateInstanceCustomQueue(baseInfo);
            customQueue.ExecuteCustomQueueMethod(instanceCustomQueue, baseInfo, "UpdateQueue");
        }

        //Set file parameters in BaseJobInfo, which gets back to reporting center.
        public virtual BaseJobInfo SetJobInfo(DownloadFileInfo fileInfo, QueueItem item)
        {
            var reportQueuInfo = item as ReportQueueInfo;
            reportQueuInfo.FileName = fileInfo.FileName;
            reportQueuInfo.FilePath = fileInfo.FilePath;
            return reportQueuInfo;
        }

        //final excel dispose
        public virtual void Dispose()
        {
            if (excelPackage!=null)
                excelPackage.Dispose();
        }
    }
}
