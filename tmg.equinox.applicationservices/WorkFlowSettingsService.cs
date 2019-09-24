using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Transactions;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices
{
    public class WorkFlowSettingsService : IWorkFlowSettingsService
    {

        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public WorkFlowSettingsService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion


        #region Public Methods
        public IList<KeyValue> GetApplicableTeamList(int tenantID)
        {
            IList<KeyValue> list = new List<KeyValue>();

            list = (from applicableTeam in this._unitOfWork.RepositoryAsync<ApplicableTeam>()
                                                .Query()
                                                .Filter(c => c.TenantID == tenantID)
                                                .Get()
                    select new KeyValue
                    {
                        Key = applicableTeam.ApplicableTeamID,
                        Value = applicableTeam.ApplicableTeamName
                    }).ToList();

            return list;
        }

        #endregion

        /// <summary>
        /// SH This method is used to add/update dynamic entries in ApplicableTeamUserMap table.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="teamId"></param>
        /// <param name="applicableTeamMapData"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ServiceResult UpdateApplicableTeamUserMap(int tenantId, int teamId, List<ApplicableTeamMapModel> applicableTeamMapData, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var applicableTeamUserMapList = (from applicableTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>()
                               .Query()
                               .Filter(c => c.ApplicableTeamID == teamId && c.IsDeleted == false)
                               .Get()
                                                 select applicableTeamUserMap).ToList();
                if (applicableTeamUserMapList.Count > 0)
                {
                    //using (var scope = new TransactionScope())
                    {
                        foreach (var data in applicableTeamUserMapList)
                        {

                            data.IsDeleted = true;
                            data.UpdatedBy = userName;
                            data.UpdatedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Update(data);
                        }
                        foreach (var mapData in applicableTeamMapData)
                        {
                            ApplicableTeamUserMap userMapToAdd = new ApplicableTeamUserMap();
                            userMapToAdd.ApplicableTeamID = teamId;
                            userMapToAdd.UserID = mapData.UserID;
                            userMapToAdd.IsTeamManager = mapData.IsManager;
                            userMapToAdd.AddedBy = userName;
                            userMapToAdd.AddedDate = DateTime.Now;
                            userMapToAdd.IsDeleted = false;
                            this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Insert(userMapToAdd);
                            this._unitOfWork.Save();
                        }
                        result.Result = ServiceResultStatus.Success;
                        //scope.Complete();
                    }

                }
                else
                {
                    foreach (var mapData in applicableTeamMapData)
                    {
                        ApplicableTeamUserMap userMapToAdd = new ApplicableTeamUserMap();
                        userMapToAdd.ApplicableTeamID = teamId;
                        userMapToAdd.UserID = mapData.UserID;
                        userMapToAdd.IsTeamManager = mapData.IsManager;
                        userMapToAdd.AddedBy = userName;
                        userMapToAdd.AddedDate = DateTime.Now;
                        userMapToAdd.IsDeleted = false;
                        this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Insert(userMapToAdd);
                        this._unitOfWork.Save();
                    }
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }


        public List<WorkFlowSettingsViewModel> GetApplicableTeamUserMap(int tenantId, int teamId)
        {
            List<WorkFlowSettingsViewModel> workFlowSettingsViewModel = null;
            ServiceResult result = new ServiceResult();
            try
            {
                workFlowSettingsViewModel = (from applicableTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(c => c.ApplicableTeamID == teamId && c.IsDeleted == false)
                                             join user in this._unitOfWork.RepositoryAsync<User>().Get()
                                             on applicableTeamUserMap.UserID equals user.UserID where user.IsActive==true
                                             select new WorkFlowSettingsViewModel
                                             {
                                                 IsManager = applicableTeamUserMap.IsTeamManager,
                                                 UserId = applicableTeamUserMap.UserID,
                                                 ApplicableTeamID = applicableTeamUserMap.ApplicableTeamID,
                                                 Username = user.UserName
                                             }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return workFlowSettingsViewModel;
        }
    }
}
