using System;
using System.Collections.Generic;
using System.Linq;

namespace tmg.equinox.web.Validator
{
    public class ErrorSection
    {
        public string SectionID { get; set; }
        public string Section { get; set; }
        public string Form { get; set; }
        public int FormInstanceID { get; set; }
        public int ID { get; set; }
        public List<ErrorRow> ErrorRows { get; set; }
    }
}
