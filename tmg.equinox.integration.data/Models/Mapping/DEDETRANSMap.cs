using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class DEDETRANSMap : EntityTypeConfiguration<DEDETRANS>
    {
        public DEDETRANSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.DEDE_PFX });

            // Properties
            this.Property(t => t.DEDE_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_DESC)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(70);

            this.Property(t => t.DEDE_RULE)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_REL_ACC_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_COB_OOP_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DEDE_SL_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DEDE_PERIOD_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DEDE_AGG_PERSON)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_AGG_PERSON_CO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_FAM_AMT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_FAM_AMT_CO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_MEME_AMT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_MEME_AMT_CO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DEDE_OPTS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.DEDE_CO_BYPASS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DEDE_MEM_SAL_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DEDE_FAM_SAL_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.DEDE_LOCK_TOKEN)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //this.Property(t => t.Hashcode)
            //    .IsRequired()
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DEDETRANS", "Trans");
            //this.Property(t => t.DEDEId).HasColumnName("DEDEId");
            this.Property(t => t.DEDE_PFX).HasColumnName("DEDE_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.DEDE_DESC).HasColumnName("DEDE_DESC");
            this.Property(t => t.DEDE_RULE).HasColumnName("DEDE_RULE");
            this.Property(t => t.DEDE_REL_ACC_ID).HasColumnName("DEDE_REL_ACC_ID");
            this.Property(t => t.DEDE_COB_OOP_IND).HasColumnName("DEDE_COB_OOP_IND");
            this.Property(t => t.DEDE_SL_IND).HasColumnName("DEDE_SL_IND");
            this.Property(t => t.DEDE_PERIOD_IND).HasColumnName("DEDE_PERIOD_IND");
            this.Property(t => t.DEDE_AGG_PERSON).HasColumnName("DEDE_AGG_PERSON");
            this.Property(t => t.DEDE_AGG_PERSON_CO).HasColumnName("DEDE_AGG_PERSON_CO");
            this.Property(t => t.DEDE_FAM_AMT).HasColumnName("DEDE_FAM_AMT");
            this.Property(t => t.DEDE_FAM_AMT_CO).HasColumnName("DEDE_FAM_AMT_CO");
            this.Property(t => t.DEDE_MEME_AMT).HasColumnName("DEDE_MEME_AMT");
            this.Property(t => t.DEDE_MEME_AMT_CO).HasColumnName("DEDE_MEME_AMT_CO");
            this.Property(t => t.DEDE_OPTS).HasColumnName("DEDE_OPTS");
            this.Property(t => t.DEDE_CO_BYPASS).HasColumnName("DEDE_CO_BYPASS");
            this.Property(t => t.DEDE_MEM_SAL_IND).HasColumnName("DEDE_MEM_SAL_IND");
            this.Property(t => t.DEDE_FAM_SAL_IND).HasColumnName("DEDE_FAM_SAL_IND");
            this.Property(t => t.DEDE_LOCK_TOKEN).HasColumnName("DEDE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            //this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
