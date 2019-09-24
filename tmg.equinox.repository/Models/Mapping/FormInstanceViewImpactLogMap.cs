using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceViewImpactLogMap : EntityTypeConfiguration<FormInstanceViewImpactLog>
    {
        public FormInstanceViewImpactLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ImpactLoggerID);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("FormInstanceViewImpactLog", "Fldr");
            this.Property(t => t.ImpactLoggerID).HasColumnName("ImpactLoggerID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.ElementID).HasColumnName("ElementID");
            this.Property(t => t.ElementName).HasColumnName("ElementName");
            this.Property(t => t.ElementLabel).HasColumnName("ElementLabel");
            this.Property(t => t.ElementPath).HasColumnName("ElementPath");
            this.Property(t => t.ElementPathName).HasColumnName("ElementPathName");
            this.Property(t => t.Keys).HasColumnName("Keys");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.UpdatedLast).HasColumnName("UpdatedLast");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.ImpactedElements).HasColumnName("ImpactedElements");
            this.Property(t => t.DocID).HasColumnName("DocID");

            //// Relationships
            //this.HasRequired(t => t.FolderVersion)
            //    .WithMany(t => t.FormInstanceActivityLogs)
            //    .HasForeignKey(d => d.FolderVersionID);

            //this.HasRequired(t => t.FormDesign)
            //    .WithMany(t => t.FormInstanceActivityLogs)
            //    .HasForeignKey(d => d.FormDesignID);

            //this.HasRequired(t => t.FormDesignVersion)
            //    .WithMany(t => t.FormInstanceActivityLogs)
            //    .HasForeignKey(d => d.FormDesignVersionID);

            //this.HasRequired(t => t.Folder)
            //    .WithMany(t => t.FormInstanceActivityLogs)
            //    .HasForeignKey(d => d.FolderID);

            this.HasRequired(t => t.FormInstance)
                .WithMany(t => t.FormInstanceViewImpactLog)
               .HasForeignKey(d => d.FormInstanceID);
        }
    }
}
