using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class BenefitsDictionaryMap : EntityTypeConfiguration<BenefitsDictionary>
    {
        public BenefitsDictionaryMap() {

            this.HasKey(t => t.ID);

            this.ToTable("BenefitsDictionary", "ODM");

            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FILE).HasColumnName("FILE");
            this.Property(t => t.NAME).HasColumnName("NAME");
            this.Property(t => t.TYPE).HasColumnName("TYPE");
            this.Property(t => t.LENGTH).HasColumnName("LENGTH");
            this.Property(t => t.FIELD_TITLE).HasColumnName("FIELD_TITLE");
            this.Property(t => t.TITLE).HasColumnName("TITLE");
            this.Property(t => t.Codes).HasColumnName("Codes");
            this.Property(t => t.CODE_VALUES).HasColumnName("CODE_VALUES");
            this.Property(t => t.YEAR).HasColumnName("YEAR");
            this.Property(t => t.MappingID).HasColumnName("MappingID");

            // Relationships
            this.HasRequired(t => t.BenefitMapping)
                .WithMany(t => t.BenefitsDictionaries)
                .HasForeignKey(d => d.MappingID);
        }
    }
}
