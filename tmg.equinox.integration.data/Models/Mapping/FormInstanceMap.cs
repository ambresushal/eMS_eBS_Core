using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class FormInstanceMap : EntityTypeConfiguration<FormInstance>
    {
        public FormInstanceMap()
        {
            // Primary Key
            this.HasKey(t => t.FormInstanceID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Name)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("FormInstance", "Fldr");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.FormInstances)
                .HasForeignKey(d => d.FolderVersionID);
            //this.HasRequired(t => t.FormDesign)
                //.WithMany(t => t.FormInstances)
                //.HasForeignKey(d => d.FormDesignID);
            //this.HasRequired(t => t.FormDesignVersion)
                //.WithMany(t => t.FormInstances)
                //.HasForeignKey(d => d.FormDesignVersionID);
            //this.HasRequired(t => t.Tenant)
                //.WithMany(t => t.FormInstances)
                //.HasForeignKey(d => d.TenantID);

        }
    }
}
