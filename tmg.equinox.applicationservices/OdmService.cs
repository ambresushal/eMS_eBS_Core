using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.domain.entities;
using tmg.equinox.applicationservices.viewmodels.CompareSync;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.applicationservices.PBPImport;
using System.Configuration;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.repository.Models.Mapping;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.applicationservices
{
    public partial class OdmService : IOdmService
    {
        #region Private Members
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IPBPImportService _pBPImportService;
        private IFolderVersionServices _folderVersionservice;
        //string ODMFILEPATH = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ODMPath"]);       
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public OdmService(IUnitOfWorkAsync unitOfWork, IPBPImportService pBPImportService, IFolderVersionServices folderVersionservice)
        {
            _unitOfWork = unitOfWork;
            this._pBPImportService = pBPImportService;
            this._folderVersionservice = folderVersionservice;
        }
        #endregion Constructor

        #region Public Methods

        public IEnumerable<ODMParentSectionDeatilsViewModel> GetParentSectionsFromFolderVersion(int tenantId, int formDesignVersionId)
        {
            IList<ODMParentSectionDeatilsViewModel> uiElementRowModelList = null;
            try
            {
                //get all elements
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetParentSectionForFormDesignVersion(tenantId, formDesignVersionId).ToList();
                var alternateLabels = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get().Where(s => s.FormDesignVersionID == formDesignVersionId).ToList();

                if (elementList.Count() > 0)
                {
                    uiElementRowModelList = (from i in elementList
                                             select new ODMParentSectionDeatilsViewModel
                                             {
                                                 Label = alternateLabels.Where(e => e.UIElementID == i.UIElementID).Select(s => s.AlternateLabel).FirstOrDefault() ?? (i.Label == null ? "[Blank]" : i.Label),
                                                 UIElementID = i.UIElementID,
                                                 //UIElementName = i.UIElementName,
                                                 GeneratedName = i.GeneratedName,
                                                 SectionName = i.Label,
                                                 SelectSection = "No"
                                             }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementRowModelList;
        }

        public List<string> SaveFiles(HttpRequestBase Request, string ODMFILEPATH)
        {
            List<string> FileNameList = new List<string>();
            //  Get all files from Request object  
            HttpFileCollectionBase files = Request.Files;
            List<string> FileNamList = new List<string>();
            string FileName, UniqueName = "";
            for (int i = 0; i < files.Count; i++)
            {
                UniqueName = Guid.NewGuid().ToString();
                HttpPostedFileBase file = files[i];
                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    FileName = testfiles[testfiles.Length - 1];
                }
                else
                {
                    FileName = file.FileName;
                }

                string[] SplitStr = FileName.Split('.');
                string FileUniqueName = SplitStr[0] + UniqueName + "." + SplitStr[1];


                string fname = Path.Combine(ODMFILEPATH, FileUniqueName);
                file.SaveAs(fname);
                FileNameList.Add(FileUniqueName);
                FileNameList.Add(FileName);
                // Get the complete folder path and store the file inside it.  
            }
            return FileNameList;
        }

        public ODMConfigrationViewModel GetPlanDetails(HttpRequestBase Request, string file, string ODMFILEPATH)
        {
            ODMConfigrationViewModel viewModel = new ODMConfigrationViewModel();

            viewModel.Description = Request.Params["description"].ToString();
            viewModel.Year = this.GetContractYear(file, ODMFILEPATH);

            AccessDBTableService ServiceObj1 = new AccessDBTableService(ODMFILEPATH + "\\" + file);

            IList<PBPPlanViewModel> pBPPlanList = ServiceObj1.ReadPBPTableData();

            List<ODMPlanConfigViewModel> ebsPlan = (from p in pBPPlanList
                                                    join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                    on p.QID equals fi.Name
                                                    join fl in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                                    on fi.FolderVersionID equals fl.FolderVersionID
                                                    join f in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                                    on fl.FolderID equals f.FolderID
                                                    join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                                    on fi.FormDesignID equals fd.FormID
                                                    where fl.FolderVersionStateID == 1
                                                    && fl.FolderVersionNumber.Split('_')[0] == viewModel.Year.ToString()
                                                    && fi.FormInstanceID == fi.AnchorDocumentID
                                                    && fi.FormDesignID == GlobalVariables.MedicalDesignID
                                                    select new ODMPlanConfigViewModel
                                                    {
                                                        QID = p.QID,
                                                        SOTFormInstanceId = fi.FormInstanceID,
                                                        // PBPFormInstanceId = ebsView.Where(a => a.AnchorDocumentID == fi.AnchorDocumentID).FirstOrDefault() == null ? 0 : ebsView.Where(a => a.AnchorDocumentID == fi.AnchorDocumentID).FirstOrDefault().FormInstanceId,
                                                        PBPFormInstanceId = (from pbp in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                                             where pbp.FormDesignID == GlobalVariables.PBPDesignID && pbp.AnchorDocumentID == fi.AnchorDocumentID
                                                                             select pbp.FormInstanceID).FirstOrDefault(),
                                                        FormDesignVersionId = fi.FormDesignVersionID,
                                                        FolderVersionId = fi.FolderVersionID,
                                                        Folder = f.Name,
                                                        FolderId = f.FolderID,
                                                        FolderVersion = fl.FolderVersionNumber,
                                                        Year = viewModel.Year,
                                                        SelectPlan = "No",
                                                        View = fd.FormName == "Medicare" ? "SOT" : "PBP",
                                                    }).ToList();

            List<ODMPlanConfigViewModel> notInEbsPlan = new List<ODMPlanConfigViewModel>();
            if (ebsPlan.Count > 0)
            {

                notInEbsPlan = pBPPlanList.Select(a => a.QID).Except(ebsPlan.Select(a => a.QID))
                    .Select(x => new ODMPlanConfigViewModel() { QID = x })
                    .ToList();

            }
            else
            {
                notInEbsPlan = (from p in pBPPlanList
                                select new ODMPlanConfigViewModel
                                {
                                    QID = p.QID,
                                }).ToList();
            }


            viewModel.MatchPlanList = ebsPlan;
            viewModel.MisMatchPlanList = notInEbsPlan;

            return viewModel;
        }

        public List<ODMMigrationQueueViewModel> GetMigrationQueue(GridPagingRequest gridPagingRequest)
        {
            List<ODMMigrationQueueViewModel> migrationQueue = new List<ODMMigrationQueueViewModel>();
            SearchCriteria criteria = new SearchCriteria();
            try
            {
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                migrationQueue = (from mb in this._unitOfWork.RepositoryAsync<MigrationBatchs>().Get()
                                  join a in this._unitOfWork.RepositoryAsync<AccessFiles>().Get()
                                  on mb.BatchId equals a.BatchId
                                  select new ODMMigrationQueueViewModel
                                  {
                                      BatchID = mb.BatchId,
                                      MDBFileName = a.FileName,
                                      MDBOriginalFileName = a.OriginalFileName,
                                      Description = mb.Description,
                                      MigratedDate = mb.QueuedDate,
                                      MigratedBy = mb.QueuedUser,
                                      Status = mb.Status
                                  }).ApplySearchCriteria(criteria).OrderByDescending(x => x.BatchID).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return migrationQueue;
        }

        public ServiceResult QueueForMigration(string planData, string pbpSections, string sotSections, string description, string year, string fileName, string originalFileName, int formDesignVersionIDSOT, int formDesignVersionIDPBP, string username)
        {
            ServiceResult result = new ServiceResult();
            int batchId = 0;
            int fileId = 0;
            var planTestData = JsonConvert.DeserializeObject(planData);
            List<JToken> planDataList = ((JArray)planTestData).ToList();

            var pbpSectionsTestData = JsonConvert.DeserializeObject(pbpSections);
            List<JToken> pbpSectionsDataList = ((JArray)pbpSectionsTestData).ToList();

            var sotSectionsTestData = JsonConvert.DeserializeObject(sotSections);
            List<JToken> sotSectionsDataList = ((JArray)sotSectionsTestData).ToList();

            batchId = CreateBatch(description, username);

            fileId = InsertFileName(batchId, fileName, originalFileName);
            for (int i = 0; i < planDataList.Count; i++)
            {
                InsertSelectedPlan(batchId, fileId, planDataList[i], formDesignVersionIDSOT, formDesignVersionIDPBP);
            }

            for (int i = 0; i < sotSectionsDataList.Count; i++)
            {
                InsertSelectedSection(batchId, sotSectionsDataList[i], "SOT");
            }

            for (int i = 0; i < pbpSectionsDataList.Count; i++)
            {
                InsertSelectedSection(batchId, pbpSectionsDataList[i], "PBP");
            }

            List<ServiceResultItem> items = new List<ServiceResultItem>();
            items.Add(new ServiceResultItem { Messages = new string[] { batchId.ToString() } });
            result.Items = items;
            result.Result = ServiceResultStatus.Success;

            return result;
        }

        public void BaselineAndCreateNewMinorVersion(List<ODMPlanConfigViewModel> migrationList, int BatchId, int userId, string username, bool isAfterODM)
        {
            ServiceResult result = new ServiceResult();

            List<JToken> newVersionList = new List<JToken>();

            for (int i = 0; i < migrationList.Count; i++)
            {
                JToken isExist = newVersionList.Where(a => a["oldVerId"].ToString() == migrationList[i].FolderVersionId.ToString()).FirstOrDefault();

                if (isExist == null)
                {
                    string CurrentVersionNumber = migrationList[i].FolderVersionNumber;
                    DateTime FolderVersionEffectiveDate = migrationList[i].EffectiveDate;
                    var builder = new VersionNumberBuilder();
                    string NextFolderVersionName = builder.GetNextMinorVersionNumber(CurrentVersionNumber, FolderVersionEffectiveDate);

                    result = _folderVersionservice.BaseLineFolder(1, 0, migrationList[i].FolderId, migrationList[i].FolderVersionId,
                    userId, username, NextFolderVersionName, !isAfterODM ? "Base Line Created For ODM" : "Base Line Created After ODM", 0, null, false, false, false);

                    if (result.Result == ServiceResultStatus.Success)
                    {
                        List<ServiceResultItem> items = result.Items.ToList();
                        int newFolderVersionId = Convert.ToInt32(items[0].Messages[0]);
                        if (!isAfterODM) updateMigrationPlan(migrationList[i], newFolderVersionId);

                        JObject ver = JObject.Parse("{'oldVerId':'','newVerId':''}");
                        ver["oldVerId"] = migrationList[i].FolderVersionId;
                        ver["newVerId"] = newFolderVersionId;

                        newVersionList.Add(ver);
                    }

                }
                else
                {
                    if (!isAfterODM) updateMigrationPlan(migrationList[i], Convert.ToInt32(isExist["newVerId"]));
                }
            }

            if (!isAfterODM) updateBatchStatus(BatchId);
        }

        public List<ODMPlanConfigViewModel> planList(int BatchId)
        {

            List<ODMPlanConfigViewModel> migrationList = (from mg in this._unitOfWork.Repository<MigrationBatchs>().Get()
                                                          join mp in this._unitOfWork.Repository<MigrationPlans>().Get()
                                                          on mg.BatchId equals mp.BatchId
                                                          join fl in this._unitOfWork.Repository<FolderVersion>().Get()
                                                          on mp.FolderVersionId equals fl.FolderVersionID
                                                          join fi in this._unitOfWork.Repository<FormInstance>().Get()
                                                          on mp.FormInstanceId equals fi.FormInstanceID
                                                          where mg.BatchId == BatchId
                                                          && (fi.FormDesignID == GlobalVariables.MedicalDesignID || fi.FormDesignID == GlobalVariables.PBPDesignID)
                                                          select new ODMPlanConfigViewModel
                                                          {
                                                              BatchID = mg.BatchId,
                                                              FolderId = mp.FolderId,
                                                              FolderVersionId = mp.FolderVersionId,
                                                              FolderVersionNumber = fl.FolderVersionNumber,
                                                              EffectiveDate = fl.EffectiveDate,
                                                              FormInstanceId = mp.FormInstanceId,
                                                              DocId = fi.DocID
                                                          }).ToList();

            return migrationList;
        }

        public bool CheckFolderIsQueued(int folderID)
        {
            bool isQueued = false;
            try
            {
                ODMPlanConfigViewModel migration = (from mg in this._unitOfWork.Repository<MigrationBatchs>().Get()
                                                    join mp in this._unitOfWork.Repository<MigrationPlans>().Get()
                                                    on mg.BatchId equals mp.BatchId
                                                    where mp.FolderId == folderID
                                                    && mg.Status == "In Progress"
                                                    && mg.IsActive == true
                                                    select new ODMPlanConfigViewModel
                                                    {
                                                        BatchID = mg.BatchId,
                                                        FolderId = mp.FolderId,
                                                    }).FirstOrDefault();

                if (migration != null)
                {
                    isQueued = true;
                }
            }
            catch (Exception ex)
            {
                isQueued = false;
            }
            return isQueued;
        }
        #endregion

        #region Private Methods
        private int InsertFileName(int batchId, string fileName, string originalFileName)
        {
            AccessFiles migPlan = new AccessFiles();
            migPlan.BatchId = batchId;
            migPlan.FileName = fileName;
            migPlan.OriginalFileName = originalFileName;

            this._unitOfWork.RepositoryAsync<AccessFiles>().Insert(migPlan);
            this._unitOfWork.Save();
            return migPlan.FileID;
        }

        private void InsertSelectedSection(int batchId, JToken section, string viewType)
        {

            MigrationBatchSection migPlan = new MigrationBatchSection();
            migPlan.BatchId = batchId;
            migPlan.SectionGeneratedName = section["GeneratedName"].ToString();
            migPlan.ViewType = viewType;

            this._unitOfWork.RepositoryAsync<MigrationBatchSection>().Insert(migPlan);
            this._unitOfWork.Save();
        }

        private void InsertSelectedPlan(int batchId, int fileId, JToken Plan, int formDesignVersionIDSOT, int formDesignVersionIDPBP)
        {

            MigrationPlans migPlanSOT = new MigrationPlans();
            migPlanSOT.BatchId = batchId;
            migPlanSOT.FileID = fileId;
            migPlanSOT.FolderId = Convert.ToInt32(Plan["FolderId"]);
            migPlanSOT.FolderVersionId = Convert.ToInt32(Plan["FolderVersionId"]);
            migPlanSOT.FormInstanceId = Convert.ToInt32(Plan["SOTFormInstanceId"]);
            migPlanSOT.FormDesignVersionId = formDesignVersionIDSOT;
            migPlanSOT.ViewType = "SOT";
            migPlanSOT.QID = Plan["QID"].ToString();
            this._unitOfWork.RepositoryAsync<MigrationPlans>().Insert(migPlanSOT);
            this._unitOfWork.Save();

            MigrationPlans migPlanPBP = new MigrationPlans();
            migPlanPBP.BatchId = batchId;
            migPlanPBP.FileID = fileId;
            migPlanPBP.FolderId = Convert.ToInt32(Plan["FolderId"]);
            migPlanPBP.FolderVersionId = Convert.ToInt32(Plan["FolderVersionId"]);
            migPlanPBP.FormInstanceId = Convert.ToInt32(Plan["PBPFormInstanceId"]);
            migPlanPBP.FormDesignVersionId = formDesignVersionIDPBP;
            migPlanPBP.ViewType = "PBP";
            migPlanPBP.QID = Plan["QID"].ToString();
            this._unitOfWork.RepositoryAsync<MigrationPlans>().Insert(migPlanPBP);
            this._unitOfWork.Save();

        }

        private int CreateBatch(string description, string username)
        {

            MigrationBatchs migBtch = new MigrationBatchs();
            migBtch.Description = description;
            migBtch.QueuedDate = DateTime.Now;
            migBtch.QueuedUser = username;
            migBtch.Status = "Queued";
            migBtch.IsActive = false;
            this._unitOfWork.RepositoryAsync<MigrationBatchs>().Insert(migBtch);
            this._unitOfWork.Save();

            return migBtch.BatchId;
        }

        private int GetContractYear(string fileName1, string filePath)
        {
            int Year = 0;
            AccessDBTableService ServiceObj1 = new AccessDBTableService(filePath + "\\" + fileName1);
            return Year = ServiceObj1.GetPlanYear();
        }

        private void updateMigrationPlan(ODMPlanConfigViewModel migrationList, int newFolderVersionId)
        {

            ODMPlanConfigViewModel model = (from fi in this._unitOfWork.Repository<FormInstance>().Get()
                                            where fi.FolderVersionID == newFolderVersionId
                                            && fi.DocID == migrationList.DocId
                                            select new ODMPlanConfigViewModel
                                            {
                                                FormInstanceId = fi.FormInstanceID
                                            }).FirstOrDefault();

            MigrationPlans migPlan = this._unitOfWork.RepositoryAsync<MigrationPlans>()
                                                                       .Query()
                                                                       .Filter(c => c.FormInstanceId == migrationList.FormInstanceId && c.BatchId == migrationList.BatchID)
                                                                       .Get()
                                                                       .FirstOrDefault();

            migPlan.FolderVersionId = newFolderVersionId;
            migPlan.FormInstanceId = model.FormInstanceId;
            this._unitOfWork.RepositoryAsync<MigrationPlans>().Update(migPlan);
            this._unitOfWork.Save();
        }

        private void updateBatchStatus(int BatchId)
        {
            MigrationBatchs migPlan = this._unitOfWork.RepositoryAsync<MigrationBatchs>()
                                                                        .Query()
                                                                        .Filter(c => c.BatchId == BatchId)
                                                                        .Get()
                                                                        .FirstOrDefault();
            migPlan.IsActive = true;
            this._unitOfWork.RepositoryAsync<MigrationBatchs>().Update(migPlan);
            this._unitOfWork.Save();
        }

        #endregion
    }
}
