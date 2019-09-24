using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class SEDSDescriptionsMasterMap : EntityTypeConfiguration<SEDSDescriptionsMaster>
    {
        public SEDSDescriptionsMasterMap()
        {
            this.HasKey(t => t.SESE_ID );

            this.ToTable("SEDS_Descriptions", "Master");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitCategory1).HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.PlaceOfService).HasColumnName("POS");
            this.Property(t => t.isActive).HasColumnName("isActive");
        }
    }
}
