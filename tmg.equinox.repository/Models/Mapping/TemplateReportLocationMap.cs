using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportLocationMap : EntityTypeConfiguration<TemplateReportLocation>
    {
        public TemplateReportLocationMap()
        {
            // Primary Key
            this.HasKey(t => t.LocationID);

            // Properties
            this.Property(t => t.LocationName).IsRequired().HasMaxLength(100);
            this.Property(t => t.LocationDescription).HasMaxLength(250);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TemplateReportLocation", "TmplRpt");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.LocationName).HasColumnName("LocationName");
            this.Property(t => t.LocationDescription).HasColumnName("LocationDescription");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.TemplateReportVersion)
                .WithMany(t => t.TemplateReportLocations)
                .HasForeignKey(t => t.TemplateReportVersionID);
        }
    }
}
