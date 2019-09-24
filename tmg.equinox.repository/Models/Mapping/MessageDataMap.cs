using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
namespace tmg.equinox.repository.Models.Mapping
{
   public class MessageDataMap : EntityTypeConfiguration<MessageData>
    {

        public MessageDataMap()
        {
            this.HasKey(t => t.MessageID);
            this.ToTable("NoticationMessage", "Setup");           
            this.Property(t => t.MessageKey).HasColumnName("MessageKey");
            this.Property(t => t.MessageText).HasColumnName("MessageText");
            this.Property(t => t.MessageType).HasColumnName("MessageType");
        }
    }
}
