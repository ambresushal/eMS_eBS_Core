using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using tmg.equinox.domain.entities.Models;
using tmg.equinox.setting;

namespace tmg.equinox.repository.core.Models.Mapping
{
    public class SettingDefinitionMap : EntityTypeConfiguration<SettingDefinition>
    {
        public SettingDefinitionMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            this.ToTable("SettingDefinition", "Frmk");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.SettingDefinitiontype).HasColumnName("SettingDefinitiontype").IsRequired();
            this.Property(t => t.SettingName).HasColumnName("SettingName").IsRequired();
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsInherited).HasColumnName("IsInherited");
            this.Property(t => t.IsVisibleToClients).HasColumnName("IsVisibleToClients");
            this.Property(t => t.CustomData).HasColumnName("CustomData");
        }
    }
}
