using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using DocumentDesignTypes = tmg.equinox.domain.entities.Enums.DocumentDesignTypes;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;
using tmg.equinox.applicationservices.viewmodels.Settings;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels.Report;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data.OleDb;
using System.Data;
using System.Data.Entity;
using System.Configuration;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.applicationservices.PBPImport;

namespace tmg.equinox.applicationservices
{
    public class PBPImportHelperServices : IPBPImportHelperServices
    {
        #region private Members
        IUnitOfWorkAsync _unitOfWorkAsync = null;
        #endregion

        #region Public Mentods
        public PBPImportHelperServices(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        public void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        public int GetFormDesignVersionID(string docName, int year)
        {
            //// c.StatusID == status is temporary. As we have two versions of Anchor , it is always picking up latest. 
            //// & it is causing problem while opening that plan after import.
            //int status = 0;
            //if (docName == DocumentName.MEDICARE)
            //    status = 3;
            //else if (docName == DocumentName.PBPVIEW)
            //    status = 1;

            int formId = this._unitOfWorkAsync.RepositoryAsync<FormDesign>().Get()
                                         .Where(s => s.FormName.Equals(docName) && s.IsActive == true)
                                         .Select(s => s.FormID).FirstOrDefault();

            int FormDesignVersionID = this._unitOfWorkAsync.RepositoryAsync<FormDesignVersion>()
                                         .Query()
                                         .Filter(c => c.FormDesignID == formId && c.EffectiveDate.Value.Year == year)
                                         .OrderBy(c => c.OrderByDescending(d => d.FormDesignVersionID))
                                         .Get().Max(s => s.FormDesignVersionID);
            return FormDesignVersionID;
        }

        public int GetPBPViewFormInstanceID(int folderVersionId, int pBPFromdesignVersionID, int? formInstanceID)
        {
            int PBPViewFormInstanceID = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                         && s.IsActive == true
                                         && s.FormDesignVersionID.Equals(pBPFromdesignVersionID)
                                         && s.AnchorDocumentID == formInstanceID
                                         )
                                         .Select(s => s.FormInstanceID).FirstOrDefault();

            return PBPViewFormInstanceID;
        }

        public int GetMedicareFormInstanceID(int folderVersionId, int medicareFormdesignVersionID, int DocId)
        {
            int MedicareFormInstanceID = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                         && s.IsActive == true
                                         && s.FormDesignVersionID.Equals(medicareFormdesignVersionID)
                                         && s.DocID.Equals(DocId)
                                         )
                                         .Select(s => s.FormInstanceID).FirstOrDefault();

            return MedicareFormInstanceID;
        }

        public int GetMedicareDocumentID(int folderVersionId, int formInstanceID)
        {
            int DocumentID = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                         && s.IsActive == true
                                         && s.FormInstanceID.Equals(formInstanceID)
                                         )
                                         .Select(s => s.DocID).FirstOrDefault();

