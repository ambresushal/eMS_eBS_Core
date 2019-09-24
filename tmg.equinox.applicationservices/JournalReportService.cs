using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersionReport;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.applicationservices
{
    public partial class JournalReportService : IJournalReportService
    {
        #region Private Memebers

        private IUnitOfWork _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }

        #endregion Private Members

        #region Constructor

        public JournalReportService(IUnitOfWork unitOfWork, IFolderVersionServices folderVersionService)
        {
            this._unitOfWork = unitOfWork;
            this._folderVersionService = folderVersionService;
        }
        #endregion Constructor

        #region ServicesMethod


        /// <summary>
        /// Gets All Journal List for current formInstance with all folderversions
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public List<JournalViewModel> GetAllJournalsList(int formInstanceId, int folderVersionId, int folderId, FormDesignVersionDetail detail)
        {
            List<JournalViewModel> sortedJournalList = new List<JournalViewModel>();
            List<FormInstanceViewModel> formsList = null;
            Dictionary<string, JournalViewModel> updatedJournalList = new Dictionary<string, JournalViewModel>();
            try
            {
                var forms = (from f in this._unitOfWork.Repository<FormInstance>()
                                                                .Query()
                                                                .Filter(f => f.FormInstanceID == formInstanceId)
                                                                .Get()
                             select new FormInstanceViewModel
                             {
                                 Name = f.Name
                             });

                if (forms != null)
                {
                    formsList = forms.ToList();
                }

                string name = formsList[0].Name;
                var journals = (from j in this._unitOfWork.Repository<Journal>()
                                                            .Query()
                                                            .Filter(f => f.FormInstance.Name == name && f.FormInstance.FolderVersionID <= folderVersionId)
                                                            .Include(j => j.AddedWorkFlowState)
                                                            .Include(j => j.ClosedWorkFlowState)
                                                            .Get()

                                select new JournalViewModel
                                {
                                    JournalID = j.JournalID,
                                    FormInstanceID = j.FormInstanceID,
                                    FolderVersionID = j.FolderVersionID,
                                    Description = j.Description,
                                    FieldName = j.FieldName,
                                    FieldPath = j.FieldPath,
                                    ActionID = j.ActionID,
                                    ActionName = j.JournalAction.ActionName,
                                    AddedWFStateID = j.AddedWorkFlowState.WorkFlowVersionStateID,
                                    AddedWFStateName = j.AddedWorkFlowState.WorkFlowState.WFStateName,
                                    ClosedWFStateID = (j.ClosedWFStateID != null ? j.ClosedWFStateID : null),
                                    ClosedWFStateName = (j.ClosedWFStateID != null ? j.ClosedWorkFlowState.WorkFlowState.WFStateName : string.Empty),
                                    AddedDate = j.AddedDate,
                                    AddedBy = j.AddedBy,
                                    UpdatedDate = j.UpdatedDate,
                                    UpdatedBy = j.UpdatedBy,
                                    FolderVersionNumber = j.FolderVersion.FolderVersionNumber
                                }).ToList();

                GetSortedJournalEntries(journals, updatedJournalList, detail, ref sortedJournalList);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return sortedJournalList;
        }

        /// <summary>
        /// Gets All Journal List for current formInstance with current folderversion
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        public List<JournalViewModel> GetCurrentVersionJournalsList(int formInstanceId, int folderVersionId, int formDesignVersionId, int tenantId, FormDesignVersionDetail detail)
        {
            List<JournalViewModel> sortedJournalList = new List<JournalViewModel>();
            try
            {
                Dictionary<string, JournalViewModel> updatedJournalList = new Dictionary<string, JournalViewModel>();

                var journals = (from j in this._unitOfWork.Repository<Journal>()
                                                            .Query()
                                                            .Filter(j => j.FormInstanceID == formInstanceId && j.FolderVersionID == folderVersionId)
                                                            .Include(j => j.AddedWorkFlowState)
                                                            .Include(j => j.ClosedWorkFlowState)
                                                            .Get()

                                select new JournalViewModel
                                {
                                    JournalID = j.JournalID,
                                    FormInstanceID = j.FormInstanceID,
                                    FolderVersionID = j.FolderVersionID,
                                    Description = j.Description,
                                    FieldName = j.FieldName,
                                    FieldPath = j.FieldPath,
                                    ActionID = j.ActionID,
                                    ActionName = j.JournalAction.ActionName,
                                    AddedWFStateID = j.AddedWorkFlowState.WorkFlowVersionStateID,
                                    AddedWFStateName = j.AddedWorkFlowState.WorkFlowState.WFStateName,
                                    ClosedWFStateID = (j.ClosedWFStateID != null ? j.ClosedWFStateID : null),
                                    ClosedWFStateName = (j.ClosedWFStateID != null ? j.ClosedWorkFlowState.WorkFlowState.WFStateName : string.Empty),
                                    AddedDate = j.AddedDate,
                                    AddedBy = j.AddedBy,
                                    UpdatedDate = j.UpdatedDate,
                                    UpdatedBy = j.UpdatedBy,
                                    FolderVersionNumber = j.FolderVersion.FolderVersionNumber
                                }).ToList();

                GetSortedJournalEntries(journals, updatedJournalList, detail, ref sortedJournalList);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return sortedJournalList;
        }

        private void GetSortedJournalEntries(List<JournalViewModel> journals, Dictionary<string, JournalViewModel> updatedJournalList, FormDesignVersionDetail detail, ref List<JournalViewModel> sortedJournalList)
        {
            if (journals != null && journals.Any())
            {

                foreach (var item in journals)
                {
                    string generatedName = "";
                    Regex regex = new Regex("[^a-zA-Z0-9.]");
                    var fieldName = GetGeneratedName(item.FieldName);

                    var fullName = item.FieldPath + " => " + fieldName;
                    fullName = fullName.Replace(" => ", ".");
                    generatedName = regex.Replace(fullName, String.Empty);


                    updatedJournalList.Add(generatedName + "_" + item.JournalID, item);

                }
                foreach (var sec in detail.Sections)
                {
                    SectionJournalEntry(sec, ref sortedJournalList, updatedJournalList);
                }
            }
        }

        private void SectionJournalEntry(SectionDesign section, ref List<JournalViewModel> sortedJournalList, Dictionary<string, JournalViewModel> updatedJournalList)
        {
            //Check for journal entry in list.               
            var journalEntries = updatedJournalList.Where(k => k.Key.Split('_')[0] == section.FullName).ToDictionary(s => s.Key, s => s.Value);
            if (journalEntries.Any())
            {
                sortedJournalList.AddRange(journalEntries.Values.OrderBy(j => j.FormInstanceID));
            }

            foreach (var ele in section.Elements)
            {
                ElementJournalEntry(ele, ref sortedJournalList, updatedJournalList);
            }
        }

        private void ElementJournalEntry(ElementDesign element, ref List<JournalViewModel> sortedJournalList, Dictionary<string, JournalViewModel> updatedJournalList)
        {
            //Check for journal entry in list
            if (element.Section != null)
            {
                SectionJournalEntry(element.Section, ref sortedJournalList, updatedJournalList);
            }
            else
            {
                var journalEntries = updatedJournalList.Where(k => k.Key.Split('_')[0] == element.FullName).ToDictionary(s => s.Key, s => s.Value);
                if (journalEntries.Any())
                {
                    sortedJournalList.AddRange(journalEntries.Values.OrderBy(j => j.FormInstanceID));
                }
            }
        }

        private string GetGeneratedName(string label)
        {
            string generatedName = "";
            if (!String.IsNullOrEmpty(label))
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                generatedName = regex.Replace(label, String.Empty);
                if (generatedName.Length > 70)
                {
                    generatedName = generatedName.Substring(0, 70);
                }

                Regex checkDigits = new Regex("^[0-9]");

                //if Label starts with numeric characters, this will append a character at the beginning.
                if (checkDigits.IsMatch(label, 0))
                {
                    generatedName = "a" + generatedName;
                }
            }
            return generatedName;
        }

        public List<JournalViewModel> GetCurrentJournal(int journalId)
        {
            List<JournalViewModel> journal = null;
            try
            {
                var journals = (from j in this._unitOfWork.Repository<Journal>()
                                                            .Query()
                                                            .Filter(j => j.JournalID == journalId)
                                                            .Include(j => j.AddedWorkFlowState)
                                                            .Include(j => j.ClosedWorkFlowState)
                                                            .Get()

                                select new JournalViewModel
                                {
                                    JournalID = j.JournalID,
                                    FormInstanceID = j.FormInstanceID,
                                    FolderVersionID = j.FolderVersionID,
                                    Description = j.Description,
                                    FieldName = j.FieldName,
                                    FieldPath = j.FieldPath,
                                    ActionID = j.ActionID,
                                    ActionName = j.JournalAction.ActionName,
                                    AddedWFStateID = j.AddedWorkFlowState.WorkFlowVersionStateID,
                                    AddedWFStateName = j.AddedWorkFlowState.WorkFlowState.WFStateName,
                                    ClosedWFStateID = (j.ClosedWFStateID != null ? j.ClosedWFStateID : null),
                                    ClosedWFStateName = (j.ClosedWFStateID != null ? j.ClosedWorkFlowState.WorkFlowState.WFStateName : string.Empty),
                                    AddedDate = j.AddedDate,
                                    AddedBy = j.AddedBy,
                                    UpdatedDate = j.UpdatedDate,
                                    UpdatedBy = j.UpdatedBy,
                                    FolderVersionNumber = j.FolderVersion.FolderVersionNumber
                                });
                if (journals != null)
                {
                    journal = journals.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return journal;
        }

        /// <summary>
        /// Gets all Resposnes for current Journal.
        /// </summary>
        /// <param name="journalId"></param>
        /// <returns></returns>
        public List<JournalResponseViewModel> GetAllJournalResponsesList(int journalId)
        {
            List<JournalResponseViewModel> journalResponsesList = null;

            try
            {
                //var jrlist = (from c in this._unitOfWork.Repository<JournalResponse>()
                //                .Query()
                //                .Include(c => c.Journal)
                //                .Filter(c => c.JournalID == journalId)
                //                .Get()

                var jrlist = (from c in this._unitOfWork.Repository<JournalResponse>().Get()
                              join j in this._unitOfWork.Repository<Journal>().Get()
                              on c.JournalID equals j.JournalID
                              where c.JournalID == journalId

                              select new JournalResponseViewModel
                              {
                                  JournalResponseID = c.JournalResponseID,
                                  JournalID = c.JournalID,
                                  Description = c.Description,
                                  AddedDate = c.AddedDate,
                                  AddedBy = c.AddedBy,
                                  UpdatedDate = c.UpdatedDate,
                                  UpdatedBy = c.UpdatedBy
                              });

                if (jrlist != null)
                {
                    journalResponsesList = jrlist.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return journalResponsesList;

        }

        public List<JournalActionViewModel> GetAllActionList()
        {
            List<JournalActionViewModel> journalActionList = null;
            try
            {
                var Actions = (from c in this._unitOfWork.Repository<JournalAction>()
                                    .Query()
                                    .Get()
                               select new JournalActionViewModel
                               {
                                   ActionId = c.ActionID,
                                   ActionName = c.ActionName,
                               });

                if (Actions.Count() > 0)
                {
                    journalActionList = Actions.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return journalActionList;
        }

        public ServiceResultStatus SaveJournalEntry(int formInstanceID, int folderVersionID, string description, string fieldName, string fieldPath, int actionID, int addedWFStateID, int? closedWFStateID, string addedBy)
        {
            Journal itemToAdd = new Journal();
            ServiceResult result = new ServiceResult();
            try
            {
                itemToAdd.FormInstanceID = formInstanceID;
                itemToAdd.FolderVersionID = folderVersionID;
                itemToAdd.Description = description;
                itemToAdd.FieldName = fieldName;
                itemToAdd.FieldPath = fieldPath;
                itemToAdd.ActionID = actionID;
                itemToAdd.AddedWFStateID = addedWFStateID;
                itemToAdd.ClosedWFStateID = closedWFStateID;
                itemToAdd.AddedBy = addedBy;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.UpdatedBy = null;
                itemToAdd.UpdatedDate = null;

                this._unitOfWork.Repository<Journal>().Insert(itemToAdd);
                this._unitOfWork.Save();

                this._folderVersionService.UpdateFolderChange(1, addedBy, null, folderVersionID);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result.Result = ServiceResultStatus.Success;
        }

        /// <summary>
        /// This function updates Journal Entry with Closed Workflow state & updated date.
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="closedWFStateID"></param>
        /// <param name="updatedBy"></param>
        /// <param name="journalId"></param>
        /// <returns></returns>
        public ServiceResultStatus UpdateJournalEntry(int actionID, int closedWFStateID, string updatedBy, int journalId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                Journal itemToUpdate = this._unitOfWork.Repository<Journal>()
                                                               .FindById(journalId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.ActionID = actionID;
                    itemToUpdate.ClosedWFStateID = closedWFStateID;
                    itemToUpdate.UpdatedBy = updatedBy;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    //Call to repository method to Update record.
                    this._unitOfWork.Repository<Journal>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    this._folderVersionService.UpdateFolderChange(1, updatedBy, null, itemToUpdate.FolderVersionID);
                    //Return success result
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result.Result;
            //return result.Result = ServiceResultStatus.Success;

        }

        /// <summary>
        /// To save data to database for Journal Response
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="folderVersionID"></param>
        /// <param name="description"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldPath"></param>
        /// <param name="actionID"></param>
        /// <param name="addedWFStateID"></param>
        /// <param name="closedWFStateID"></param>
        /// <param name="addedBy"></param>
        /// <returns></returns>
        public ServiceResultStatus SaveJournalResponse(string response, string addedBy, int journalId)
        {
            JournalResponse itemToAdd = new JournalResponse();
            ServiceResult result = new ServiceResult();
            try
            {
                itemToAdd.JournalID = journalId;
                itemToAdd.Description = response;
                itemToAdd.AddedBy = addedBy;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.UpdatedBy = null;
                itemToAdd.UpdatedDate = null;

                this._unitOfWork.Repository<JournalResponse>().Insert(itemToAdd);
                this._unitOfWork.Save();

                Journal journalEntry = this._unitOfWork.Repository<Journal>()
                                                               .FindById(journalId);
                this._folderVersionService.UpdateFolderChange(1, addedBy, null, journalEntry.FolderVersionID);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result.Result = ServiceResultStatus.Success;
        }

        public bool CheckAllJournalEntryIsClosed(int folderVersionId, int formInstanceId)
        {
            var journalListCount = this._unitOfWork.Repository<Journal>()
                                                    .Query()
                                                    .Filter(c => c.FolderVersionID == folderVersionId && c.ActionID == 1 && c.FormInstanceID == formInstanceId)
                                                    .Get()
                                                    .ToList();
            if (journalListCount.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion ServicesMethod

        #region Private Methods



        #endregion Private Methods
    }
}
