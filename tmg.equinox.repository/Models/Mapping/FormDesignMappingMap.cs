using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignMappingMap : EntityTypeConfiguration<FormDesignMapping>
    {
        public FormDesignMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignMapID);

            // Table & Column Mappings
            this.ToTable("FormDesignMapping", "UI");
            this.Property(t => t.FormDesignMapID).HasColumnName("FormDesignMapID");
            this.Property(t => t.AnchorDesignID).HasColumnName("AnchorDesignID");
            this.Property(t => t.TargetDesignID).HasColumnName("TargetDesignID");
        }
    }
}
