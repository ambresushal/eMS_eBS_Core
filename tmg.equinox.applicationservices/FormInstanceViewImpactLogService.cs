using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class FormInstanceViewImpactLogService : IFormInstanceViewImpactLogService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        public FormInstanceViewImpactLogService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public ServiceResult SaveFormInstanceImpactlogData(int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, IList<SourceElement> viewImpactLog)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                if (viewImpactLog != null)
                {
                    //var formInstance = this._unitOfWork.Repository<FormInstance>().Query().Filter(c => c.FormInstanceID == formInstanceId).Get().FirstOrDefault();
                    int docID = (from frmInstance in this._unitOfWork.Repository<FormInstance>().Get()
                                 where frmInstance.FormInstanceID == formInstanceId
                                 select frmInstance.DocID).FirstOrDefault();
                    for (int i = 0; i < viewImpactLog.Count; i++)
                    {
                        FormInstanceViewImpactLog impactLog = new FormInstanceViewImpactLog();
                        impactLog.FormInstanceID = formInstanceId;
                        impactLog.FolderID = folderId;
                        impactLog.FolderVersionID = folderVersionId;
                        impactLog.FormDesignID = formDesignId;
                        impactLog.FormDesignVersionID = formDesignVersionId;
                        impactLog.ElementID = viewImpactLog[i].ElementID;
                        impactLog.ElementName = viewImpactLog[i].ElementName;
                        impactLog.ElementLabel = viewImpactLog[i].ElementLabel;
                        impactLog.ElementPath = viewImpactLog[i].ElementPath;
                        impactLog.ElementPathName = viewImpactLog[i].ElementPathName;
                        impactLog.Keys = viewImpactLog[i].Keys;
                        impactLog.Description = viewImpactLog[i].Description;
                        impactLog.ImpactedElements = JsonConvert.SerializeObject(viewImpactLog[i].ImpactedElements);
                        impactLog.UpdatedBy = viewImpactLog[i].UpdatedBy;
                        impactLog.UpdatedLast = DateTime.Now;
                        //impactLog.DocID = formInstance.DocID;
                        impactLog.DocID = docID;
                        this._unitOfWork.RepositoryAsync<FormInstanceViewImpactLog>().Insert(impactLog);
                    }

                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }
        public List<SourceElement> GetFormInstanceImpactLogData(int formInstanceID)
        {

            List<SourceElement> impactedFields = new List<SourceElement>();
            SourceElement srcElement = new SourceElement();
            SearchCriteria criteria = new SearchCriteria();
            try
            {

                List<FormInstanceViewImpactLog> viewImpactList = this._unitOfWork.Repository<FormInstanceViewImpactLog>().Query()
                                                                  .Filter(c => c.FormInstanceID == formInstanceID)
                                                                  .Get()
                                                                  .OrderByDescending(x => x.UpdatedLast).ThenByDescending(c => c.ImpactLoggerID).ToList();
                for (int i = 0; i < viewImpactList.Count; i++)
                {
                    srcElement = new SourceElement();
                    srcElement.ID = i;
                    srcElement.ElementID = viewImpactList[i].ElementID;
                    srcElement.ElementName = viewImpactList[i].ElementName;
                    srcElement.ElementLabel = viewImpactList[i].ElementLabel;
                    srcElement.ElementPath = viewImpactList[i].ElementPath;
                    srcElement.ElementPathName = viewImpactList[i].ElementPathName;
                    srcElement.Description = viewImpactList[i].Description;
                    srcElement.Keys = "";
                    srcElement.UpdatedBy = viewImpactList[i].UpdatedBy;
                    srcElement.UpdatedDate = viewImpactList[i].UpdatedLast;
                    srcElement.ImpactedElements = JsonConvert.DeserializeObject<List<ImpactedElement>>(viewImpactList[i].ImpactedElements);
                    int counter = 0;
                    srcElement.ImpactedElements.ForEach(x => x.ID = counter++);
                    impactedFields.Add(srcElement);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return impactedFields;
        }
        public List<ActivityLogModel> GetFormInstanceActivityLogData(int formInstanceID, string compiledList)
        {
            List<ActivityLogModel> activityLogData = new List<ActivityLogModel>();
            try
            {
                activityLogData = (from c in this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>()
                                   .Query()
                                   .Filter(c => c.FormInstanceID == formInstanceID)
                                   .Get()
                                   .OrderByDescending(x => x.UpdatedLast).ThenByDescending(c => c.FormInstanceActivityLogID)
                                   select new ActivityLogModel
                                   {
                                       Description = c.Description,
                                       SubSectionName = c.ElementPath,
                                       ElementPath = c.ElementPath,
                                       Field = c.Field,
                                       RowNum = c.RowNumber,
                                       UpdatedBy = c.UpdatedBy,
                                       UpdatedLast = c.UpdatedLast,
                                       IsNewRecord = false,
                                       FormInstanceID = c.FormInstanceID,
                                   }).ToList();
            }
            catch (Exception ex)
            {
            }
            return activityLogData;
        }

        public List<DropDownElementItem> GetDDLDropDownItems(int formDesignID)
        {
            List<DropDownElementItem> dropDownItems = new List<DropDownElementItem>();
            try
            {
                dropDownItems = (from item in this._unitOfWork.Repository<DropDownElementItem>().Query().Get()
                                     join element in this._unitOfWork.Repository<UIElement>().Query().Get()
                                     on item.UIElementID equals element.UIElementID
                                     where element.FormID == formDesignID && item.Value!=item.DisplayText
                                select item).ToList();
            }
            catch (Exception)
            {
            }
            return dropDownItems;
        }

      
    }
}
