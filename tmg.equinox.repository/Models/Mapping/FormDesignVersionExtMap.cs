using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignVersionExtMap : EntityTypeConfiguration<FormDesignVersionExt>
    {
        public FormDesignVersionExtMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignVersionExtID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Comments)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("FormDesignVersionExt", "UI");
            this.Property(t => t.FormDesignVersionExtID).HasColumnName("FormDesignVersionExtID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.ExtendedColNames).HasColumnName("ExtendedColNames");
            this.Property(t => t.ExcelConfiguration).HasColumnName("ExcelConfiguration");
        }
    }
}
