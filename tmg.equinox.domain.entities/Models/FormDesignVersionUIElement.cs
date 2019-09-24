using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignVersionUIElement : Entity
    {
        public FormDesignVersionUIElement()
        {
        }
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public bool IsContainer { get; set; }
        public string UIElementFullName { get; set; }
        public int UIElementDataTypeID { get; set; }
        public string GeneratedName { get; set; }
        public int UIElementTypeID { get; set; }
        public string ApplicationDataTypeName { get; set; }
        public string TypeDescription { get; set; }

    }
}
