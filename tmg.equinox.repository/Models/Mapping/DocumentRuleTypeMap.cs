using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentRuleTypeMap : EntityTypeConfiguration<DocumentRuleType>
    {
        public DocumentRuleTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentRuleTypeID);

            // Table & Column Mappings
            this.ToTable("DocumentRuleType", "UI");
            this.Property(t => t.DocumentRuleTypeID).HasColumnName("DocumentRuleTypeID");
            this.Property(t => t.DocumentRuleTypeCode).HasColumnName("DocumentRuleTypeCode");
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
