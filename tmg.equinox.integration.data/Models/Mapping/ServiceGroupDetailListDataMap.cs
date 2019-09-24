using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class ServiceGroupDetailListDataMap : EntityTypeConfiguration<ServiceGroupDetailListData>
    {
        public ServiceGroupDetailListDataMap()
        {
            this.HasKey(t => new { t.ServiceGroupHeader, t.SESE_ID, t.AccumulatorsList, t.ProcessGovernance1up });

            this.ToTable("ServiceGroupDetailListModeldata", "ModelData");
            this.Property(t => t.ServiceGroupHeader).HasColumnName("ServiceGroupHeader");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.ServiceDefaultRule).HasColumnName("ServiceDefaultRule");
            this.Property(t => t.ServiceDefaultAltRule).HasColumnName("ServiceDefaultAltRule");
            this.Property(t => t.ServiceModelSESEID).HasColumnName("ServiceModelSESEID");
            this.Property(t => t.AccumulatorsList).HasColumnName("AccumulatorsList");
            this.Property(t => t.LimitModelSESEID).HasColumnName("LimitModelSESEID");
            this.Property(t => t.SESE_RULE_ALT_COND).HasColumnName("SESE_RULE_ALT_COND");
            this.Property(t => t.WeightCounter).HasColumnName("AccumWeightCntr");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
