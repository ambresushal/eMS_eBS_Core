using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class ConfigRulesTesterDataMap : EntityTypeConfiguration<ConfigRulesTesterData>
    {
        public ConfigRulesTesterDataMap()
        {
            // Primary Key
            this.HasKey(t => t.RuleTersterId);

            // Table & Column Mappings
            this.ToTable("ConfigRuleTestData", "UI");
            this.Property(t => t.RuleTersterId).HasColumnName("RuleTersterId");
            this.Property(t => t.FormDesignVersionId).HasColumnName("FormDesignVersionId");
            this.Property(t => t.UIElementId).HasColumnName("UIElementId");
            this.Property(t => t.RuleId).HasColumnName("RuleId");
            this.Property(t => t.TestData).HasColumnName("TestData");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
