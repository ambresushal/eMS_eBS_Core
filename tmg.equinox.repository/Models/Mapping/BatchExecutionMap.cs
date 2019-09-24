using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class BatchExecutionMap : EntityTypeConfiguration<BatchExecution>
    {
        public BatchExecutionMap()
        {

            //primary key column
            this.HasKey(t => t.BatchExecutionID);

            //Restricted Length Columns              

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.RollBackComments)
                 .HasMaxLength(1000);

            this.ToTable("BatchExecution", "GU");
            // Configuring Name of DB columns to map Entity properties 
            this.Property(t => t.BatchExecutionID).HasColumnName("BatchExecutionID");
            this.Property(t => t.BatchID).IsRequired().HasColumnName("BatchID"); 
            this.Property(t => t.BatchExecutionStatusID).HasColumnName("BatchExecutionStatusID");
            this.Property(t => t.StartDateTime).IsRequired().HasColumnName("StartDateTime");
            this.Property(t => t.EndDateTime).IsRequired().HasColumnName("EndDateTime");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate   ");
            this.Property(t => t.AddedBy).IsRequired().HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.RollBackComments).HasColumnName("RollBackComments");

            this.HasRequired(t => t.Batch)
                .WithMany(t => t.BatchExecutions)
                .HasForeignKey(t => t.BatchID);

            this.HasRequired(t => t.BatchExecutionStatus)
               .WithMany(t => t.BatchExecutions)
               .HasForeignKey(t => t.BatchExecutionStatusID);
        }
    }
}
