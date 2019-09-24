using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class IASFolderMapMap : EntityTypeConfiguration<IASFolderMap>
    {
        public IASFolderMapMap()
        {
            // Primary Key
            this.HasKey(t => t.IASFolderMapID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.AccountName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.FolderName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.FolderVersionNumber)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FormName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Owner)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IASFolderMap", "GU");
            this.Property(t => t.IASFolderMapID).HasColumnName("IASFolderMapID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.AccountName).HasColumnName("AccountName");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderName).HasColumnName("FolderName");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FormName).HasColumnName("FormName");
            this.Property(t => t.Owner).HasColumnName("FolderOwner");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.IASFolderMaps)
                .HasForeignKey(d => d.GlobalUpdateID);

            this.HasRequired(t => t.Folder)
                .WithMany(t => t.IASFolderMaps)
                .HasForeignKey(d => d.FolderID);

            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.IASFolderMaps)
                .HasForeignKey(d => d.FolderVersionID);

            this.HasRequired(t => t.FormInstance)
                .WithMany(t => t.IASFolderMaps)
                .HasForeignKey(d => d.FormInstanceID);

        }
    }
}
