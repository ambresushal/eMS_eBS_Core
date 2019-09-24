using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class GlobalUpateExecutionLogMap : EntityTypeConfiguration<GlobalUpateExecutionLog>
    {
        public GlobalUpateExecutionLogMap()
        {
            // Primary Key
            this.HasKey(t => t.GlobalUpateExecutionLogID);

            // Properties
            this.Property(t => t.Comments)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NewFolderVersionNumber)
               .IsRequired()
               .HasMaxLength(20);

            this.Property(t => t.Result)
                .IsRequired()
               .HasMaxLength(20);

            this.Property(t => t.BatchID)
               .IsRequired();

            // Table & Column Mappings
            this.ToTable("GlobalUpateExecutionLog", "GU");
            this.Property(t => t.GlobalUpateExecutionLogID).IsRequired().HasColumnName("GlobalUpateExecutionLogID");
            this.Property(t => t.BatchID).IsRequired().HasColumnName("BatchID");
            this.Property(t => t.OldFolderVersionID).IsRequired().HasColumnName("OldFolderVersionID");
            this.Property(t => t.NewFolderVersionID).IsRequired().HasColumnName("NewFolderVersionID");
            this.Property(t => t.Result).IsRequired().HasColumnName("Result");
            this.Property(t => t.Comments).IsRequired().HasColumnName("Comments"); 
            this.Property(t => t.NewFolderVersionNumber).IsRequired().HasColumnName("NewFolderVersionNumber");

        }
    }
}
