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
    public class EmailTemplatePlaceHolderMap : EntityTypeConfiguration<EmailTemplatePlaceHolder>       
    {
        public EmailTemplatePlaceHolderMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("EmailTemplatePlaceHolder", "Frmk");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.PlaceHolder).HasColumnName("PlaceHolder").IsRequired();
        }
    }
}
