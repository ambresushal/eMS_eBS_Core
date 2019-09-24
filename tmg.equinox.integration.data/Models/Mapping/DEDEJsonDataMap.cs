using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class DEDEJsonDataMap : EntityTypeConfiguration<DEDEJsonData>
    {
        public DEDEJsonDataMap()
        {
            // Primary Key
            //this.HasKey(t => t.ProductID);
            this.HasKey(t => new {t.ProductID, t.BenefitSet, t.ACAC_ACC_NO});
            //this.Property(t => t.ProductID)
            //    .HasMaxLength(8);

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.DEDE_DESC)
                .HasMaxLength(255);

            this.Property(t => t.DEDE_SL_IND)
                .HasMaxLength(255);

            this.Property(t => t.DEDE_PERIOD_IND)
                .HasMaxLength(255);

            this.Property(t => t.DEDE_OPTS)
                .HasMaxLength(255);
            
            this.Property(t => t.DEDE_CO_BYPASS)
                .HasMaxLength(255);
            
            this.Property(t => t.DEDE_MEM_SAL_IND)
                .HasMaxLength(255);

            this.Property(t => t.DEDE_FAM_SAL_IND)
                .HasMaxLength(255);

            

            // Table & Column Mappings
            this.ToTable("DEDEJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
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
            this.Property(t => t.DEDE_CO_BYPASS).HasColumnName("DEDE_CO_BYPASS");
            this.Property(t => t.DEDE_MEM_SAL_IND).HasColumnName("DEDE_MEM_SAL_IND");
            this.Property(t => t.DEDE_FAM_SAL_IND).HasColumnName("DEDE_FAM_SAL_IND");
        }
    }
}
