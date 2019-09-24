using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class PDVCHashDataMap : EntityTypeConfiguration<PDVCHash>
    {
        public PDVCHashDataMap()
        {
            this.HasKey(t =>t.ProcessGovernance1up);
            // Table & Column Mappings
            this.ToTable("PDVCHash", "Hash");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SEPYPFX).HasColumnName("SEPYPFX");
            this.Property(t => t.DEDEPFX).HasColumnName("DEDEPFX");
            this.Property(t => t.LTLTPFX).HasColumnName("LTLTPFX");
            this.Property(t => t.DEDEHash).HasColumnName("DEDEHash");
            this.Property(t => t.LTLTMainHash).HasColumnName("LTLTMainHash");
            this.Property(t => t.SEPYHash).HasColumnName("SEPYHash");
            this.Property(t => t.EFF_DT).HasColumnName("EFF_DT");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.isNewDEDE).HasColumnName("isNewDEDE");
            this.Property(t => t.isNewLTLT).HasColumnName("isNewLTLT");
            this.Property(t => t.isNewSEPY).HasColumnName("isNewSEPY");
        }
    }
}
