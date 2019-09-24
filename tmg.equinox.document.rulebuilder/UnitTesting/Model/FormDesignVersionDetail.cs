using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.UnitTesting.Model
{
   public class FormDesignVersionDetail
    {
       public List<SectionDesign> Sections { get; set; }
    }

   public class SectionDesign
   {
       public int ID { get; set; }
       public string Name { get; set; }
       public string GeneratedName { get; set; }
       public string FullName { get; set; }
       public string Label { get; set; }
       public string HelpText { get; set; }
       public int Sequence { get; set; }
       public int ChildCount { get; set; }
       public bool Visible { get; set; }
       public bool Enabled { get; set; }
       public int? LayoutColumn { get; set; }
       public string LayoutClass { get; set; }
       public string CustomHtml { get; set; }
       public List<ElementDesign> Elements { get; set; }
       public string SectionNameTemplate { get; set; }
   }

   public class ElementDesign
   {
       public int ElementID { get; set; }
       public string Name { get; set; }
       public string GeneratedName { get; set; }
       public string FullName { get; set; }
       public string Label { get; set; }
       public string AlternateLabel { get; set; }
       public bool IsLabel { get; set; }
       public bool Multiline { get; set; }
       public bool MultiSelect { get; set; }
       public string HelpText { get; set; }
       public bool Enabled { get; set; }
       public string Type { get; set; }
       public string DataType { get; set; }
       public int DataTypeID { get; set; }
       public string Value { get; set; }
       public bool SpellCheck { get; set; }
       public int MaxLength { get; set; }
       public string DefaultValue { get; set; }
       public string OptionLabel { get; set; }
       public string OptionLabelNo { get; set; }
       public SectionDesign Section { get; set; }
       public bool HasCustomRule { get; set; }
       public bool IsMatch { get; set; }
       public bool Visible { get; set; }
       public bool IsPrimary { get; set; }
       public List<int> RulesToExecute { get; set; }
       public DateTime? MaxDate { get; set; }
       public DateTime? MinDate { get; set; }
       public bool? IsDropDownTextBox { get; set; }
       public bool? IsSortRequired { get; set; }
       public bool CheckDuplicate { get; set; }
       public bool IsKey { get; set; }
       public bool IsRichTextBox { get; set; }
   }
}
