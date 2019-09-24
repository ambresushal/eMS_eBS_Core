using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignUserSettingMap :  EntityTypeConfiguration<FormDesignUserSetting>
    {
        public FormDesignUserSettingMap()
        {
            // Primary Key
           this.HasKey(t => t.FormDesignUserSettingID);

            // Properties
            this.Property(t => t.LevelAt)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Key)
                .IsRequired()
                .HasMaxLength(200);


            this.Property(t => t.Data)
                .IsRequired();                


            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

           this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FormDesignUserSetting", "fldr");
            this.Property(t => t.FormDesignUserSettingID).HasColumnName("FormDesignUserSettingID");
            this.Property(t => t.Key).HasColumnName("Key");
            this.Property(t => t.Data).HasColumnName("Data");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }

    }
}
