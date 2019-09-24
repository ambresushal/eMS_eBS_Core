using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UIElement : Entity
    {
        public UIElement()
        {
            this.DataSourceMappings = new List<DataSourceMapping>();
            this.DataSourceMappings1 = new List<DataSourceMapping>();
            this.FormDesignVersionUIElementMaps = new List<FormDesignVersionUIElementMap>();
            this.PropertyRuleMaps = new List<PropertyRuleMap>();
            this.UIElement1 = new List<UIElement>();
            this.Validators = new List<Validator>();
            this.ServiceDefinitions = new List<ServiceDefinition>();
            this.ServiceParameters = new List<ServiceParameter>();
        }

        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public string Label { get; set; }
        public Nullable<int> ParentUIElementID { get; set; }
        public bool IsContainer { get; set; }
        public Nullable<bool> Enabled { get; set; }
        public Nullable<bool> Visible { get; set; }
        public int Sequence { get; set; }
        public bool RequiresValidation { get; set; }
        public string HelpText { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public int FormID { get; set; }
        public int UIElementDataTypeID { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> HasCustomRule { get; set; }
        public string CustomRule { get; set; }
        public bool CheckDuplicate { get; set; }
        public Nullable<bool> AllowGlobalUpdates { get; set; }
        public int ViewType { get; set; }
        public bool IsStandard { get; set; }
        public string MDMName { get; set; }
        public string ExtendedProperties { get; set; }
        public Nullable<int> DataSourceElementDisplayModeID { get; set; }
        public bool IsSameSectionRuleSource { get; set; }
        public virtual ApplicationDataType ApplicationDataType { get; set; }

        public virtual DataSourceElementDisplayMode DataSourceElementDisplayMode { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings1 { get; set; }

        public virtual FormDesign FormDesign { get; set; }
        public virtual ICollection<FormDesignVersionUIElementMap> FormDesignVersionUIElementMaps { get; set; }
        public virtual ICollection<PropertyRuleMap> PropertyRuleMaps { get; set; }

        public virtual ICollection<UIElement> UIElement1 { get; set; }
        public virtual UIElement UIElement2 { get; set; }
        public virtual ICollection<Validator> Validators { get; set; }
        public string GeneratedName { get; set; }
        public virtual ICollection<TemplateUIMap> TemplateUIMap { get; set; }
        public virtual ICollection<ServiceDefinition> ServiceDefinitions { get; set; }
        public virtual ICollection<ServiceParameter> ServiceParameters { get; set; }
        public virtual ICollection<FormDesignElementValue> FormDesignElementValues { get; set; }
        public virtual ICollection<IASElementExport> IASElementExports { get; set; }
        public virtual ICollection<RuleGu> RulesGu { get; set; }
        public virtual ICollection<IASElementImport> IASElementImports { get; set; }
        public virtual ICollection<DocumentRule> DocumentRule { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
