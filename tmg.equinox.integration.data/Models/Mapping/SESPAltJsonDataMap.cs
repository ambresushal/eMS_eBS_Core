using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESPAltJsonDataMap : EntityTypeConfiguration<SESPAltJsonData>
    {
        public SESPAltJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.SESE_ID });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESP_PEN_IND)
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.SESP_PEN_CALC_IND)
                .HasMaxLength(2);

            this.Property(t => t.EXCD_ID)
               .HasMaxLength(6);

            this.Property(t => t.SERLMessage)
                .IsFixedLength()
               .HasMaxLength(250);

            this.Property(t => t.DisallowedMessage)
                .IsFixedLength()
               .HasMaxLength(250);

            this.Property(t => t.IsCovered)
                           .IsFixedLength()
                          .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("SESPAltJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SESP_PEN_IND).HasColumnName("SESP_PEN_IND");
            this.Property(t => t.SESP_PEN_CALC_IND).HasColumnName("SESP_PEN_CALC_IND");
            this.Property(t => t.SESP_PEN_AMT).HasColumnName("SESP_PEN_AMT");
            this.Property(t => t.SESP_PEN_PCT).HasColumnName("SESP_PEN_PCT");
            this.Property(t => t.SESP_PEN_MAX_AMT).HasColumnName("SESP_PEN_MAX_AMT");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");
            this.Property(t => t.SERLMessage).HasColumnName("SERLMessage");
            this.Property(t => t.DisallowedMessage).HasColumnName("DisallowedMessage");
            this.Property(t => t.IsCovered).HasColumnName("IsCovered");
        }
    }
}
