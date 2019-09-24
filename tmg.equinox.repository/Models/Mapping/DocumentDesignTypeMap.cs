using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentDesignTypeMap : EntityTypeConfiguration<DocumentDesignType>
    {
        public DocumentDesignTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentDesignTypeID);

            // Table & Column Mappings
            this.ToTable("DocumentDesignType", "UI");
            this.Property(t => t.DocumentDesignTypeID).HasColumnName("DocumentDesignTypeID");
            this.Property(t => t.DocumentDesignName).HasColumnName("DocumentDesignName");
        }
    }
}
