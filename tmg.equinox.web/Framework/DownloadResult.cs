using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.reporting.Base.Model;

namespace tmg.equinox.web.Framework
{
    public class DownloadResult : ActionResult
    {
        DownloadFileInfo FileInfo { get; set; }
        public DownloadResult()
        {
        }

        public DownloadResult(DownloadFileInfo fileInfo)
        {
            //fileInfo = Utility.CreateZipFile(fileInfo);
            this.FileInfo = fileInfo;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string fileName = Path.GetFileName(FileInfo.FileName);
            if (!String.IsNullOrEmpty(fileName))
            {
                context.HttpContext.Response.AddHeader("content-disposition",
               "attachment; filename=" + fileName);
            }

            context.HttpContext.Response.TransmitFile(Path.Combine(FileInfo.FilePath, fileName));
        }
    }
}