using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class SBCollateralProcessQueueMap : EntityTypeConfiguration<SBCollateralProcessQueue>
    {
        public SBCollateralProcessQueueMap()
        {
            this.HasKey(t => t.SBDesignCollateralConfiguration1Up);

            this.ToTable("SBCollateralProcessQueue", "Setup");

            this.Property(t => t.SBDesignCollateralConfiguration1Up).HasColumnName("SBCollateralProcessQueue1Up");
            this.Property(t => t.CollateralProcessQueue1Up).HasColumnName("CollateralProcessQueue1Up");
            this.Property(t => t.AccountID).HasColumnName("AccountID");
            this.Property(t => t.AccountName).HasColumnName("AccountName");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderName).HasColumnName("FolderName");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FormInstanceName).HasColumnName("FormInstanceName");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.FolderVersionEffectiveDate).HasColumnName("FolderVersionEffectiveDate");
        }
    }
}
