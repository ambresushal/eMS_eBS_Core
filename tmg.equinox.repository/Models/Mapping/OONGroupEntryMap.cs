using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class OONGroupEntryMap : EntityTypeConfiguration<OONGroupEntry>
    {
        public OONGroupEntryMap()
        {
            // Primary Key
            this.HasKey(t => t.OONGroupEntryID);

            // Properties
            this.Property(t => t.BenefitCode)
                .HasMaxLength(5);
            this.Property(t => t.BenefitGroup)
                .HasMaxLength(100);
            this.Property(t => t.BenefitName)
                .HasMaxLength(500);
            this.Property(t => t.BenefitType)
                .HasMaxLength(20);
            this.Property(t => t.FieldSubType)
                .HasMaxLength(20);
            this.Property(t => t.FieldType)
                .HasMaxLength(20);
            this.Property(t => t.IsActive);
            this.Property(t => t.Package)
                .HasMaxLength(10);
            this.Property(t => t.SOTFieldPath)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("OONGroupEntry", "ML");
            this.Property(t => t.OONGroupEntryID).HasColumnName("OONGroupEntryID");
            this.Property(t => t.BenefitCode).HasColumnName("BenefitCode");
            this.Property(t => t.BenefitGroup).HasColumnName("BenefitGroup");
            this.Property(t => t.BenefitName).HasColumnName("BenefitName");
            this.Property(t => t.BenefitType).HasColumnName("BenefitType");
            this.Property(t => t.FieldSubType).HasColumnName("FieldSubType");
            this.Property(t => t.FieldType).HasColumnName("FieldType");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Package).HasColumnName("Package");
            this.Property(t => t.SOTFieldPath).HasColumnName("SOTFieldPath");
            this.Property(t => t.FormDesignVersionId).HasColumnName("FormDesignVersionId");
        }
    }
}
