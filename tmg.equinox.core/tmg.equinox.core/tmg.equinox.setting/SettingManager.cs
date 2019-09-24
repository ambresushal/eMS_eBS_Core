using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.setting.Common;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.setting
{
    public class SettingManager : ISettingManager
    {
        ISettingProvider _settingProvider;
        private repository.interfaces.IUnitOfWorkAsync _unitOfWork { get; set; }


        public SettingManager(ISettingProvider settingProvider)
        {

            _settingProvider = settingProvider;
        }
        public string GetSettingValue(string name)
        {
            //get it from db
            var setting=  _settingProvider.GetSettingDefinitions().FirstOrDefault(m=>m.SettingName==name);

            if (setting == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(setting.DefaultValue))
                return "";

            return setting.DefaultValue;           
        }

        public T GetSettingValue<T>(string name)
        {
            //get it from db
            var value = GetSettingValue(name);
            return (T)Convert.ChangeType(value, typeof(T)); ;
        }

        public string GetSettingValue(string type, string name)
        {
            //get it from db

            var settings = _settingProvider.GetSettingDefinitions().FirstOrDefault(m => m.SettingDefinitiontype == type && m.SettingName == name);

            if (settings == null)
                return "";
            var setting = settings.DefaultValue;
            return setting;
        }
        public List<string> GetSettingValueSplit(string type, string name)
        {
            //get it from db

            var setting = _settingProvider.GetSettingDefinitions().FirstOrDefault(m => m.SettingDefinitiontype == type && m.SettingName == name).DefaultValue;

           
            if (setting==null)
            {
                var complist = new List<string>();
                complist.Add("PdfA1a");
                return complist;
            }
            return setting.Split(',').ToList();
        }

        public void Add(string definitionType, string name, string value, string displayName = null, string description = null,
            bool isVisibleToClients = false)
        {
            var settingDefinition = new SettingDefinition(name, value, definitionType, displayName, description, isVisibleToClients);

            if (GetSettingValue(name)=="")
            {
                _settingProvider.Add(settingDefinition);
            }
            else
            {
                throw new Exception("Setting Already Exists, Unable to save");
            }
        }
        public void Update(string name, string value, string displayName = null, string description = null,
            bool isVisibleToClients = false)
        {
            var setting = Get(name);

            if (setting == null)
            {
                throw new Exception("Setting Does not Exists, Unable to save");
            }
            setting.DefaultValue = value;
            setting.SettingName = name;
            if (!string.IsNullOrEmpty(displayName))
                setting.DisplayName = displayName;            
            if (!string.IsNullOrEmpty(description))
            setting.Description = description;
            if (isVisibleToClients)
            setting.IsVisibleToClients = isVisibleToClients;

            _settingProvider.Update(setting);
        }

        private SettingDefinition Get(string name)
        {
            return _settingProvider.Get(name);            
        }
        public void SaveLockSetting(string value)
        {
            var setting = Get(SettingConstant.UNLOCK_TIME_OUT);

            if (setting == null)
            {
                Add(SettingConstant.UNLOCK_TIME_OUT_TYPE, SettingConstant.UNLOCK_TIME_OUT, value, "Lock Time Out", "Holds idle time period in minute allow system to unlock the section/View", true);
            }
            else
            {
                Update(SettingConstant.UNLOCK_TIME_OUT, value, null, null, true);
            }
        }
        public void SaveAccelerateStartDateForTaskSetting(string value)
        {
            var setting = Get(SettingConstant.ACCELERATE_START_DATE_FOR_TASK);

            if (setting == null)
            {
                Add(SettingConstant.TYPE_TASK, SettingConstant.ACCELERATE_START_DATE_FOR_TASK, value, "Accelerate start date for task", "Accelerate the assigned start date for task(s) upon approve/complete a workflow state ", true);
            }
            else
            {
                Update(SettingConstant.ACCELERATE_START_DATE_FOR_TASK, value, null, null, true);
            }
        }


        public List<SettingDefinition>  GetSettingInfo()
        {

            List<SettingDefinition> collectionSettingDef = _settingProvider.GetSettingInfo();         
            return collectionSettingDef;
        }

    }
}