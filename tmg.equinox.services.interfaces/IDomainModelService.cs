using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IDomainModelService
    {
        ServiceResult Create(int tenantId, int formDesignVersionId);

        ServiceResult Update(int tenantId, int formDesignVersionId);

        ServiceResult Delete(int tenantId, int formDesignVersionId);

        ServiceResult CheckDataSourceMappings(string username, int tenantId, int? formDesignId);

    }
}
