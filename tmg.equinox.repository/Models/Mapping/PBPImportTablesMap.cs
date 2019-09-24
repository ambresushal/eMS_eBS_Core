using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPImportTablesMap : EntityTypeConfiguration<PBPImportTables>
    {
        public PBPImportTablesMap()
        {
           
            this.ToTable("PBPImportTables", "Setup");
            this.Property(t => t.PBPTableID).HasColumnName("PBPTableID");
            this.Property(t => t.PBPTableName).HasColumnName("PBPTableName");
            this.Property(t => t.PBPTableSequence).HasColumnName("PBPTableSequence");
            this.Property(t => t.EBSTableName).HasColumnName("EBSTableName");
            this.HasKey(t => t.PBPTableID);
        }
    }
}