            return DocumentID;
        }

        public ServiceResult UpdateImportQueueStatus(int PBPImportQueueID, domain.entities.Enums.ProcessStatusMasterCode status)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SqlParameter statusCode = new SqlParameter("@Status", (int)status);
                SqlParameter pBPImportQueueID = new SqlParameter("@PBPImportQueueID", PBPImportQueueID);

                var ResultService = this._unitOfWorkAsync.Repository<PBPImportQueue>()
                          .ExecuteSql("exec dbo.UpdatePBPImportQueue @Status,@PBPImportQueueID",
                          statusCode, pBPImportQueueID);
                if (ResultService != null)
                {
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                IPBPImportActivityLogServices Obj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                Obj.AddPBPImportActivityLog(PBPImportQueueID, "", null, null, null, ex);
            }
            return result;
        }


        public int GetMedicareDocumentIDByName(int MEDICAREFORMDESIGNVERSIONID, int DocId, int newFolderVersionId)
        {
            int NewFormInstanceId = 0;

            string DocumentName = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.DocID.Equals(DocId)
                                         && s.IsActive == true
                                         && s.FormDesignVersionID.Equals(MEDICAREFORMDESIGNVERSIONID)
                                         )
                                         .Select(s => s.Name).FirstOrDefault();

            NewFormInstanceId = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.IsActive == true
                                         && s.Name.Equals(DocumentName)
                                         && s.FolderVersionID.Equals(newFolderVersionId)
                                         && s.FormDesignVersionID.Equals(MEDICAREFORMDESIGNVERSIONID)
                                         )
                                         .Select(s => s.FormInstanceID).FirstOrDefault();

            return NewFormInstanceId;
        }

        public PBPPlanConfigViewModel GetFormInstanceIdForDelete(int MEDICAREFORMDESIGNVERSIONID, PBPPlanConfigViewModel ViewModel)
        {
            FolderVersionViewModel LatestFolderVersion = (from list in this._unitOfWorkAsync.Repository<FolderVersion>()
                                                              .Get()
                                                              .Where(s => s.FolderID.Equals(ViewModel.FolderId))
                                                          select new FolderVersionViewModel
                                                          {
                                                              FolderId = list.FolderID,
                                                              FolderVersionId = list.FolderVersionID,
                                                              FolderVersionNumber = list.FolderVersionNumber,
                                                              FolderVersionStateID = list.FolderVersionStateID
                                                          }).OrderByDescending(s => s.FolderVersionId)
                                                        .FirstOrDefault();
            if (LatestFolderVersion != null)
            {
                //it willonly delete in-progress folder
                if (LatestFolderVersion.FolderVersionStateID.Equals((int)FolderVersionState.INPROGRESS))
                {
                    ViewModel.FolderVersionId = LatestFolderVersion.FolderVersionId;
                    FormInstance FormInstanceDetail = null;
                    if (!String.IsNullOrEmpty(ViewModel.QID))
                    {
                        FormInstanceDetail = this._unitOfWorkAsync.RepositoryAsync<FormInstance>()
                                                          .Get()
                                                          .Where(s => s.FolderVersionID.Equals(ViewModel.FolderVersionId)
                                                                  && s.IsActive.Equals(true)
                                                                  && s.Name.Equals(ViewModel.QID)
                                                          ).FirstOrDefault();
                    }
                    else
                    {
                        FormInstanceDetail = this._unitOfWorkAsync.RepositoryAsync<FormInstance>()
                                                          .Get()
                                                          .Where(s => s.FolderVersionID.Equals(ViewModel.FolderVersionId)
                                                                  && s.IsActive.Equals(true)
                                                                  && s.DocID.Equals(ViewModel.DocumentId)
                                                                  && s.FormDesignVersionID.Equals(MEDICAREFORMDESIGNVERSIONID)
                                                          ).FirstOrDefault();
                    }
                    if (FormInstanceDetail != null)
                    {
                        ViewModel.FormInstanceId = FormInstanceDetail.FormInstanceID;
                    }
                }
            }
            return ViewModel;
        }

        public int GetFormInstanceForTerminate(int folderVersionId, int documentId)
        {
            int FormInstanceId = 0;
            const int MEDICAREFORMDESIGNID = 2359;
            FormInstance FormInstanceDetail = null;
                FormInstanceDetail = this._unitOfWorkAsync.RepositoryAsync<FormInstance>()
                                                  .Get()
                                                  .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                                          && s.IsActive.Equals(true)
                                                          && s.DocID.Equals(documentId)
                                                          && s.FormDesignID.Equals(MEDICAREFORMDESIGNID)
                                                  ).FirstOrDefault();
            if (FormInstanceDetail != null)
            {
                FormInstanceId = FormInstanceDetail.FormInstanceID;
            }
            return FormInstanceId;
        }

         public int GetEffectiveFormDesignVersionID(string formDesignName, int planYear)
        {
            int formId = this._unitOfWorkAsync.RepositoryAsync<FormDesign>().Get()
                                         .Where(s => s.FormName.Equals(formDesignName) && s.IsActive == true)
                                         .Select(s => s.FormID).FirstOrDefault();
            var FormDesignVersionObj = this._unitOfWorkAsync.RepositoryAsync<FormDesignVersion>()
                                         .Query()
                                         .Filter(c => c.FormDesignID == formId
                                                && c.EffectiveDate.Value.Year.Equals(planYear)
                                                && c.StatusID.Equals(3)
                                                )
                                         .Get()
                                         .FirstOrDefault();
            if (FormDesignVersionObj != null)
            {
                return FormDesignVersionObj.FormDesignVersionID;
            }
            else 
            {
              var  FormDesignVersion = this._unitOfWorkAsync.RepositoryAsync<FormDesignVersion>()
                                         .Query()
                                         .Filter(c => c.FormDesignID == formId)
                                         .OrderBy(c => c.OrderByDescending(d => d.FormDesignVersionID))
                                         .Get().FirstOrDefault();
                return FormDesignVersion.FormDesignVersionID;
            }
        }
        #endregion
    }
}
