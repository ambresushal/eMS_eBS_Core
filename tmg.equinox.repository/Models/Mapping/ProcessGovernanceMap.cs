using System;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models.CCRTranslator;

namespace tmg.equinox.repository.Models.Mapping
{

    public class ProcessGovernanceMap : EntityTypeConfiguration<ProcessGovernance>
    {

        public ProcessGovernanceMap()
        {
            this.HasKey(t => t.ProcessGovernance1up);
            this.ToTable("ProcessGovernance", "Setup");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.Processor1Up).HasColumnName("Processor1Up");
            this.Property(t => t.ProcessStatus1Up).HasColumnName("ProcessStatus1Up");
            this.Property(t => t.ProcessType).HasColumnName("ProcessType");
            this.Property(t => t.RunDate).HasColumnName("RunDate");
            this.Property(t => t.InRecoveryMode).HasColumnName("InRecoveryMode");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.HasError).HasColumnName("HasError");
            this.Property(t => t.ErrorDescription).HasColumnName("ErrorDescription");
        }
    }
}
