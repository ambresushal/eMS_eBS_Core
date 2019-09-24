using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class FolderVersionMap : EntityTypeConfiguration<FolderVersion>
    {
        public FolderVersionMap()
        {
            // Primary Key
            this.HasKey(t => t.FolderVersionID);

            // Properties
            this.Property(t => t.FolderVersionNumber)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Comments)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("FolderVersion", "Fldr");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.WFStateID).HasColumnName("WFStateID");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.VersionTypeID).HasColumnName("VersionTypeID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.FolderVersionStateID).HasColumnName("FolderVersionStateID");
            this.Property(t => t.FolderVersionBatchID).HasColumnName("FolderVersionBatchID");

            // Relationships
            this.HasRequired(t => t.Folder)
                .WithMany(t => t.FolderVersions)
                .HasForeignKey(d => d.FolderID);
            //this.HasOptional(t => t.FolderVersionBatch)
               // .WithMany(t => t.FolderVersions)
                //.HasForeignKey(d => d.FolderVersionBatchID);
            //this.HasRequired(t => t.FolderVersion2)
            //    .WithOptional(t => t.FolderVersion1);
            //this.HasRequired(t => t.Tenant)
              //  .WithMany(t => t.FolderVersions)
                //.HasForeignKey(d => d.TenantID);
            //this.HasRequired(t => t.VersionType)
              //  .WithMany(t => t.FolderVersions)
               // .HasForeignKey(d => d.VersionTypeID);
            //this.HasRequired(t => t.WorkFlowState)
              //  .WithMany(t => t.FolderVersions)
               // .HasForeignKey(d => d.WFStateID);

        }
    }
}
