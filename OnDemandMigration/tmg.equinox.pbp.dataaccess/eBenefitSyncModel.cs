namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class eBenefitSyncModel : DbContext
    {
        public eBenefitSyncModel()
            : base("name=eBenefitSyncModel")
        {
        }

        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<FolderVersion> FolderVersions { get; set; }
        public virtual DbSet<FormInstance> FormInstances { get; set; }
        public virtual DbSet<FormInstanceDataMap> FormInstanceDataMaps { get; set; }
        public virtual DbSet<FormDesign> FormDesigns { get; set; }
        public virtual DbSet<FormDesignVersion> FormDesignVersions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folder>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Folder>()
                .Property(e => e.PrimaryContent)
                .IsUnicode(false);

            modelBuilder.Entity<Folder>()
                .Property(e => e.AddedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Folder>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Folder>()
                .HasMany(e => e.Folder1)
                .WithOptional(e => e.Folder2)
                .HasForeignKey(e => e.ParentFolderId);

            modelBuilder.Entity<Folder>()
                .HasMany(e => e.FolderVersions)
                .WithRequired(e => e.Folder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FolderVersion>()
                .Property(e => e.FolderVersionNumber)
                .IsUnicode(false);

            modelBuilder.Entity<FolderVersion>()
                .Property(e => e.AddedBy)
                .IsUnicode(false);

            modelBuilder.Entity<FolderVersion>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<FolderVersion>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<FolderVersion>()
                .Property(e => e.CatID)
                .IsUnicode(false);

            modelBuilder.Entity<FolderVersion>()
                .HasOptional(e => e.FolderVersion1)
                .WithRequired(e => e.FolderVersion2);

            modelBuilder.Entity<FolderVersion>()
                .HasMany(e => e.FormInstances)
                .WithRequired(e => e.FolderVersion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormInstance>()
                .Property(e => e.AddedBy)
                .IsUnicode(false);

            modelBuilder.Entity<FormInstance>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<FormInstance>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FormInstance>()
                .Property(e => e.ProductJsonHash)
                .IsUnicode(false);

            modelBuilder.Entity<FormInstance>()
                .HasMany(e => e.FormInstanceDataMaps)
                .WithRequired(e => e.FormInstance)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormDesign>()
                .Property(e => e.FormName)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesign>()
                .Property(e => e.DisplayText)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesign>()
                .Property(e => e.AddedBy)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesign>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesign>()
                .HasMany(e => e.FormInstances)
                .WithRequired(e => e.FormDesign)
                .HasForeignKey(e => e.FormDesignID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormDesign>()
                .HasMany(e => e.FormDesignVersions)
                .WithOptional(e => e.FormDesign)
                .HasForeignKey(e => e.FormDesignID);

            modelBuilder.Entity<FormDesignVersion>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesignVersion>()
                .Property(e => e.RuleExecutionTreeJSON)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesignVersion>()
                .Property(e => e.RuleEventMapJSON)
                .IsUnicode(false);

            modelBuilder.Entity<FormDesignVersion>()
                .HasMany(e => e.FormInstances)
                .WithRequired(e => e.FormDesignVersion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormDesignVersion>()
                .HasOptional(e => e.FormDesignVersion1)
                .WithRequired(e => e.FormDesignVersion2);
        }
    }
}
