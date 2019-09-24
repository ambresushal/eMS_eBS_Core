using System;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class SESERuleCategoryMap : EntityTypeConfiguration<SESERuleCategory>
    {
        public SESERuleCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.SESERuleCategoryID);

            // Table & Column Mappings
            this.ToTable("SESERuleCategory", "Master");
            this.Property(t => t.SESERuleCategoryID).HasColumnName("SESERuleCategoryID");
            this.Property(t => t.RuleCategory).HasColumnName("RuleCategory");
        }
    }
}
