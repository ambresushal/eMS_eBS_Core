using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FolderVersionCategoryMap : EntityTypeConfiguration<FolderVersionCategory>
    {

        public FolderVersionCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.FolderVersionCategoryID);


            this.ToTable("FolderVersionCategory", "Fldr");
            this.Property(t => t.FolderVersionCategoryID).HasColumnName("FolderVersionCategoryID");
            this.Property(t => t.FolderVersionCategoryName).HasColumnName("FolderVersionCategoryName");
            this.Property(t => t.FolderVersionGroupID).HasColumnName("FolderVersionGroupID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");          
            
        }


    }
}
