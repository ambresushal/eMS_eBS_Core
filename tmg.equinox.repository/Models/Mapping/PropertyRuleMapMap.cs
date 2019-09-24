using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PropertyRuleMapMap : EntityTypeConfiguration<PropertyRuleMap>
    {
        public PropertyRuleMapMap()
        {
            // Primary Key
            this.HasKey(t => t.PropertyRuleMapID);

            // Properties
            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("PropertyRuleMap", "UI");
            this.Property(t => t.PropertyRuleMapID).HasColumnName("PropertyRuleMapID");
            this.Property(t => t.RuleID).HasColumnName("RuleID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.TargetPropertyID).HasColumnName("TargetPropertyID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsCustomRule).HasColumnName("IsCustomRule");

            // Relationships
            this.HasRequired(t => t.Rule)
                .WithMany(t => t.PropertyRuleMaps)
                .HasForeignKey(d => d.RuleID);
            this.HasRequired(t => t.TargetProperty)
                .WithMany(t => t.PropertyRuleMaps)
                .HasForeignKey(d => d.TargetPropertyID);
            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.PropertyRuleMaps)
                .HasForeignKey(d => d.UIElementID);

        }
    }
}
