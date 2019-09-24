using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESRTRANSMap : EntityTypeConfiguration<SESRTRANS>
    {
        public SESRTRANSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SERL_REL_ID, t.SESE_ID, t.SESR_WT_CTR });

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

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);


            //this.Property(t => t.Action)
            //                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //this.Property(t => t.Hashcode)
            //    //.IsRequired()
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SESRTRANS", "Trans");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESR_WT_CTR).HasColumnName("SESR_WT_CTR");
            this.Property(t => t.SESR_OPTS).HasColumnName("SESR_OPTS");
            this.Property(t => t.SESR_LOCK_TOKEN).HasColumnName("SESR_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
           // this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
