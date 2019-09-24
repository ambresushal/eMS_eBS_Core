using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ReportSetting : Entity
    {     
        public ReportSetting()
        {
            isMapping = false;
        }
       
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportTemplatePath { get; set; }

        public string OutputPath { get; set; }

        public  string ReportNameFormat { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public bool isMapping { get; set; }
        [NotMapped]
        public virtual ICollection<ReportMappingField> mappings { get; set; }
        public DateTime? AddedDate { get; set; }
        public string SQLstatement { get; set; }
        public bool Visible { get; set; }

    }
}
