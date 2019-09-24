using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceRuleExecutionServerLogMap : EntityTypeConfiguration<FormInstanceRuleExecutionServerLog>
    {
        public FormInstanceRuleExecutionServerLogMap()
        {
            // Primary Key
            this.HasKey(t => t.RowID);

            // Table & Column Mappings
            this.ToTable("FormInstanceRuleExecutionServerLog", "Fldr");
            this.Property(t => t.RowID).HasColumnName("RowID");
            this.Property(t => t.SessionID).HasColumnName("SessionID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.LoadedElement).HasColumnName("LoadedElement");
            this.Property(t => t.IsParentRow).HasColumnName("IsParentRow");
            this.Property(t => t.ParentRowID).HasColumnName("ParentRowID");
            this.Property(t => t.OnEvent).HasColumnName("OnEvent");
            this.Property(t => t.ElementID).HasColumnName("ElementID");
            this.Property(t => t.OldValue).HasColumnName("OldValue");
            this.Property(t => t.NewValue).HasColumnName("NewValue");
            this.Property(t => t.ImpactedElementID).HasColumnName("ImpactedElementID");
            this.Property(t => t.RuleID).HasColumnName("RuleID");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.LogFor).HasColumnName("LogFor");
        }
    }
}
