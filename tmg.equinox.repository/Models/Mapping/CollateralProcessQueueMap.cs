using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class CollateralProcessQueueMap : EntityTypeConfiguration<CollateralProcessQueue>
    {
        public CollateralProcessQueueMap()

        {
            this.HasKey(t => t.CollateralProcessQueue1Up);
            this.ToTable("CollateralProcessQueue", "Setup");
            this.Property(t => t.CollateralProcessQueue1Up).HasColumnName("CollateralProcessQueue1Up");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ProductName).HasColumnName("ProductName");
            this.Property(t => t.AccountID).HasColumnName("AccountID");
            this.Property(t => t.AccountName).HasColumnName("AccountName");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderName).HasColumnName("FolderName");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FormInstanceName).HasColumnName("FormInstanceName");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.ProcessStatus1Up).HasColumnName("ProcessStatus1Up");
            this.Property(t => t.TemplateReportID).HasColumnName("TemplateReportID");
            this.Property(t => t.TemplateReportVersionID).HasColumnName("TemplateReportVersionID");
            this.Property(t => t.TemplateReportVersionEffectiveDate).HasColumnName("TemplateReportVersionEffectiveDate");
            this.Property(t => t.CollateralStorageLocation).HasColumnName("CollateralStorageLocation");
            this.Property(t => t.HasError).HasColumnName("HasError");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.ErrorDescription).HasColumnName("ErrorDescription");
            this.Property(t => t.FilePath).HasColumnName("FilePath");
            this.Property(t => t.WordFilePath).HasColumnName("WordFilePath");            
        }
    }
}
