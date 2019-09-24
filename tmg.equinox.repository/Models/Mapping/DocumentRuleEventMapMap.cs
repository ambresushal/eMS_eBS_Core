using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentRuleEventMapMap : EntityTypeConfiguration<DocumentRuleEventMap>
    {
        public DocumentRuleEventMapMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentRuleEventMapID);

            // Table & Column Mappings
            this.ToTable("DocumentRuleEventMap", "UI");
            this.Property(t => t.DocumentRuleID).HasColumnName("DocumentRuleID");
            this.Property(t => t.DocumentRuleEventTypeID).HasColumnName("DocumentRuleEventTypeID");
        }

    }
}
