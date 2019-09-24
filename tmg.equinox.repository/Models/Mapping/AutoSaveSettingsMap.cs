using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AutoSaveSettingsMap : EntityTypeConfiguration<AutoSaveSettings>
    {
        public AutoSaveSettingsMap()
        {
            // Primary Key
            this.HasKey(t => t.SettingsID);




            // Table & Column Mappings
            this.ToTable("AutoSaveSettings", "Sec");
            this.Property(t => t.SettingsID).HasColumnName("SettingsID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.IsAutoSaveEnabled).HasColumnName("IsAutoSaveEnabled");
            this.Property(t => t.Duration).HasColumnName("Duration");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            this.HasRequired(t => t.Tenant)
               .WithMany(t => t.AutoSaveSettings)
               .HasForeignKey(d => d.TenantID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.AutoSaveSettings)
                .HasForeignKey(d => d.UserID);
        }
    }
}
