using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ComplexOperatorMap : EntityTypeConfiguration<ComplexOperator>
    {
        public ComplexOperatorMap()
        {
            // Primary Key
            this.HasKey(t => t.ComplexOperatorID);

            // Properties
            this.Property(t => t.Factor)
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("ComplexOperator", "UI");
            this.Property(t => t.ComplexOperatorID).HasColumnName("ComplexOperatorID");
            this.Property(t => t.ExpressionID).HasColumnName("ExpressionID");
            this.Property(t => t.OperatorID).HasColumnName("OperatorID");
            this.Property(t => t.Factor).HasColumnName("Factor");
            this.Property(t => t.FactorValue).HasColumnName("FactorValue");
        }
    }
}
