using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ElementDocumentRuleMap : EntityTypeConfiguration<ElementDocumentRule>
    {
        public ElementDocumentRuleMap()
        {
            // Primary Key
            this.HasKey(t => t.ElementDocumentRuleID);
            // Properties
            this.Property(t => t.FormDesignID)
                .IsRequired();
            this.Property(t => t.FormDesignVersionID)
                .IsRequired();
            this.Property(t => t.ParentFormDesignVersionID);
            this.Property(t => t.SourceField)
                .IsRequired()
                .HasMaxLength(200);
            this.Property(t => t.SourcePath)
                .IsRequired()
                .HasMaxLength(1000);
            this.Property(t => t.TargetFields)
                .IsRequired()
                .HasMaxLength(2000);
            this.Property(t => t.RuleJSON)
                .IsRequired();
            this.Property(t => t.AddedBy)
                .IsRequired();
            this.Property(t => t.AddedDate)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ElementDocumentRule", "ML");
            this.Property(t => t.ElementDocumentRuleID).HasColumnName("ElementDocumentRuleID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.RuleJSON).HasColumnName("RuleJSON");
            this.Property(t => t.SourceField).HasColumnName("SourceField");
            this.Property(t => t.SourcePath).HasColumnName("SourcePath");
            this.Property(t => t.TargetFields).HasColumnName("TargetFields");
            this.Property(t => t.TargetPaths).HasColumnName("TargetPaths");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.RunOnMigration).HasColumnName("RunOnMigration");
        }
    }
}
