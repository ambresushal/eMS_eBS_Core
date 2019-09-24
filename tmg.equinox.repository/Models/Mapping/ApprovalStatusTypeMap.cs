using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    //public class ApprovalStatusTypeMap : EntityTypeConfiguration<ApprovalStatusType>
    //{
    //    public ApprovalStatusTypeMap()
    //    {
    //        // Primary Key
    //        this.HasKey(t => t.ApprovalStatusID);

    //        // Properties
    //        this.Property(t => t.ApprovalStatus)
    //            .IsRequired()
    //            .HasMaxLength(200);

    //        this.Property(t => t.AddedBy)
    //            .IsRequired()
    //            .HasMaxLength(20);

    //        this.Property(t => t.UpdatedBy)
    //            .HasMaxLength(20);

    //        // Table & Column Mappings
    //        this.ToTable("ApprovalStatusType", "Fldr");
    //        this.Property(t => t.ApprovalStatusID).HasColumnName("ApprovalStatusID");
    //        this.Property(t => t.ApprovalStatus).HasColumnName("ApprovalStatus");
    //        this.Property(t => t.TenantID).HasColumnName("TenantID");
    //        this.Property(t => t.AddedDate).HasColumnName("AddedDate");
    //        this.Property(t => t.AddedBy).HasColumnName("AddedBy");
    //        this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
    //        this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

    //        // Relationships
    //        this.HasRequired(t => t.Tenant)
    //            .WithMany(t => t.ApprovalStatusTypes)
    //            .HasForeignKey(d => d.TenantID);

    //    }
    //}
}
