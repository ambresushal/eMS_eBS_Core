using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Base.Interface;

namespace tmg.equinox.reporting.Base.Model
{
    public class DownloadFileInfo : IResult
    {
        public int queueId { get; set; }
        public int ReportId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string SourcePath { get; set; }
    }
}
