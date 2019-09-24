using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AccountMap : EntityTypeConfiguration<Account>
    {
        public AccountMap()
        {
            // Primary Key
            this.HasKey(t => t.AccountID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.AccountName)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Account", "Accn");
            this.Property(t => t.AccountID).HasColumnName("AccountID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.AccountName).HasColumnName("AccountName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.Accounts)
                .HasForeignKey(d => d.TenantID);
            this.HasRequired(t => t.Tenant1)
                .WithMany(t => t.Accounts1)
                .HasForeignKey(d => d.TenantID);

        }
    }
}
