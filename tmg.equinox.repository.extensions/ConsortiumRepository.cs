using System.Linq;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ConsortiumRepository
    {
        public static bool IsConsortiumNameExists(this IRepositoryAsync<Consortium> consortiumRepository, int tenantId, int consortiumID, string consortiumName)
        {
            if (consortiumID > 0)
            {
                return consortiumRepository
                        .Query()
                        .Filter(c => c.ConsortiumName == consortiumName && c.TenantID == tenantId && c.ConsortiumID != consortiumID)
                        .Get()
                        .Any();
            }
            else
            {
                return consortiumRepository
                       .Query()
                       .Filter(c => c.ConsortiumName == consortiumName && c.TenantID == tenantId && c.IsActive == true)
                       .Get()
                       .Any();
            }
        }
    }
}
