using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ConsortiumMap : EntityTypeConfiguration<Consortium>
    {
        public ConsortiumMap()
        {
            // Primary Key
            this.HasKey(t => t.ConsortiumID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.ConsortiumName)                
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Consortium", "Fldr");
            this.Property(t => t.ConsortiumID).HasColumnName("ConsortiumID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.ConsortiumName).HasColumnName("ConsortiumName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            //this.HasRequired(t => t.Tenant)
            //    .WithMany(t => t.Consortiums)
            //    .HasForeignKey(d => d.TenantID);
            //this.HasRequired(t => t.Tenant1)
            //    .WithMany(t => t.Accounts1)
            //    .HasForeignKey(d => d.TenantID);

        }
    }
}




