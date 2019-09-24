using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class AuditReportMap : EntityTypeConfiguration<AuditReport>
    {
        public AuditReportMap()
        {
            // Primary Key
            this.HasKey(t => t.AuditReportID);

            // Properties
            this.Property(t => t.UpdateStatus)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.AddedBy)
               .IsRequired()
               .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
               .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("AuditReport", "GU");
            this.Property(t => t.AuditReportID).HasColumnName("AuditReportID");
           // this.Property(t => t.IASFolderMapID).IsRequired().HasColumnName("IASFolderMapID");
            this.Property(t => t.BatchID).IsRequired().HasColumnName("BatchID");
            this.Property(t => t.UpdateStatus).HasColumnName("UpdateStatus");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).IsRequired().HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            this.HasRequired(t => t.Batch)
                .WithMany(t => t.AuditReports)
                .HasForeignKey(d => d.BatchID);

            //this.HasRequired(t => t.IASFolderMap)
            //   .WithMany(t => t.AuditReports)
            //   .HasForeignKey(d => d.IASFolderMapID);

        }
    }
}
