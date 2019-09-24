using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESE481ArcMap : EntityTypeConfiguration<SESE481Arc>
    {
        public SESE481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SESE_Id);

            // Properties
            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_DESC)
                .IsFixedLength()
                .HasMaxLength(70);

            this.Property(t => t.SESE_CM_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_PA_AMT_REQ)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_PA_UNIT_REQ)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_PA_PROC_REQ)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_VALID_SEX)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_SEX_EXCD_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_AGE_EXCD_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_COV_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_COV_EXCD_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_RULE_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_CALC_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_REL_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_OPTS)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.SESE_ID_XLOW)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_DESC_XLOW)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.SESE_DIS_EXCD_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_FSA_REIMB_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_HSA_REIMB_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESE_HRA_DED_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SESE481", "arc_481");
            this.Property(t => t.SESE_Id).HasColumnName("SESE_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SESE_DESC).HasColumnName("SESE_DESC");
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
            this.Property(t => t.SESE_RULE_TYPE).HasColumnName("SESE_RULE_TYPE");
            this.Property(t => t.SESE_CALC_IND).HasColumnName("SESE_CALC_IND");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_OPTS).HasColumnName("SESE_OPTS");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.SESE_ID_XLOW).HasColumnName("SESE_ID_XLOW");
            this.Property(t => t.SESE_DESC_XLOW).HasColumnName("SESE_DESC_XLOW");
            this.Property(t => t.SESE_DIS_EXCD_ID).HasColumnName("SESE_DIS_EXCD_ID");
            this.Property(t => t.SESE_MAX_CPAY_PCT).HasColumnName("SESE_MAX_CPAY_PCT");
            this.Property(t => t.SESE_FSA_REIMB_IND).HasColumnName("SESE_FSA_REIMB_IND");
            this.Property(t => t.SESE_HSA_REIMB_IND).HasColumnName("SESE_HSA_REIMB_IND");
            this.Property(t => t.SESE_HRA_DED_IND).HasColumnName("SESE_HRA_DED_IND");
            this.Property(t => t.SESE_LOCK_TOKEN).HasColumnName("SESE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
