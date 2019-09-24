using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTIDJsonDataMap : EntityTypeConfiguration<LTIDJsonData>
    {
        public LTIDJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.ACAC_ACC_NO });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.IDCD_ID_REL)
                .HasMaxLength(255);

            this.Property(t => t.IDCD_TYPE)
                .HasMaxLength(510);

            // Table & Column Mappings
            this.ToTable("LTIDJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.IDCD_ID_REL).HasColumnName("IDCD_ID_REL");
            this.Property(t => t.IDCD_TYPE).HasColumnName("IDCD_TYPE");
        }
    }
}
