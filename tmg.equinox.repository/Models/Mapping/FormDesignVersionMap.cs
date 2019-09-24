using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignVersionMap : EntityTypeConfiguration<FormDesignVersion>
    {
        public FormDesignVersionMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignVersionID);

            // Properties
            this.Property(t => t.VersionNumber)
                .HasMaxLength(15);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Comments)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("FormDesignVersion", "UI");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.VersionNumber).HasColumnName("VersionNumber");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.FormDesignVersionData).HasColumnName("FormDesignVersionData");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.FormDesignTypeID).HasColumnName("FormDesignTypeID");
            this.Property(t => t.RuleExecutionTreeJSON).HasColumnName("RuleExecutionTreeJSON");
            this.Property(t => t.RuleEventMapJSON).HasColumnName("RuleEventMapJSON");
            this.Property(t => t.PBPViewImpacts).HasColumnName("PBPViewImpacts");

            // Relationships
            this.HasOptional(t => t.FormDesign)
                .WithMany(t => t.FormDesignVersions)
                .HasForeignKey(d => d.FormDesignID);
            this.HasRequired(t => t.Status)
                .WithMany(t => t.FormDesignVersions)
                .HasForeignKey(d => d.StatusID);
            this.HasRequired(t => t.FormDesignType)
                .WithMany(t => t.FormDesignVersions)
                .HasForeignKey(d => d.FormDesignTypeID);

        }
    }
}
