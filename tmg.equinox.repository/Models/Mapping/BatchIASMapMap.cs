using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class BatchIASMapMap : EntityTypeConfiguration<BatchIASMap>
    {
        public BatchIASMapMap()
        {
            // Primary Key
            this.HasKey(t => t.BatchIASMapID);
         

            this.ToTable("BatchIASMap", "GU");
            this.Property(t => t.BatchIASMapID).HasColumnName("BatchIASMapID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            
            // Relationships
            this.HasRequired(t => t.Batch)
                .WithMany(t => t.BatchIASMaps)
                .HasForeignKey(d => d.BatchID);

            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.BatchIASMaps)
                .HasForeignKey(d => d.GlobalUpdateID);
        }
    }
}
