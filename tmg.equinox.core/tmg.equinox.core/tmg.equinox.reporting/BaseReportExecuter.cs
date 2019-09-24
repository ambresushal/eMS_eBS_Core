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

namespace tmg.equinox.reporting
{

    public class BaseReportExecuter<QueueItem> : IReportExecuter<QueueItem>
    {
     
       
        public BaseReportExecuter()
        {
            
        }

        public virtual string PrepareSQLStatement(ReportSetting reportSetting, QueueItem item)
        {
            //get the appropriate SP name and paramater based on ReportSetting
            return reportSetting.SQLstatement;
        }

        public virtual void WriteInContainer(KeyValuePair<string, object> row, IResult result, bool isNewRow, int rowNo, int colNo)
        {
            //and assign values in cell : sheet created in below method
        }

        public virtual IResult CreateContainerBasedOnTemplate(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            return new DownloadFileInfo(); 
        }

        public IResult CreateFile(ReportSetting reportSetting, ICollection<DataHolder> dt)
        {
            //write a common logic to create sheet based template  and reportSetting 
            string templateFilePath = reportSetting.ReportTemplatePath;

            string fileName = reportSetting.ReportName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

                //* copy templae in to new output path and rename the file name" example d:\download\Guid\reportName_date.xls"
            fileName = reportSetting.OutputPath + String.Format("{0}", fileName);                
           
            var downloadFileInfo = new DownloadFileInfo();
            downloadFileInfo.FileName = fileName;
            downloadFileInfo.FilePath = reportSetting.OutputPath;

            return downloadFileInfo; ;
        }

        public virtual void  UpdateResultInQueue(IResult result, QueueItem item)
        {
            //update the path and result
            Base.Model.DownloadFileInfo fileInfo = (Base.Model.DownloadFileInfo)result;
            var customQueue = new CustomQueueMangement();
            var baseInfo = SetJobInfo(fileInfo, item);
            baseInfo.Status = "Succeeded";
            var instanceCustomQueue = customQueue.CreateInstanceCustomQueue(baseInfo);
            customQueue.ExecuteCustomQueueMethod(instanceCustomQueue, baseInfo, "UpdateQueue");
        }

        public virtual BaseJobInfo SetJobInfo(DownloadFileInfo fileInfo, QueueItem item)
        {
            return item as BaseJobInfo;              
        }

        public virtual void Dispose()
        {
            //sheet.Dispose();
            //package.Dispose();
        }
    }
}
