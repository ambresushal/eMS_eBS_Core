using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.documentcomparer.RepeaterCompareUtils;

namespace tmg.equinox.web.FormDesignManager.ConfigView
{
    public class ConfigViewHelper
    {
        private JsonSerializerSettings _jsonWriter = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
        public List<JToken> Except(List<JToken> source, List<JToken> target, string keys)
        {
            List<string> compKeys = keys.Split(',').ToList();
            return source.Except(target, new RepeaterEqualityComparer(compKeys)).ToList();
        }

        public List<JToken> Intersect(List<JToken> source, List<JToken> target, string keys)
        {
            List<string> compKeys = keys.Split(',').ToList();
            return source.Intersect(target, new RepeaterEqualityComparer(compKeys)).ToList();
        }

        public Dictionary<string, List<JToken>> GetCompareResult(List<JToken> source, List<JToken> target, List<string> keys)
        {
            Dictionary<string, List<JToken>> differences = new Dictionary<string, List<JToken>>();
            List<JToken> allRows = new List<JToken>();

            var deletedRows = this.Except(source, target, keys[0]);
            differences.Add("DELETED", deletedRows);
            allRows.AddRange(deletedRows);

            var addedRows = target.Where(s => (string)s[keys[0]] == null || (string)s[keys[0]] == string.Empty).ToList();
            differences.Add("ADDED", addedRows);
            allRows.AddRange(addedRows);

            var commonRows = source.Intersect(target, new RepeaterEqualityComparer(keys)).ToList();
            allRows.AddRange(commonRows);

            var updatedRows = new List<JToken>();
            if (target.Count != (addedRows.Count + commonRows.Count))
            {
                updatedRows = this.Except(target, allRows, keys[0]);
            }
            differences.Add("UPDATED", updatedRows);
            return differences;
        }

        public List<string> GetKeys()
        {
            List<string> keys = new List<string>() {
                                "UIElementID", "UIElementName", "Label",
                                "ElementType", "Layout","DataType", "MaxLength",
                                "Required", "IsVisible", "IsEnable",
                                "Parent", "IsKey", "IsMultiselect",
                                "HelpText", "Formats", "Items","AreRulesModified",
                                "AdvancedConfiguration", "ExtProp","DefaultValue",
                                "ViewType","AllowBulkUpdate","OptionYes","OptionNo","CustomHtml"
                            };
            return keys;
        }

        public UIElementAddModel GetUIElementAddModel(int tenantId, int formDesignId, int formDesignVersionId, JToken row)
        {
            var model = JsonConvert.DeserializeObject<UIElementAddModel>(row.ToString(), _jsonWriter);
            model.TenantID = tenantId;
            model.FormDesignVersionID = formDesignVersionId;
            model.ApplicationDataTypeID = model.ApplicationDataTypeID == 0 ? 2 : model.ApplicationDataTypeID;
            if (row["ExtProp"] != null)
            {
                model.ExtendedProperties = Convert.ToString(row["ExtProp"]);
            }
            return model;
        }

        public UIElementConfigUpdateModel GetUIElementUpdateModel(int tenantId, int formDesignId, int formDesignVersionId, JToken row)
        {
            string viewType = Convert.ToString(row["ViewType"]);
            int viewTypeId = viewType == "Folder View" ? 1 : viewType == "SOT View" ? 2 : 3;
            row["ViewType"] = viewTypeId;
            var model = JsonConvert.DeserializeObject<UIElementConfigUpdateModel>(row.ToString(), _jsonWriter);
            model.TenantID = tenantId;
            model.FormDesignVersionID = formDesignVersionId;
            model.FormDesignID = formDesignId;
            model.IsMultiLine = Convert.ToString(row["ElementType"]) == "Multiline TextBox" ? true : false;
            model.ApplicationDataTypeID = model.ApplicationDataTypeID == 0 ? 2 : model.ApplicationDataTypeID;
            if (row["ExtProp"] != null)
            {
                model.ExtendedProperties = Convert.ToString(row["ExtProp"]);
            }
            return model;
        }

        public List<UIElementRowViewModel> GetAllUIElementsForDownload(string elements)
        {
            return JsonConvert.DeserializeObject<List<UIElementRowViewModel>>(elements, _jsonWriter);
        }
    }
}
