using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public  class BatchExecutionStatusMap: EntityTypeConfiguration<BatchExecutionStatus>
    {
        public BatchExecutionStatusMap()
        {
            // Primary Key
            this.HasKey(t => t.BatchExecutionStatusID);

            // Properties
            this.Property(t => t.StatusName)
                .HasMaxLength(50);

            this.Property(t => t.Description)
               .IsRequired()
               .HasMaxLength(200);
            

            // Table & Column Mappings
            this.ToTable("BatchExecutionStatus", "GU");
            this.Property(t => t.BatchExecutionStatusID).HasColumnName("BatchExecutionStatusID");
            this.Property(t => t.StatusName).IsRequired().HasColumnName("StatusName");
            this.Property(t => t.Description).HasColumnName("Description");          

        }
    }
}
