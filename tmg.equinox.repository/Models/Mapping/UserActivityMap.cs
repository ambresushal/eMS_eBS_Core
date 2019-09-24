using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class UserActivityMap : EntityTypeConfiguration<UserActivity>
    {
        public UserActivityMap()
        {
            // Primary Key
            this.HasKey(t => t.EventID);

            // Properties
            this.Property(t => t.Category)
                .HasMaxLength(10);

            this.Property(t => t.Event)
                .HasMaxLength(20);

            this.Property(t => t.UserName)
                .HasMaxLength(50);

            this.Property(t => t.Host)
                .HasMaxLength(50);

            this.Property(t => t.RequestUrl)
                .HasMaxLength(500);

            this.Property(t => t.AppDomain)
                .HasMaxLength(50);

            this.Property(t => t.UserAgent)
                .HasMaxLength(20);

            this.Property(t => t.Severity)
                .HasMaxLength(20);

            this.Property(t => t.Message)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("UserActivity", "Sec");
            this.Property(t => t.EventID).HasColumnName("EventID");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.Event).HasColumnName("Event");
            this.Property(t => t.TimeUtc).HasColumnName("TimeUtc");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Host).HasColumnName("Host");
            this.Property(t => t.RequestUrl).HasColumnName("RequestUrl");
            this.Property(t => t.AppDomain).HasColumnName("AppDomain");
            this.Property(t => t.UserAgent).HasColumnName("UserAgent");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.Severity).HasColumnName("Severity");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.Message).HasColumnName("Message");
        }
    }
}
