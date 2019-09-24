using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class PDDSDataMap: EntityTypeConfiguration<PDDSData>
    {
        public PDDSDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.ProcessGovernance1up });
            this.ToTable("PDDSModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDDS_DESC).HasColumnName("PDDS_DESC");
            this.Property(t => t.PDDS_PROD_TYPE).HasColumnName("PDDS_PROD_TYPE");
            this.Property(t => t.PDDS_APP_TYPE).HasColumnName("PDDS_APP_TYPE");
            this.Property(t => t.PDDS_UM_IND).HasColumnName("PDDS_UM_IND");
            this.Property(t => t.PDDS_MED_PRICE_IND).HasColumnName("PDDS_MED_PRICE_IND");
            this.Property(t => t.PDDS_MED_CLMS_IND).HasColumnName("PDDS_MED_CLMS_IND");
            this.Property(t => t.PDDS_DEN_PD_IND).HasColumnName("PDDS_DEN_PD_IND");
            this.Property(t => t.PDDS_DEN_CLMS_IND).HasColumnName("PDDS_DEN_CLMS_IND");
            this.Property(t => t.PDDS_CLED_IND).HasColumnName("PDDS_CLED_IND");
            this.Property(t => t.PDDS_CAP_IND).HasColumnName("PDDS_CAP_IND");
            this.Property(t => t.PDDS_DOFR_IND).HasColumnName("PDDS_DOFR_IND");
            this.Property(t => t.PDDS_OPTOUT_IND).HasColumnName("PDDS_OPTOUT_IND");
            this.Property(t => t.PDDS_PREM_IND).HasColumnName("PDDS_PREM_IND");
            this.Property(t => t.PDDS_INT_STATE_IND).HasColumnName("PDDS_INT_STATE_IND");
            this.Property(t => t.PDDS_MCTR_BCAT).HasColumnName("PDDS_MCTR_BCAT");
            this.Property(t => t.PDDS_MCTR_VAL1).HasColumnName("PDDS_MCTR_VAL1");
            this.Property(t => t.PDDS_DEN_UM_IND).HasColumnName("PDDS_DEN_UM_IND");
            this.Property(t => t.PDDS_DEN_PRICE_IND).HasColumnName("PDDS_DEN_PRICE_IND");
            this.Property(t => t.PDDS_OON_IND).HasColumnName("PDDS_OON_IND");
            this.Property(t => t.PDDS_ALT_DISP_IND).HasColumnName("PDDS_ALT_DISP_IND");
            this.Property(t => t.PDDS_DISP_IND).HasColumnName("PDDS_DISP_IND");
            this.Property(t => t.PDDS_ORD_SYS_IND).HasColumnName("PDDS_ORD_SYS_IND");
            this.Property(t => t.PDDS_OOA_IND).HasColumnName("PDDS_OOA_IND");      
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.PDDS_MCTR_VAL2).HasColumnName("PDDS_MCTR_VAL2");
            this.Property(t => t.NotesTitle).HasColumnName("NotesTitle");
        }
    }
}
