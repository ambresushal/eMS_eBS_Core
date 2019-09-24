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
    public class EmailTemplateMap : EntityTypeConfiguration<EmailTemplate>    
    {
        public EmailTemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("EmailTemplate", "Frmk");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.TemplateName).HasColumnName("TemplateName").IsRequired();
            this.Property(t => t.TemplateType).HasColumnName("TemplateType").IsRequired();
            this.Property(t => t.EmailSubject).HasColumnName("EmailSubject").IsRequired();
            this.Property(t => t.EmailContent).HasColumnName("EmailContent").IsRequired();
            this.Property(t => t.IsHTML).HasColumnName("IsHTML").IsRequired();
        }
    }
}
