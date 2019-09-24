using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class QhpService : IQhpService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public QhpService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;

        }
        #endregion Constructor

        #region Public Methods
        public ServiceResult AddTemplate(QhpUploadTemplateViewModel viewModel)
        {
            ServiceResult result = null;
            try
            {
                if (viewModel != null)
                {
                    UploadTemplate template = new UploadTemplate();
                    template.TemplateName = viewModel.TemplateName;
                    template.FileType = viewModel.FileType;
                    template.TemplateGuid = viewModel.TemplateGuid;
                    template.FolderVersionID = viewModel.FolderVersionID;
                    template.FolderID = viewModel.FolderID;
                    template.UplodedBy = viewModel.UplodedBy;
                    template.UploadDate = DateTime.Now;
                    template.IsTemplateImported = false;
                    template.TenantID = viewModel.TenantID;

                    this._unitOfWork.Repository<UploadTemplate>().Insert(template);
                    this._unitOfWork.Save();

                    result = new ServiceResult();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result = new ServiceResult();
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public List<FormInstanceViewModel> GetFormInstanceList(int tenantId, int folderVersionId, int folderId, int formDesignID)
        {
            List<FormInstanceViewModel> formInstanceList = null;
            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.FormDesignID == formDesignID && c.IsActive== true)
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID
                                     });

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

    }
}
