using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    class WorkFlowVersionStatesAccessMap : EntityTypeConfiguration<WorkFlowVersionStatesAccess>
    {
        public WorkFlowVersionStatesAccessMap()
        {
            // Primary Key
            this.HasKey(t => t.WorkFlowVersionStatesAccessID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WorkFlowVersionStatesAccess", "WF");
            this.Property(t => t.WorkFlowVersionStatesAccessID).HasColumnName("WorkFlowVersionStatesAccessID");
            this.Property(t => t.WorkFlowVersionStateID).HasColumnName("WorkFlowVersionStateID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.RoleID).HasColumnName("RoleID");
            //this.Property(t => t.IsOwner).HasColumnName("IsOwner");
            //this.Property(t => t.EditPermission).HasColumnName("EditPermission");

             //Relationships
            this.HasRequired(t => t.WorkFlowVersionStates)
                .WithMany(t => t.WorkFlowVersionStatesAccess)
                .HasForeignKey(d => d.WorkFlowVersionStateID);
            this.HasRequired(t => t.UserRole)
                .WithMany(t => t.WorkFlowVersionStatesAccess)
                .HasForeignKey(d => d.RoleID);

        }
    }
}
