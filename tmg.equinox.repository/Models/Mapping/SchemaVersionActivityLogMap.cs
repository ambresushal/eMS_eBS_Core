using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;

namespace tmg.equinox.repository.Models.Mapping
{
    class SchemaVersionActivityLogMap : EntityTypeConfiguration<SchemaVersionActivityLog>
    {
        public SchemaVersionActivityLogMap()
        {

            // Primary Key
            this.HasKey(t => t.ID);


            // Table & Column Mappings
            this.ToTable("SchemaVersionActivityLog", "Rpt");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.DesignVersionId).HasColumnName("DesignVersionID").IsRequired();
            this.Property(t => t.DesignVersion).HasColumnName("DesignVersion").IsRequired();
            this.Property(t => t.ObjectType).HasColumnName("ObjectType");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.CreationDate).HasColumnName("CreationDate"); 
        }
    }
}
