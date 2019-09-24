namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PBPMigrationModel : DbContext
    {
        public PBPMigrationModel()
            : base("name=PBPMigration")
        {
        }

        public virtual DbSet<AccessPBPFile> AccessPBPFiles { get; set; }
        public virtual DbSet<FormDesignVersionJSON> FormDesignVersionJSONs { get; set; }
        public virtual DbSet<MigrationPlan> MigrationPlans { get; set; }
        public virtual DbSet<PBPBenefitMapping> PBPBenefitMappings { get; set; }
        public virtual DbSet<PBPBenefitsDictionary> PBPBenefitsDictionaries { get; set; }

        public virtual DbSet<MigrationBatch> MigrationBatches { get; set; }
         
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PBPBenefitsDictionary>()
                .Property(e => e.YEAR)
                .IsFixedLength();
        }
    }
}
