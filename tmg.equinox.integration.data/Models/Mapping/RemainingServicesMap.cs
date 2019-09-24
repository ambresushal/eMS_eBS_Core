using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class RemainingServicesMap : EntityTypeConfiguration<RemainingServices>
    {
        public RemainingServicesMap()
        {
            this.HasKey(t => t.ProductID);
            this.ToTable("RemainingServices", "Temp");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SESEHash).HasColumnName("SESEHash");
            this.Property(t => t.SETRHash).HasColumnName("SETRHash");
            this.Property(t => t.SESPHash).HasColumnName("SESPHash");
            this.Property(t => t.SESERuleCategory).HasColumnName("SESERuleCategory");
            this.Property(t => t.NewSESERuleCategory).HasColumnName("NewSESERuleCategory");
            this.Property(t => t.IsCreateRule).HasColumnName("IsCreateRule");
            this.Property(t => t.IsAltRule).HasColumnName("IsAltRule");
            this.Property(t => t.IsCovered).HasColumnName("IsCovered");
            this.Property(t => t.NewRule).HasColumnName("NewRule");
            this.Property(t => t.RuleDescription).HasColumnName("RuleDescription");           
        }
    }
}
