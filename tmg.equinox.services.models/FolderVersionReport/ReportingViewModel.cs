using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.applicationservices.viewmodels.Reporting
{
   public class ReportingViewModel : ViewModelBase
    {
       
        public int FolderVersionId { get; set; }
        public int FolderId { get; set; }
        public string FolderVersionNumber { get; set; }
        public int TenantID { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string SourceForm { get; set; }
        public string TargetForm { get; set; }       
        public bool IsPortfolio { get; set; } 
        public int FormDesignId { get; set; }
        public int FormInstanceId { get; set; }
        public int FormDesignVersionId { get; set; }
        
        public string SourceInstanceId { get; set; }
        public string SourceInstanceName { get; set; }
        public string TargetInstanceId { get; set; }
        public string TargetInstanceName { get; set; }

        public string LabelName { get; set; }
        public string GeneratedName { get; set; }
        public string UILabelName { get; set; }
        public string DataSourceName { get; set; }
        public int DataSourceId { get; set; }
        public int UIElementID { get; set; }
        public string RadioOptionLabelYes { get; set; }
        public string RadioOptionLabelNo { get; set; }
        public bool ? Visable { get; set; }
        public string PropertyRule { get; set; }
        public int RuleId { get; set; }
        public string UIElementName { get; set; }
    }
}
