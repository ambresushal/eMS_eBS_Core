using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.FindnReplace
{
    public class AnnotationProcessor
    {
        private int _formInstanceId;
        private ReplaceCriteria _replaceCriteria;
        private IFormDesignService _formDesignServices;
        private IFormInstanceDataServices _formInstanceDataService;
        private IFolderVersionServices _folderVerionService;
        private int? _userId;
        private string _userName;
        private FormDesignVersionDetail _detail;
        private FormInstanceDataManager _formDataInstanceManager;

        public AnnotationProcessor(int formInstanceId, IFormDesignService formDesignServices, IFormInstanceDataServices dataService, IFolderVersionServices folderVerionService, int? userId, string userName, FormDesignVersionDetail detail)
        {
            this._formInstanceId = formInstanceId;
            this._formDesignServices = formDesignServices;
            this._formInstanceDataService = dataService;
            this._folderVerionService = folderVerionService;
            this._userId = userId;
            this._userName = userName;
            this._detail = detail;
        }

        public List<AnnotationViewModel> Process()
        {
            List<AnnotationViewModel> commentList = new List<AnnotationViewModel>();
            _formDataInstanceManager = new FormInstanceDataManager(1, _userId, _formInstanceDataService, _userName, _folderVerionService);
            commentList = ProcessDocument(_formInstanceId);
            return commentList;
        }

        private List<AnnotationViewModel> ProcessDocument(int formInstanceId)
        {
            List<AnnotationViewModel> commentLog = new List<AnnotationViewModel>();
            Parallel.ForEach(_detail.Sections.OrderBy(x => x.Sequence), sec =>
            {
                var log = ProcessSection(formInstanceId, sec.Name);
                commentLog.AddRange(log);
            });

            return commentLog;
        }

        private List<AnnotationViewModel> ProcessSection(int formInstanceId, string sectionName)
        {
            List<AnnotationViewModel> commentLog = new List<AnnotationViewModel>();
            SectionDesign sectionDesign = GetSectionDesignData(sectionName);
            if (sectionDesign != null)
            {
                JObject sectionData = GetSectionData(sectionDesign.FullName);
                this.ProcessElements(formInstanceId, sectionDesign.Elements, sectionData, "Section", ref commentLog);
            }

            return commentLog;
        }

        private void ProcessElements(int formInstanceId, List<ElementDesign> elementList, JObject sectionData, string parent, ref List<AnnotationViewModel> commentLog)
        {
            foreach (ElementDesign element in elementList)
            {
                if (element.Section != null)
                {
                    ProcessElements(formInstanceId, element.Section.Elements, sectionData, "Section", ref commentLog);
                }
                else
                {
                    try
                    {
                        if (element.IsRichTextBox)
                        {
                            ProcessElement(formInstanceId, element, parent, sectionData, ref commentLog);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        string customMsg = "An error occurred while finding annotation for the field  => " + element.FullName;
                        Exception customException = new Exception(customMsg, ex);
                        ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                    }
                }
            }
        }

        private void ProcessElement(int formInstanceId, ElementDesign element, string parent, JObject sectionData, ref List<AnnotationViewModel> commentLog)
        {
            string targetData = string.Empty;
            if (string.Equals(parent, "Section"))
            {
                targetData = sectionData.SelectToken(element.FullName).ToString();
                if (!string.IsNullOrWhiteSpace(targetData))
                {
                    AnnotationExtractor objExtractor = new AnnotationExtractor(targetData);
                    var result = objExtractor.GetAnnotation();
                    result.ForEach(a => { a.ElementPath = element.FullName; a.Field = element.Label; });
                    commentLog.AddRange(result);
                }
            }

        }

        private SectionDesign GetSectionDesignData(string sectionName)
        {
            return _detail.Sections.Where(s => s.Name == sectionName).FirstOrDefault();
        }

        private JObject GetSectionData(string sectionName)
        {
            string sectionData = _formDataInstanceManager.GetSectionData(_formInstanceId, sectionName, false, _detail, false, false);
            return JObject.Parse(sectionData);
        }

    }
}
