using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class DisallowedMessagesSRCMap : EntityTypeConfiguration<DisallowedMessagesSRC>
    {
        public DisallowedMessagesSRCMap()
        {
            this.HasKey(t => new { t.Description, t.DisallowedExecutionCodeID, t.ProcessGovernance1up });

            this.ToTable("DisallowedMessagesSRC", "SRC");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.DisallowedExecutionCodeID).HasColumnName("DisallowedExecutionCodeID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.Action).HasColumnName("Action");
        }
    }
}
