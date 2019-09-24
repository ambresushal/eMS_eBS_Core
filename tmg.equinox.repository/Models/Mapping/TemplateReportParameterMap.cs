using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportParameterMap : EntityTypeConfiguration<TemplateReportParameter>
    {
        public TemplateReportParameterMap()
        {
            // Primary Key
            this.HasKey(t => t.ParameterTypeID);

            // Properties
            this.Property(t => t.ParameterName).IsRequired().HasMaxLength(50);
            this.Property(t => t.ParameterDescription).HasMaxLength(250);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TemplateReportParameter", "TmplRpt");
            this.Property(t => t.ParameterTypeID).HasColumnName("ParameterTypeID");
            this.Property(t => t.ParameterName).HasColumnName("ParameterName");
            this.Property(t => t.ParameterDescription).HasColumnName("ParameterDescription");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

        }
    }
}
