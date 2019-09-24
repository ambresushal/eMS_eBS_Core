using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class EmailTemplatePlaceHolderMappingMap : EntityTypeConfiguration<EmailTemplatePlaceHolderMapping>      
    {
        public EmailTemplatePlaceHolderMappingMap()
    {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("EmailTemplatePlaceHolderMapping", "Frmk");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.EmailTemplateId).HasColumnName("EmailTemplateId").IsRequired();
            this.Property(t => t.PlaceHolderId).HasColumnName("PlaceHolderId").IsRequired();
            this.Property(t => t.Description).HasColumnName("Description");

            //////// Relationships
            this.HasRequired<EmailTemplate>(t => t.Template)
                .WithMany(r => r.PlaceHolders)
                .HasForeignKey<int>(t => t.EmailTemplateId).WillCascadeOnDelete(false);

            //////// Relationships
            this.HasRequired<EmailTemplatePlaceHolder>(t => t.PlaceHolder)
                .WithMany(r => r.PlaceHolders)
                .HasForeignKey<int>(t => t.PlaceHolderId).WillCascadeOnDelete(false);
        }
}
}
