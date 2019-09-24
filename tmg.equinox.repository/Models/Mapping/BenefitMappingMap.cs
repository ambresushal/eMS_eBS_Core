using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class BenefitMappingMap : EntityTypeConfiguration<BenefitMapping>
    {
        public BenefitMappingMap()
        {
            this.HasKey(t => t.MappingID);

            this.ToTable("BenefitMapping", "ODM");

            this.Property(t => t.MappingID).HasColumnName("MappingID");
            this.Property(t => t.PBPFile).HasColumnName("PBPFile");
            this.Property(t => t.ColumnName).HasColumnName("ColumnName");
            this.Property(t => t.DataType).HasColumnName("DataType");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.FieldTitle).HasColumnName("FieldTitle");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Codes).HasColumnName("Codes");
            this.Property(t => t.Code_Values).HasColumnName("Code_Values");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.MappingType).HasColumnName("MappingType");
            this.Property(t => t.DocumentPath).HasColumnName("DocumentPath");
            this.Property(t => t.ElementType).HasColumnName("ElementType");
            this.Property(t => t.IsArrayElement).HasColumnName("IsArrayElement");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.ViewType).HasColumnName("ViewType");
            this.Property(t => t.SOTDocumentPath).HasColumnName("SOTDocumentPath");
            this.Property(t => t.SOTPrefix).HasColumnName("SOTPrefix");
            this.Property(t => t.SOTSuffix).HasColumnName("SOTSuffix");
            this.Property(t => t.IfBlankThenValue).HasColumnName("IfBlankThenValue");
            this.Property(t => t.IsYesNoField).HasColumnName("IsYesNoField");
            this.Property(t => t.IsCheckBothFields).HasColumnName("IsCheckBothFields");
            this.Property(t => t.SetSimilarValues).HasColumnName("SetSimilarValues");
            this.Property(t => t.SectionGeneratedName).HasColumnName("SectionGeneratedName");
        }
    }
}
