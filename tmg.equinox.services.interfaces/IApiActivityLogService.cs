using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.WebApi;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IApiActivityLogService
    {
        bool Insert(ActivityLogViewModel activityLogEntry);
    }
}
