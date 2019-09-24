using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RepeaterKeyFilterMap : EntityTypeConfiguration<RepeaterKeyFilter>
    {
        public RepeaterKeyFilterMap()
        {
            // Primary Key
            this.HasKey(t => t.RepeaterKeyID);

            // Properties
            this.Property(t => t.RepeaterKeyValue)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("RepeaterKeyFilter", "UI");
            this.Property(t => t.RepeaterKeyID).HasColumnName("RepeaterKeyID");
            this.Property(t => t.ExpressionID).HasColumnName("ExpressionID");
            this.Property(t => t.RepeaterKey).HasColumnName("RepeaterKey");
            this.Property(t => t.RepeaterKeyValue).HasColumnName("RepeaterKeyValue");
            this.Property(t => t.IsRightOperand).HasColumnName("IsRightOperand");
            this.Property(t => t.PropertyRuleMapID).HasColumnName("PropertyRuleMapID");
        }
    }
}
