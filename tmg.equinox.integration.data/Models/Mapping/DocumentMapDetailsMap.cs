using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentMapDetailsMap : EntityTypeConfiguration<DocumentMapDetails>
    {
        public DocumentMapDetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentMapDetailsID);

            // Table & Column Mappings
            this.ToTable("DocumentMapDetails", "dbo");
            this.Property(t => t.DocumentMapDetailsID).HasColumnName("DocumentMapDetailsID");
            this.Property(t => t.MappingID).HasColumnName("MappingID");
            this.Property(t => t.ColumnName).HasColumnName("ColumnName");
            this.Property(t => t.ColumnMapTo).HasColumnName("ColumnMapTo");


            // Relationships
            this.HasRequired(t => t.DocumentMap)
                .WithMany(t => t.DocumentMapDetails)
                .HasForeignKey(d => d.MappingID);
        }
    }
}
