using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDBCJsonDataMap : EntityTypeConfiguration<PDBCJsonData>
    {
        public PDBCJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.PDBC_TYPE, t.PDBC_PFX });

            // Properties
            this.Property(t => t.ProductID)
                .HasMaxLength(255);

            this.Property(t => t.PDBC_TYPE)
                .HasMaxLength(255);

            this.Property(t => t.PDBC_PFX)
                .HasMaxLength(255);

            this.Property(t => t.PDBC_OPTS)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("PDBCJsonData", "jsondata");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.PDBC_EFF_DT).HasColumnName("PDBC_EFF_DT");
            this.Property(t => t.PDBC_TERM_DT).HasColumnName("PDBC_TERM_DT");
            this.Property(t => t.PDBC_LOCK_TOKEN).HasColumnName("PDBC_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
        }
    }
}
