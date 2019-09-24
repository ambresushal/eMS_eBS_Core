using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class BenefitSummaryInfoDataMap : EntityTypeConfiguration<BenefitSummaryInfoData>
    {
        public BenefitSummaryInfoDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BSBS_TYPE, t.BSTX_SEQ_NO, t.ProcessGovernance1up });

            this.ToTable("BSBSModelData", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BSBS_TYPE).HasColumnName("BSBS_TYPE");
            this.Property(t => t.BSBS_DESC).HasColumnName("BSBS_DESC");
            this.Property(t => t.BSTX_TEXT_varchar).HasColumnName("BSTX_TEXT2");
            this.Property(t => t.BSTX_TEXT).HasColumnName("BSTX_TEXT");
            this.Property(t => t.BSTX_SEQ_NO).HasColumnName("BSTX_SEQ_NO");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");

        }
    }
}
