using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ApplicableTeamUserMapMap : EntityTypeConfiguration<ApplicableTeamUserMap>
    {
        public ApplicableTeamUserMapMap()
        {
            // Primary Key
            this.HasKey(t => t.ApplicableTeamUserMapID);
           
            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);
            // Table & Column Mappings

            this.ToTable("ApplicableTeamUserMap", "Fldr");
            this.Property(t => t.ApplicableTeamID).HasColumnName("ApplicableTeamID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.IsTeamManager).HasColumnName("IsTeamManager");
            this.Property(t => t.ApplicableTeamUserMapID).HasColumnName("ApplicableTeamUserMapID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");


            // Relationships
            this.HasRequired(t => t.ApplicableTeam)
                .WithMany(t => t.ApplicableTeamUserMaps)
                .HasForeignKey(d => d.ApplicableTeamID);

            this.HasRequired(t => t.User)
                .WithMany(t => t.ApplicableTeamUserMaps)
                .HasForeignKey(d => d.UserID);

        }



    }
}
