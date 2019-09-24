using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class SBCReportServiceMasterMap :EntityTypeConfiguration<SBCReportServiceMaster>
    {
        public SBCReportServiceMasterMap()
        {

            // Primary Key
            this.HasKey(t => t.SBCReportServiceMasterID);

            // Properties
            this.Property(t => t.ReportPlaceHolderID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Category1Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Category2Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Category3Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.POSName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NetworkName)
                .IsRequired()
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("SBCReportServiceMaster", "Rpt");
            this.Property(t => t.SBCReportServiceMasterID).HasColumnName("SBCReportServiceMasterID");
            this.Property(t => t.ReportPlaceHolderID).HasColumnName("ReportPlaceHolderID");
            this.Property(t => t.Category1Name).HasColumnName("Category1Name");
            this.Property(t => t.Category2Name).HasColumnName("Category2Name");
            this.Property(t => t.Category3Name).HasColumnName("Category3Name");
            this.Property(t => t.POSName).HasColumnName("POSName");
            this.Property(t => t.NetworkName).HasColumnName("NetworkName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.SBCReportServiceMasters)
                .HasForeignKey(d => d.TenantID);

        }
    }
}
