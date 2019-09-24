using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DataSource : Entity
    {
        public DataSource()
        {
            this.DataSourceMappings = new List<DataSourceMapping>();
            this.RepeaterUIElements = new List<RepeaterUIElement>();
            this.SectionUIElements = new List<SectionUIElement>();
           
        }

        public int DataSourceID { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceDescription { get; set; }
        public string Type { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
        public virtual ICollection<RepeaterUIElement> RepeaterUIElements { get; set; }
        public virtual ICollection<SectionUIElement> SectionUIElements { get; set; }
    }
}
