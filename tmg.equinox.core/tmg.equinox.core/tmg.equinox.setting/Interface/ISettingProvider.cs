using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.setting.Interface
{
    public interface ISettingProvider
    {
        List<SettingDefinition> GetSettingDefinitions();
        List<SettingDefinition> GetSettingInfo();
        void Add(SettingDefinition settingDefinition);
        void Update(SettingDefinition settingDefinition);

        SettingDefinition Get(string name);
    }
}
