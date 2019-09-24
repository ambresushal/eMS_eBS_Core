using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SEDFREPMap : EntityTypeConfiguration<SEDFREP>
    {
        public SEDFREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX, t.SEDF_EFF_DT, t.SESE_ID });

            // Properties
            this.Property(t => t.PDBC_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEPC_PRICE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEDF_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_UM_REQ_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_UM_BP_REF_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_UM_MAT_IND)
                .IsRequired();

            this.Property(t => t.SEDF_RWH_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_CAP_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_OPTS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SEDFREP", "Rep");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.SEDF_EFF_DT).HasColumnName("SEDF_EFF_DT");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SEDF_TERM_DT).HasColumnName("SEDF_TERM_DT");
            this.Property(t => t.SEPC_PRICE_ID).HasColumnName("SEPC_PRICE_ID");
            this.Property(t => t.SEDF_TYPE).HasColumnName("SEDF_TYPE");
            this.Property(t => t.SEDF_UM_REQ_IND).HasColumnName("SEDF_UM_REQ_IND");
            this.Property(t => t.SEDF_UM_BP_REF_IND).HasColumnName("SEDF_UM_BP_REF_IND");
            this.Property(t => t.SEDF_UM_MAT_IND).HasColumnName("SEDF_UM_MAT_IND");
            this.Property(t => t.SEDF_RWH_IND).HasColumnName("SEDF_RWH_IND");
            this.Property(t => t.SEDF_CAP_IND).HasColumnName("SEDF_CAP_IND");
            this.Property(t => t.SEDF_OPTS).HasColumnName("SEDF_OPTS");
            this.Property(t => t.SEDF_UM_AUTH_WAIVE).HasColumnName("SEDF_UM_AUTH_WAIVE");
            this.Property(t => t.SEDF_UM_AUTH_AMT).HasColumnName("SEDF_UM_AUTH_AMT");
            this.Property(t => t.SEDF_UM_UNITS_WAIVE).HasColumnName("SEDF_UM_UNITS_WAIVE");
            this.Property(t => t.SEDF_AUTH_WV_DAYS).HasColumnName("SEDF_AUTH_WV_DAYS");
            this.Property(t => t.SEDF_LOCK_TOKEN).HasColumnName("SEDF_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
