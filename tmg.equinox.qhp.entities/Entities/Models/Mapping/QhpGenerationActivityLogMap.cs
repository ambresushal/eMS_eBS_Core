using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.qhp.entities.Entities.Models.Mapping
{
    public class QhpGenerationActivityLogMap : EntityTypeConfiguration<QhpGenerationActivityLog>
    {
        public QhpGenerationActivityLogMap()
        {
            // Primary Key
            this.HasKey(t => t.QhpActivityLogID);

            // Properties
            this.Property(t => t.Category)
                .HasMaxLength(50);

            this.Property(t => t.Event)
                .HasMaxLength(100);

            this.Property(t => t.UserName)
                .HasMaxLength(50);

            this.Property(t => t.Host)
                .HasMaxLength(50);

            this.Property(t => t.URI)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("QhpGenerationActivityLog", "Qhp");
            this.Property(t => t.QhpActivityLogID).HasColumnName("QhpActivityLogID");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.Event).HasColumnName("Event");
            this.Property(t => t.TimeUtc).HasColumnName("TimeUtc");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Host).HasColumnName("Host");
            this.Property(t => t.URI).HasColumnName("URI");
            this.Property(t => t.Message).HasColumnName("Message");
        }
    }
}
