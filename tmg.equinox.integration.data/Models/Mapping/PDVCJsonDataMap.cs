using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDVCJsonDataMap : EntityTypeConfiguration<PDVCJsonData>
    {
        public PDVCJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.BenefitSet, t.PDVC_TIER, t.PDVC_TYPE, t.PDVC_EFF_DT, t.PDVC_SEQ_NO });

            // Properties
            this.Property(t => t.BenefitSet)
                .IsFixedLength()
                .HasMaxLength(50);

            this.Property(t => t.PDVC_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_PCP)
                .IsFixedLength()
               .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_IN)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_PAR)
                .IsFixedLength()
               .HasMaxLength(1);

            this.Property(t => t.PDVC_PR_NONPAR)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PC_NR)
                .IsFixedLength()
               .HasMaxLength(1);

            this.Property(t => t.PDVC_PC_OBT)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_PC_VIOL)
                .IsFixedLength()
               .HasMaxLength(1);

            this.Property(t => t.PDVC_REF_NR)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_REF_OBT)
                .IsFixedLength()
               .HasMaxLength(1);

            this.Property(t => t.PDVC_REF_VIOL)
                 .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDVC_LOBD_PTR)
                 .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("PDVCJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
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
        }
    }
}
