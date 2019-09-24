using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportTemplateRow
    {
        public string TemplateRowName { get; set; }
        public List<ReportTemplateRowSource> Sources { get; set; }
        public ReportTemplateRowType RowType { get; set; }
        public string Title { get; set; }
        public bool HasParent { get; set; }
        public string RichText { get; set; }
        public ReportTemplateRow Parent { get; set; }
        public ReportTemplateRowFilter Filter { get; set; }
        public List<ReportTemplateRow> ChildRows { get; set; }
    }
}
