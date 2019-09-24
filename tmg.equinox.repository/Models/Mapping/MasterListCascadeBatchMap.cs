using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListCascadeBatchMap : EntityTypeConfiguration<MasterListCascadeBatch>
    {
        public MasterListCascadeBatchMap()
        {
            // Primary Key
            this.HasKey(t => t.MasterListCascadeBatchID);
            // Properties
            this.Property(t => t.EndDate);
            this.Property(t => t.Message);
            this.Property(t => t.FormDesignVersionID)
                .IsRequired();
            this.Property(t => t.QueuedDate)
                .IsRequired();
            this.Property(t => t.StartDate);
            this.Property(t => t.Status)
                .IsRequired();
            this.Property(t => t.QueuedBy);

            // Table & Column Mappings
            this.ToTable("MasterListCascadeBatch", "ML");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.MasterListCascadeBatchID).HasColumnName("MasterListCascadeBatchID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.QueuedDate).HasColumnName("QueuedDate");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.QueuedBy).HasColumnName("QueuedBy");

            this.HasRequired(t => t.MasterListCascadeStatus)
                .WithMany(t => t.MasterListCascadeBatches)
                .HasForeignKey(d => d.Status);
        }
    }
}
