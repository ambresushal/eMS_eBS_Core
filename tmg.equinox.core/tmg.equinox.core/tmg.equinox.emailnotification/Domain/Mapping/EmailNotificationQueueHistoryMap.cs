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
    public class EmailNotificationQueueHistoryMap : EntityTypeConfiguration<EmailNotificationQueueHistory>
    {
        public EmailNotificationQueueHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("EmailNotificationQueueHistory", "Frmk");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.CreatedDateTime).HasColumnName("CreatedDateTime").IsRequired();
            this.Property(t => t.ToBeSendDateTime).HasColumnName("ToBeSendDateTime").IsRequired();
            this.Property(t => t.EmailSubject).HasColumnName("EmailSubject").IsRequired();
            this.Property(t => t.EmailBody).HasColumnName("EmailBody").IsRequired();
            this.Property(t => t.FromAddress).HasColumnName("FromAddress");
            this.Property(t => t.FromDisplayName).HasColumnName("FromDisplayName");
            this.Property(t => t.ToAddresses).HasColumnName("ToAddresses").IsRequired();
            this.Property(t => t.CCAddresses).HasColumnName("CCAddresses");
            this.Property(t => t.BCCAddresses).HasColumnName("BCCAddresses");
            this.Property(t => t.EmailImportance).HasColumnName("EmailImportance").IsRequired();
            this.Property(t => t.Attachments).HasColumnName("Attachments");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.SourceDescription).HasColumnName("SourceDescription");
            this.Property(t => t.IsHTML).HasColumnName("IsHTML");
            this.Property(t => t.SentDateTime).HasColumnName("SentDateTime").IsRequired();
            this.Property(t => t.Status).HasColumnName("Status").IsRequired();
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");

        }
    }
}
