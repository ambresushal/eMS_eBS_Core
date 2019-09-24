using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DocumentUpdateTrackerMap : EntityTypeConfiguration<DocumentUpdateTracker>
    {
        public DocumentUpdateTrackerMap()
        {
            // Primary Key
            this.HasKey(t => t.ForminstanceID);

            // Properties
            this.Property(t => t.Status);

            this.Property(t => t.OldJsonHash);

            this.Property(t => t.CurrentJsonHash);

            this.Property(t => t.AddedDate);

            this.Property(t => t.UpdatedDate);

            // Table & Column Mappings
            this.ToTable("DocumentUpdateTracker", "MDM");
            this.Property(t => t.ForminstanceID).HasColumnName("ForminstanceID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.OldJsonHash).HasColumnName("OldJsonHash");
            this.Property(t => t.CurrentJsonHash).HasColumnName("CurrentJsonHash");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }
    }

    public class SchemaUpdateTrackerMap : EntityTypeConfiguration<SchemaUpdateTracker>
    {
        public SchemaUpdateTrackerMap()
        {
            // Primary Key
            this.HasKey(t => t.SchemaUpdateTrackerID);

            // Properties
            //this.Property(t => t.FormdesignID);
            //this.Property(t => t.FormdesignVersionID);
            //this.Property(t => t.Status);

            //this.Property(t => t.OldJsonHash);

            //this.Property(t => t.CurrentJsonHash);

            //this.Property(t => t.AddedDate);
            //this.Property(t => t.UpdatedDate);

            // Table & Column Mappings
            this.ToTable("SchemaUpdateTracker", "MDM");
            this.Property(t => t.SchemaUpdateTrackerID).HasColumnName("SchemaUpdateTrackerID");
            this.Property(t => t.FormdesignID).HasColumnName("FormdesignID");
            this.Property(t => t.FormdesignVersionID).HasColumnName("FormdesignVersionID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.OldJsonHash).HasColumnName("OldJsonHash");
            this.Property(t => t.CurrentJsonHash).HasColumnName("CurrentJsonHash");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }
    }

    public class MDMLogMap : EntityTypeConfiguration<MDMLog>
    {
        public MDMLogMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);
            this.Property(t => t.ForminstanceID);

            this.Property(t => t.FormDesignID);

            this.Property(t => t.FormDesignVersionID);

            this.Property(t => t.ErrorDescription);
            this.Property(t => t.Error);

            this.Property(t => t.AddedDate);

            // Table & Column Mappings
            this.ToTable("Log", "MDM");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.ForminstanceID).HasColumnName("ForminstanceID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.ErrorDescription).HasColumnName("ErrorDescription");
            this.Property(t => t.Error).HasColumnName("Error");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
        }
    }
}
