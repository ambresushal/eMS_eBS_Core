using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListCascadeDocumentRuleMap : EntityTypeConfiguration<MasterListCascadeDocumentRule>
    {
        public MasterListCascadeDocumentRuleMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentRuleID);

            // Properties
            this.Property(t => t.DisplayText)
                .HasMaxLength(5000);

            this.Property(t => t.Description)
                .HasMaxLength(5000);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.TargetElementPath)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("MasterListCascadeDocumentRule", "ML");
            this.Property(t => t.DocumentRuleID).HasColumnName("DocumentRuleID");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DocumentRuleTypeID).HasColumnName("DocumentRuleTypeID");
            this.Property(t => t.DocumentRuleTargetTypeID).HasColumnName("DocumentRuleTargetTypeID");
            this.Property(t => t.RuleJSON).HasColumnName("RuleJSON");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.MasterListFormDesignID).HasColumnName("MasterListFormDesignID");
            this.Property(t => t.MasterListFormDesignVersionID).HasColumnName("MasterListFormDesignVersionID");
            this.Property(t => t.TargetUIElementID).HasColumnName("TargetUIElementID");
            this.Property(t => t.TargetElementPath).HasColumnName("TargetElementPath");
            this.Property(t => t.CompiledRuleJSON).HasColumnName("CompiledRuleJSON");
            this.Property(t => t.SequenceNo).HasColumnName("Sequence");

        }
    }
}
