using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListCascadeBatchDetailMap : EntityTypeConfiguration<MasterListCascadeBatchDetail>
    {
        public MasterListCascadeBatchDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.MasterListCascadeDetailID);
            // Properties
            this.Property(t => t.IsTargetMasterList)
                .IsRequired();
            this.Property(t => t.MasterListCascadeBatchID)
                .IsRequired();
            this.Property(t => t.MasterListCascadeID)
                .IsRequired();
            this.Property(t => t.Message);
            this.Property(t => t.NewFolderVersionID)
                .IsRequired();
            this.Property(t => t.NewFormInstanceID)
                .IsRequired();
            this.Property(t => t.PreviousFolderVersionID)
                .IsRequired();
            this.Property(t => t.PreviousFormInstanceID)
                .IsRequired();
            this.Property(t => t.ProcessedDate);
            this.Property(t => t.Status)
                .IsRequired();
            this.Property(t => t.TargetFolderID)
                .IsRequired();
            // Table & Column Mappings
            this.ToTable("MasterListCascadeBatchDetail", "ML");
            this.Property(t => t.IsTargetMasterList).HasColumnName("IsTargetMasterList");
            this.Property(t => t.MasterListCascadeBatchID).HasColumnName("MasterListCascadeBatchID");
            this.Property(t => t.MasterListCascadeDetailID).HasColumnName("MasterListCascadeDetailID");
            this.Property(t => t.MasterListCascadeID).HasColumnName("MasterListCascadeID");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.NewFolderVersionID).HasColumnName("NewFolderVersionID");
            this.Property(t => t.NewFormInstanceID).HasColumnName("NewFormInstanceID");
            this.Property(t => t.PreviousFolderVersionID).HasColumnName("PreviousFolderVersionID");
            this.Property(t => t.PreviousFormInstanceID).HasColumnName("PreviousFormInstanceID");
            this.Property(t => t.ProcessedDate).HasColumnName("ProcessedDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.TargetFolderID).HasColumnName("TargetFolderID");


            this.HasRequired(t => t.MasterListCascadeStatus)
                .WithMany(t => t.MasterListCascadeBatchDetails)
                .HasForeignKey(d => d.Status);

            this.HasRequired(t => t.MasterListCascadeBatch)
                .WithMany(t => t.MasterListCascadeBatchDetails)
                .HasForeignKey(d => d.MasterListCascadeBatchID);
        }
    }
}
