using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportingTableInfoMap : EntityTypeConfiguration<ReportingTableInfo>
    {
        public ReportingTableInfoMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("ReportingTableInfo", "Rpt");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Name).HasColumnName("Name").IsRequired();
            this.Property(t => t.SchemaName).HasColumnName("SchemaName").IsRequired();
            this.Property(t => t.ParentName).HasColumnName("ParentName");
            this.Property(t => t.DesignId).HasColumnName("DesignId").IsRequired();
            this.Property(t => t.DesignVersionId).HasColumnName("DesignVersionId").IsRequired();
            this.Property(t => t.CreationDate).HasColumnName("CreationDate").IsRequired();
            this.Property(t => t.Label).HasColumnName("Label");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.DesignType).HasColumnName("DesignType");
            this.Property(t => t.DesignVersionNumber).HasColumnName("DesignVersionNumber");
        }
    }
}
