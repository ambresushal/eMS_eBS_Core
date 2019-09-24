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
    public class ReplaceTextProcessor : IReplaceTextProcessor
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

        public ReplaceTextProcessor(DocumentInfo documentInfo, ReplaceCriteria criteria, IFormDesignService formDesignServices, IFormInstanceDataServices dataService,
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
                    activityLog = ProcessSection(_documentInfo.FormInstanceId, _documentInfo.CurrentSection);
                    break;
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
            int[] Ids = Array.ConvertAll(_replaceCriteria.SelectedInstances.Split(','), int.Parse);
            var formInstances = _folderVerionService.GetFormInstanceList(1, _documentInfo.FolderVersionId, _documentInfo.FolderId, 0, Ids);
            foreach (var doc in formInstances.OrderBy(s => s.FormInstanceID))
            {
                var log = ProcessDocument(doc.FormInstanceID);
                changeLog.AddRange(log);
            }

            return changeLog;
        }

        private List<ChangeLogModel> ProcessDocument(int formInstanceId)
        {
            List<ChangeLogModel> changeLog = new List<ChangeLogModel>();
            foreach (var sec in _detail.Sections.OrderBy(x => x.Sequence))
            {
                var log = ProcessSection(formInstanceId, sec.Name);
                changeLog.AddRange(log);
            }

            return changeLog;
        }

        private List<ChangeLogModel> ProcessSection(int formInstanceId, string sectionName)
        {
            List<ChangeLogModel> changeLog = new List<ChangeLogModel>();
            SectionDesign sectionDesign = GetSectionDesignData(sectionName);
            if (sectionDesign != null)
            {
                JObject sectionData = GetSectionData(sectionDesign.FullName);
                this.ProcessElements(formInstanceId, sectionDesign.Elements, sectionData, "Section", ref changeLog);
                _formDataInstanceManager.SetCacheData(_documentInfo.FormInstanceId, sectionDesign.FullName, sectionData.ToString());
                _formDataInstanceManager.SaveSectionsData(_documentInfo.FormInstanceId, true, _folderVerionService, _formDesignServices, _detail, sectionDesign.FullName);
            }

            return changeLog;
        }

        private void ProcessElements(int formInstanceId, List<ElementDesign> elementList, JObject sectionData, string parent, ref List<ChangeLogModel> changeLog)
        {
            foreach (ElementDesign element in elementList)
            {
                if (element.Section != null)
                {
                    ProcessElements(formInstanceId, element.Section.Elements, sectionData, "Section", ref changeLog);
                }
                else if (element.Repeater != null)
                {
                    ProcessElements(formInstanceId, element.Repeater.Elements, sectionData, "Repeater", ref changeLog);
                }
                else
                {
                    try
                    {
                        if (element.IsRichTextBox)
                        {
                            ProcessElement(formInstanceId, element, parent, sectionData, ref changeLog);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        string customMsg = "An error occurred while replacing text for element => " + element.FullName;
                        Exception customException = new Exception(customMsg, ex);
                        ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                    }
                }
            }
        }

        private void ProcessElement(int formInstanceId, ElementDesign element, string parent, JObject sectionData, ref List<ChangeLogModel> changeLog)
        {
            string targetData = string.Empty;
            if (string.Equals(parent, "Section"))
            {
                targetData = sectionData.SelectToken(element.FullName).ToString();
                if (!string.IsNullOrWhiteSpace(targetData))
                {
                    FindnReplace objFind = new FindnReplace(targetData, _replaceCriteria.FindText, _replaceCriteria.ReplaceWith, _replaceCriteria.IsMatch);
                    string result = objFind.FindText();
                    if (!string.Equals(targetData, result, StringComparison.OrdinalIgnoreCase))
                    {
                        sectionData.SelectToken(element.FullName).Replace(JToken.Parse(result));
                        changeLog.Add(new ChangeLogModel() { FormInstanceID = formInstanceId, ElementPath = element.FullName, OldValue = targetData, NewValue = result });
                    }
                }
            }
            else
            {
                int index = 0;
                string[] path = element.FullName.Split('.');
                List<JToken> repaterRows = sectionData.SelectToken(string.Join(".", path.Take(path.Length - 1))).ToList();
                foreach (var row in repaterRows)
                {
                    targetData = row[path[path.Length - 1]].ToString();
                    if (!string.IsNullOrWhiteSpace(targetData))
                    {
                        FindnReplace objFind = new FindnReplace(targetData, _replaceCriteria.FindText, _replaceCriteria.ReplaceWith, _replaceCriteria.IsMatch);
                        string result = objFind.FindText();
                        if (!string.Equals(targetData, result, StringComparison.OrdinalIgnoreCase))
                        {
                            row[path[path.Length - 1]] = result;
                            changeLog.Add(new ChangeLogModel() { FormInstanceID = formInstanceId, ElementPath = element.FullName, Key = index.ToString(), OldValue = targetData, NewValue = result });
                        }
                    }
                    index = index + 1;
                }
            }
        }

        private SectionDesign GetSectionDesignData(string sectionName)
        {
            return _detail.Sections.Where(s => s.Name == sectionName).FirstOrDefault();
        }

        private JObject GetSectionData(string sectionName)
        {
            string sectionData = _formDataInstanceManager.GetSectionData(_documentInfo.FormInstanceId, sectionName, false, _detail, false, false);
            return JObject.Parse(sectionData);
        }
    }
}
