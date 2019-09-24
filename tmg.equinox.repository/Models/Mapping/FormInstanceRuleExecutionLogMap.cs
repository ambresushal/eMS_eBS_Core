using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceRuleExecutionLogMap : EntityTypeConfiguration<FormInstanceRuleExecutionLog>
    {
        public FormInstanceRuleExecutionLogMap()
        {
            // Primary Key
            this.HasKey(t => t.RuleExecutionLoggerID);

            // Table & Column Mappings
            this.ToTable("FormInstanceRuleExecutionLog", "Fldr");
            this.Property(t => t.RuleExecutionLoggerID).HasColumnName("RuleExecutionLoggerID");
            this.Property(t => t.SessionID).HasColumnName("SessionID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersion).HasColumnName("FolderVersion");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.IsParentRow).HasColumnName("IsParentRow");
            this.Property(t => t.ParentRowID).HasColumnName("ParentRowID");
            this.Property(t => t.OnEvent).HasColumnName("OnEvent");
            this.Property(t => t.ElementID).HasColumnName("ElementID");
            this.Property(t => t.ElementLabel).HasColumnName("ElementLabel");
            this.Property(t => t.ElementPath).HasColumnName("ElementPath");
            this.Property(t => t.OldValue).HasColumnName("OldValue");
            this.Property(t => t.NewValue).HasColumnName("NewValue");
            this.Property(t => t.ImpactedElementID).HasColumnName("ImpactedElementID");
            this.Property(t => t.ImpactedElementLabel).HasColumnName("ImpactedElementLabel");
            this.Property(t => t.ImpactedElementPath).HasColumnName("ImpactedElementPath");
            this.Property(t => t.ImpactDescription).HasColumnName("ImpactDescription");
            this.Property(t => t.PropertyType).HasColumnName("PropertyType");
            this.Property(t => t.RuleID).HasColumnName("RuleID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsNewRecord).HasColumnName("IsNewRecord");
        }
    }
}
