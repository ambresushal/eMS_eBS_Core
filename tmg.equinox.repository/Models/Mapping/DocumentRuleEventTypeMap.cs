using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentRuleEventTypeMap : EntityTypeConfiguration<DocumentRuleEventType>
    {
        public DocumentRuleEventTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentRuleEventTypeID);

            // Table & Column Mappings
            this.ToTable("DocumentRuleEventType", "UI");
            this.Property(t => t.DocumentRuleEventTypeCode).HasColumnName("DocumentRuleEventTypeCode");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
        }
    }
}
