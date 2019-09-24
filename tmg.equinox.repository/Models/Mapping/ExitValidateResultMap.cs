using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ExitValidateResultMap : EntityTypeConfiguration<ExitValidateResult>
    {
        public ExitValidateResultMap()
        {
            // Primary Key
            this.HasKey(t => t.ExitValidateResultID);

            // Properties
            this.Property(t => t.ExitValidateQueueID)
                .IsRequired();
            this.Property(t => t.FormInstanceID)
                .IsRequired();
            this.Property(t => t.ContractNumber)
                .HasMaxLength(5);
            this.Property(t => t.PlanName)
                .HasMaxLength(200);
            this.Property(t => t.Section)
                .HasMaxLength(500);
            this.Property(t => t.Status)
                .HasMaxLength(50);
            this.Property(t => t.Result)
                .HasMaxLength(50);
            this.Property(t => t.Error)
                .HasMaxLength(5000);
            this.Property(t => t.Question)
                .HasMaxLength(5000);
            this.Property(t => t.Screen)
                .HasMaxLength(5000);
            this.Property(t => t.PBPFIELD)
                .HasMaxLength(200);
            this.Property(t => t.PBPCOLUMN)
                .HasMaxLength(200);
            // Table & Column Mappings
            this.ToTable("ExitValidateResult", "Setup");
            this.Property(t => t.ExitValidateResultID).HasColumnName("ExitValidateResultID");
            this.Property(t => t.ExitValidateQueueID).HasColumnName("ExitValidateQueueID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.ContractNumber).HasColumnName("ContractNumber");
            this.Property(t => t.PlanName).HasColumnName("PlanName");
            this.Property(t => t.Section).HasColumnName("Section");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.Error).HasColumnName("Error");
            this.Property(t => t.Question).HasColumnName("Question");
            this.Property(t => t.Screen).HasColumnName("Screen");
            this.Property(t => t.PBPFIELD).HasColumnName("PBPFIELD");
            this.Property(t => t.PBPCOLUMN).HasColumnName("PBPCOLUMN");
        }
    }
}
