using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormReportVersionMap : EntityTypeConfiguration<FormReportVersion>
    {
        public FormReportVersionMap()
        {
            // Primary Key Column
            this.HasKey(t => t.ReportVersionID);

            // Restricted Length Columns
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Configuring the Table name that this entity is mapped to.
            this.ToTable("FormReportVersion", "Rpt");

            // Configuring Name of DB columns to map Entity properties 
            this.Property(t => t.ReportVersionID).HasColumnName("ReportVersionID");
            this.Property(t => t.ReportID).IsRequired().HasColumnName("ReportID");
            this.Property(t => t.TenantID).IsRequired().HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.FormReportVersions)
                .HasForeignKey(t => t.TenantID);

            this.HasRequired(t => t.FormReport)
                .WithMany(t => t.FormReportVersions)
                .HasForeignKey(t => t.ReportID);
        }
    }
}
