using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormReferenceMapMap : EntityTypeConfiguration<FormReferenceMap>
    {
        public FormReferenceMapMap()
        {
            // Primary Key
            this.HasKey(t => t.FormReferenceID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);


            // Table & Column Mappings
            this.ToTable("FormReferenceMap", "Fldr");
            this.Property(t => t.FormReferenceID).HasColumnName("FormReferenceID");
            this.Property(t => t.ReferenceAccountID).HasColumnName("ReferenceAccountID");
            this.Property(t => t.ReferenceFolderID).HasColumnName("ReferenceFolderID");
            this.Property(t => t.ReferenceFolderVersionID).HasColumnName("ReferenceFolderVersionID");
            this.Property(t => t.ReferenceFormInstanceID).HasColumnName("ReferenceFormInstanceID");
            this.Property(t => t.ReferenceConsortiumID).HasColumnName("ReferenceConsortiumID");
            this.Property(t => t.TargetFormInstanceID).HasColumnName("TargetFormInstanceID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");

        }
    }
}
