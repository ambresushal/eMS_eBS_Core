using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportingTableColumnInfoMap : EntityTypeConfiguration<ReportingTableColumnInfo>
    {
        public ReportingTableColumnInfoMap()
        {

            // Primary Key
            this.HasKey(t => t.ID);


            // Table & Column Mappings
            this.ToTable("ReportingTableColumnInfo","Rpt");
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.ReportingTableInfo_ID).HasColumnName("ReportingTableInfo_ID").IsRequired();
            this.Property(t => t.Name).HasColumnName("Name").IsRequired();
            this.Property(t => t.DataType).HasColumnName("DataType").IsRequired();
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.isNullable).HasColumnName("IsNullable");
            this.Property(t => t.IsPrimaryKey).HasColumnName("IsPrimaryKey");
            this.Property(t => t.isUnique).HasColumnName("IsUnique");
            this.Property(t => t.IsIdentity).HasColumnName("IsIdentity");
            this.Property(t => t.IdentityIncrement).HasColumnName("IdentityIncrement");
            this.Property(t => t.IdentitySeed).HasColumnName("IdentitySeed");
            this.Property(t => t.isForiegnKey).HasColumnName("IsForiegnKey");
            this.Property(t => t.ForiegnKeyTableName).HasColumnName("ForiegnKeyTableName");
            this.Property(t => t.ForiegnKeyColumnName).HasColumnName("ForiegnKeyColumnName");
            this.Property(t => t.valuePath).HasColumnName("valuePath");
            this.Property(t => t.ReferenceTable).HasColumnName("ReferenceTable");
            this.Property(t => t.CustomType).HasColumnName("CustomType");
            this.Property(t => t.Label).HasColumnName("Label");
            this.Property(t => t.Description).HasColumnName("Description");

            //////// Relationships
            this.HasRequired<ReportingTableInfo>(t => t.ReportingTableInfo)
                .WithMany(r => r.Columns)
                .HasForeignKey<int>(t => t.ReportingTableInfo_ID).WillCascadeOnDelete(false); 
        }
    }
}
