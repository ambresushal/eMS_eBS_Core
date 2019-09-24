using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListImportMap : EntityTypeConfiguration<MasterListImport>
    {
        public MasterListImportMap()
        {
            // Primary Key
            this.HasKey(t => t.MLImportID);

            this.ToTable("MasterListImport", "Fldr");
            this.Property(t => t.MLImportID).HasColumnName("MLImportID");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.FilePath).HasColumnName("FilePath");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.Status).HasColumnName("Status");

        }

    }
}
