using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Base.Model;

namespace tmg.equinox.reporting.Base
{
    public class Utility
    {
        public static DownloadFileInfo CreateZipFile(DownloadFileInfo fileInfo)
        {
            //var zipFileName = String.Format("Attachment_{0}.zip", Guid.NewGuid().ToString());
            //var newPath = fileInfo.FilePath.Replace("temp", "download");
            //if (!Directory.Exists(newPath))
            //    Directory.CreateDirectory(newPath);
            //newPath = String.Format(@"{0}\\{1}", newPath, zipFileName);
            //System.IO.Compression.ZipFile.CreateFromDirectory(fileInfo.SourcePath, newPath);

            var zipFileName = String.Format("{0}.zip", fileInfo.FileName);
            var newPath = fileInfo.SourcePath.Replace("temp", "download");
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            newPath = String.Format(@"{0}\\{1}", newPath, zipFileName);
            System.IO.Compression.ZipFile.CreateFromDirectory(fileInfo.FilePath, newPath);

            return new DownloadFileInfo()
            {

                FileName = zipFileName,
                FilePath = newPath
            };
        }
    }
    /*
    public class DownloadResult : ActionResult
    {
        DownloadFileInfo _fileInfo;
        public DownloadResult()
        {
        }

        public DownloadResult(DownloadFileInfo fileInfo)
        {
            this._fileInfo = fileInfo;
        }



        public override void ExecuteResult(ControllerContext context)
        {
            if (!String.IsNullOrEmpty(_fileInfo.FileName))
            {
                context.HttpContext.Response.AddHeader("content-disposition",
               "attachment; filename=" + this._fileInfo.FileName);
            }
            context.HttpContext.Response.TransmitFile(_fileInfo.FilePath);
        }
    }
    */
}
