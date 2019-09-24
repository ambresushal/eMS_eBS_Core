using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TenantMap : EntityTypeConfiguration<Tenant>
    {
        public TenantMap()
        {
            // Primary Key
            this.HasKey(t => t.TenantID);

            // Properties
            this.Property(t => t.TenantName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdateBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Tenant", "Sec");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.TenantName).HasColumnName("TenantName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.CreateDt).HasColumnName("CreateDt");
            this.Property(t => t.UpdateDt).HasColumnName("UpdateDt");
        }
    }
}
