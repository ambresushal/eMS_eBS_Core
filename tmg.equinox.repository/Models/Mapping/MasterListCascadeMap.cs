using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListCascadeMap : EntityTypeConfiguration<MasterListCascade>
    {
        public MasterListCascadeMap()
        {
            // Primary Key
            this.HasKey(t => t.MasterListCascadeID);
            // Properties
            this.Property(t => t.TargetDocumentType)
                .IsRequired();
            this.Property(t => t.IsActive)
                .IsRequired();
            this.Property(t => t.MasterListDesignID)
                .IsRequired();
            this.Property(t => t.MasterListDesignVersionID)
                .IsRequired();
            this.Property(t => t.MasterListJSONPath)
                .IsRequired()
                .HasMaxLength(500);
            this.Property(t => t.TargetDesignID)
                .IsRequired();
            this.Property(t => t.TargetDesignVersionID)
                .IsRequired();
            this.Property(t => t.TargetDocumentType)
                .IsRequired();
            this.Property(t => t.UpdateExpressionRule)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("MasterListCascade", "ML");
            this.Property(t => t.MasterListCascadeID).HasColumnName("MasterListCascadeID");
            this.Property(t => t.FilterExpressionRule).HasColumnName("FilterExpressionRule");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.MasterListDesignID).HasColumnName("MasterListDesignID");
            this.Property(t => t.MasterListDesignVersionID).HasColumnName("MasterListDesignVersionID");
            this.Property(t => t.MasterListJSONPath).HasColumnName("MasterListJSONPath");
            this.Property(t => t.TargetDesignID).HasColumnName("TargetDesignID");
            this.Property(t => t.TargetDocumentType).HasColumnName("TargetDocumentType");
            this.Property(t => t.TargetDesignVersionID).HasColumnName("TargetDesignVersionID");
            this.Property(t => t.UpdateExpressionRule).HasColumnName("UpdateExpressionRule");
            this.Property(t => t.CompareMacroJSON).HasColumnName("CompareMacroJSON");
            this.Property(t => t.MasterListCompareJSON).HasColumnName("MasterListCompareJSON");

            this.HasRequired(t => t.MasterListCascadeTargetDocumentType)
                .WithMany(t => t.MasterListCascades)
                .HasForeignKey(d => d.TargetDocumentType);
        }
    }
}
