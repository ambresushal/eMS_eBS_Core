using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WFVersionStatesApprovalTypeMap : EntityTypeConfiguration<WFVersionStatesApprovalType>
    {
        public WFVersionStatesApprovalTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.WFVersionStatesApprovalTypeID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WFVersionStatesApprovalType", "WF");
            this.Property(t => t.WFVersionStatesApprovalTypeID).HasColumnName("WFVersionStatesApprovalTypeID");
            this.Property(t => t.WorkFlowVersionStateID).HasColumnName("WorkFlowVersionStateID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.WorkFlowStateApprovalTypeID).HasColumnName("WorkFlowStateApprovalTypeID");

             //Relationships
            this.HasRequired(t => t.WorkFlowVersionStates)
                .WithMany(t => t.WFVersionStatesApprovalType)
                .HasForeignKey(d => d.WorkFlowVersionStateID);
            //this.HasRequired(t => t.WorkFlowStateApprovalTypeMaster)
            //    .WithMany(t => t.WFVersionStatesApprovalType)
            //    .HasForeignKey(d => d.WFVersionStatesApprovalTypeID);

        }
    }
}
