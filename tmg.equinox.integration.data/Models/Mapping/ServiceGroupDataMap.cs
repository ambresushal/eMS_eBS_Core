using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping 
{
    public class ServiceGroupDataMap : EntityTypeConfiguration<ServiceGroupModel>
    {
        public ServiceGroupDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID });

            this.ToTable("ServiceGroupModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.ServiceGroupHeaderDescription).HasColumnName("ServiceGroupHeaderDescription");
        }
    }
}
