using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormReportVersionMap : Entity
    {

        public int ReportVersionMapID { get; set; }
        public int ReportVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string PlaceHolderID { get; set; }
        public string Description { get; set; }
        public string FormDataPath { get; set; }
        public string ValueExpression { get; set; }
        public string FilterExpression { get; set; }
        public string FilterExpressionValue { get; set; }
        public string MapType { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ValueFormat { get; set; }
        public string CoveredServiceID { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }

        public virtual FormReportVersion FormReportVersion { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
    }
}
