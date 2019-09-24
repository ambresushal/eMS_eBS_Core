using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class ServiceMasterMap:EntityTypeConfiguration<ServiceMaster>
    {
        public ServiceMasterMap()
        {
           this.HasKey(t => new { t.SESE_ID, t.SESE_RULE });
            // Table & Column Mappings
            this.ToTable("ServiceMaster", "Master");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SESEHashCode).HasColumnName("SESEHashCode");
            this.Property(t => t.SETRHashCode).HasColumnName("SETRHashCode");
            this.Property(t => t.SESPHashCode).HasColumnName("SESPHashCode");
            this.Property(t => t.SESERuleCategory).HasColumnName("SESERuleCategory");            
        }
    }
}
