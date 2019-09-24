using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace tmg.equinox.domain.entities.Utility
{
    public static partial class AppSettings
    {
        #region Private Properties
        public static int TransactionTimeOutPeriod;
        #endregion

        #region Constructor
        static AppSettings()
        {
            TransactionTimeOutPeriod = Convert.ToInt32(ConfigurationManager.AppSettings["TransactionTimeOutSpan"]);
        }
        #endregion
    }
}
