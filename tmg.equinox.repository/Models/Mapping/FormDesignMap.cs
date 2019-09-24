using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignMap : EntityTypeConfiguration<FormDesign>
    {
        public FormDesignMap()
        {
            // Primary Key
            this.HasKey(t => t.FormID);

            // Properties
            this.Property(t => t.FormName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.DisplayText)
                .HasMaxLength(100);

            this.Property(t => t.Abbreviation)
                .HasMaxLength(7);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            //this.Property(t => t.IsMasterList);
                
                
               

            // Table & Column Mappings
            this.ToTable("FormDesign", "UI");
            this.Property(t => t.FormID).HasColumnName("FormID");
            this.Property(t => t.FormName).HasColumnName("FormName");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Abbreviation).HasColumnName("Abbreviation");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsMasterList).HasColumnName("IsMasterList");
            this.Property(t => t.IsAliasDesignMasterList).HasColumnName("IsAliasDesignMasterList");
            this.Property(t => t.UsesAliasDesignMasterList).HasColumnName("UsesAliasDesignMasterList");
            this.Property(t => t.DocumentDesignTypeID).HasColumnName("DocumentDesignTypeID");
            this.Property(t => t.DocumentLocationID).HasColumnName("DocumentLocationID");
            this.Property(t => t.IsSectionLock).HasColumnName("IsSectionLock");

        }
    }
}
