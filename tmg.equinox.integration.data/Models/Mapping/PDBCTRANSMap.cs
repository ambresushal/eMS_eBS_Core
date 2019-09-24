using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDBCTRANSMap : EntityTypeConfiguration<PDBCTRANS>
    {
        public PDBCTRANSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDPD_ID, t.PDBC_TYPE, t.PDBC_PFX });

            // Properties
            this.Property(t => t.PDBC_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            //this.Property(t => t.PDPD_ID)
            //    .IsRequired()
            //    .IsFixedLength()
            //    .HasMaxLength(8);

            this.Property(t => t.PDBC_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDBC_OPTS)
                .HasMaxLength(4);

            

            // Table & Column Mappings
            this.ToTable("PDBCTRANS", "Trans");
            //this.Property(t => t.PDBCId).HasColumnName("PDBCId");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.PDBC_EFF_DT).HasColumnName("PDBC_EFF_DT");
            this.Property(t => t.PDBC_TERM_DT).HasColumnName("PDBC_TERM_DT");
            this.Property(t => t.PDBC_OPTS).HasColumnName("PDBC_OPTS");
            //this.Property(t => t.PDBC_LOCK_TOKEN).HasColumnName("PDBC_LOCK_TOKEN");
            //this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            //this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            //this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            //this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
           // this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
