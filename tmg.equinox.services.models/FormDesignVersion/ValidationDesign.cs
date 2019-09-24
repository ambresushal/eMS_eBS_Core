using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class ValidationDesign
    {
        public string FullName { get; set; }
        public string UIElementName { get; set; }
        public bool? IsRequired { get; set; }
        public bool IsError { get; set; }
        public string Regex { get; set; }
        public string ValidationMessage { get; set; }
        public bool? HasMaxLength { get; set; }
        public int MaxLength { get; set; }
        public string DataType { get; set; }
        public bool IsActive { get; set; }
        public string MaskExpression { get; set; }
        public string LibraryRegexName { get; set; }
        public string PlaceHolder { get; set; }
        public bool MaskFlag { get; set; }
    }
}
