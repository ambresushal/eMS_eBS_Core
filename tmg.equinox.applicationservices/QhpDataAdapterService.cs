using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.interfaces;
using FolderVersionStateEnum = tmg.equinox.domain.entities.Enums.FolderVersionState;
//using tmg.equinox.qhp.entities.Entities.Models;
namespace tmg.equinox.applicationservices
{
    public class QhpDataAdapterService : IQhpDataAdapterServices
    {
        private ILoggingService _loggingService;

        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }


        #endregion Private Members

        #region Constructor
        public QhpDataAdapterService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
        }
        #endregion

        #region Public Properties


        /// <summary>
        /// Fetch Json Data of FormInstance as per formName
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="formName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetFormInstanceJsonData(int tenantId, int folderVersionId, string formName)
        {

            var retjsondataDictionary = new Dictionary<string, string>();
            try
            {
                var formInstanceData =
                    (from forminstancedatamap in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                     join forminstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                         on forminstancedatamap.FormInstanceID equals forminstance.FormInstanceID
                     join fldrVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                         on forminstance.FolderVersionID equals fldrVersion.FolderVersionID
                     join formdesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                        on forminstance.FormDesignID equals formdesign.FormID
                     where fldrVersion.FolderVersionID == folderVersionId && formdesign.FormName == formName
                     select new
                     {
                         Name = forminstance.Name,
                         frmName = formdesign.FormName,
                         formData = forminstancedatamap.FormData

                     }).AsQueryable();
                retjsondataDictionary = formInstanceData.ToDictionary(x => x.Name, x => x.formData);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return retjsondataDictionary;
        }


        public Dictionary<string, string> GetMasterFormInstanceJsonData(int tenantId, int folderVersionId)
        {
            Dictionary<string, string> retMasterjsondata = null;
            try
            {
                var folderVersionList = (from fldrVer in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                         join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                             on fldrVer.FolderID equals fldr.FolderID
                                         join formGrp in this._unitOfWork.RepositoryAsync<FormDesignGroup>().Get()
                                             on fldr.FormDesignGroupId equals formGrp.FormDesignGroupID
                                         where formGrp.GroupName == "Master List"
                                         && fldrVer.FolderVersionStateID == (int)FolderVersionStateEnum.RELEASED
                                         orderby fldrVer.EffectiveDate descending
                                         select new FolderVersionViewModel()
                                         {
                                             FolderId = fldrVer.FolderID,
                                             FolderVersionId = fldrVer.FolderVersionID,
                                             EffectiveDate = fldrVer.EffectiveDate
                                         }).ToList();

                foreach (var folderver in folderVersionList)
                {
                    var eff = folderver.EffectiveDate;
                    var fvid = folderver.FolderVersionId;
                    var isValid = this._unitOfWork.RepositoryAsync<FolderVersion>()
                        .Query()
                        .Filter(fil => fil.FolderVersionID == folderVersionId)
                        .Get().ToList().Exists(x => eff <= x.EffectiveDate);
                    if (isValid)
                    {
                        var formInstanceData =
                             (from forminstancedatamap in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                              join forminstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                  on forminstancedatamap.FormInstanceID equals forminstance.FormInstanceID
                              where forminstance.FolderVersionID == folderver.FolderVersionId
                              select new
                              {
                                  Name = forminstance.Name,
                                  json = forminstancedatamap.FormData
                              });
                        retMasterjsondata = formInstanceData.ToDictionary(x => x.Name, x => x.json);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return retMasterjsondata;

        }
      
        
        #endregion Public Methods
    }
}
