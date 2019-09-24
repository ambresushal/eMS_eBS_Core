using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.setting;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.net
{
    public abstract class EmailSenderConfiguration : IEmailSenderConfiguration
    {
        public virtual string DefaultFromAddress
        {
            get { return GetNotEmptySettingValue(EmailSettingNames.DefaultFromAddress); }
        }

        public virtual string DefaultFromDisplayName
        {
            get { return SettingManager.GetSettingValue(EmailSettingNames.DefaultFromDisplayName); }
        }

        protected readonly ISettingManager SettingManager;

        /// <summary>
        /// Creates a new <see cref="EmailSenderConfiguration"/>.
        /// </summary>
        protected EmailSenderConfiguration(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        /// <summary>
        /// Gets a setting value by checking. Throws <see cref="Exception"/> if it's null or empty.
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <returns>Value of the setting</returns>
        protected string GetNotEmptySettingValue(string name)
        {
            var value = SettingManager.GetSettingValue(name);

            if (string.IsNullOrEmpty( value))
            {
                throw new Exception($"Setting value for '{name}' is null or empty!");
            }

            return value;
        }
    }
}
