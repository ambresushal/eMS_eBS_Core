using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPSoftwareVersionMap: EntityTypeConfiguration<PBPSoftwareVersion>
    {
        public PBPSoftwareVersionMap()
        {
            this.HasKey(t => t.PBPSoftwareVersion1Up);

            this.ToTable("PBPSoftwareVersion", "PBP");
            this.Property(t => t.PBPSoftwareVersion1Up).HasColumnName("PBPSoftwareVersion1Up");
            this.Property(t => t.PBPSoftwareVersionName).HasColumnName("PBPSoftwareVersionName");
            this.Property(t => t.TestQaVesrion).HasColumnName("TestQaVesrion");
            this.Property(t => t.IsLicenseVersion).HasColumnName("IsLicenseVersion");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
