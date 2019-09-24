using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.net
{
    public interface IEmailSenderConfiguration
    {
        /// <summary>
        /// Default from address.
        /// </summary>
        string DefaultFromAddress { get; }

        /// <summary>
        /// Default display name.
        /// </summary>
        string DefaultFromDisplayName { get; }
    }
}
