using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESRSRCMap : EntityTypeConfiguration<SESRSRC>
    {
        public SESRSRCMap()
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

            // Table & Column Mappings
            this.ToTable("SESRSRC", "SRC");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESR_WT_CTR).HasColumnName("SESR_WT_CTR");
            this.Property(t => t.SESR_OPTS).HasColumnName("SESR_OPTS");
            this.Property(t => t.SESR_LOCK_TOKEN).HasColumnName("SESR_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.Action).HasColumnName("Action");    
        }
    }
}
