using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;
namespace tmg.equinox.repository.Models.Mapping
{
   public class CopyFromAuditTrailMap:EntityTypeConfiguration<CopyFromAuditTrail>
    {
       public CopyFromAuditTrailMap(){
        // Primary Key
           this.HasKey(t => t.CopyFromAuditTrailID);

            // Properties
            this.ToTable("CopyFromAuditTrail", "Fldr");
            this.Property(t => t.CopyFromAuditTrailID).HasColumnName("CopyFromAuditTrailID");
            this.Property(t => t.DestinationDocumentID).HasColumnName("DestinationDocumentID");
            this.Property(t => t.AccountID).HasColumnName("AccountID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.SourceFolderVersionID).HasColumnName("SourceFolderVersionID");
            this.Property(t => t.DestinationDocumentID).HasColumnName("DestinationDocumentID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.SourceDocumentID).HasColumnName("SourceDocumentID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");        
           
       }
    }
}
