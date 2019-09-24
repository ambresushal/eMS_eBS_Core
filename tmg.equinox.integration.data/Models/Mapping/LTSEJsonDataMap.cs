using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTSEJsonDataMap : EntityTypeConfiguration<LTSEJsonData>
    {
        public LTSEJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.ACAC_ACC_NO, t.SESE_ID, t.LTSE_WT_CTR });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.SESE_ID)
                .HasMaxLength(510);
          
            // Table & Column Mappings
            this.ToTable("LTSEJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.LTSE_WT_CTR).HasColumnName("LTSE_WT_CTR");
        }
    }
}
