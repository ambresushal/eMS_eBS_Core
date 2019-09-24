using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ApiActivityLogMap : EntityTypeConfiguration<ApiActivityLog>
    {
        public ApiActivityLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ApiActivityLogID);

            // Table & Column Mappings
            this.ToTable("ApiActivityLog", "LOG");
            this.Property(t => t.ApiActivityLogID).HasColumnName("ApiActivityLogID");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Machine).HasColumnName("Machine");
            this.Property(t => t.RequestIpAddress).HasColumnName("RequestIpAddress");
            this.Property(t => t.RequestContentType).HasColumnName("RequestContentType");
            this.Property(t => t.RequestContentBody).HasColumnName("RequestContentBody");
            this.Property(t => t.RequestUri).HasColumnName("RequestUri");
            this.Property(t => t.RequestMethod).HasColumnName("RequestMethod");
            this.Property(t => t.RequestRouteData).HasColumnName("RequestRouteData");
            this.Property(t => t.RequestHeaders).HasColumnName("RequestHeaders");
            this.Property(t => t.RequestDateTime).HasColumnName("RequestDateTime");
            this.Property(t => t.ResponseContentType).HasColumnName("ResponseContentType");
            this.Property(t => t.ResponseContentBody).HasColumnName("ResponseContentBody");
            this.Property(t => t.ResponseStatusCode).HasColumnName("ResponseStatusCode");
            this.Property(t => t.ResponseHeaders).HasColumnName("ResponseHeaders");
            this.Property(t => t.ResponseDateTime).HasColumnName("ResponseDateTime");

        }
    }
}
