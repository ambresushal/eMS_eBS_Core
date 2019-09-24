using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class ReportServiceViewModel
    {
        public string ReportPlaceHolderID { get; set; }
        public string Value { get; set; }
        public string ValueFormat { get; set; }
        public string JSONPath { get; set; }
        public string JSONExpression { get; set; }
        public string FilterExpression { get; set; }
        public string FilterExpressionValue { get; set; }
        public string FormDesignID { get; set; }
        public string CoveredServiceID { get; set; }
        public string MapType { get; set; }

    }
}
