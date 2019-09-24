using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace DesignGenerator
{
    public class DesignBuilder
    {
        public int _uiElementId = 70000;
        public FormDesignVersionDetail Build()
        {
            List<string> excludedTables = new List<string>() { "DELETED_PLANS", "HISTORY", "PATCH_HISTORY", "USER", "VERSION" };
            FormDesignVersionDetail detail = null;
            detail = new FormDesignVersionDetail();
            detail.TenantID = 1;
            detail.FormDesignId = 1111;
            detail.FormDesignVersionId = 1111;
            detail.FormVersion = "2017_1.0";
            detail.FormName = "PBP";
            detail.IsNewFormInstance = false;

            detail.Rules = new List<RuleDesign>();
            detail.DataSources = new List<DataSourceDesign>();
            detail.ElementRuleMaps = new List<ElementRuleMap>();
            detail.Validations = new List<ValidationDesign>();
            detail.Duplications = new List<DuplicationDesign>();

            detail.Sections = new List<SectionDesign>();


            DataTable tables = DataTableManager.GetTables();
            foreach (DataRow row in tables.Rows)
            {
                if (!excludedTables.Contains(Convert.ToString(row[0])))
                {
                    if (Convert.ToInt16(row[1]) > 11)
                    {
                        detail.Sections.Add(GetSection(Convert.ToString(row[0]), true));
                    }
                    else
                    {
                        detail.Sections.Add(GetSection(Convert.ToString(row[0]), false));
                    }
                }
            }

            detail.JSONData = detail.GetDefaultJSONDataObject();
            return detail;
        }

        private SectionDesign GetSection(string sectionName, bool hasRepeater)
        {
            int uiElementId = ++_uiElementId;
            SectionDesign design = new SectionDesign();
            design.Elements = new List<ElementDesign>();
            design.AccessPermissions = GetPersmissions(uiElementId);
            design.ID = uiElementId;
            design.Name = sectionName;
            design.FullName = sectionName;
            design.GeneratedName = sectionName;
            design.Label = sectionName;
            design.Enabled = true;
            design.Visible = true;
            design.LayoutColumn = 3;
            design.LayoutClass = "threeColumn";
            if (hasRepeater)
            {
                string repeaterName = "RPT" + sectionName;
                design.Elements.Add(GetElement(sectionName, repeaterName, "repeater", 100, GetRepeater(sectionName, repeaterName)));
            }
            else
            {
                design.Elements = GetChildElements(sectionName, null);
            }
            design.ChildCount = design.Elements.Count;

            return design;
        }

        private RepeaterDesign GetRepeater(string sectionName, string elementName)
        {
            int uiElementId = ++_uiElementId;
            RepeaterDesign design = null;
            design = new RepeaterDesign();
            design.Elements = new List<ElementDesign>();
            design.ID = uiElementId;
            design.Name = elementName;
            design.GeneratedName = elementName;
            design.FullName = sectionName + "." + elementName;
            design.Label = elementName;
            design.RepeaterType = "child";
            design.RowCount = 1;
            design.LayoutColumn = 3;
            design.LayoutClass = "threeColumn";
            design.Elements = GetChildElements(sectionName, design.FullName);
            design.ChildCount = design.Elements.Count;
            // Properties for Configuring Param Query Features 
            design.DisplayTopHeader = true;
            design.DisplayTitle = true;
            design.FrozenColCount = 0;
            design.FrozenRowCount = 0;
            design.AllowPaging = true;
            design.RowsPerPage = 25;
            design.FilterMode = "toolbar";
            design.RepeaterUIElementProperties = new tmg.equinox.applicationservices.viewmodels.UIElement.RepeaterUIElementPropertyModel();

            return design;
        }

        private List<ElementDesign> GetChildElements(string name, string repeaterName)
        {
            List<ElementDesign> elements = new List<ElementDesign>();

            DataTable columns = DataTableManager.GetTableColumns(name);

            foreach (DataRow row in columns.Rows)
            {
                elements.Add(GetElement(repeaterName ?? name, Convert.ToString(row[0]), Convert.ToString(row[1]), Convert.ToInt16(row[2]), null));
            }

            return elements;
        }

        private ElementDesign GetElement(string sectionName, string elementName, string type, int length, RepeaterDesign repeater)
        {
            int uiElementId = ++_uiElementId;
            ElementDesign design = new ElementDesign();
            design.ElementID = uiElementId;
            design.Name = elementName;
            design.GeneratedName = elementName;
            design.Label = elementName;
            design.Enabled = true;
            design.Visible = true;
            design.Type = GetElementType(type);
            design.MaxLength = length;
            design.DataType = "string";
            design.DataTypeID = 2;
            design.FullName = sectionName + "." + elementName;
            design.HasCustomRule = false;
            design.IsLabel = false;
            design.SpellCheck = false;
            design.Multiline = length > 100 ? true : false;
            design.MultiSelect = false;
            design.OptionLabel = "";
            design.OptionLabelNo = "";
            design.Items = null;
            design.Repeater = repeater;
            design.Section = null;
            design.MaxDate = null;
            design.MinDate = null;
            design.IsDropDownTextBox = false;
            design.IsDropDownFilterable = false;
            design.IsSortRequired = false;
            design.CheckDuplicate = false;
            design.IsRichTextBox = false;
            design.IsPrimary = true;

            return design;
        }

        private string GetElementType(string type)
        {
            string elementType = "text";
            List<string> types = new List<string>() { "radio", "checkbox", "select", "calendar", "repeater", "tab", "section", "label", "blank", "SelectInput", "text" };
            if (type == "nvarchar")
            {
                elementType = "text";
            }
            else if (type == "bit")
            {
                elementType = "radio";
            }
            else if (type == "DateTime")
            {
                elementType = "calendar";
            }
            else if (type == "repeater")
            {
                elementType = "repeater";
            }
            return elementType;
        }

        private List<ElementAccessViewModel> GetPersmissions(int uiElementId)
        {
            List<ElementAccessViewModel> permissions = new List<ElementAccessViewModel>();
            Dictionary<int, string> roles = new Dictionary<int, string>();
            roles.Add(10, "Audit");
            roles.Add(15, "GU Super User");
            roles.Add(9, "Manager");
            roles.Add(8, "Product");
            roles.Add(13, "Product Audit");
            roles.Add(7, "Read Only");
            roles.Add(14, "Security");
            roles.Add(11, "Super User");
            roles.Add(1, "TMG Super User");
            roles.Add(12, "WF Specialist");

            foreach (var role in roles)
            {
                permissions.Add(GetPersmissionByRole(role.Key, role.Value, uiElementId));
            }
            return permissions;
        }

        private ElementAccessViewModel GetPersmissionByRole(int roleId, string roleName, int uiElementId)
        {
            return new ElementAccessViewModel()
            {
                RoleID = roleId,
                IsEditable = true,
                IsVisible = true,
                ResourceID = uiElementId,
                RoleName = roleName
            };
        }
    }
}
