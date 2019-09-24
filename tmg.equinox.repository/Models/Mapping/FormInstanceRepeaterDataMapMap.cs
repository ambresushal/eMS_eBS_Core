using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceRepeaterDataMapMap : EntityTypeConfiguration<FormInstanceRepeaterDataMap>
    {

        public FormInstanceRepeaterDataMapMap()
        {

            // Primary Key
            this.HasKey(t => t.FormInstanceRepeaterDataMapID);


            // Table & Column Mappings
            this.ToTable("FormInstanceRepeaterDataMap", "Fldr");
            this.Property(t => t.FormInstanceRepeaterDataMapID).HasColumnName("FormInstanceRepeaterDataMapID");
            this.Property(t => t.FormInstanceDataMapID).HasColumnName("FormInstanceDataMapID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.RepeaterUIElementID).HasColumnName("RepeaterUIElementID");
            this.Property(t => t.SectionID).HasColumnName("SectionID");
            this.Property(t => t.FullName).HasColumnName("FullName");
            this.Property(t => t.RepeaterData).HasColumnName("RepeaterData");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships           
            //this.HasOptional(t => t.FormInstanceDataMap)
            //    .WithMany(t => t.FormInstanceRepeaterDataMaps)
            //    .HasForeignKey(d => d.FormInstanceDataMapID);

            this.HasRequired<FormInstance>(s => s.FormInstance)
                        .WithMany(s => s.FormInstanceRepeaterDataMap2)
                        .HasForeignKey(s => s.FormInstanceID);


            this.HasRequired<FormInstanceDataMap>(s => s.FormInstanceDataMap)
                .WithMany(t => t.FormInstanceRepeaterDataMaps)
                .HasForeignKey(s => s.FormInstanceDataMapID);

        }
    }
}
