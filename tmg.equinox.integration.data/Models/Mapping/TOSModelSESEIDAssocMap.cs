using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class TOSModelSESEIDAssocMap : EntityTypeConfiguration<TOSModelSESEIDAssoc>
    {
        public TOSModelSESEIDAssocMap()
        {
            this.HasKey(t => t.SESE_ID);

            this.ToTable("TOSModelSESEIDAssoc", "dbo");

            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.ModelSESE_ID).HasColumnName("ModelSESE_ID");
        }
    }
}
