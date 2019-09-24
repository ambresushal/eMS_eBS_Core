using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class PDBCDataMap : EntityTypeConfiguration<PDBCData>
    {
        public PDBCDataMap()
        {            
            this.HasKey(t => new { t.PDPD_ID, t.PDBC_TYPE, t.PDBC_PFX, t.ProcessGovernance1up });

            // Table & Column Mappings
            this.ToTable("PDBCModeldata", "ModelData");            
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.PDBC_OPTS).HasColumnName("PDBC_OPTS");
            this.Property(t => t.PDBC_EFF_DT).HasColumnName("PDBC_EFF_DT");
            this.Property(t => t.PDBC_TERM_DT).HasColumnName("PDBC_TERM_DT");
            this.Property(t => t.CreateNewPFX).HasColumnName("CreateNewPFX");
            this.Property(t => t.IsPFXNew).HasColumnName("IsPFXNew");
        }        
    }
}
