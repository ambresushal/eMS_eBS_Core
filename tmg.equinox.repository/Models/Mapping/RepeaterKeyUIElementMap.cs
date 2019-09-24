using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RepeaterKeyUIElementMap : EntityTypeConfiguration<RepeaterKeyUIElement>
    {
        public RepeaterKeyUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.RepeaterKeyElementID);

            // Table & Column Mappings
            this.ToTable("RepeaterKeyUIElement", "UI");
            this.Property(t => t.RepeaterKeyElementID).HasColumnName("RepeaterKeyElementID");
            this.Property(t => t.RepeaterUIElementID).HasColumnName("RepeaterUIElementID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");

        }
    }
}
