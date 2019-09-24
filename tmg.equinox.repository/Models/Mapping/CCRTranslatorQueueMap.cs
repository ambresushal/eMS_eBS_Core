using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models.CCRTranslator;

namespace tmg.equinox.repository.Models.Mapping
{
    public class CCRTranslatorQueueMap : EntityTypeConfiguration<CCRTranslatorQueue>
    {
        public CCRTranslatorQueueMap()
        {
            this.HasKey(t => t.TranslatorQueue1Up);
            this.ToTable("TranslatorQueue", "Setup");

            this.Property(t => t.TranslatorQueue1Up).HasColumnName("TranslatorQueue1Up");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");
            this.Property(t => t.ConsortiumID).HasColumnName("ConsortiumID");
            this.Property(t => t.AccountID).HasColumnName("AccountID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FormInstanceName).HasColumnName("FormInstanceName");
            this.Property(t => t.ProductName).HasColumnName("ProductName");
            this.Property(t => t.FolderName).HasColumnName("FolderName");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.ProcessStatus1Up).HasColumnName("ProcessStatus1Up");
            this.Property(t => t.HasError).HasColumnName("HasError");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsProductNew).HasColumnName("IsProductNew");
            this.Property(t => t.GenerateNewProduct).HasColumnName("GenerateNewProduct");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.AccountName).HasColumnName("AccountName");
            this.Property(t => t.ConsortiumName).HasColumnName("ConsortiumName");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.ErrorDescription).HasColumnName("ErrorDescription");
            this.Property(t => t.IsRetro).HasColumnName("IsRetro");
            this.Property(t => t.IsShell).HasColumnName("IsShell");
            this.Property(t => t.ProductName).HasColumnName("ProductName");
        }
    }
}
