using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTLTJsonDataMap : EntityTypeConfiguration<LTLTJsonData>
    {
        public LTLTJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.ACAC_ACC_NO });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.LTLT_CAT)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_LEVEL)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_PERIOD_IND)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_RULE)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_IX_IND)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_IX_TYPE)
                .HasMaxLength(255);

            this.Property(t => t.EXCD_ID)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_OPTS)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_SAL_IND)
                .HasMaxLength(255);

            this.Property(t => t.LTLT_EXCL_DED_IND_NVL)
                .HasMaxLength(255);

            this.Property(t => t.IDCD_ID_REL)
                .HasMaxLength(255);

            this.Property(t => t.IDCD_TYPE)
                .HasMaxLength(255);

            this.Property(t => t.LTIP_IPCD_ID_LOW)
                .HasMaxLength(255);

            this.Property(t => t.LTIP_IPCD_ID_HIGH)
                .HasMaxLength(255);

            this.Property(t => t.PRPR_MCTR_TYPE)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("LTLTJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.LTLT_CAT).HasColumnName("LTLT_CAT");
            this.Property(t => t.LTLT_LEVEL).HasColumnName("LTLT_LEVEL");
            this.Property(t => t.LTLT_PERIOD_IND).HasColumnName("LTLT_PERIOD_IND");
            this.Property(t => t.LTLT_RULE).HasColumnName("LTLT_RULE");
            this.Property(t => t.LTLT_IX_IND).HasColumnName("LTLT_IX_IND");
            this.Property(t => t.LTLT_IX_TYPE).HasColumnName("LTLT_IX_TYPE");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");
            this.Property(t => t.LTLT_AMT1).HasColumnName("LTLT_AMT1");
            this.Property(t => t.LTLT_AMT2).HasColumnName("LTLT_AMT2");
            this.Property(t => t.LTLT_OPTS).HasColumnName("LTLT_OPTS");
            this.Property(t => t.LTLT_SAL_IND).HasColumnName("LTLT_SAL_IND");
            this.Property(t => t.LTLT_DAYS).HasColumnName("LTLT_DAYS");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.LTLT_EXCL_DED_IND_NVL).HasColumnName("LTLT_EXCL_DED_IND_NVL");
            this.Property(t => t.IDCD_ID_REL).HasColumnName("IDCD_ID_REL");
            this.Property(t => t.IDCD_TYPE).HasColumnName("IDCD_TYPE");
            this.Property(t => t.LTIP_IPCD_ID_LOW).HasColumnName("LTIP_IPCD_ID_LOW");
            this.Property(t => t.PRPR_MCTR_TYPE).HasColumnName("PRPR_MCTR_TYPE");
        }
    }
}
