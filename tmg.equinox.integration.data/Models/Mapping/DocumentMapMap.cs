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
    public class DocumentMapMap : EntityTypeConfiguration<DocumentMap>
    {
        public DocumentMapMap()
        {
            // Primary Key
            this.HasKey(t => t.MappingID);

            // Properties

            // Table & Column Mappings
            this.ToTable("DocumentMap", "dbo");
            this.Property(t => t.MappingID).HasColumnName("MappingID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.SourceMapTable).HasColumnName("SourceMapTable");
            this.Property(t => t.DestinationMapTable).HasColumnName("DestinationMapTable");

            // Relationships
        }
    }
}
