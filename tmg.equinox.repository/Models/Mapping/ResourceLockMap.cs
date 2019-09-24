using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ResourceLockMap : EntityTypeConfiguration<ResourceLock>
    {
        public ResourceLockMap()
        {
            
        }
    }
}
