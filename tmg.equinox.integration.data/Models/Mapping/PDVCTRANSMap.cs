using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDVCTRANSMap : EntityTypeConfiguration<PDVCTRANS>
    {
        public PDVCTRANSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDVCId });

            // Properties
            this.Property(t => t.PDPD_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PDVC_TIER)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PDVC_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_SEQ_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PDVC_PR_PCP)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_IN)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_PAR)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_NONPAR)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PC_NR)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PC_OBT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PC_VIOL)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_REF_NR)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_REF_OBT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_REF_VIOL)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_LOBD_PTR)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEPY_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.DEDE_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.LTLT_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.DPPY_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.CGPY_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("PDVCTRANS", "Trans");
            this.Property(t => t.PDVCId).HasColumnName("PDVCId");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDVC_TIER).HasColumnName("PDVC_TIER");
            this.Property(t => t.PDVC_TYPE).HasColumnName("PDVC_TYPE");
            this.Property(t => t.PDVC_EFF_DT).HasColumnName("PDVC_EFF_DT");
            this.Property(t => t.PDVC_SEQ_NO).HasColumnName("PDVC_SEQ_NO");
            this.Property(t => t.PDVC_TERM_DT).HasColumnName("PDVC_TERM_DT");
            this.Property(t => t.PDVC_PR_PCP).HasColumnName("PDVC_PR_PCP");
            this.Property(t => t.PDVC_PR_IN).HasColumnName("PDVC_PR_IN");
            this.Property(t => t.PDVC_PR_PAR).HasColumnName("PDVC_PR_PAR");
            this.Property(t => t.PDVC_PR_NONPAR).HasColumnName("PDVC_PR_NONPAR");
            this.Property(t => t.PDVC_PC_NR).HasColumnName("PDVC_PC_NR");
            this.Property(t => t.PDVC_PC_OBT).HasColumnName("PDVC_PC_OBT");
            this.Property(t => t.PDVC_PC_VIOL).HasColumnName("PDVC_PC_VIOL");
            this.Property(t => t.PDVC_REF_NR).HasColumnName("PDVC_REF_NR");
            this.Property(t => t.PDVC_REF_OBT).HasColumnName("PDVC_REF_OBT");
            this.Property(t => t.PDVC_REF_VIOL).HasColumnName("PDVC_REF_VIOL");
            this.Property(t => t.PDVC_LOBD_PTR).HasColumnName("PDVC_LOBD_PTR");
            this.Property(t => t.SEPY_PFX).HasColumnName("SEPY_PFX");
            this.Property(t => t.DEDE_PFX).HasColumnName("DEDE_PFX");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.DPPY_PFX).HasColumnName("DPPY_PFX");
            this.Property(t => t.CGPY_PFX).HasColumnName("CGPY_PFX");
            this.Property(t => t.PDVC_LOCK_TOKEN).HasColumnName("PDVC_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
