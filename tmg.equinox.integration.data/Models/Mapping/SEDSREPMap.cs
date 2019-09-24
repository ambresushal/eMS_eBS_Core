using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class SEDSREPMap : EntityTypeConfiguration<SEDSREP>
    {
        public SEDSREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.sese_id });

            // Properties
            this.Property(t => t.sese_id)
                .IsFixedLength()
                .HasMaxLength(255);

            this.Property(t => t.seds_desc)
                .IsFixedLength()
                .HasMaxLength(255);

            this.Property(t => t.seds_type)
                .IsFixedLength()
                .HasMaxLength(255);

            this.Property(t => t.sese_id_xlow)
                .IsFixedLength()
                .HasMaxLength(255);

            this.Property(t => t.seds_desc_xlow)
                .IsFixedLength()
                .HasMaxLength(255);

            this.Property(t => t.SEDS_MCTR_RPT_TYPE_NVL)
                .IsFixedLength()
                .HasMaxLength(255);

            this.Property(t => t.BatchID)
                .IsFixedLength()
                .HasMaxLength(100);

            this.Property(t => t.Hashcode)
                .IsFixedLength()
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("SEDSREP", "REP");
            this.Property(t => t.sese_id).HasColumnName("sese_id");
            this.Property(t => t.seds_desc).HasColumnName("seds_desc");
            this.Property(t => t.seds_type).HasColumnName("seds_type");
            this.Property(t => t.sese_id_xlow).HasColumnName("sese_id_xlow");
            this.Property(t => t.seds_desc_xlow).HasColumnName("seds_desc_xlow");
            this.Property(t => t.seds_lock_token).HasColumnName("seds_lock_token");
            this.Property(t => t.atxr_source_id).HasColumnName("atxr_source_id");
            this.Property(t => t.rec_creat_dt).HasColumnName("rec_creat_dt");
            this.Property(t => t.rec_updt_dt).HasColumnName("rec_updt_dt");
            this.Property(t => t.SEDS_MCTR_RPT_TYPE_NVL).HasColumnName("SEDS_MCTR_RPT_TYPE_NVL");

            this.Property(t => t.BatchID).HasColumnName("BatchID");
            //this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
