using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class FolderMap : EntityTypeConfiguration<Folder>
    {
        public FolderMap()
        {
            // Primary Key
            this.HasKey(t => t.FolderID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.PrimaryContent)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Folder", "Fldr");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.IsPortfolio).HasColumnName("IsPortfolio");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.MarketSegmentID).HasColumnName("MarketSegmentID");
            this.Property(t => t.PrimaryContent).HasColumnName("PrimaryContent");
            this.Property(t => t.PrimaryContentID).HasColumnName("PrimaryContentID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.ParentFolderId).HasColumnName("ParentFolderId");
            this.Property(t => t.FormDesignGroupId).HasColumnName("FormDesignGroupId");

            // Relationships
            //this.HasOptional(t => t.FormDesignGroup)
                //.WithMany(t => t.Folders)
                //.HasForeignKey(d => d.FormDesignGroupId);
            this.HasOptional(t => t.Folder2)
                .WithMany(t => t.Folder1)
                .HasForeignKey(d => d.ParentFolderId);
            //this.HasRequired(t => t.MarketSegment)
               // .WithMany(t => t.Folders)
                //.HasForeignKey(d => d.MarketSegmentID);
            //this.HasRequired(t => t.Tenant)
              //  .WithMany(t => t.Folders)
               // .HasForeignKey(d => d.TenantID);
            //this.HasOptional(t => t.User)
              //  .WithMany(t => t.Folders)
               // .HasForeignKey(d => d.PrimaryContentID);

        }
    }
}
