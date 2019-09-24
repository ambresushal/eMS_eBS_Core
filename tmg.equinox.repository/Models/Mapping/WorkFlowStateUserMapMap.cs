using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowStateUserMapMap : EntityTypeConfiguration<WorkFlowStateUserMap>
    {
        public WorkFlowStateUserMapMap()
        {
            // Primary Key
            this.HasKey(t =>t.WFStateUserMapID);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WorkFlowStateUserMap", "Fldr");
            this.Property(t => t.WFStateUserMapID).HasColumnName("WFStateUserMapID");
            this.Property(t => t.UserID).HasColumnName("UserID");            
            this.Property(t => t.WFStateID).HasColumnName("WFStateID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.ApprovedWFStateID).HasColumnName("ApprovedWFStateID");
            this.Property(t => t.ApplicableTeamID).HasColumnName("ApplicableTeamID");
            
            

            this.HasRequired(t => t.Folder)
                .WithMany(t => t.WorkFlowStateUserMaps)
                .HasForeignKey(d => d.FolderID);

            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.WorkFlowStateUserMaps)
                .HasForeignKey(d => d.FolderVersionID);

            this.HasRequired(t => t.WorkFlowVersionState)
                .WithMany(t => t.WorkFlowStateUserMaps)
                .HasForeignKey(d => d.WFStateID);

            this.HasRequired(t => t.User)
                .WithMany(t => t.WorkFlowStateUserMaps)
                .HasForeignKey(d => d.UserID);

            this.HasRequired(t => t.WorkFlowState1)
               .WithMany(t => t.WorkFlowStateUserMaps1)
               .HasForeignKey(d => d.ApprovedWFStateID);
        }
    }
}
