using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ApplicableTeamMap : EntityTypeConfiguration<ApplicableTeam>
    {
        public ApplicableTeamMap()
        {

            //Primary key
            this.HasKey(t => t.ApplicableTeamID);

            // Properties
            this.Property(t => t.ApplicableTeamName)
                .IsRequired()
                .HasMaxLength(200);

            this.ToTable("ApplicableTeam", "Fldr");
            this.Property(t => t.ApplicableTeamID).HasColumnName("ApplicableTeamID");
            this.Property(t => t.ApplicableTeamName).HasColumnName("ApplicableTeamName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");



        }
    }
}
