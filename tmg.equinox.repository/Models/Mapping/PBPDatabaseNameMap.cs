using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class PBPDatabaseNameMap : EntityTypeConfiguration<PBPDatabase>
    {
       public PBPDatabaseNameMap()
       {
           // Primary Key
           this.HasKey(t => t.PBPDatabase1Up);
           // Properties
           this.Property(t => t.DataBaseName)
                .HasMaxLength(100);

           this.Property(t => t.DataBaseDescription)
               .HasMaxLength(100);

           this.Property(t => t.CreatedBy)
               .HasMaxLength(100);
           this.Property(t => t.UpdatedBy)
               .HasMaxLength(100);
           // Table & Column Mappings
           this.ToTable("PBPDatabase", "Setup");
           this.Property(t => t.PBPDatabase1Up).HasColumnName("PBPDatabase1Up");
           this.Property(t => t.DataBaseName).HasColumnName("DataBaseName");
           this.Property(t => t.DataBaseDescription).HasColumnName("DataBaseDescription");
           this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
           this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
           this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
           this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
           this.Property(t => t.IsActive).HasColumnName("IsActive");
       }
    }
}
