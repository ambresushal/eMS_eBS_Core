using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListCascadeStatusMap : EntityTypeConfiguration<MasterListCascadeStatus>
    {
        public MasterListCascadeStatusMap()
        {
            // Primary Key
            this.HasKey(t => t.StatusID);

            // Properties
            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("MasterListCascadeStatus", "ML");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
