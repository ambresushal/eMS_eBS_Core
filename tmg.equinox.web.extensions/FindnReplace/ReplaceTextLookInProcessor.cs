using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.FindnReplace
{
    public class ReplaceTextLookInProcessor : IReplaceTextProcessor
    {
        private DocumentInfo _documentInfo;
        private ReplaceCriteria _replaceCriteria;
        private IFormDesignService _formDesignServices;
        private IFormInstanceDataServices _formInstanceDataService;
        private IFolderVersionServices _folderVerionService;
        private int? _userId;
        private string _userName;
        private FormDesignVersionDetail _detail;
        private FormInstanceDataManager _formDataInstanceManager;

        public ReplaceTextLookInProcessor(DocumentInfo documentInfo, ReplaceCriteria criteria, IFormDesignService formDesignServices, IFormInstanceDataServices dataService,
                                    IFolderVersionServices folderVerionService, int? userId, string userName, FormDesignVersionDetail detail)
        {
            this._documentInfo = documentInfo;
            this._replaceCriteria = criteria;
            this._formDesignServices = formDesignServices;
            this._formInstanceDataService = dataService;
            this._folderVerionService = folderVerionService;
            this._userId = userId;
            this._userName = userName;
            this._detail = detail;
        }

        public List<ChangeLogModel> Process()
        {
            List<ChangeLogModel> activityLog = new List<ChangeLogModel>();
            _formDataInstanceManager = new FormInstanceDataManager(1, _userId, _formInstanceDataService, _userName, _folderVerionService);

            switch (_replaceCriteria.ReplaceWithIn)
            {
                case "Section":
                case "Document":
                    activityLog = ProcessDocument(_documentInfo.FormInstanceId);
                    break;
                case "Folder":
                    activityLog = ProcessFolder();
                    break;
            }
            return activityLog;
        }
        private List<ChangeLogModel> ProcessFolder()
        {
            List<ChangeLogModel> changeLog = new List<ChangeLogModel>();
            var formInstances = _folderVerionService.GetFormInstanceList(1, _documentInfo.FolderVersionId, _documentInfo.FolderId);
            Parallel.ForEach(formInstances.OrderBy(s => s.FormInstanceID), doc =>
            {
                var log = ProcessDocument(doc.FormInstanceID);
                changeLog.AddRange(log);
            });

            return changeLog;
        }

        private List<ChangeLogModel> ProcessDocument(int formInstanceId)
        {
            List<ChangeLogModel> changeLog = new List<ChangeLogModel>();
            string formData = _formDataInstanceManager.GetFormInstanceData(formInstanceId, false, _detail);
            JObject objFormData = JObject.Parse(formData);
            ProcessElement(formInstanceId, _replaceCriteria.SpecificPath, objFormData, ref changeLog);
            string updatedFormData = objFormData.ToString();
            _formDataInstanceManager.SetFormInstanceData(formInstanceId, updatedFormData);
            _folderVerionService.SaveFormInstanceDataCompressed(formInstanceId, updatedFormData);
            return changeLog;
        }

        private void ProcessElement(int formInstanceId, string specificPath, JObject formData, ref List<ChangeLogModel> changeLog)
        {
            string targetData = string.Empty;
            JToken elementToken = formData.SelectToken(specificPath);

            if (elementToken == null)
            {
                string[] path = specificPath.Split('.');
                elementToken = formData.SelectToken(string.Join(".", path.Take(path.Length - 1)));
            }

            if (elementToken.Type == JTokenType.Array)
            {
                int index = 0;
                List<JToken> repaterRows = elementToken.ToList();
                foreach (var row in repaterRows)
                {
                    targetData = row["RichText"].ToString();
                    if (!string.IsNullOrWhiteSpace(targetData))
                    {
                        FindnReplace objFind = new FindnReplace(targetData, _replaceCriteria.FindText, _replaceCriteria.ReplaceWith, _replaceCriteria.IsMatch);
                        string result = objFind.FindText();
                        if (!string.Equals(targetData, result, StringComparison.OrdinalIgnoreCase))
                        {
                            row["RichText"] = result;
                            changeLog.Add(new ChangeLogModel() { FormInstanceID = formInstanceId, ElementPath = specificPath + ".RichText", Key = index.ToString(), OldValue = targetData, NewValue = result });
                        }
                    }
                    index = index + 1;
                }
            }
            else
            {
                targetData = elementToken.ToString();
                if (!string.IsNullOrWhiteSpace(targetData))
                {
                    FindnReplace objFind = new FindnReplace(targetData, _replaceCriteria.FindText, _replaceCriteria.ReplaceWith, _replaceCriteria.IsMatch);
                    string result = objFind.FindText();
                    if (!string.Equals(targetData, result, StringComparison.OrdinalIgnoreCase))
                    {
                        formData.SelectToken(specificPath).Replace(JToken.Parse(result));
                        changeLog.Add(new ChangeLogModel() { FormInstanceID = formInstanceId, ElementPath = specificPath, OldValue = targetData, NewValue = result });
                    }
                }
            }
        }
    }
}
