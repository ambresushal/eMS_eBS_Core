using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.mlcascade.documentcomparer;
using tmg.equinox.domain.viewmodels;

namespace tmg.equinox.core.masterlistcascade
{
    class ActivityLogger
    {
        private int _formInstanceID;
        private int _previousFormInstanceID;
        private int _userID;
        private string _userName;
        private FormDesignVersionDetail _detail;
        private DocumentFilterResult _previousDocumentResult;
        private DocumentFilterResult _currentDocumentResult;
        private MasterListCascadeViewModel _mlcViewModel;
        private IFolderVersionServices _folderVersionService;
        internal ActivityLogger(FormDesignVersionDetail detail,DocumentFilterResult previousDocumentResult,DocumentFilterResult currentDocumentResult, MasterListCascadeViewModel mlcViewModel,IFolderVersionServices folderVersionService,int userID, string userName)
        {
            _formInstanceID = currentDocumentResult.FormInstanceID;
            _previousFormInstanceID = previousDocumentResult.FormInstanceID; 
            _detail = detail;
            _previousDocumentResult = previousDocumentResult;
            _currentDocumentResult = currentDocumentResult;
            _mlcViewModel = mlcViewModel;
            _folderVersionService = folderVersionService;
            _userID = userID;
            _userName = userName;
        }

        internal void LogChanges()
        {
            string sourceDocument = _folderVersionService.GetFormInstanceDataCompressed(1, _previousFormInstanceID);
            string targetDocument = _folderVersionService.GetFormInstanceDataCompressed(1, _formInstanceID);
            CompareDocument source = GetCompareDocument(_previousDocumentResult);
            CompareDocument target = GetCompareDocument(_currentDocumentResult);
            string macro = _mlcViewModel.CompareMacroJSON;
            string matchType = "";
            DocumentComparer comparer = new DocumentComparer(sourceDocument, targetDocument, source,target,macro,matchType, CompareDocumentSource.GENERATEREPORT);
            DocumentCompareResult result = comparer.Compare();
            LogActivity(result);
        }

        internal CompareDocument GetCompareDocument(DocumentFilterResult result)
        {
            CompareDocument doc = new CompareDocument();
            doc.AccountName = "";
            doc.DocumentName = "";
            doc.DocumentType = "";
            doc.EffectiveDate = result.EffectiveDate.ToShortDateString();
            doc.FolderName = "";
            doc.FolderVerionNumber = result.FolderVersionNumber;
            return doc;
        }

        private void LogActivity(DocumentCompareResult result)
        {
            List<ActivityLogModel> activityLogs = new List<ActivityLogModel>();
            if (result.IsMatch == false)
            {
                if(result.Results != null)
                {
                    foreach(var res in result.Results)
                    {
                        if(res is SectionCompareResult)
                        {
                            SectionCompareResult sectionRes = (SectionCompareResult)res;
                            if(sectionRes.IsMatch == false)
                            {
                                LogActivityForSection((SectionCompareResult)res, ref activityLogs);
                            }
                        }
                        if(res is RepeaterCompareResult)
                        {
                            RepeaterCompareResult repeaterRes = (RepeaterCompareResult)res;
                            if(repeaterRes.IsMatch == false)
                            {
                                LogActivityForRepeater(repeaterRes, ref activityLogs);
                            }
                        }
                    }
                }
            }
            _folderVersionService.SaveFormInstanceAvtivitylogData(1, _formInstanceID, _currentDocumentResult.FolderID, _currentDocumentResult.FolderVersionID, _currentDocumentResult.FormDesignID,
                _currentDocumentResult.FormDesignVersionID, activityLogs);
        }

        private void LogActivityForSection(SectionCompareResult result,ref List<ActivityLogModel> activityLogs)
        {
            if (result.Fields != null)
            {
                foreach(var field in result.Fields)
                {
                    if(field.IsMatch == false)
                    {
                        ActivityLogModel model = new ActivityLogModel();
                        string activityDescription = "Value of {0} is changed from {1} to {2}.";
                        model.Description = String.Format(activityDescription, field.FieldName, field.SourceValue, field.TargetValue);
                        model.ElementPath = result.Path.Replace("."," => ");
                        model.Field = field.FieldName;
                        model.FolderVersionName = "";
                        model.FormInstanceID = _formInstanceID;
                        model.IsNewRecord = true;
                        model.SubSectionName = "";
                        model.UpdatedBy = _userName;
                        model.RowNum = "";
                        model.UpdatedLast = DateTime.Now;
                        activityLogs.Add(model);
                    }
                }
            }
        }

        private void LogActivityForRepeater(RepeaterCompareResult result,ref List<ActivityLogModel> activityLogs)
        {
            
            if (result.Rows != null)
            {
                int idx = 0;
                foreach (var row in result.Rows)
                {
                    idx++;
                    if (row.IsMatch == false)
                    {
                        foreach(var field in row.Fields)
                        {
                            if(field.IsMatch == false)
                            {
                                ActivityLogModel model = new ActivityLogModel();
                                string activityDescription = "Row {0} in {1} - [{2}] was changed from {3} to {4}.";
                                model.Description = String.Format(activityDescription, idx, result.RepeaterName, field.FieldName, field.SourceValue, field.TargetValue);
                                model.ElementPath = result.Path.Replace(".", " => ");
                                model.Field = field.FieldName;
                                model.FolderVersionName = "";
                                model.FormInstanceID = _formInstanceID;
                                model.IsNewRecord = true;
                                model.SubSectionName = "";
                                model.UpdatedBy = _userName;
                                model.RowNum = "";
                                model.UpdatedLast = DateTime.Now;
                                activityLogs.Add(model);
                            }
                        }
                    }
                }
            }
        }
    }
}
