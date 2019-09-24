using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
  public  class PBPImportEmailNotificationMap : EntityTypeConfiguration<PBPImportEmailNotification>
    {
        public PBPImportEmailNotificationMap()
        {
            this.HasKey(s => s.PBPImportEmailNotification1Up);

            this.ToTable("PBPImportEmailNotification", "Setup");
            this.Property(t => t.PBPImportEmailNotification1Up).HasColumnName("PBPImportEmailNotification1Up");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
