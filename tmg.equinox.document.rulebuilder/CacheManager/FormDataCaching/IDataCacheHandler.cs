using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.Framework.Caching
{
    public interface IDataCacheHandler
    {
        
        void Add(int formInstanceId, int? userId);

        bool Remove(int formInstanceId, int? userId);

        string  IsExists(int tenantId, int formInstanceId, int? userId);

        string GetSection(int formInstanceId, string sectionName, int? userId);

        string UpdateSection(int formInstanceId, string sectionName, string sectionData, int? userId);

    }
}
