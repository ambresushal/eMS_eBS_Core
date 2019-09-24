using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ServiceJsonDataMap : EntityTypeConfiguration<ServiceJsonData>
    {
        public ServiceJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.SESE_ID });

            // Properties
            this.Property(t => t.BenefitSet)
                .HasMaxLength(50);

            this.Property(t => t.ServiceGroup)
                .HasMaxLength(50);

            this.Property(t => t.SESE_ID)
                .HasMaxLength(4);

            this.Property(t => t.SESE_CM_IND)
               .HasMaxLength(1);

            this.Property(t => t.SESE_PA_AMT_REQ)
               .HasMaxLength(1);

            this.Property(t => t.SESE_PA_UNIT_REQ)
               .HasMaxLength(1);

            this.Property(t => t.SESE_PA_PROC_REQ)
               .HasMaxLength(1);

            this.Property(t => t.SESE_VALID_SEX)
               .HasMaxLength(1);

            this.Property(t => t.SESE_SEX_EXCD_ID)
               .HasMaxLength(3);

            this.Property(t => t.SESE_AGE_EXCD_ID)
               .HasMaxLength(3);

            this.Property(t => t.SESE_COV_EXCD_ID)
               .HasMaxLength(3);

            this.Property(t => t.SESE_CALC_IND)
               .HasMaxLength(1);

            this.Property(t => t.SESE_OPTS)
               .HasMaxLength(8);

            this.Property(t => t.SESE_DIS_EXCD_ID)
               .HasMaxLength(3);

            this.Property(t => t.SESE_FSA_REIMB_IND)
               .HasMaxLength(1);

            this.Property(t => t.SESE_HSA_REIMB_IND)
               .HasMaxLength(1);

            this.Property(t => t.SESE_HRA_DED_IND)
               .HasMaxLength(1);

            this.Property(t => t.SESE_MAX_CPAY_ACT_NVL)
               .HasMaxLength(48);

            this.Property(t => t.SESE_CPAY_EXCD_ID_NVL)
               .HasMaxLength(48);

            // Table & Column Mappings
            this.ToTable("ServiceJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ServiceGroup).HasColumnName("ServiceGroup");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_CM_IND).HasColumnName("SESE_CM_IND");
            this.Property(t => t.SESE_PA_AMT_REQ).HasColumnName("SESE_PA_AMT_REQ");
            this.Property(t => t.SESE_PA_UNIT_REQ).HasColumnName("SESE_PA_UNIT_REQ");
            this.Property(t => t.SESE_PA_PROC_REQ).HasColumnName("SESE_PA_PROC_REQ");
            this.Property(t => t.SESE_VALID_SEX).HasColumnName("SESE_VALID_SEX");
            this.Property(t => t.SESE_SEX_EXCD_ID).HasColumnName("SESE_SEX_EXCD_ID");
            this.Property(t => t.SESE_MIN_AGE).HasColumnName("SESE_MIN_AGE");
            this.Property(t => t.SESE_MAX_AGE).HasColumnName("SESE_MAX_AGE");
            this.Property(t => t.SESE_AGE_EXCD_ID).HasColumnName("SESE_AGE_EXCD_ID");
            this.Property(t => t.SESE_COV_TYPE).HasColumnName("SESE_COV_TYPE");
            this.Property(t => t.SESE_COV_EXCD_ID).HasColumnName("SESE_COV_EXCD_ID");
            this.Property(t => t.SESE_CALC_IND).HasColumnName("SESE_CALC_IND");
            this.Property(t => t.SESE_OPTS).HasColumnName("SESE_OPTS");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.SESE_DIS_EXCD_ID).HasColumnName("SESE_DIS_EXCD_ID");
            this.Property(t => t.SESE_MAX_CPAY_PCT).HasColumnName("SESE_MAX_CPAY_PCT");
            this.Property(t => t.SESE_FSA_REIMB_IND).HasColumnName("SESE_FSA_REIMB_IND");
            this.Property(t => t.SESE_HSA_REIMB_IND).HasColumnName("SESE_HSA_REIMB_IND");
            this.Property(t => t.SESE_HRA_DED_IND).HasColumnName("SESE_HRA_DED_IND");
            this.Property(t => t.SESE_MAX_CPAY_ACT_NVL).HasColumnName("SESE_MAX_CPAY_ACT_NVL");
            this.Property(t => t.SESE_CPAY_EXCD_ID_NVL).HasColumnName("SESE_CPAY_EXCD_ID_NVL");
        }
    }
}
