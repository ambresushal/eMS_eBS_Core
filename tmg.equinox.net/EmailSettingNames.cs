using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.net
{
    /// <summary>
    /// Declares names of the settings defined by <see cref="EmailSettingProvider"/>.
    /// </summary>
    public static class EmailSettingNames
    {
        /// <summary>
        /// tmg.equinox.net.Mail.DefaultFromAddress
        /// </summary>
        public const string DefaultFromAddress = "tmg.equinox.net.Mail.DefaultFromAddress";

        /// <summary>
        /// tmg.equinox.net.Mail.DefaultFromDisplayName
        /// </summary>
        public const string DefaultFromDisplayName = "tmg.equinox.net.Mail.DefaultFromDisplayName";

        /// <summary>
        /// SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.Host
            /// </summary>
            public const string Host = "tmg.equinox.net.Mail.Smtp.Host";

            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.Port
            /// </summary>
            public const string Port = "tmg.equinox.net.Mail.Smtp.Port";

            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.UserName
            /// </summary>
            public const string UserName = "tmg.equinox.net.Mail.Smtp.UserName";

            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.Password
            /// </summary>
            public const string Password = "tmg.equinox.net.Mail.Smtp.Password";

            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.Domain
            /// </summary>
            public const string Domain = "tmg.equinox.net.Mail.Smtp.Domain";

            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.EnableSsl
            /// </summary>
            public const string EnableSsl = "tmg.equinox.net.Mail.Smtp.EnableSsl";

            /// <summary>
            /// tmg.equinox.net.Mail.Smtp.UseDefaultCredentials
            /// </summary>
            public const string UseDefaultCredentials = "tmg.equinox.net.Mail.Smtp.UseDefaultCredentials";
        }
    }
}
