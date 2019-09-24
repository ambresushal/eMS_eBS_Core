using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class BenefitHashMap : EntityTypeConfiguration<BenefitHash>
    {
        public BenefitHashMap()
        {
            this.HasKey(t => t.ProductID);
            this.ToTable("BenefitHash", "Hash");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SETRHash).HasColumnName("SETRHash");
            this.Property(t => t.SESPHash).HasColumnName("SESPHash");
            this.Property(t => t.SETRAltHash).HasColumnName("SETRAltHash");
            this.Property(t => t.SESPAltHash).HasColumnName("SESPAltHash");
            this.Property(t => t.IsCovered).HasColumnName("IsCovered");
            this.Property(t => t.IsCoveredAlt).HasColumnName("IsCoveredAlt");
            this.Property(t => t.SESERule).HasColumnName("SESERule");
            this.Property(t => t.SESEAltRule).HasColumnName("SESEAltRule");
            this.Property(t => t.SESEHash).HasColumnName("SESEHash");
            this.Property(t => t.SESEAltHash).HasColumnName("SESEAltHash");
            this.Property(t => t.SESERuleCategory).HasColumnName("SESERuleCategory");
            this.Property(t => t.SESEAltRuleCategory).HasColumnName("SESEAltRuleCategory");
            this.Property(t => t.HasHashCodeMatchRule).HasColumnName("HasHashCodeMatchRule");
            this.Property(t => t.HasHashCodeMatchAltRule).HasColumnName("HasHashCodeMatchAltRule");
            //this.Property(t => t.).HasColumnName("");

        }
    }
}
