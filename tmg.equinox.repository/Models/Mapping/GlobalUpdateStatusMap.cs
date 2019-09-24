using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{

    public class GlobalUpdateStatusMap : EntityTypeConfiguration<GlobalUpdateStatus>
    {
        public GlobalUpdateStatusMap()
        {
            // Primary Key
            this.HasKey(t => t.GlobalUpdateStatusID);

            // Properties
            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.ToTable("GlobalUpdateStatus", "GU");
            this.Property(t => t.GlobalUpdateStatusID).HasColumnName("GlobalUpdateStatusID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
        }
    }
}
