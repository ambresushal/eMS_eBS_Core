using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class DEDEDataMap : EntityTypeConfiguration<DEDEData>
    {
        public DEDEDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BenefitSet, t.ACAC_ACC_NO, t.ProcessGovernance1up });

            this.ToTable("DEDEModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.DEDE_DESC).HasColumnName("DEDE_DESC");
            this.Property(t => t.DEDE_RULE).HasColumnName("DEDE_RULE");
            this.Property(t => t.DEDE_REL_ACC_ID).HasColumnName("DEDE_REL_ACC_ID");
            this.Property(t => t.DEDE_COB_OOP_IND).HasColumnName("DEDE_COB_OOP_IND");
            this.Property(t => t.DEDE_SL_IND).HasColumnName("DEDE_SL_IND");
            this.Property(t => t.DEDE_PERIOD_IND).HasColumnName("DEDE_PERIOD_IND");
            this.Property(t => t.DEDE_AGG_PERSON).HasColumnName("DEDE_AGG_PERSON");
            this.Property(t => t.DEDE_FAM_AMT).HasColumnName("DEDE_FAM_AMT");
            this.Property(t => t.DEDE_MEME_AMT).HasColumnName("DEDE_MEME_AMT");
            this.Property(t => t.DEDE_AGG_PERSON_CO).HasColumnName("DEDE_AGG_PERSON_CO");
            this.Property(t => t.DEDE_FAM_AMT_CO).HasColumnName("DEDE_FAM_AMT_CO");
            this.Property(t => t.DEDE_MEME_AMT_CO).HasColumnName("DEDE_MEME_AMT_CO");
            this.Property(t => t.DEDE_CO_BYPASS).HasColumnName("DEDE_CO_BYPASS");
            this.Property(t => t.DEDE_MEM_SAL_IND).HasColumnName("DEDE_MEM_SAL_IND");
            this.Property(t => t.DEDE_FAM_SAL_IND).HasColumnName("DEDE_FAM_SAL_IND");
            this.Property(t => t.EBCLSelection).HasColumnName("EBCLSelection");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up"); 
        }
    }
}
