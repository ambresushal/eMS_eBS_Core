using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.web.FindnReplace
{
    public class ActivityLogManager
    {
        private int _folderId;
        private int _folderVersionId;
        private int _formDesignVersionId;
        private int _formInstanceId;
        private FormDesignVersionDetail _detail;
        private string _userName;
        private string _descriptionFormat = "Value of {0} is changed from {1} to {2}.";
        private IFolderVersionServices _folderVersionServices;
        private IUIElementService _uiElementService;
        private IEnumerable<UIElementRowModel> _elementList;
        public ActivityLogManager(int folderId, int folderVersionId, int formDesginVersionId, int formInstanceId, FormDesignVersionDetail detail, string userName, IFolderVersionServices folderVersionServices, IUIElementService uiElementService)
        {
            this._folderId = folderId;
            this._folderVersionId = folderVersionId;
            this._formDesignVersionId = formDesginVersionId;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._userName = userName;
            this._folderVersionServices = folderVersionServices;
            this._uiElementService = uiElementService;
        }

        public ServiceResult SaveActivityLog(List<ChangeLogModel> changeLogs)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                _elementList = _uiElementService.GetUIElementListForFormDesignVersion(1, _formDesignVersionId);
                var activityLogs = this.BuildActivityLogModel(changeLogs);
                if (activityLogs != null && activityLogs.Count > 0)
                {
                    result = _folderVersionServices.SaveFormInstanceAvtivitylogData(1, _formInstanceId, _folderId, _folderVersionId, 0, _formDesignVersionId, activityLogs);
                }
            }
            catch (Exception ex)
            {
                bool error = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }

            return result;
        }

        private List<ActivityLogModel> BuildActivityLogModel(List<ChangeLogModel> changeLogs)
        {
            List<ActivityLogModel> activityLogData = new List<ActivityLogModel>();

            changeLogs = changeLogs.OrderBy(a => a.FormInstanceID).ToList();
            foreach (var entry in changeLogs)
            {
                try
                {
                    string field = GetElementLabel(entry.ElementPath);
                    ActivityLogModel objModel = new ActivityLogModel()
                    {
                        FormInstanceID = entry.FormInstanceID,
                        Description = String.Format(_descriptionFormat, field, entry.OldValue, entry.NewValue),
                        ElementPath = GetElementFullPath(entry.ElementPath),
                        Field = field,
                        FolderVersionName = "",
                        IsNewRecord = true,
                        SubSectionName = "",
                        UpdatedBy = _userName,
                        RowNum = entry.Key,
                        UpdatedLast = DateTime.Now
                    };
                    activityLogData.Add(objModel);
                }
                catch (Exception ex)
                {
                    bool result = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                }
            }

            return activityLogData;
        }

        private string GetElementFullPath(string elementPath)
        {
            string fullPath = "";
            string[] paths = elementPath.Split('.');
            string path = paths[0];
            for (int i = 0; i < paths.Length - 1;)
            {
                var element = this._elementList.Where(s => s.UIElementPath == path).FirstOrDefault();
                if (element != null)
                {
                    fullPath = i == 0 ? element.Label : (fullPath + " => " + element.Label);
                }
                path = path + "." + paths[++i];
            }

            return fullPath;
        }

        private string GetElementLabel(string elementPath)
        {
            string field = string.Empty;
            var element = this._elementList.Where(s => s.UIElementPath == elementPath).FirstOrDefault();
            if (element != null)
            {
                field = element.Label;
            }

            return field;
        }
    }
}
