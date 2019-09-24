using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignTypeMap : EntityTypeConfiguration<FormDesignType>
    {
        public FormDesignTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignTypeID);

            // Properties
            this.Property(t => t.DisplayText)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FormDesignType", "UI");
            this.Property(t => t.FormDesignTypeID).HasColumnName("FormDesignTypeID");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
        }
    }
}
