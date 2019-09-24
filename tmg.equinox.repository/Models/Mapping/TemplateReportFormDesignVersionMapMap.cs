using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportFormDesignVersionMapMap : EntityTypeConfiguration<TemplateReportFormDesignVersionMap>
    {
        public TemplateReportFormDesignVersionMapMap()
        {
            // Primary Key
            this.HasKey(t => t.TemplateReportFormDesignVersionMapID);

            // Properties
            this.Property(t => t.DataSourceName).IsRequired().HasMaxLength(50);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TemplateReportFormDesignVersionMap", "TmplRpt");
            this.Property(t => t.TemplateReportFormDesignVersionMapID).HasColumnName("TemplateReportFormDesignVersionMapID");
            this.Property(t => t.TemplateReportID).HasColumnName("TemplateReportID");
            this.Property(t => t.TemplateReportVersionID).HasColumnName("TemplateReportVersionID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.DataSourceName).HasColumnName("DataSourceName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.TemplateReportFormDesignVersionMaps)
                .HasForeignKey(t => t.FormDesignID);

            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.TemplateReportFormDesignVersionMaps)
                .HasForeignKey(t => t.FormDesignVersionID);

            this.HasRequired(t => t.TemplateReport)
                .WithMany(t => t.TemplateReportFormDesignVersionMaps)
                .HasForeignKey(t => t.TemplateReportID);

            this.HasRequired(t => t.TemplateReportVersion)
                .WithMany(t => t.TemplateReportFormDesignVersionMaps)
                .HasForeignKey(t => t.TemplateReportVersionID);
        }
    }
}
