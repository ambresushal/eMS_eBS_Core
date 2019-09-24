using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ProductMigrationQueueMap : EntityTypeConfiguration<ProductMigrationQueue>
    {
        public ProductMigrationQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.ProductMigrationQueue1Up);

            // Table & Column Mappings
            this.ToTable("ProductMigrationQueue", "fit");
            this.Property(t => t.ProductMigrationQueue1Up).HasColumnName("ProductMigrationQueue1Up");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.FolderVersion).HasColumnName("FolderVersion");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.ServiceGroup).HasColumnName("ServiceGroup");
            this.Property(t => t.MajorFolderVersion).HasColumnName("MajorFolderVersion");        
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.isProcessed).HasColumnName("isProcessed");
            //this.Property(t => t.isFolderConsider).HasColumnName("isFolderConsider");
            this.Property(t => t.HasError).HasColumnName("HasError");
            this.Property(t => t.ErrorDescriotion).HasColumnName("ErrorDescriotion");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.isActive).HasColumnName("isActive");
        }
    }
}
