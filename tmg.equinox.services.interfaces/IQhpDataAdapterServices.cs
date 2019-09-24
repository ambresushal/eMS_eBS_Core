using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IQhpDataAdapterServices
    {
        Dictionary<string, string> GetFormInstanceJsonData(int tenantId, int folderVersionId, string formName);
        Dictionary<string, string> GetMasterFormInstanceJsonData(int tenantId, int folderVersionId);
    }
}
