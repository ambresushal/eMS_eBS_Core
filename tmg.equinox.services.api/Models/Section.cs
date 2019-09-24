using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Models
{
    public class SectionResult
    {

        public SectionResult()
        {
            ErrorRows = new List<Models.ErrorRow>();
        }
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string Form { get; set; }
        public List<ErrorRow> ErrorRows { get; set; }
    }
}