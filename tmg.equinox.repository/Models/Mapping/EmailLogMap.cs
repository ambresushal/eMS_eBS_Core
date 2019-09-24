using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class EmailLogMap : EntityTypeConfiguration<EmailLog>
    {
        public EmailLogMap()
        {
            //Primary key
            this.HasKey(t => t.EmailLogID);

            // Properties
            this.Property(t => t.To)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.Cc)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.Bcc)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.Comments)
                .IsRequired()
                .HasMaxLength(2000);


            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.EmailContent)
                .IsRequired()
                .HasMaxLength(Int32.MaxValue);

            this.ToTable("EmailLog", "Fldr");
            this.Property(t => t.EmailLogID).HasColumnName("EmailLogID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersionStateID).HasColumnName("FolderVersionStateID");
            this.Property(t => t.To).HasColumnName("To");
            this.Property(t => t.Cc).HasColumnName("Cc");
            this.Property(t => t.Bcc).HasColumnName("Bcc");
            this.Property(t => t.EmailContent).HasColumnName("EmailContent");
            this.Property(t => t.FolderEffectiveDate).HasColumnName("FolderEffectiveDate");
            this.Property(t => t.ApprovedWorkFlowStateID).HasColumnName("ApprovedWorkFlowStateID");
            this.Property(t => t.CurrentWorkFlowStateID).HasColumnName("CurrentWorkFlowStateID");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.EmailSentDateTime).HasColumnName("EmailSentDateTime");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");

            //Relationships
            this.HasOptional(t => t.Account)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.AccountID);

            this.HasRequired(t => t.Folder)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.FolderID);

            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.FolderVersionID);

            this.HasRequired(t => t.FolderVersionState)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.FolderVersionStateID);

            this.HasRequired(t => t.WorkFlowState)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.ApprovedWorkFlowStateID);

            this.HasRequired(t => t.WorkFlowState)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.CurrentWorkFlowStateID);

            this.HasRequired(t => t.User)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.UserID);

            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(d => d.TenantID);
        }
    }
}


