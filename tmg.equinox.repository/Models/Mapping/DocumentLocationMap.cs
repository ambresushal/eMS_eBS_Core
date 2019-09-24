using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentLocationMap : EntityTypeConfiguration<DocumentLocation>
    {
        public DocumentLocationMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentLocationID);

            // Table & Column Mappings
            this.ToTable("DocumentLocation", "UI");
            this.Property(t => t.DocumentLocationID).HasColumnName("DocumentLocationID");
            this.Property(t => t.DocumentLocationCode).HasColumnName("DocumentLocationCode");
        }
    }
}
