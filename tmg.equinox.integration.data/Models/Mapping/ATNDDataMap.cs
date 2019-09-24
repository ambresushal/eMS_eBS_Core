using System;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class ATNDDataMap : EntityTypeConfiguration<ATNDData>
    {
        public ATNDDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.ATND_SEQ_NO, t.ProcessGovernance1Up });

            this.ToTable("ATNDModelData", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.ATND_SEQ_NO).HasColumnName("ATND_SEQ_NO");
            this.Property(t => t.ATND_TEXT).HasColumnName("ATND_TEXT");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1up");
        }
    }
}

