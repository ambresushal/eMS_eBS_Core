using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ACDESRCMap : EntityTypeConfiguration<ACDESRC>
    {
        public ACDESRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ACDE_ACC_TYPE, t.ACAC_ACC_NO, t.ACDE_DESC, t.ACDE_LOCK_TOKEN, t.ATXR_SOURCE_ID, t.ProcessGovernance1up });

            // Properties
            this.Property(t => t.ACDE_ACC_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ACDE_DESC)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(70);

            this.Property(t => t.ACDE_LOCK_TOKEN)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);            

            // Table & Column Mappings
            this.ToTable("ACDESRC", "SRC");
            this.Property(t => t.ACDE_ACC_TYPE).HasColumnName("ACDE_ACC_TYPE");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.ACDE_LOCK_TOKEN).HasColumnName("ACDE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Acronym).HasColumnName("Acronym");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");                       
        }
    }
}
