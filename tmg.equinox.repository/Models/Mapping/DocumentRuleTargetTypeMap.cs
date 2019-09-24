using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentRuleTargetTypeMap : EntityTypeConfiguration<DocumentRuleTargetType>
    {
        public DocumentRuleTargetTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentRuleTargetTypeID);

            // Table & Column Mappings
            this.ToTable("DocumentRuleTargetType", "UI");
            this.Property(t => t.DocumentRuleTargetTypeID).HasColumnName("DocumentRuleTargetTypeID");
            this.Property(t => t.DocumentRuleTargetTypeCode).HasColumnName("DocumentRuleTargetTypeCode");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
