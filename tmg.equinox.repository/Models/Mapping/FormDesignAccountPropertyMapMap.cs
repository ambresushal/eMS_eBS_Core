using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignAccountPropertyMapMap : EntityTypeConfiguration<FormDesignAccountPropertyMap>
    {
        public FormDesignAccountPropertyMapMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignAccountPropertyMapID);

            // Table & Column Mappings
            this.ToTable("FormDesignAccountPropertyMap", "Accn");         
          
            this.Property(t=>t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t=>t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t=>t.AccountPropertyName).HasColumnName("AccountPropertyName");
            this.Property(t=>t.AccountPropertyPath).HasColumnName("AccountPropertyPath");
            this.Property(t=>t.TenantID).HasColumnName("TenantID");
            this.Property(t=>t.AddedDate).HasColumnName("AddedDate");
            this.Property(t=>t.AddedBy).HasColumnName("AddedBy");
            this.Property(t=>t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t=>t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t=>t.UpdatedDate).HasColumnName("UpdatedDate");

            this.HasRequired(t => t.Tenant)
                 .WithMany(t => t.FormDesignAccountPropertyMaps)
                 .HasForeignKey(d => d.TenantID);
           
            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.FormDesignAccountPropertyMaps)
                .HasForeignKey(d => d.FormDesignVersionID);

            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.FormDesignAccountPropertyMaps)
                .HasForeignKey(d => d.FormDesignID);
        }
    }
}
