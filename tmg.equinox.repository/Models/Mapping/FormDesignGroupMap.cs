using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignGroupMap : EntityTypeConfiguration<FormDesignGroup>
    {
        public FormDesignGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignGroupID);

            // Properties
            this.Property(t => t.GroupName)
                .HasMaxLength(100);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FormDesignGroup", "UI");
            this.Property(t => t.FormDesignGroupID).HasColumnName("FormDesignGroupID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsMasterList).HasColumnName("IsMasterList");
        }
    }
}
