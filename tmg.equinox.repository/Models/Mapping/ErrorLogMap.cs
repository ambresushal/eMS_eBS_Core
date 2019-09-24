using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ErrorLogMap : EntityTypeConfiguration<ErrorLog>
    {
        public ErrorLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ErrorLogID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.RuleErrorDescription)
               .HasMaxLength(2000);

            this.Property(t => t.DataSourceErrorDescription)
              .HasMaxLength(2000);

            this.Property(t => t.ValidationErrorDescription)
               .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("ErrorLog", "GU");
            this.Property(t => t.ErrorLogID).HasColumnName("ErrorLogID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.IASElementExportID).HasColumnName("IASElementExportID");
            this.Property(t => t.RuleErrorDescription).HasColumnName("RuleErrorDescription");
            this.Property(t => t.DataSourceErrorDescription).HasColumnName("DataSourceErrorDescription");
            this.Property(t => t.ValidationErrorDescription).HasColumnName("ValidationErrorDescription");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.ErrorLogs)
                .HasForeignKey(d => d.GlobalUpdateID);

            this.HasRequired(t => t.IASElementExport)
                .WithMany(t => t.ErrorLogs)
                .HasForeignKey(d => d.IASElementExportID);

        }
    }
}
