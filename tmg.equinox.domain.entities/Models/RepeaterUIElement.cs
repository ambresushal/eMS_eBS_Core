using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RepeaterUIElement : UIElement
    {
        public int UIElementTypeID { get; set; }
        public int LayoutTypeID { get; set; }
        public Nullable<int> ChildCount { get; set; }
        public Nullable<int> DataSourceID { get; set; }
        public virtual LayoutType LayoutType { get; set; }
        //public virtual UIElement UIElement { get; set; }
        public virtual DataSource DataSource { get; set; }
        public bool LoadFromServer { get; set; }
        public bool IsDataRequired { get; set; }
        public bool AllowBulkUpdate { get; set; }

        // Properties for Configuring Param Query Features 
        public bool DisplayTopHeader { get; set; }
        public bool DisplayTitle { get; set; }
        public int FrozenColCount { get; set; }
        public int FrozenRowCount { get; set; }
        public bool AllowPaging { get; set; }
        public int RowsPerPage { get; set; }
        public bool AllowExportToExcel { get; set; }
        public bool AllowExportToCSV { get; set; }
        public string FilterMode { get; set; }

        public RepeaterUIElement Clone(string username, DateTime addedDate)
        {
            RepeaterUIElement element = new RepeaterUIElement();
            element.UIElementName = this.UIElementName;
            element.Label = this.Label;
            element.ParentUIElementID = this.ParentUIElementID;
            element.IsContainer = this.IsContainer;
            element.Enabled = this.Enabled;
            element.Visible = this.Visible;
            element.Sequence = this.Sequence;
            element.RequiresValidation = this.RequiresValidation;
            element.HelpText = this.HelpText;
            element.AddedBy = username;
            element.AddedDate = addedDate;
            element.IsActive = this.IsActive;
            element.FormID = this.FormID;
            element.UIElementDataTypeID = this.UIElementDataTypeID;
            element.HasCustomRule = this.HasCustomRule;
            element.CustomRule = this.CustomRule;
            element.GeneratedName = this.GeneratedName;
            element.UIElementTypeID = this.UIElementTypeID;
            element.LayoutTypeID = this.LayoutTypeID;
            element.ChildCount = this.ChildCount;
            element.DataSourceID = this.DataSourceID;
            element.LoadFromServer = this.LoadFromServer;
            element.IsDataRequired = this.IsDataRequired;
            element.AllowBulkUpdate = this.AllowBulkUpdate;
            element.Enabled = this.Enabled;

            element.Validators = new List<Validator>();
            foreach (var item in this.Validators)
            {
                element.Validators.Add(item.Clone(username, addedDate));
            }

            element.PropertyRuleMaps = new List<PropertyRuleMap>();
            foreach (var item in this.PropertyRuleMaps)
            {
                element.PropertyRuleMaps.Add(item.Clone(username, addedDate));
            }
            element.DisplayTopHeader = this.DisplayTopHeader;
            element.DisplayTitle = this.DisplayTitle;
            element.FrozenColCount = this.FrozenColCount;
            element.FrozenRowCount = this.FrozenRowCount;
            element.AllowPaging = this.AllowPaging;
            element.RowsPerPage = this.RowsPerPage;
            element.AllowExportToExcel = this.AllowExportToExcel;
            element.AllowExportToCSV = this.AllowExportToCSV;
            element.FilterMode = this.FilterMode;

            return element;
        }
    }
}
