using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class PDVCDataMap : EntityTypeConfiguration<PDVCData>
    {
        public PDVCDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BenefitSet, t.PDVC_TYPE, t.PDVC_SEQ_NO, t.ProcessGovernance1up });
            // Table & Column Mappings
            this.ToTable("PDVCModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.PDVC_TIER).HasColumnName("PDVC_TIER");
            this.Property(t => t.PDVC_TYPE).HasColumnName("PDVC_TYPE");
            this.Property(t => t.PDVC_SEQ_NO).HasColumnName("PDVC_SEQ_NO");
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
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.PDVC_EFF_DT).HasColumnName("PDVC_EFF_DT");
            this.Property(t => t.PDVC_TERM_DT).HasColumnName("PDVC_TERM_DT");

            this.Property(t => t.SEPY_PFX).HasColumnName("SEPY_PFX");
            this.Property(t => t.DEDE_PFX).HasColumnName("DEDE_PFX");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.DPPY_PFX).HasColumnName("DPPY_PFX");
            this.Property(t => t.CGPY_PFX).HasColumnName("CGPY_PFX");
            this.Property(t => t.BSME_PFX).HasColumnName("BSME_PFX");
            this.Property(t => t.SEPY_SHDW_PFX_NVL).HasColumnName("SEPY_SHDW_PFX_NVL");
            this.Property(t => t.DEDE_SHDW_PFX_NVL).HasColumnName("DEDE_SHDW_PFX_NVL");
            this.Property(t => t.LTLT_SHDW_PFX_NVL).HasColumnName("LTLT_SHDW_PFX_NVL");
            this.Property(t => t.DPPY_SHDW_PFX_NVL).HasColumnName("DPPY_SHDW_PFX_NVL");
            this.Property(t => t.CGPY_SHDW_PFX_NVL).HasColumnName("CGPY_SHDW_PFX_NVL");
            this.Property(t => t.DEDEHashcode).HasColumnName("DEDEHashcode");
            this.Property(t => t.LTLTHashcode).HasColumnName("LTLTHashcode");
            this.Property(t => t.LTSEHashcode).HasColumnName("LTSEHashcode");
            this.Property(t => t.LTIPHashcode).HasColumnName("LTIPHashcode");
            this.Property(t => t.LTIDHashcode).HasColumnName("LTIDHashcode");
            this.Property(t => t.LTPRHashcode).HasColumnName("LTPRHashcode");
            this.Property(t => t.PDVCHashcode).HasColumnName("PDVCHashcode");
            this.Property(t => t.SEPYHashCode).HasColumnName("SEPYHashCode");
            this.Property(t => t.LTLTMainHash).HasColumnName("LTLTMainHash");
            this.Property(t => t.StandardProduct).HasColumnName("StandardProduct");
            this.Property(t => t.ImSHDW).HasColumnName("ImSHDW");
            this.Property(t => t.ImNotInQueue).HasColumnName("ImNotInQueue");
        }
    }
}
