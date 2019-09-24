using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;

namespace tmg.equinox.applicationservices
{
    public class FolderService : IFolderService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
   
        public FolderService(IUnitOfWorkAsync unitOfWork, IFolderVersionServices folderVersionService)
        {
            this._unitOfWork = unitOfWork;
            this._folderVersionService = folderVersionService;
        }

        public FolderViewModel GetFolderById(int folderId)
        {
            FolderViewModel folder = null;
            try
            {
                folder = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                          where (fld.FolderID == folderId)
                          select new FolderViewModel
                          {
                              FolderID = fld.FolderID,
                              FolderName = fld.Name,
                              MarketSegment = fld.MarketSegment.Description,
                              PrimaryContact = fld.PrimaryContent
                          }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folder;
        }

        public List<FolderViewModel> GetFoldersByAccount(int accountId)
        {
            List<FolderViewModel> folders = null;
            try
            {
                //var currentFolders = from s in "st"
                //                     select new
                //                     {
                //                         FolderID = 0,
                //                         FolderVersionID = 0,
                //                         VersionCount = 0
                //                     };

                //currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                //                 join f in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                //                 on s.FolderVersionID equals f.FolderVersionID
                //                 join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(fd => fd.IsMasterList != true) //&& fd.DocumentDesignTypeID == 1
                //                 on f.FormDesignID equals fd.FormID
                //                 join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio != true)
                //                 on s.FolderID equals fl.FolderID
                //                 where s.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.RELEASED ||
                //                        s.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.INPROGRESS
                //                 group s by s.FolderID into g
                //                 select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Where(x => x.FolderVersionStateID != (int)domain.entities.Enums.FolderVersionState.BASELINED).Distinct().Count() };

              //  List<int> queuedFolders = _facetTranslatorService.GetFoldersQueuedForMigration();


                folders = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                           join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fldv.FolderID equals fldr.FolderID
                           //join c in currentFolders on new { fldv.FolderID, fldv.FolderVersionID } equals new { c.FolderID, c.FolderVersionID }
                           join cat in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get()
                               on fldv.CategoryID equals cat.FolderVersionCategoryID
                           join cons in this._unitOfWork.RepositoryAsync<Consortium>().Get() on fldv.ConsortiumID equals cons.ConsortiumID
                           into wt1
                           from wt in wt1.DefaultIfEmpty()
                           join map in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fldr.FolderID equals map.FolderID
                           join accn in this._unitOfWork.RepositoryAsync<Account>().Get() on map.AccountID equals accn.AccountID
                           join st in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get() on fldv.WFStateID equals st.WorkFlowVersionStateID
                           join fldvws in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get() on new { p1 = fldv.FolderVersionID, p2 = fldv.WFStateID }
                           equals new { p1 = fldvws.FolderVersionID, p2 = (int?)fldvws.WFStateID }
                         //  join pmq in queuedFolders on fldr.FolderID equals pmq into gpmq
                          // from pmqn in gpmq.DefaultIfEmpty()
                           where (accn.IsActive == true
                                 && accn.AccountID == accountId
                                 && (fldr.IsPortfolio != true)
                                 && fldr.Name != "Master List"
                                 && (fldv.FolderVersionStateID != (int)domain.entities.Enums.FolderVersionState.BASELINED && fldv.IsActive == true)
                                  )
                           select new FolderViewModel
                           {
                               FolderID = fldr.FolderID,
                               FolderName = fldr.Name,
                               MarketSegment = fldr.MarketSegment.Description,
                               PrimaryContact = fldr.PrimaryContent
                           }).Distinct().ToList();


                //folders = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                //           join accfldmp in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                //           on fld.FolderID equals accfldmp.FolderID
                //           where (accfldmp.AccountID == accountId)
                //           select new FolderViewModel
                //           {
                //               FolderID = fld.FolderID,
                //               FolderName = fld.Name,
                //               MarketSegment = fld.MarketSegment.Description,
                //               PrimaryContact = fld.PrimaryContent
                //           }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folders;
        }

        public FolderViewModel GetFolderByName(string folderName)
        {
            FolderViewModel folder = null;
            try
            {
                folder = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                          where (fld.Name == folderName)
                          select new FolderViewModel
                          {
                              FolderID = fld.FolderID,
                              FolderName = fld.Name,
                              MarketSegment = fld.MarketSegment.Description,
                              PrimaryContact = fld.PrimaryContent
                          }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folder;
        }
        public List<FolderViewModel> GetPortfolioFolders(int skip, int pageSize, ref int total)
        {
            List<FolderViewModel> folders = null;
            try
            {
                total = this._unitOfWork.RepositoryAsync<Folder>().Get().Count();

                folders = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                           where (fld.IsPortfolio == true)
                           orderby fld.FolderID
                           select new FolderViewModel
                           {
                               FolderID = fld.FolderID,
                               FolderName = fld.Name,
                               MarketSegment = fld.MarketSegment.Description,
                               PrimaryContact = fld.PrimaryContent
                           }).Skip(skip).Take(pageSize).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folders;
        }

        public List<FolderViewModel> GetFolderList(int skip, int pageSize, ref int total)
        {
            List<FolderViewModel> folders = null;
            try
            {
                total = this._unitOfWork.RepositoryAsync<Folder>().Get().Count();

                folders = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                           where (fld.IsPortfolio == false)
                           orderby fld.FolderID
                           select new FolderViewModel
                           {
                               FolderID = fld.FolderID,
                               FolderName = fld.Name,
                               MarketSegment = fld.MarketSegment.Description,
                               PrimaryContact = fld.PrimaryContent
                           }).Skip(skip).Take(pageSize).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folders;
        }

        public ServiceResult AddFolder(int tenantId, int? accountId, string folderName, DateTime effectiveDate, bool isPortfolio, int? userId, string primaryContact, string marketSegment, string category)
        {
            ServiceResult result = null;
            FolderVersionViewModel addedFolder = null;
            try
            {
                string catID = string.Empty;
                string addedBy = primaryContact;
                int marketSegmentId = this._unitOfWork.RepositoryAsync<MarketSegment>().Get().Where(s => s.MarketSegmentName == marketSegment).Select(s => s.MarketSegmentID).FirstOrDefault();
                int? categoryID = this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get().Where(s => s.FolderVersionCategoryName == category).Select(s => s.FolderVersionCategoryID).FirstOrDefault();

                result = new ServiceResult();
                if (accountId == null)
                {
                    if (!this._unitOfWork.RepositoryAsync<Folder>().IsFolderNameExists(tenantId, 0, folderName))
                    {
                        addedFolder = CreateFolderWithVersion(tenantId, accountId, folderName, effectiveDate, isPortfolio, userId, primaryContact, marketSegmentId, 0, categoryID, catID, addedBy, null);
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "This Folder Name already exists for this Account. Please enter a different Folder Name." } });
                    }
                }
                else
                {
                    if (!this._unitOfWork.RepositoryAsync<AccountFolderMap>().IsFolderNameExistsInAccount(tenantId, accountId, 0, folderName))
                    {
                        addedFolder = CreateFolderWithVersion(tenantId, accountId, folderName, effectiveDate, isPortfolio, userId, primaryContact, marketSegmentId, null, categoryID, catID, addedBy, null);
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "This Folder Name already exists for this Account. Please enter a different Folder Name." } });
                    }
                }
                this._unitOfWork.Save();
                if (addedFolder != null)
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { addedFolder.FolderId.ToString() } });
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

        public ServiceResult DeleteFolder(int tenantId, int folderId)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                var folder = this._unitOfWork.RepositoryAsync<Folder>().Get().Where(s => s.FolderID == folderId).FirstOrDefault();
                if (folder == null)
                {
                    result.Result = ServiceResultStatus.Failure;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "This Folder does not exist." } });
                }
                else
                {
                    result = _folderVersionService.DeleteFolder(tenantId, folderId);
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

        private FolderVersionViewModel CreateFolderWithVersion(int tenantId, int? accountId, string folderName, DateTime folderEffectiveDate, bool isPortfolio, int? userId, string primaryContact, int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string addedBy, int? originalFolderID)
        {
            int? formDesignGroupID = null;

            if (!isPortfolio)
            {
                formDesignGroupID = this._unitOfWork.RepositoryAsync<FormDesignGroup>().GetFormDeisgnGroupID(tenantId, folderType: "Account");
            }
            if (accountId == null)
            {
                formDesignGroupID = this._unitOfWork.RepositoryAsync<FormDesignGroup>().GetFormDeisgnGroupID(tenantId, folderType: folderName);
                consortiumID = null;
                formDesignGroupID = null;
            }


            Folder folderToAdd = new Folder();
            folderToAdd.TenantID = tenantId;
            folderToAdd.Name = folderName;
            folderToAdd.IsPortfolio = isPortfolio;
            folderToAdd.MarketSegmentID = marketSegmentId;
            folderToAdd.PrimaryContent = primaryContact;
            folderToAdd.PrimaryContentID = userId;
            folderToAdd.AddedBy = addedBy;
            folderToAdd.AddedDate = DateTime.Now;
            folderToAdd.ParentFolderId = originalFolderID;
            folderToAdd.FormDesignGroupId = formDesignGroupID;

            this._unitOfWork.RepositoryAsync<Folder>().Insert(folderToAdd);
            this._unitOfWork.Save();

            //Add entry to AccountFolderMap table
            if (accountId.HasValue)
            {
                AccountFolderMap accountFolderMap = new AccountFolderMap();
                accountFolderMap.AccountID = accountId.Value;
                accountFolderMap.FolderID = folderToAdd.FolderID;

                this._unitOfWork.RepositoryAsync<AccountFolderMap>().Insert(accountFolderMap);
            }
            WorkFlowVersionState workflowState = null;
            //Retrieve sequence of Setup State   
            if (categoryID != null)
            {
                workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)categoryID, isPortfolio);

            }
            //Adding Initial Minor Version and FolderVersionWorkFlowState for respective Folder   
            FolderVersionViewModel addVersion = new FolderVersionViewModel();
            if (workflowState != null)
            {
                addVersion = _folderVersionService.AddFolderVersion(tenantId, folderEffectiveDate, addedBy,
                                          folderToAdd.FolderID, workflowState.WorkFlowVersionStateID, consortiumID, categoryID, catID, userId); //Modification No.2
            }
            else
            {
                addVersion = _folderVersionService.AddFolderVersion(tenantId, folderEffectiveDate, addedBy,
                                              folderToAdd.FolderID, null, consortiumID, categoryID, catID, userId); //Modification No.2
            }


            return addVersion;
        }
    }
}
