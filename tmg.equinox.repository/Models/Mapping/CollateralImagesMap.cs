using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class CollateralImagesMap: EntityTypeConfiguration<CollateralImages>
    {
        public CollateralImagesMap()
        {
            this.HasKey(t => t.ID);
            this.ToTable("CollateralImages", "Setup");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ImagePath).HasColumnName("ImagePath");
            this.Property(t => t.creationDate).HasColumnName("creationDate");
            this.Property(t => t.Name).HasColumnName("Name");

        }
    }
}
