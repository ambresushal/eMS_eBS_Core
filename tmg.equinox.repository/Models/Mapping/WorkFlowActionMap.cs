using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowActionMap : EntityTypeConfiguration<WorkFlowAction>
    {
        public WorkFlowActionMap()
        {
            // Primary Key
            this.HasKey(t => t.ActionID);

            // Table & Column Mappings
            this.ToTable("Action", "WF");
            this.Property(t => t.ActionID).HasColumnName("ActionID");
            this.Property(t => t.ActionName).HasColumnName("ActionName");
        }
    }
}
