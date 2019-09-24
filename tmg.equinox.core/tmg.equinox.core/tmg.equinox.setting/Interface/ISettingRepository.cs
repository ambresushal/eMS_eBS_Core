using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.setting.Interface
{
    public interface ISettingRepository
    {
        List<SettingDefinition> GetSetting();
        void Add(SettingDefinition settingDefinition);
        void Update(SettingDefinition settingDefinition);
        List<SettingDefinition> GetSettingInfo();
        SettingDefinition Get(string name);


    }
}
