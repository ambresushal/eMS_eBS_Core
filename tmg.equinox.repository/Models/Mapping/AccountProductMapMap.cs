using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AccountProductMapMap : EntityTypeConfiguration<AccountProductMap>
    {
        public AccountProductMapMap()
        {
            // Primary Key
            this.HasKey(t => t.AccountProductMapID);

            // Properties
            this.Property(t => t.ProductType)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ProductID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("AccountProductMap", "Accn");
            this.Property(t => t.AccountProductMapID).HasColumnName("AccountProductMapID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.ProductType).HasColumnName("ProductType");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.PlanCode).HasColumnName("PlanCode");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.ServiceGroup).HasColumnName("ServiceGroup");
            //this.Property(t => t.ProductName).HasColumnName("ProductName");
            this.Property(t => t.ANOCChartPlanType).HasColumnName("ANOCChartPlanType");
            this.Property(t => t.RXBenefit).HasColumnName("RXBenefit");
            this.Property(t => t.SNPType).HasColumnName("SNPType");
            // Relationship
            this.HasRequired(t => t.Folder)
                .WithMany(t => t.AccountProductMaps)
                .HasForeignKey(d => d.FolderID);

            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.AccountProductMaps)
                .HasForeignKey(d => d.FolderVersionID);

            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.AccountProductMaps)
                .HasForeignKey(d => d.TenantID);

            this.HasRequired(t => t.FormInstance)
               .WithMany(t => t.AccountProductMaps)
               .HasForeignKey(d => d.FormInstanceID);

        }
    }
}
