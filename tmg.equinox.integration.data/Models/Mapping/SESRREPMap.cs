using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESRREPMap : EntityTypeConfiguration<SESRREP>
    {
        public SESRREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SERL_REL_ID, t.SESE_ID });

            // Properties
            this.Property(t => t.SERL_REL_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESR_OPTS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);



            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SESRREP", "Rep");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESR_WT_CTR).HasColumnName("SESR_WT_CTR");
            this.Property(t => t.SESR_OPTS).HasColumnName("SESR_OPTS");
            this.Property(t => t.SESR_LOCK_TOKEN).HasColumnName("SESR_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
