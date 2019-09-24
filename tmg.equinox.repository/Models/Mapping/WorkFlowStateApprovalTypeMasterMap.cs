﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowStateApprovalTypeMasterMap : EntityTypeConfiguration<WorkFlowStateApprovalTypeMaster>
    {
        public WorkFlowStateApprovalTypeMasterMap()
        {
            // Primary Key
            this.HasKey(t => t.WorkFlowStateApprovalTypeID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.WorkFlowStateApprovalTypeName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("StateApprovalType", "WF");
            this.Property(t => t.WorkFlowStateApprovalTypeID).HasColumnName("WorkFlowStateApprovalTypeID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.WorkFlowStateApprovalTypeName).HasColumnName("WorkFlowStateApprovalTypeName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.WorkFlowStateApprovalTypeMaster)
                .HasForeignKey(d => d.TenantID);

        }
    }
}
