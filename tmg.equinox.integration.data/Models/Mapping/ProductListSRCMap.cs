using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class ProductListSRCMap : EntityTypeConfiguration<ProductListSRC>
    {
        public ProductListSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProcessGovernance1Up , t.FormInstanceID});



            // Table & Column Mappings
            this.ToTable("ProductList", "SRC");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.ServiceGroup).HasColumnName("ServiceGroup");
            this.Property(t => t.AccountName).HasColumnName("AccountName");
            this.Property(t => t.FolderName).HasColumnName("FolderName");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.IsReleasedVersion).HasColumnName("IsReleasedVersion");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");
            this.Property(t => t.FolderVersionStateID).HasColumnName("FolderVersionStateID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.ProductList1up).HasColumnName("ProductList1up").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.Property(t => t.ProcessStatus1up).HasColumnName("ProcessStatus1up");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
        }
    }
}
