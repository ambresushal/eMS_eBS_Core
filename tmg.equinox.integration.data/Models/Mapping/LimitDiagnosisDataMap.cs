﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class LimitDiagnosisDataMap:EntityTypeConfiguration<LimitDiagnosisData>
    {
        public LimitDiagnosisDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.ACAC_ACC_NO, t.IDCD_ID_REL, t.IDCD_TYPE, t.BenefitSet, t.ProcessGovernance1up });

            this.ToTable("LTIDModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.IDCD_ID_REL).HasColumnName("IDCD_ID_REL");
            this.Property(t => t.IDCD_TYPE).HasColumnName("IDCD_TYPE");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
        }
    }
}