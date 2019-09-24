using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.Models.Mapping
{
   public class PBPExportToMDBMappingMap : EntityTypeConfiguration<PBPExportToMDBMapping>
    {
        public PBPExportToMDBMappingMap()
        {
            this.HasKey(t => t.PBPExportToMDBMapping1Up);
            this.ToTable("PBPExportToMDBMapping", "PBP");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.JsonPath).HasColumnName("JsonPath");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsRepeater).HasColumnName("IsRepeater");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.MappingType).HasColumnName("MappingType");
            this.Property(t => t.IsBlankAllow).HasColumnName("IsBlankAllow");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
            this.Property(t => t.Year).HasColumnName("Year");
        }
    }
}
