using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowVersionStatesMap : EntityTypeConfiguration<WorkFlowVersionState>
    {
        public WorkFlowVersionStatesMap()
        {
            // Primary Key
            this.HasKey(t => t.WorkFlowVersionStateID);
            this.ToTable("WorkFlowVersionState", "WF");
            this.Property(t => t.WorkFlowVersionStateID).HasColumnName("WorkFlowVersionStateID");
            this.Property(t => t.WorkFlowVersionID).HasColumnName("WorkFlowVersionID");
            this.Property(t => t.WFStateID).HasColumnName("WFStateID");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.WFStateGroupID).HasColumnName("WFStateGroupID");

            this.HasRequired(t => t.WorkFlowCategoryMapping)
                .WithMany(t => t.WorkFlowVersionStates)
                .HasForeignKey(d => d.WorkFlowVersionID);

            this.HasRequired(t => t.WorkFlowState)
                .WithMany(t => t.WorkFlowVersionStates)
                .HasForeignKey(d => d.WFStateID);
        }
    }
}
