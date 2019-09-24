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
   public class FormInstanceProxyNumberMap : EntityTypeConfiguration<FormInstanceProxyNumber>
    {
       public FormInstanceProxyNumberMap(){
        // Primary Key
           this.HasKey(t => t.ProxyNumberID);

            // Properties
            this.ToTable("FormInstanceProxyNumber", "Fldr");
            this.Property(t => t.ProxyNumberID).HasColumnName("ProxyNumberID");
            this.Property(t => t.ProxyNumber).HasColumnName("ProxyNumber");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.IsUsed).HasColumnName("IsUsed");
       }
    }
}
