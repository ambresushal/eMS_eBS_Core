using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowStateFolderVersionMapMap : EntityTypeConfiguration<WorkFlowStateFolderVersionMap>
    {
        public WorkFlowStateFolderVersionMapMap()
        {

            this.HasKey(t => t.WFStateFolderVersionMapID);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WorkFlowStateFolderVersionMap", "Fldr");
            this.Property(t => t.WFStateFolderVersionMapID).HasColumnName("WFStateFolderVersionMapID");
            this.Property(t => t.ApplicableTeamID).HasColumnName("ApplicableTeamID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");

            this.HasRequired(t => t.Folder)
                .WithMany(t => t.WorkFlowStateFolderVersionMaps)
                .HasForeignKey(d => d.FolderID);

            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.WorkFlowStateFolderVersionMaps)
                .HasForeignKey(d => d.FolderVersionID);

            this.HasRequired(t => t.ApplicableTeam)
                .WithMany(t => t.WorkFlowStateFolderVersionMaps)
                .HasForeignKey(d => d.ApplicableTeamID);
            

        }

    }
}
