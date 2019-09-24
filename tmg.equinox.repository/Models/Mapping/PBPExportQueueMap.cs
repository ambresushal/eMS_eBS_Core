using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPExportQueueMap : EntityTypeConfiguration<PBPExportQueue>
    {
        public PBPExportQueueMap()
        {
            this.HasKey(t => t.PBPExportQueueID);
            this.ToTable("PBPExportQueue", "Setup");
            this.Property(t => t.PBPExportQueueID).HasColumnName("PBPExportQueueID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ExportName).HasColumnName("ExportName");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.ExportedDate).HasColumnName("ExportedDate");
            this.Property(t => t.ExportedBy).HasColumnName("ExportedBy");
            this.Property(t => t.ExportStartDate).HasColumnName("ExportStartDate");
            this.Property(t => t.ExportEndDate).HasColumnName("ExportEndDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.PlanYear).HasColumnName("PlanYear");
            this.Property(t => t.PBPFilePath).HasColumnName("PBPFilePath");
            this.Property(t => t.PlanAreaFilePath).HasColumnName("PlanAreaFilePath");
            this.Property(t => t.VBIDFilePath).HasColumnName("VBIDFilePath");
            this.Property(t => t.ZipFilePath).HasColumnName("ZipFilePath");
            this.Property(t => t.MDBSchemaPath).HasColumnName("MDBSchemaPath");
            this.Property(t => t.PBPDatabase1Up).HasColumnName("PBPDatabase1Up");
            this.Property(t => t.ImportStatus).HasColumnName("ImportStatus");
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
            this.Property(t => t.JobId).HasColumnName("JobId");
        }
    }
}
