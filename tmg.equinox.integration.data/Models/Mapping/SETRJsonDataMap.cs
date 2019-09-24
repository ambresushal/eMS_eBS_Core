using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SETRJsonDataMap : EntityTypeConfiguration<SETRJsonData>
    {
        public SETRJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.SESE_ID, t.SETR_TIER_NO });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SETR_OPTS)
                .HasMaxLength(4);
           
            // Table & Column Mappings
            this.ToTable("SETRJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SETR_TIER_NO).HasColumnName("SETR_TIER_NO");
            this.Property(t => t.SETR_ALLOW_AMT).HasColumnName("SETR_ALLOW_AMT");
            this.Property(t => t.SETR_ALLOW_CTR).HasColumnName("SETR_ALLOW_CTR");
            this.Property(t => t.SETR_COPAY_AMT).HasColumnName("SETR_COPAY_AMT");
            this.Property(t => t.SETR_COIN_PCT).HasColumnName("SETR_COIN_PCT");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.SETR_OPTS).HasColumnName("SETR_OPTS");
        }
    }
}
