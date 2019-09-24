using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignDataPathMap : EntityTypeConfiguration<FormDesignDataPath>
    {
        public FormDesignDataPathMap()
        {

            // Primary Key
            this.HasKey(t => t.FormDesignDataPathID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Path)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.AddedDate)
                .IsRequired();

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FormDesignDataPath", "Rpt");
            this.Property(t => t.FormDesignDataPathID).HasColumnName("FormDesignDataPathID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.FormDesignDataPaths)
                .HasForeignKey(d => d.TenantID);

            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.FormDesignDataPaths)
                .HasForeignKey(d => d.FormDesignID);

            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.FormDesignDataPaths)
                .HasForeignKey(d => d.FormDesignVersionID);
        }

    }
}
