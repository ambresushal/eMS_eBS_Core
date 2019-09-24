using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.models
{
    public partial class ReportingTableInfo : Entity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string SchemaName { get; set; }
        public string ParentName { get; set; }
        public ICollection<ReportingTableColumnInfo> Columns { get; set; }
        public int DesignId { get; set; }
        public int DesignVersionId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string DesignType { get; set; }
        public string DocumentPath { get; set; }
        public string DesignVersionNumber { get; set; }

        [NotMapped]
        public ICollection<ReportingTableColumnInfo> PreColumns { get; set; }
    }


    public partial class SchemaVersionActivityLog : Entity
    {
        public int ID { get; set; }
        public string DesignVersion { get; set; }
        public int DesignVersionId { get; set; }

        public string ObjectType { get; set; }

        public string Operation { get; set; }

        public string Value  { get; set; }

        public DateTime CreationDate { get; set; }

        public string DesignType { get; set; }

        public string Label { get; set; }

        public string CustomType { get; set; }
        public string ValuePath { get; set; }
    }
}
