using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ACCMREPMap : EntityTypeConfiguration<ACCMREP>
    {
        public ACCMREPMap()
        {
            // Primary Key
            //this.HasKey(t => new { t.PDPD_ID, t.ACCM_TYPE, t.ACCM_EFF_DT, t.ACCM_SEQ_NO, t.ACCM_TERM_DT, t.ACAC_ACC_NO, t.ACCM_DESC, t.ACCM_PFX, t.ACCM_LOCK_TOKEN, t.ATXR_SOURCE_ID, t.BatchID });
            this.HasKey(t => new { t.ACCMId });
            // Properties
            this.Property(t => t.PDPD_ID)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.ACCM_TYPE)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.ACCM_SEQ_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ACCM_DESC)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.ACCM_PFX)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.ACCM_LOCK_TOKEN)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BatchID)
               .IsRequired()
               .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCMREP", "Rep");
            this.Property(t => t.PDPD_ID).HasColumnName("ACCMId");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.ACCM_TYPE).HasColumnName("ACCM_TYPE");
            this.Property(t => t.ACCM_EFF_DT).HasColumnName("ACCM_EFF_DT");
            this.Property(t => t.ACCM_SEQ_NO).HasColumnName("ACCM_SEQ_NO");
            this.Property(t => t.ACCM_TERM_DT).HasColumnName("ACCM_TERM_DT");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACCM_DESC).HasColumnName("ACCM_DESC");
            this.Property(t => t.ACCM_PFX).HasColumnName("ACCM_PFX");
            this.Property(t => t.ACCM_LOCK_TOKEN).HasColumnName("ACCM_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
