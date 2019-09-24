using System.Data.Entity;

namespace tmg.equinox.integration.domain
{
    public static class DbContextExtension
    {
        public static void ApplyStateChanges(this DbContext dbContext)
        {
            foreach (var dbEntityEntry in dbContext.ChangeTracker.Entries())
            {
                var entityState = dbEntityEntry.State;

                //if (entityState != null)
                //    throw new InvalidCastException(
                //        "All entites must implement " +
                //        "the IObjectState interface, this interface " +
                //        "must be implemented so each entites state" +
                //        "can explicitely determined when updating graphs.");

                dbEntityEntry.State = ConvertState(entityState);
            }
        }

        private static EntityState ConvertState(EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    return EntityState.Added;
                case EntityState.Modified:
                    return EntityState.Modified;
                case EntityState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}
