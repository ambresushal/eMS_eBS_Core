using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceComplianceValidationlogMap : EntityTypeConfiguration<FormInstanceComplianceValidationlog>
    {
        public FormInstanceComplianceValidationlogMap()
        {
            this.HasKey(t => t.logId);

            // Table & Column Mappings
            this.ToTable("FormInstanceComplianceValidationlog", "Fldr");
            this.Property(t => t.ComplianceType).HasColumnName("ComplianceType").HasMaxLength(50);
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.No).HasColumnName("No");
            this.Property(t => t.Error).HasColumnName("Error");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.ValidationType).HasColumnName("ValidationType").HasMaxLength(20); 

        }
    }
}
