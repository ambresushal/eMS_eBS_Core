using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportTemplate
    {
        List<ReportTemplateRow> _rows;
        public ReportTemplate()
        {
            _rows = new List<ReportTemplateRow>();
        }
       public List<ReportTemplateRow> Rows
        {
            get
            {
                return _rows;
            }
        }                
    }
}
