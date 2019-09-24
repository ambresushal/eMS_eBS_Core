using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.web.FormDesignManager
{
    public class FormDesignActivityLog
    {
        private IFormDesignService _formDesignService;
        StringDictionary _descriptions = new StringDictionary();


        public FormDesignActivityLog(IFormDesignService formDesignService)
        {
            this._formDesignService = formDesignService;
            _descriptions = FillDescriptions();
        }

        public List<ActivityLog> GetActivityLogData(int formDesignId, int formDesignVersionId)
        {
            List<ActivityLog> activityLogData = new List<ActivityLog>();

            int formDesignPreviousVersionId = this._formDesignService.GetPreviousFormDesignVersion(1, formDesignId, formDesignVersionId);

            List<FormDesignVersionActivityLog> logData = this._formDesignService.GetFormDesignVersionActivityLogData(formDesignId, formDesignVersionId, formDesignPreviousVersionId);
            if (logData != null && logData.Count > 0)
            {
                var properties = GetProperties(logData[0]);
                foreach (FormDesignVersionActivityLog row in logData)
                {
                    ActivityLog log = new ActivityLog();
                    log.ElementPath = row.JsonPath;
                    log.ElementLabel = row.Label;
                    log.Version = row.FormdesignVersion;
                    log.UpdatedBy = row.UpdatedBy;
                    log.UpdatedDate = row.UpdatedDate;
                    foreach (var p in properties)
                    {
                        string propertyName = p.Name;
                        string value = p.GetValue(row, null) != null ? p.GetValue(row, null).ToString() : "";
                        if (propertyName == "Operation")
                            log.Description += _descriptions[value] != null ? _descriptions[value] + "\n" : "";
                        else
                        {
                            if (value == "No")
                                log.Description += _descriptions[propertyName] != null ? _descriptions[propertyName] + "\n" : "";
                        }
                    }

                    if (log.Description != null && log.Description != "")
                        activityLogData.Add(log);
                }
            }
            return activityLogData;
        }

        private StringDictionary FillDescriptions()
        {
            StringDictionary descriptions = new StringDictionary();
            descriptions.Add("IsEnable", "Enable rule has been changed.");
            descriptions.Add("IsVisible", "Visibility has been changed.");
            descriptions.Add("IsviewType", "ViewType has been changed.");
            descriptions.Add("IsDatatype", "Datatype has been changed.");
            descriptions.Add("IsMultiSelect", "MultiSelect property has been changed.");
            descriptions.Add("IsMultiLine", "MultiLine property has been changed.");
            descriptions.Add("IsMaxLength", "MaxLength property has been changed.");
            descriptions.Add("IsLabel", "Label has been changed.");
            descriptions.Add("IsRequired", "IsRequired property has been changed.");
            descriptions.Add("IsStandardFormat", "IsStandardFormat property has been changed.");
            descriptions.Add("isSelectStandardFormat", "IsSelectStandardFormat property has been changed.");
            descriptions.Add("IsRuleMatch", "Config rule has been changed.");
            descriptions.Add("IsExpressionMatch", "Expression Rule has been changed.");
            descriptions.Add("IsDropdownItemsMatch", "Dropdown Items have been changed.");
            descriptions.Add("Second Version", "This field is newly added in current version.");
            descriptions.Add("First Version", "This field is removed from current version.");
            return descriptions;
        }

        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }
    }
}
