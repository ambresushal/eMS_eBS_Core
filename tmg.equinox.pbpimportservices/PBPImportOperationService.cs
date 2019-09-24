using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.dependencyresolution;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.pbpimportservices
{
    public class PBPImportOperationService
    {
        #region Private Members
        private IPBPImportServices _pbpImportServices;
        private IFolderVersionServices _folderVersionService;
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private ILoggingService _loggingService;
        private IDomainModelService _domainModelService;
        private string connectionString;
        #endregion

        #region Constructor
        public PBPImportOperationService(string strConnectionString)
        {
            connectionString = strConnectionString;
            UnityConfig.RegisterComponents();
            _pbpImportServices = UnityConfig.Resolve<IPBPImportServices>();
            _folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
            _unitOfWorkAsync = UnityConfig.Resolve<IUnitOfWorkAsync>();
            _loggingService = UnityConfig.Resolve<ILoggingService>();
            _domainModelService = UnityConfig.Resolve<IDomainModelService>();
        }
        #endregion

        #region Public Methods
        public bool StartPBPImportProcess()
        {
            bool isSuccessfull = false;

            // Fetch All the Queued files records from the table PBP Import for Process

            IEnumerable<PBPImportBatchViewModel> collQueuedPBPImportBatchList = _pbpImportServices.GetQueuedPBPImportBatch();
            //Perform Import operation, first take all the data from access to our SQL Server
            PBPImportOperation(connectionString, collQueuedPBPImportBatchList);
            //After performing import operation go for creating form line or base line functions
            FormInstanceOperation(collQueuedPBPImportBatchList);

            return isSuccessfull;
        }
        public bool ExecutePBPImportProcess()
        {
            bool pbpImpServiceResult = false;
            try
            {
                pbpImpServiceResult = StartPBPImportProcess();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pbpImpServiceResult;
        }
        #endregion

        #region Private Methods

        private void PBPImportOperation(string connectionString, IEnumerable<PBPImportBatchViewModel> collQueuedPBPImportBatchList)
        {
            PBPImportOperation objPBPImportOperation;

            foreach (PBPImportBatchViewModel pbpImportBatchObj in collQueuedPBPImportBatchList)
            {
                if (pbpImportBatchObj.PBPImportQueueViewModel != null)
                {
                    foreach (PBPImportQueueViewModel pbpImportQueueObj in pbpImportBatchObj.PBPImportQueueViewModel)
                    {
                        // Check File is present on the location if yes go ahead, or add an log entry
                        if (File.Exists(pbpImportQueueObj.Location))
                        {
                            IEnumerable<PBPImportTablesViewModel> collPBPImportTablesList = _pbpImportServices.GetPBPImportTablesList();
                            objPBPImportOperation = new PBPImportOperation(pbpImportQueueObj.Location, connectionString);
                            // Add the PBP MDF file into SQL Server Database
                            objPBPImportOperation.PerformImportOperationWithSequence(pbpImportBatchObj.PBPImportBatchID, collPBPImportTablesList, "");
                        }
                        else
                        {
                            // Throw error that the file is not present at location, add a log entry
                            _pbpImportServices.AddPBPImportActivityLog(pbpImportQueueObj.PBPImportQueueID, pbpImportQueueObj.PBPImportBatchID, pbpImportQueueObj.FileName, null, "Error : File is not present at location", pbpImportQueueObj.CreatedBy);
                        }
                    }
                }
            }
        }

        private void FormInstanceOperation(IEnumerable<PBPImportBatchViewModel> collQueuedPBPImportBatchList)
        {
            foreach (PBPImportBatchViewModel pbpImportBatchObj in collQueuedPBPImportBatchList)
            {
                if (pbpImportBatchObj.PBPImportQueueViewModel != null)
                {
                    foreach (PBPImportQueueViewModel pbpImportQueueObj in pbpImportBatchObj.PBPImportQueueViewModel)
                    {
                        // Featch FormDesignVersionId 
                        int tenentId = 1;
                        int formdesignVersionId = 0;
                        string folderType = "Account";
                        formdesignVersionId = GetFormDesignVersionId(pbpImportQueueObj.FolderID, tenentId, pbpImportQueueObj.FolderVersionID, folderType);

                        // Get distinct QID for current batchId  from our SQL Server Database

                        List<string> importedQIDList = _pbpImportServices.GetDistinctQUIDForPBPImportBatchID(pbpImportQueueObj.PBPImportBatchID);

                        // Check the QUID is already present in the table of DOCID and FormInstanceID 

                        List<string> newQIDList = new List<string>();
                        bool allowBaseLine = false;
                        IEnumerable<PBPFormInstanceViewModel> objPBPFormInstanceVM = _pbpImportServices.GetFormInstanceForBatchID(pbpImportQueueObj.FolderID);
                        if (objPBPFormInstanceVM != null && objPBPFormInstanceVM.Count<PBPFormInstanceViewModel>() > 0)
                        {
                            foreach (string strUid in importedQIDList)
                            {
                                bool _isQIDPresent = false;
                                foreach (PBPFormInstanceViewModel objFIVM in objPBPFormInstanceVM)
                                {
                                    if (strUid.Equals(objFIVM.QID))
                                    {
                                        _isQIDPresent = allowBaseLine = true;
                                        break;
                                    }
                                }
                                if (!_isQIDPresent)
                                {
                                    if (!newQIDList.Contains(strUid))
                                        newQIDList.Add(strUid);
                                }
                            }
                        }

                        if (allowBaseLine)
                        {
                            // if the QID is present then create baseline operation
                            CreateBaseLineForQID(pbpImportQueueObj, newQIDList, formdesignVersionId, importedQIDList);
                        }
                        else
                        {
                            // if the QUID is not present then create new FormIntance
                            CreateNewFormInstanceForQUID(pbpImportQueueObj.PBPImportBatchID, pbpImportQueueObj.PBPImportQueueID, formdesignVersionId, pbpImportQueueObj.FolderID, pbpImportQueueObj.FolderVersionID, pbpImportQueueObj.FileName, pbpImportQueueObj.CreatedBy, importedQIDList);
                        }
                        // update status of pbp queue to complete
                        _pbpImportServices.UpdatePBPImportQueueStatus(pbpImportQueueObj.PBPImportQueueID,4);
                    }
                    // update status of pbp quee to complete
                    _pbpImportServices.UpdatePBPImportBatchStatus(pbpImportBatchObj.PBPImportBatchID,4);
                }
            }
        }

        private void CreateBaseLineForQID(PBPImportQueueViewModel pbpImportQueueObj, List<string> QIDList, int formdesignVersionId, List<string> importedQIDList)
        {
            FolderVersionViewModel objFolderVersionDetails = _folderVersionService.GetCurrentFolderVersion(pbpImportQueueObj.FolderID, pbpImportQueueObj.FolderVersionID);

            if (objFolderVersionDetails != null)
            {
                // Create New folderversion
                string newFolderVersion = string.Empty;
                string strVersion = objFolderVersionDetails.FolderVersionNumber;
                string[] versionArr = strVersion.Split('_');
                if (versionArr.Length > 0)
                {
                    double d = Convert.ToDouble(versionArr[1]);
                    d = d + 0.01;
                    newFolderVersion = versionArr[0] + "_" + d.ToString();
                }
                ServiceResult result = _folderVersionService.BaseLineFolder(1, 0, pbpImportQueueObj.FolderID, pbpImportQueueObj.FolderVersionID, 0,
                              pbpImportQueueObj.CreatedBy, newFolderVersion, "Baseline created on PBP Import", 0, objFolderVersionDetails.EffectiveDate, false, isNotApproved: false, isNewVersion: false);

                string strNewFolderVersion = string.Empty;

                if (result.Items.First().Messages.Count() > 0)
                    strNewFolderVersion = result.Items.First().Messages[0];

                int folderVersionAfterBaseLine = 0;
                if (int.TryParse(strNewFolderVersion, out folderVersionAfterBaseLine))
                {
                    PBPObjectJson objPBPObjectJson = new PBPObjectJson(pbpImportQueueObj.PBPImportBatchID);
                    objPBPObjectJson.GenerateJsonForBatchId();

                    List<FormInstanceViewModel> obj1 = _folderVersionService.GetAnchorFormInstanceList(1, folderVersionAfterBaseLine);
                    foreach (FormInstanceViewModel objFormInst in obj1)
                    {
                        //objFormInst.FormInstanceID
                        try
                        {
                            //importedQIDList
                            string strQID = importedQIDList.Find(p => p.Equals(objFormInst.FormDesignName));
                            List<FormInstanceViewModel> childFormInstanceList = _folderVersionService.GetChildElementsOfFormInstance(1, objFormInst.FormInstanceID);
                            FormInstanceViewModel form = childFormInstanceList.Where(c => c.FormDesignName.Contains("@@PBP")).FirstOrDefault();
                            int pbpformisntanceid = 0;

                            if (form != null)
                                pbpformisntanceid = form.FormInstanceID;
                            if (!string.IsNullOrEmpty(strQID))
                            {
                                MapPBPData objMapPBPData = new MapPBPData(objFormInst.FormInstanceID, strQID, pbpImportQueueObj.PBPImportBatchID, pbpImportQueueObj.CreatedBy);
                                objMapPBPData.MapPBPDataToMedicare();

                                if (pbpformisntanceid != 0)
                                {
                                    ImportDatatoFolder objImportDatatoFolder = new ImportDatatoFolder(pbpImportQueueObj.PBPImportBatchID, strQID, pbpformisntanceid);
                                    objImportDatatoFolder.UpdatePBPView();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            _pbpImportServices.AddPBPImportActivityLog(pbpImportQueueObj.PBPImportQueueID, pbpImportQueueObj.PBPImportBatchID, pbpImportQueueObj.FileName, null, ex.Message, pbpImportQueueObj.CreatedBy);
                        }
                    }
                }
                if (QIDList.Count > 0)
                {

                    int folderVersionId = 0;

                    IList<FolderVersions> objFolderVersionList = _folderVersionService.GetFolderVersionByFolderId(pbpImportQueueObj.FolderID);
                    if (objFolderVersionList != null && objFolderVersionList.Count > 0)
                    {
                        folderVersionId = objFolderVersionList[objFolderVersionList.Count - 1].FolderVersionID;
                    }
                    FolderVersionViewModel objFolderVersionVM = _folderVersionService.GetCurrentFolderVersion(pbpImportQueueObj.FolderID, folderVersionId);

                    CreateNewFormInstanceForQUID(pbpImportQueueObj.PBPImportBatchID, pbpImportQueueObj.PBPImportQueueID, formdesignVersionId, pbpImportQueueObj.FolderID, folderVersionId, pbpImportQueueObj.FileName, pbpImportQueueObj.CreatedBy, QIDList);
                }
            }
        }

        private void CreateNewFormInstanceForQUID(int pbpImportBatchID, int pbpImportQueueID, int formdesignVersionId, int folderId, int folderVersionID, string fileName, string addedBy, List<string> QIDList)
        {

            PBPObjectJson objPBPObjectJson = new PBPObjectJson(pbpImportBatchID);
            objPBPObjectJson.GenerateJsonForBatchId();

            foreach (string strQID in QIDList)
            {
                try
                {
                    ServiceResult result = new ServiceResult();
                    int newFormInstanceId = 0;
                    int tenentId = 1;

                    result = _folderVersionService.CreateFormInstance(tenentId, folderVersionID, formdesignVersionId, newFormInstanceId, false, strQID, addedBy);
                    newFormInstanceId = Convert.ToInt32(result.Items.FirstOrDefault().Messages.FirstOrDefault());

                    FormDesignService objFormDesignService = new FormDesignService(_unitOfWorkAsync, _loggingService, _domainModelService);
                    string formDesignJSON = objFormDesignService.GetCompiledFormDesignVersion(1, formdesignVersionId);
                    FormDesignVersionDetail detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignJSON);

                    string defaultJSONData = detail.GetDefaultJSONDataObject();
                    _folderVersionService.SaveFormInstanceData(1, folderVersionID, newFormInstanceId, defaultJSONData, addedBy);

                    List<FormInstanceViewModel> childFormInstanceList = _folderVersionService.GetChildElementsOfFormInstance(1, newFormInstanceId);
                    FormInstanceViewModel form = childFormInstanceList.Where(c => c.FormDesignName.Contains("@@PBP")).FirstOrDefault();
                    int pbpformisntanceid = 0;

                    if (form != null)
                        pbpformisntanceid = form.FormInstanceID;

                    try
                    {
                        MapPBPData objMapPBPData = new MapPBPData(newFormInstanceId, strQID, pbpImportBatchID, addedBy);
                        objMapPBPData.MapPBPDataToMedicare();

                        if (pbpformisntanceid != 0)
                        {
                            ImportDatatoFolder objImportDatatoFolder = new ImportDatatoFolder(pbpImportBatchID, strQID, pbpformisntanceid);
                            objImportDatatoFolder.UpdatePBPView();
                        }

                    }
                    catch (Exception ex)
                    {
                        _pbpImportServices.AddPBPImportActivityLog(pbpImportQueueID, pbpImportBatchID, fileName, null, ex.Message + strQID, addedBy);
                    }

                    // After creating FormInstance add entries into the DOCID and FormInstanceID , folderId
                    _pbpImportServices.AddPBPFormInstance(newFormInstanceId, 1, pbpImportBatchID, strQID, folderId);
                    _pbpImportServices.AddPBPImportActivityLog(pbpImportQueueID, pbpImportBatchID, fileName, null, "FormInstance created successfully for " + strQID, addedBy);
                }
                catch (Exception ex)
                {
                    _pbpImportServices.AddPBPImportActivityLog(pbpImportQueueID, pbpImportBatchID, fileName, null, "FormInstance created unsuccessfully for " + strQID + " Error : " + ex.Message, addedBy);
                }
            }
        }

        private int GetFormDesignVersionId(int folderId, int tenentId, int folderVersionID, string folderType)
        {
            int formdesignVersionId = 0;
            DateTime effDate = _folderVersionService.GetFolderVersionEffectiveDate(folderVersionID);
            IEnumerable<FormTypeViewModel> formTypeList = _folderVersionService.GetFormTypeList(tenentId, folderType, effDate, folderId);
            if (formTypeList != null)
            {
                foreach (FormTypeViewModel formtypeVM in formTypeList)
                {
                    if (formtypeVM.FormTypeName == "Medicare")
                    {
                        formdesignVersionId = formtypeVM.FormVersionDesignID;
                        break;
                    }
                }
            }
            return formdesignVersionId;
        }
        #endregion
    }
}
