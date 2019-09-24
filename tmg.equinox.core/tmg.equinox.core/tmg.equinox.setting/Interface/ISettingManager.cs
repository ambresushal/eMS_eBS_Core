using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.setting.Interface
{
    public interface ISettingManager
    {
           string GetSettingValue(string name);
            T GetSettingValue<T>(string name);
            string GetSettingValue(string type, string name);
            List<string> GetSettingValueSplit(string type, string name);

            void Add(string definitionType, string name, string value, string displayName = null, string description = null,
            bool isVisibleToClients = false);


            void Update(string name, string value, string displayName = null, string description = null,
            bool isVisibleToClients = false);

            void SaveLockSetting(string value);

             void SaveAccelerateStartDateForTaskSetting(string value);

            List<SettingDefinition> GetSettingInfo();

    }
}
