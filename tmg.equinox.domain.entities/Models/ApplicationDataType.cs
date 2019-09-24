using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ApplicationDataType : Entity
    {
        public ApplicationDataType()
        {
            this.UIElements = new List<UIElement>();
            this.ServiceDefinitions = new List<ServiceDefinition>();
            this.ServiceParameters = new List<ServiceParameter>();
        }

        public int ApplicationDataTypeID { get; set; }
        public string ApplicationDataTypeName { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string DisplayText { get; set; }
        public virtual ICollection<UIElement> UIElements { get; set; }
        public virtual ICollection<ServiceDefinition> ServiceDefinitions { get; set; }
        public virtual ICollection<ServiceParameter> ServiceParameters { get; set; }
    }
}
