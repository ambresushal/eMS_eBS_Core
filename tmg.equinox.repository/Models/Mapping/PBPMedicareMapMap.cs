using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPMedicareMapMap : EntityTypeConfiguration<PBPMedicareMap>
    {
        public PBPMedicareMapMap()
        {
            this.ToTable("PBPMedicareMap", "PBP");
            this.Property(t => t.PBPMedicareMapID).HasColumnName("PBPMedicareMapID");
            this.Property(t => t.ElementPath).HasColumnName("ElementPath");
            this.Property(t => t.FieldPath).HasColumnName("FieldPath");
            this.Property(t => t.PBPTableName).HasColumnName("PBPTableName");
            this.Property(t => t.PBPFieldName).HasColumnName("PBPFieldName");
            this.Property(t => t.IsCustomRule).HasColumnName("IsCustomRule");
            this.Property(t => t.CustomRuleTypeId).HasColumnName("CustomRuleTypeId");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsFullMigration).HasColumnName("IsFullMigration");
        }
    }
}
