using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPImportQueueMap : EntityTypeConfiguration<PBPImportQueue>
    {
        public PBPImportQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.PBPImportQueueID);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(400);

            this.Property(t => t.PBPFileName)
                .HasMaxLength(100);

            this.Property(t => t.PBPPlanAreaFileName)
               .HasMaxLength(100);

            this.Property(t => t.Location)
                .HasMaxLength(400);
            this.Property(t => t.PBPDataBase)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("PBPImportQueue", "Setup");
            this.Property(t => t.PBPImportQueueID).HasColumnName("PBPImportQueueID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.PBPFileName).HasColumnName("PBPFileName");
            this.Property(t => t.PBPPlanAreaFileName).HasColumnName("PBPPlanAreaFileName");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.PBPDatabase1Up).HasColumnName("PBPDatabase1Up");
            this.Property(t => t.ImportStartDate).HasColumnName("ImportStartDate");
            this.Property(t => t.ImportEndDate).HasColumnName("ImportEndDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.PBPDataBase).HasColumnName("PBPDataBase");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsFullMigration).HasColumnName("IsFullMigration");
            this.Property(t => t.JobId).HasColumnName("JobId");
            this.Property(t => t.JobLocation).HasColumnName("JobLocation");
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
            this.Property(t => t.ImportStatus).HasColumnName("ImportStatus");
            this.Property(t => t.PBPFileDisplayName).HasColumnName("PBPFileDisplayName");
            this.Property(t => t.PBPPlanAreaFileDisplayName).HasColumnName("PBPPlanAreaFileDisplayName");
        }
    }
}
