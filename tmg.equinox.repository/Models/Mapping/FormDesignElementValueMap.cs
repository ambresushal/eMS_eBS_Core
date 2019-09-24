using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{

    public class FormDesignElementValueMap : EntityTypeConfiguration<FormDesignElementValue>
    {

        public FormDesignElementValueMap()
        {


            // Primary Key
            this.HasKey(t => t.FormDesignElementValueID);

            // Properties
            this.Property(t => t.ElementFullPath)
                .HasMaxLength(2000);

            this.Property(t => t.NewValue)
                .HasMaxLength(2000);

            this.Property(t => t.AddedBy)
                .HasMaxLength(50);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);

            this.ToTable("FormDesignElementValue", "GU");
            this.Property(t => t.FormDesignElementValueID).HasColumnName("FormDesignElementValueID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.ElementFullPath).HasColumnName("ElementFullPath");
            this.Property(t => t.IsUpdated).HasColumnName("IsUpdated");
            this.Property(t => t.NewValue).HasColumnName("NewValue");
            this.Property(t => t.ElementHeaderName).HasColumnName("ElementHeaderName");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.FormDesignElementValues)
                .HasForeignKey(d => d.FormDesignID);

            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.FormDesignElementValues)
                .HasForeignKey(d => d.FormDesignVersionID);

            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.FormDesignElementValues)
                .HasForeignKey(d => d.GlobalUpdateID);

            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.FormDesignElementValues)
                .HasForeignKey(d => d.UIElementID);
        }

    }
}
