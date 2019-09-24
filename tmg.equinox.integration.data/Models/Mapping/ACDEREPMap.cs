using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ACDEREPMap : EntityTypeConfiguration<ACDEREP>
    {
        public ACDEREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ACDE_ACC_TYPE, t.ACAC_ACC_NO, t.ACDE_DESC, t.ACDE_LOCK_TOKEN, t.ATXR_SOURCE_ID, t.BatchID });

            // Properties
            this.Property(t => t.ACDE_ACC_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ACDE_DESC)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(70);

            this.Property(t => t.ACDE_LOCK_TOKEN)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACDEREP", "Rep");
            this.Property(t => t.ACDE_ACC_TYPE).HasColumnName("ACDE_ACC_TYPE");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.ACDE_LOCK_TOKEN).HasColumnName("ACDE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
