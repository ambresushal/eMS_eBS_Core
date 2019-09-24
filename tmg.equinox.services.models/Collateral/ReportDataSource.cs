using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class ReportDataSource
    {
        public List<ReportSource> DataSources { get; set; }
        public string ReportName { get; set; }
        public string Location { get; set; }
    }

    public class ReportSource
    {
        public XmlDocument Xml { get; set; }
        public string DataSourceName { get; set; }

    }
}
