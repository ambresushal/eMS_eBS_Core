using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class CBCNamingConventionsMap :EntityTypeConfiguration<CBCNamingConventions>
    {
        public CBCNamingConventionsMap()
        {
            // Primary Key
            this.HasKey(t => new { t.NamingConvention1up });

            // Table & Column Mappings
            this.ToTable("CBCNamingConvention", "fit");
            this.Property(t => t.NamingConvention1up).HasColumnName("NamingConvention1up");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.LOBD_ID).HasColumnName("LOBD_ID");
            this.Property(t => t.BenefitSetName).HasColumnName("BenefitSetName");
        }
    }
}
