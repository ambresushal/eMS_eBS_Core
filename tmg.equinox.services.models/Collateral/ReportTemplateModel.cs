using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class ReportTemplateModel
    {
        public int TenantID { get; set; }
        public int DocumentDesignId { get; set; }
        public string DocumentDesignName { get; set; }
        public List<DocumentVersion> DocumentVersions { get; set; }
        public string DataSourceName { get; set; }
        public DocumentVersion SelectedVersion { get; set; }
        public int FormDesignVersionID { get; set; }
        public int? TemplateReportFormDesignVersionMapID { get; set; }

        //public string DSourceName { get; set; }
        //public string IsSelected { get; set; }
        //public string DVersion { get; set; }        
    }

    public class DocumentVersion
    {
        public int VersionId { get; set; }
        public string VersionNo { get; set; }
        public DateTime? EffctiveDate { get; set; }
    }
}
