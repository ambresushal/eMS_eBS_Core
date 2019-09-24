using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESPREPMap : EntityTypeConfiguration<SESPREP>
    {
        public SESPREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SESE_ID, t.SESE_RULE });

            // Properties
            this.Property(t => t.SESE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESP_PEN_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESP_PEN_CALC_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.EXCD_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SERL_REL_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESP_OPTS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);


            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SESPREP", "Rep");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SESP_PEN_IND).HasColumnName("SESP_PEN_IND");
            this.Property(t => t.SESP_PEN_CALC_IND).HasColumnName("SESP_PEN_CALC_IND");
            this.Property(t => t.SESP_PEN_AMT).HasColumnName("SESP_PEN_AMT");
            this.Property(t => t.SESP_PEN_PCT).HasColumnName("SESP_PEN_PCT");
            this.Property(t => t.SESP_PEN_MAX_AMT).HasColumnName("SESP_PEN_MAX_AMT");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESP_OPTS).HasColumnName("SESP_OPTS");
            this.Property(t => t.SESP_LOCK_TOKEN).HasColumnName("SESP_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
