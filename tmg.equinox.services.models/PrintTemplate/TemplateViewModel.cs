using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PrintTemplate
{
    public class TemplateViewModel
    {
        public int? TemplateID { get; set; }
        public int TemplateUIMapID { get; set; }
        public Nullable<int> FormDesignID { get; set; }
        public string FormDesignName { get; set; }
        public string VersionName { get; set; }
        public Nullable<int> FormDesignVersionID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool isActive { get; set; }
        public bool IncludeInPDF { get; set; }
        public int? UIElementID { get; set; }
        public string UIElementName{get;set;}
        public string Label { get; set; }
        public string parent { get; set; }
        public int Sequence {get;set;}
        public int level {get;set;}
        public string ElementType { get; set; }
        public bool isLeaf { get; set; }
        public bool isExt { get; set; }
        public bool loaded { get; set; }
        public List<TemplateViewModel> uiElementList { get; set; }
    }
    
}
