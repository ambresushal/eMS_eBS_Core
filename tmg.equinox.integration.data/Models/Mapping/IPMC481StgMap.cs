using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class IPMC481StgMap : EntityTypeConfiguration<IPMC481Stg>
    {
        public IPMC481StgMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX, t.IPCD_LOW, t.IPMC_EFF_DT, t.IPMC_TERM_DT, t.IPCD_HIGH, t.IPMC_UM_IND, t.IPMC_UM_AUTH_AMT, t.IPMC_UM_UNITS_WAIVE, t.IPMC_AUTH_WV_DAYS, t.IPMC_LOCK_TOKEN, t.ATXR_SOURCE_ID, t.BatchID });

            // Properties
            this.Property(t => t.PDBC_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.IPCD_LOW)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.IPCD_HIGH)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.IPMC_UM_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.IPMC_UM_AUTH_AMT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IPMC_UM_UNITS_WAIVE)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IPMC_AUTH_WV_DAYS)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IPMC_LOCK_TOKEN)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("IPMC481", "stg_481");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.IPCD_LOW).HasColumnName("IPCD_LOW");
            this.Property(t => t.IPMC_EFF_DT).HasColumnName("IPMC_EFF_DT");
            this.Property(t => t.IPMC_TERM_DT).HasColumnName("IPMC_TERM_DT");
            this.Property(t => t.IPCD_HIGH).HasColumnName("IPCD_HIGH");
            this.Property(t => t.IPMC_UM_IND).HasColumnName("IPMC_UM_IND");
            this.Property(t => t.IPMC_UM_AUTH_AMT).HasColumnName("IPMC_UM_AUTH_AMT");
            this.Property(t => t.IPMC_UM_UNITS_WAIVE).HasColumnName("IPMC_UM_UNITS_WAIVE");
            this.Property(t => t.IPMC_AUTH_WV_DAYS).HasColumnName("IPMC_AUTH_WV_DAYS");
            this.Property(t => t.IPMC_LOCK_TOKEN).HasColumnName("IPMC_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
