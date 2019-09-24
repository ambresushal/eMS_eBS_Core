using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportCoveredServicesMap : EntityTypeConfiguration<ReportCoveredServices>
    {
        public ReportCoveredServicesMap()
        {
             //Primary Key Column
            this.HasKey(t => t.ID);

            // Restricted Length Columns
            //this.Property(t => t.AddedBy)
            //    .IsRequired()
            //    .HasMaxLength(20);

            //this.Property(t => t.UpdatedBy)
            //    .HasMaxLength(20);

            // Configuring the Table name that this entity is mapped to.
            this.ToTable("ReportCoveredServices", "Rpt");

            // Configuring Name of DB columns to map Entity properties 
            this.Property(t => t.ServiceID).HasColumnName("ServiceID");
            this.Property(t => t.BenefitCategory1).IsRequired().HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).IsRequired().HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.OfficeVisitCategory).IsRequired().HasColumnName("OfficeVisitCategory");
        }
    }
}
