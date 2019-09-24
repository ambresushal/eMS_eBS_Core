using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ExpressionGuMap : EntityTypeConfiguration<ExpressionGu>
    {
        public ExpressionGuMap()
        {
            // Primary Key
            this.HasKey(t => t.ExpressionID);

            // Properties
            this.Property(t => t.RightOperand)
                .HasMaxLength(500);

            this.Property(t => t.LeftOperand)
                .HasMaxLength(500);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("Expression", "GU");
            this.Property(t => t.ExpressionID).HasColumnName("ExpressionID");
            this.Property(t => t.LeftOperand).HasColumnName("LeftOperand");
            this.Property(t => t.RightOperand).HasColumnName("RightOperand");
            this.Property(t => t.OperatorTypeID).HasColumnName("OperatorTypeID");
            this.Property(t => t.LogicalOperatorTypeID).HasColumnName("LogicalOperatorTypeID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.RuleID).HasColumnName("RuleID");
            this.Property(t => t.ParentExpressionID).HasColumnName("ParentExpressionID");
            this.Property(t => t.ExpressionTypeID).HasColumnName("ExpressionTypeID");
            this.Property(t => t.IsRightOperandElement).HasColumnName("IsRightOperandElement");
            // Relationships
            this.HasOptional(t => t.Expression2)
                .WithMany(t => t.Expression1)
                .HasForeignKey(d => d.ParentExpressionID);

            this.HasOptional(t => t.ExpressionType)
                .WithMany(t => t.ExpressionsGu)
                .HasForeignKey(d => d.ExpressionTypeID);

            this.HasRequired(t => t.LogicalOperatorType)
                .WithMany(t => t.ExpressionsGu)
                .HasForeignKey(d => d.LogicalOperatorTypeID);

            this.HasRequired(t => t.OperatorType)
                .WithMany(t => t.ExpressionsGu)
                .HasForeignKey(d => d.OperatorTypeID);

            this.HasRequired(t => t.Rule)
                .WithMany(t => t.ExpressionsGu)
                .HasForeignKey(d => d.RuleID);

        }
    }
}
