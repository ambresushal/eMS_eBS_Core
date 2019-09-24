using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ErrorLog : Entity
    {
        public ErrorLog()
        {

        }

        public int ErrorLogID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int IASElementExportID { get; set; }
        public string RuleErrorDescription { get; set; }
        public string DataSourceErrorDescription { get; set; }
        public string ValidationErrorDescription { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }
        public virtual IASElementExport IASElementExport { get; set; }

    }
}
