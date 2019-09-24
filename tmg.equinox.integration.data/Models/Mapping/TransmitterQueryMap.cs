using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class TransmitterQueryMap : EntityTypeConfiguration<TransmitterQuery>
    {
        public TransmitterQueryMap()
        {
            this.HasKey(t => t.TransmitterQuery1Up);
            this.ToTable("TransmitterQuery", "Temp");
            this.Property(t => t.TransmitterQuery1Up).HasColumnName("TransmitterQuery1Up");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");
            this.Property(t => t.TranslatorGovernance1Up).HasColumnName("TranslatorGovernance1Up");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.QueryType).HasColumnName("QueryType");
            this.Property(t => t.Component).HasColumnName("Component");
            this.Property(t => t.SqlQuery).HasColumnName("SqlQuery");
            this.Property(t => t.ProductSequenceNo).HasColumnName("ProductSequenceNo");            
        }
    }
}
