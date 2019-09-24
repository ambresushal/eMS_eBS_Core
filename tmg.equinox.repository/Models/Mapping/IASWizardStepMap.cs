using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class IASWizardStepMap : EntityTypeConfiguration<IASWizardStep>
    {

        public IASWizardStepMap()
        {
            // Primary Key
            this.HasKey(t => t.IASWizardStepID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("IASWizardStep", "GU");
            this.Property(t => t.IASWizardStepID).HasColumnName("IASWizardStepID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            

            // Relationships
            
        }
    }
}
