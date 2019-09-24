using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTPRJsonDataMap : EntityTypeConfiguration<LTPRJsonData>
    {
        public LTPRJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.ACAC_ACC_NO });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.PRPR_MCTR_TYPE)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("LTPRJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.PRPR_MCTR_TYPE).HasColumnName("PRPR_MCTR_TYPE");
        }
    }
}
