using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.setting
{
    public partial class SettingDefinition : Entity
    {
        public int ID { get; set; }
        public string SettingDefinitiontype { get; set; }
        public string SettingName { get; set; }
        public string DefaultValue { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsInherited { get; set; }
        public bool IsVisibleToClients { get; set; }
        public string CustomData { get; set; }
       //  public string DefinitionType { get; set; }

        public SettingDefinition()
        {

        }
        public SettingDefinition(
           string settingName,
           string defaultValue,
            string definitionType,
           string displayName = null,
           string description = null,
            bool isVisibleToClients = false,
           bool isInherited = true,
           string customData = null)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException(nameof(settingName));
            }

            SettingName = settingName;
            DefaultValue = defaultValue;
            DisplayName = displayName;
            Description = description;
            IsVisibleToClients = isVisibleToClients;
            IsInherited = isInherited;
            CustomData = customData;
            SettingDefinitiontype = definitionType;
        }
    }
}
