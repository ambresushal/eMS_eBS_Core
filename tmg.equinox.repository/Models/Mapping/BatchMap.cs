using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class BatchMap : EntityTypeConfiguration<Batch>
    {
        public BatchMap()
        {

            //primary key column
            this.HasKey(t => t.BatchID);

            //Restricted Length Columns
            this.Property(t => t.BatchName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ApprovedBy)
               .HasMaxLength(50);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);


            this.ToTable("Batch", "GU");
            // Configuring Name of DB columns to map Entity properties 
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.BatchName).IsRequired().HasColumnName("BatchName");
            this.Property(t => t.ExecutionType).IsRequired().HasColumnName("ExecutionType");
            this.Property(t => t.ScheduleDate).HasColumnName("ScheduleDate");
            this.Property(t => t.ScheduledTime).HasColumnName("ScheduledTime");
            this.Property(t => t.IsApproved).IsRequired().HasColumnName("IsApproved");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).IsRequired().HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.ApprovedDate).HasColumnName("ApprovedDate");           

    }
    }
}
