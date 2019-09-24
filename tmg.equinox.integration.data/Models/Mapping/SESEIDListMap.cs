using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class SESEIDListMap: EntityTypeConfiguration<SESEIDList>
    {
        public SESEIDListMap()
        {
            this.HasKey(t => new { t.SESE_ID });

            //Table and column mappings
            this.ToTable("SESEIDList", "SRC");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitCategory1).HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.PlaceOfService).HasColumnName("PlaceOfService");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
