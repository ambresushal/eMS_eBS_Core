using System;
using System.Collections.Generic;
using System.Linq;

namespace tmg.equinox.web.Validator
{
    public class ErrorRow
    {
        public int ID;
        public string RepeaterGridID { get; set; }
        public string ElementID { get; set; }
        public string Form { get; set; }
        public string FormInstance { get; set; }
        public string SectionID { get; set; }
        public string Section { get; set; }
        public int FormInstanceID { get; set; }
        public string SubSectionName { get; set; }
        public string Field { get; set; }
        public string GeneratedName { get; set; }
        public int ColumnNumber { get; set; }
        public string RowIdProperty { get; set; }
        public string RowNum { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string KeyValue { get; set; }
    }
}
