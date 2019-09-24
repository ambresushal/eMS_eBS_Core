using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RepeaterUIElementPropertyMap : EntityTypeConfiguration<RepeaterUIElementProperties>
    {
        public RepeaterUIElementPropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.RepeaterUIElementPropertyID);

            // Table & Column Mappings
            this.ToTable("RepeaterUIElementProperties", "UI");
            this.Property(t => t.RepeaterUIElementPropertyID).HasColumnName("RepeaterUIElementPropertyID");
            this.Property(t => t.RepeaterUIElementID).HasColumnName("RepeaterUIElementID");
            this.Property(t => t.RowTemplate).HasColumnName("RowTemplate");
            this.Property(t => t.HeaderTemplate).HasColumnName("HeaderTemplate");
            this.Property(t => t.FooterTemplate).HasColumnName("FooterTemplate");

        }
    }
}
