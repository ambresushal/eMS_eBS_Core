using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormReportVersionMapMap : EntityTypeConfiguration<tmg.equinox.domain.entities.Models.FormReportVersionMap>
    {
        public FormReportVersionMapMap()
        {
            // Primary Key Column
            this.HasKey(t => t.ReportVersionMapID);

            // Restricted Length Columns
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Configuring the Name of the DB Table that this class maps to
            this.ToTable("FormReportVersionMap", "Rpt");

            this.Property(t => t.ReportVersionMapID).HasColumnName("ReportVersionMapID");
            this.Property(t => t.ReportVersionID).HasColumnName("ReportVersionID");
            this.Property(t => t.PlaceHolderID).IsRequired().HasColumnName("PlaceHolderID");
            this.Property(t => t.MapType).HasColumnName("MapType");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.FormDataPath).HasColumnName("FormDataPath");
            this.Property(t => t.ValueExpression).HasColumnName("ValueExpression");
            this.Property(t => t.FormDesignID).IsRequired().HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).IsRequired().HasColumnName("FormDesignVersionID");
            this.Property(t => t.CoveredServiceID).HasColumnName("CoveredServiceID"); 
            this.Property(t => t.AddedBy).IsRequired().HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.FilterExpression).HasColumnName("FilterExpression");
            this.Property(t => t.FilterExpressionValue).HasColumnName("FilterExpressionValue");
            this.Property(t => t.ValueFormat).HasColumnName("ValueFormat");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

            this.HasRequired(t => t.FormReportVersion)
                .WithMany(t => t.FormReportVersionMaps)
                .HasForeignKey(t => t.ReportVersionID);

            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.FormReportVersionMaps)
                .HasForeignKey(t => t.FormDesignID);

            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.FormReportVersionMaps)
                .HasForeignKey(t => t.FormDesignVersionID);
        }
    }
}
