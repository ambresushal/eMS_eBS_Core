using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ServiceDesignVersionServiceDefinitionMapMap : EntityTypeConfiguration<ServiceDesignVersionServiceDefinitionMap>
    {
        public ServiceDesignVersionServiceDefinitionMapMap()
        {
            // Primary Key
            this.HasKey(t => t.ServiceDesignVersionServiceDefinitionMapID);

            // Properties
            this.Property(t => t.ServiceDesignVersionID)
                .IsRequired();

            this.Property(t => t.ServiceDefinitionID)
                .IsRequired();

            this.Property(t => t.EffectiveDate)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ServiceDesignVersionServiceDefinitionMap", "ws");
            this.Property(t => t.ServiceDesignVersionServiceDefinitionMapID).HasColumnName("ServiceDesignVersionServiceDefinitionMapID");
            this.Property(t => t.ServiceDesignVersionID).HasColumnName("ServiceDesignVersionID");
            this.Property(t => t.ServiceDefinitionID).HasColumnName("ServiceDefinitionID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.CancelDate).HasColumnName("CancelDate");

            // Relationships
            this.HasRequired(c => c.ServiceDefinition)
                .WithMany(c => c.ServiceDesignVersionServiceDefinitionMaps)
                .HasForeignKey(c => c.ServiceDefinitionID);

            this.HasRequired(c => c.ServiceDesignVersion)
                .WithMany(c => c.ServiceDesignVersionServiceDefinitionMaps)
                .HasForeignKey(c => c.ServiceDesignVersionID);
        }
    }
}
