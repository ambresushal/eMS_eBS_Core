using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class CollateralProcessUploadMap : EntityTypeConfiguration<CollateralProcessUpload>
    {
        public CollateralProcessUploadMap()

        {
            this.HasKey(t => t.ID);
            this.ToTable("CollateralProcessUpload", "Setup");
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
            this.Property(t => t.WordFile).HasColumnName("WordFile");
            this.Property(t => t.PrintxFile).HasColumnName("PrintXFile");
            this.Property(t => t.File508).HasColumnName("File508");
            this.Property(t => t.HasError).HasColumnName("HasError");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.ErrorDescription).HasColumnName("ErrorDescription");
            this.Property(t => t.CollateralName).HasColumnName("CollateralName");
        }
    }
}
