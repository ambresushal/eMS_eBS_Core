using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDDS481ArcMap : EntityTypeConfiguration<PDDS481Arc>
    {
        public PDDS481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.PDDS_Id);

            // Properties
            this.Property(t => t.PDPD_ID)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PDDS_DESC)
                .HasMaxLength(70);

            this.Property(t => t.PDDS_UM_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_MED_PRICE_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_MED_CLMS_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_DEN_UM_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_DEN_PD_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_DEN_PRICE_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_DEN_CLMS_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_PREM_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_CLED_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_CAP_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_INT_STATE_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_MCTR_BCAT)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDDS_MCTR_VAL1)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDDS_MCTR_VAL2)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDDS_APP_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_PROD_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_DOFR_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDDS_OPTOUT_IND)
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
            this.ToTable("PDDS481", "arc_481");
            this.Property(t => t.PDDS_Id).HasColumnName("PDDS_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDDS_DESC).HasColumnName("PDDS_DESC");
            this.Property(t => t.PDDS_UM_IND).HasColumnName("PDDS_UM_IND");
            this.Property(t => t.PDDS_MED_PRICE_IND).HasColumnName("PDDS_MED_PRICE_IND");
            this.Property(t => t.PDDS_MED_CLMS_IND).HasColumnName("PDDS_MED_CLMS_IND");
            this.Property(t => t.PDDS_DEN_UM_IND).HasColumnName("PDDS_DEN_UM_IND");
            this.Property(t => t.PDDS_DEN_PD_IND).HasColumnName("PDDS_DEN_PD_IND");
            this.Property(t => t.PDDS_DEN_PRICE_IND).HasColumnName("PDDS_DEN_PRICE_IND");
            this.Property(t => t.PDDS_DEN_CLMS_IND).HasColumnName("PDDS_DEN_CLMS_IND");
            this.Property(t => t.PDDS_PREM_IND).HasColumnName("PDDS_PREM_IND");
            this.Property(t => t.PDDS_CLED_IND).HasColumnName("PDDS_CLED_IND");
            this.Property(t => t.PDDS_CAP_IND).HasColumnName("PDDS_CAP_IND");
            this.Property(t => t.PDDS_INT_STATE_IND).HasColumnName("PDDS_INT_STATE_IND");
            this.Property(t => t.PDDS_MCTR_BCAT).HasColumnName("PDDS_MCTR_BCAT");
            this.Property(t => t.PDDS_MCTR_VAL1).HasColumnName("PDDS_MCTR_VAL1");
            this.Property(t => t.PDDS_MCTR_VAL2).HasColumnName("PDDS_MCTR_VAL2");
            this.Property(t => t.PDDS_APP_TYPE).HasColumnName("PDDS_APP_TYPE");
            this.Property(t => t.PDDS_PROD_TYPE).HasColumnName("PDDS_PROD_TYPE");
            this.Property(t => t.PDDS_DOFR_IND).HasColumnName("PDDS_DOFR_IND");
            this.Property(t => t.PDDS_OPTOUT_IND).HasColumnName("PDDS_OPTOUT_IND");
            this.Property(t => t.PDDS_LOCK_TOKEN).HasColumnName("PDDS_LOCK_TOKEN");
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
