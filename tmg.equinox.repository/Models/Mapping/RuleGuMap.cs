using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RuleGuMap : EntityTypeConfiguration<RuleGu>
    {
        public RuleGuMap()
        {
            // Primary Key
            this.HasKey(t => t.RuleID);

            // Properties
            this.Property(t => t.RuleName)
                .IsRequired();
            this.Property(t => t.IsResultSuccessElement)
                .IsRequired();
            this.Property(t => t.IsResultFailureElement)
                .IsRequired();

            this.Property(t => t.RuleDescription)
                .HasMaxLength(1000);

            this.Property(t => t.ResultSuccess)
                .HasMaxLength(1000);

            this.Property(t => t.ResultFailure)
                .HasMaxLength(1000);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Rule", "GU");
            this.Property(t => t.RuleID).HasColumnName("RuleID");
            this.Property(t => t.RuleName).HasColumnName("RuleName");
            this.Property(t => t.RuleDescription).HasColumnName("RuleDescription");
            this.Property(t => t.RuleTargetTypeID).HasColumnName("RuleTargetTypeID");
            this.Property(t => t.ResultSuccess).HasColumnName("ResultSuccess");
            this.Property(t => t.ResultFailure).HasColumnName("ResultFailure");
            this.Property(t => t.IsResultFailureElement).HasColumnName("IsResultFailureElement");
            this.Property(t => t.IsResultSuccessElement).HasColumnName("IsResultSuccessElement");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.TargetPropertyID).HasColumnName("TargetPropertyID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            // Relationships
            this.HasRequired(t => t.RuleTargetType)
                .WithMany(t => t.RulesGu)
                .HasForeignKey(d => d.RuleTargetTypeID);

            this.HasRequired(t => t.TargetProperty)
                .WithMany(t => t.RulesGu)
                .HasForeignKey(d => d.TargetPropertyID);

            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.RulesGu)
                .HasForeignKey(d => d.UIElementID);

            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.RulesGu)
                .HasForeignKey(d => d.GlobalUpdateID);
        }
    }
}
