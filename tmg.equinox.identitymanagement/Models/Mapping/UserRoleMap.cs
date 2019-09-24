using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.identitymanagement.Models.Mapping
{
    public class UserRoleMap : EntityTypeConfiguration<ApplicationRole>
    {
        public UserRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            
            // Table & Column Mappings
            this.ToTable("UserRole", "Sec");
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);


            // Table & Column Mappings
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Id).HasColumnName("RoleId");
            //this.HasRequired(u => u.Section).WithMany(r=>r.Roles).HasForeignKey(t => t.RoleID);
            /*this.Property(t => t.Id).HasColumnName("RoleID");*/
           
        }
    }
}


