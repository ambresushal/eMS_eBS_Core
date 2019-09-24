using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignHistoryMap : EntityTypeConfiguration<FormDesignHistory>
    {
        public FormDesignHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignHistoryId);

            // Properties
            this.Property(t => t.FormDesignVersionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EnteredBy)
                .HasMaxLength(20);

            this.Property(t => t.TenantID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Action)
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("FormDesignHistory", "Arc");
            this.Property(t => t.FormDesignVersionId).HasColumnName("FormDesignVersionId");
            this.Property(t => t.FormDesignVersionData).HasColumnName("FormDesignVersionData");
            this.Property(t => t.EnteredBy).HasColumnName("EnteredBy");
            this.Property(t => t.EnteredDate).HasColumnName("EnteredDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.FormDesignHistoryId).HasColumnName("FormDesignHistoryId");
        }
    }
}
