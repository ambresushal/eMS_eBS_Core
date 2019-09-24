using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TargetRepeaterKeyFilterMap : EntityTypeConfiguration<TargetRepeaterKeyFilter>
    {
        public TargetRepeaterKeyFilterMap()
        {
            // Primary Key
            this.HasKey(t => t.TargetRepeaterKeyID);

            // Properties
            this.Property(t => t.RepeaterKeyValue)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("TargetRepeaterKeyFilter", "UI");
            this.Property(t => t.TargetRepeaterKeyID).HasColumnName("TargetRepeaterKeyID");
            this.Property(t => t.RuleID).HasColumnName("RuleID");
            this.Property(t => t.RepeaterKey).HasColumnName("RepeaterKey");
            this.Property(t => t.RepeaterKeyValue).HasColumnName("RepeaterKeyValue");
            this.Property(t => t.PropertyRuleMapID).HasColumnName("PropertyRuleMapID");
        }
    }
}
