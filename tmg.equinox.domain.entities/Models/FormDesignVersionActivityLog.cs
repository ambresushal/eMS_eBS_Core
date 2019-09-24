using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormDesignVersionActivityLog : Entity
    {
        public string Formdesign { get; set; }
        public string FormdesignVersion { get; set; }
        public int UIElementID { get; set; }
        public string UiElementName { get; set; }
        public string JsonPath { get; set; }
        public string Label { get; set; }
        public string IsEnable { get; set; }
        public string IsVisible { get; set; }
        public string IsviewType { get; set; }
        public string IsDatatype { get; set; }
        public string IsMultiSelect { get; set; }
        public string IsMultiLine { get; set; }
        public string IsMaxLength { get; set; }
        public string IsLabel { get; set; }
        public string IsRequired { get; set; }
        public string IsStandardFormat { get; set; }
        public string isSelectStandardFormat { get; set; }
        public string IsRuleMatch { get; set; }
        public string IsExpressionMatch { get; set; }
        public string IsDropdownItemsMatch { get; set; }
        public string Operation { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
