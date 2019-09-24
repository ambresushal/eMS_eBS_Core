using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.setting.Common;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.applicationservices
{
    public class AutoSaveSettingsService : IAutoSaveSettingsService
    {
        #region Variables

        private IUnitOfWorkAsync _unitOfWork;
        private ISettingManager _settingManager;

        #endregion


        #region Constructor

        public AutoSaveSettingsService(IUnitOfWorkAsync unitOfWork, ISettingManager settingManager)
        {
            this._unitOfWork = unitOfWork;
            this._settingManager = settingManager;
        }

        #endregion


        #region Public Methods

        //Get AutoSave Settings for current User
        public AutoSaveSettingsViewModel GetAutoSaveSettingsForTenant(int tenantId)
        {
            AutoSaveSettingsViewModel autoSaveSettings=null;
            try
            {
                autoSaveSettings = (from settings in this._unitOfWork.RepositoryAsync<AutoSaveSettings>().Get()
                                    where settings.TenantID == tenantId
                                    select new AutoSaveSettingsViewModel
                                    {
                                        Duration = settings.Duration,
                                        IsAutoSaveEnabled = settings.IsAutoSaveEnabled,
                                        TenantID = settings.TenantID,
                                        SettingsID = settings.SettingsID,
                                        UserID = settings.UserID
                                       
                                         }).FirstOrDefault();

                if (autoSaveSettings == null)
                {
                    autoSaveSettings = new AutoSaveSettingsViewModel();
                
                }
                autoSaveSettings.AccelerateStartDateForTask = GetAccelerateStartDateForTaskSetting();


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return autoSaveSettings;
        }
        public bool GetAccelerateStartDateForTaskSetting()
        {
            var accelerateStartDateForTaskSettingSetting = _settingManager.GetSettingValue(SettingConstant.ACCELERATE_START_DATE_FOR_TASK);

            if (string.IsNullOrEmpty(accelerateStartDateForTaskSettingSetting))
            {
                accelerateStartDateForTaskSettingSetting = "false";
                SaveAccelerateStartDateForTaskSetting(accelerateStartDateForTaskSettingSetting);
            }
            if (accelerateStartDateForTaskSettingSetting.ToString().ToLower() == "false")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private void SaveAccelerateStartDateForTaskSetting(string accelerateStartDateForTask)
        {
            _settingManager.SaveAccelerateStartDateForTaskSetting(accelerateStartDateForTask);
           //_settingManager.Add("Task", "AccelerateStartDateForTask", accelerateStartDateForTask, "Accelerate start date for task", "Accelerate the assigned start date for task(s) upon approve/complete a workflow state ", true);
        }
    
        public AutoSaveSettingsViewModel GetAutoSaveDuration(int tenantID)
        {

            AutoSaveSettingsViewModel autoSaveSettings = null;
            try
            {
                autoSaveSettings = (from settings in this._unitOfWork.RepositoryAsync<AutoSaveSettings>().Get()
                                    where settings.TenantID == tenantID
                                    select new AutoSaveSettingsViewModel
                                    {
                                        Duration = settings.Duration,
                                        IsAutoSaveEnabled=settings.IsAutoSaveEnabled
                                        
                                    }).FirstOrDefault();

                if (autoSaveSettings == null)
                {
                    autoSaveSettings = new AutoSaveSettingsViewModel();
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return autoSaveSettings;
        }



        //Save Method
        public ServiceResult SaveAutoSaveSettings(AutoSaveSettingsViewModel settingsViewModel, int? CurrentUserId, string userName)
        {
            ServiceResult result = new ServiceResult();
            result.Result = ServiceResultStatus.Failure;
            try
            {
                var  currentUserAutoSaveData = this._unitOfWork.RepositoryAsync<AutoSaveSettings>().Query()
                                                        .Filter(e => e.TenantID == settingsViewModel.TenantID)
                                                        .Get().FirstOrDefault();

                if (currentUserAutoSaveData != null)
                {
                   
                    currentUserAutoSaveData.IsAutoSaveEnabled = settingsViewModel.IsAutoSaveEnabled;
                    currentUserAutoSaveData.TenantID = settingsViewModel.TenantID;
                    currentUserAutoSaveData.UserID = CurrentUserId.Value;
                    currentUserAutoSaveData.Duration = settingsViewModel.Duration;
                    currentUserAutoSaveData.UpdatedDate = DateTime.Now;
                    currentUserAutoSaveData.UpdatedBy = userName;
                    this._unitOfWork.RepositoryAsync<AutoSaveSettings>().Update(currentUserAutoSaveData);
                }
                else
                {
                    AutoSaveSettings autoSaveSettings = new AutoSaveSettings();
                    autoSaveSettings.IsAutoSaveEnabled = settingsViewModel.IsAutoSaveEnabled;
                    autoSaveSettings.TenantID = settingsViewModel.TenantID;
                    autoSaveSettings.UserID = CurrentUserId.Value;
                    autoSaveSettings.Duration = settingsViewModel.Duration;
                    autoSaveSettings.AddedDate = DateTime.Now;
                    autoSaveSettings.AddedBy = userName;
                    this._unitOfWork.RepositoryAsync<AutoSaveSettings>().Insert(autoSaveSettings);
                   
                }
                this._unitOfWork.Save();

                //_settingManager.Update("AccelerateStartDateForTask", settingsViewModel.AccelerateStartDateForTask.ToString().ToLower(), null, null, true);

                _settingManager.SaveAccelerateStartDateForTaskSetting(settingsViewModel.AccelerateStartDateForTask.ToString().ToLower());

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
               bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return result;
        }

        #endregion


        #region Private Methods


        #endregion
        
       
    }
}
