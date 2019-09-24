using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RuleMap : EntityTypeConfiguration<Rule>
    {
        public RuleMap()
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
                .HasMaxLength(7000);

            this.Property(t => t.ResultSuccess)
                .HasMaxLength(1000);

            this.Property(t => t.ResultFailure)
                .HasMaxLength(1000);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Rule", "UI");
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
            this.Property(t => t.RunOnLoad).HasColumnName("RunOnLoad");
            this.Property(t => t.RuleCode).HasColumnName("RuleCode");
            this.Property(t => t.IsStandard).HasColumnName("IsStandard");
            this.Property(t => t.TargetTypeID).HasColumnName("TargetTypeID");

            // Relationships
            this.HasRequired(t => t.RuleTargetType)
                .WithMany(t => t.Rules)
                .HasForeignKey(d => d.RuleTargetTypeID);

        }
    }
}
