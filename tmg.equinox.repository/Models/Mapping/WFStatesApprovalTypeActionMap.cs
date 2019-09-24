using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WFStatesApprovalTypeActionMap : EntityTypeConfiguration<WFStatesApprovalTypeAction>
    {
        public WFStatesApprovalTypeActionMap()
        {
            // Primary Key
            this.HasKey(t => t.WFStatesApprovalTypeActionID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WFStatesApprovalTypeAction", "WF");
            this.Property(t => t.WFStatesApprovalTypeActionID).HasColumnName("WFStatesApprovalTypeActionID");
            this.Property(t => t.WFVersionStatesApprovalTypeID).HasColumnName("WFVersionStatesApprovalTypeID");
            this.Property(t => t.ActionID).HasColumnName("ActionID");
            this.Property(t => t.ActionResponse).HasColumnName("ActionResponse");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            //Relationships
            this.HasRequired(t => t.WFVersionStatesApprovalType)
                .WithMany(t => t.WFStatesApprovalTypeActions)
                .HasForeignKey(d => d.WFVersionStatesApprovalTypeID);
        }
    }
}
