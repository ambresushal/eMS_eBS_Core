using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.facet.data.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class ProductLevelHashDataMap : EntityTypeConfiguration<ProductLevelHash>
    {
        public ProductLevelHashDataMap()
        {
            this.HasKey(t => t.ProcessGovernance1up);
            // Table & Column Mappings
            this.ToTable("ProductLevelHash", "Hash");
            this.Property(p => p.ProductID).HasColumnName("ProductID");
            this.Property(p => p.PDPDHash).HasColumnName("PDPDHash");
            this.Property(p => p.EBCLHash).HasColumnName("EBCLHash");
            this.Property(p => p.BSBSHash).HasColumnName("BSBSHash");
            this.Property(p => p.PDDSHash).HasColumnName("PDDSHash");
            this.Property(p => p.EBCLPFX).HasColumnName("EBCLPFX");
            this.Property(p => p.ProcessGovernance1up).HasColumnName("ProcessGovernance1Up");
            this.Property(p => p.BSBSPFX).HasColumnName("BSBSPFX");
            this.Property(p => p.isNewEBCL).HasColumnName("isNewEBCL");
            this.Property(p => p.isNewBSBS).HasColumnName("isNewBSBS");
            this.Property(p => p.IsUsingNewBSBS).HasColumnName("IsUsingNewBSBS");
        }
    }
}
