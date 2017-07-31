// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.5
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace QpxExpress.Data
{

    public interface ITicketsContext : System.IDisposable
    {
        System.Data.Entity.DbSet<Country> Countries { get; set; } // Country
        System.Data.Entity.DbSet<Trip> Trips { get; set; } // Trip
        System.Data.Entity.DbSet<TripDestination> TripDestinations { get; set; } // TripDestination
        System.Data.Entity.DbSet<VwRemainingTrip> VwRemainingTrips { get; set; } // vw_RemainingTrips
        System.Data.Entity.DbSet<VwTrip> VwTrips { get; set; } // vw_Trip
        System.Data.Entity.DbSet<VwTripFare> VwTripFares { get; set; } // vw_TripFare

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
        System.Data.Entity.Infrastructure.DbChangeTracker ChangeTracker { get; }
        System.Data.Entity.Infrastructure.DbContextConfiguration Configuration { get; }
        System.Data.Entity.Database Database { get; }
        System.Data.Entity.Infrastructure.DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        System.Data.Entity.Infrastructure.DbEntityEntry Entry(object entity);
        System.Collections.Generic.IEnumerable<System.Data.Entity.Validation.DbEntityValidationResult> GetValidationErrors();
        System.Data.Entity.DbSet Set(System.Type entityType);
        System.Data.Entity.DbSet<TEntity> Set<TEntity>() where TEntity : class;
        string ToString();

        // Stored Procedures
        System.Collections.Generic.List<SpGetTripFareReturnModel> SpGetTripFare(int? destinationId, string ticketType, bool? businessClass, System.DateTime? requestDate);
        System.Collections.Generic.List<SpGetTripFareReturnModel> SpGetTripFare(int? destinationId, string ticketType, bool? businessClass, System.DateTime? requestDate, out int procResult);
        System.Threading.Tasks.Task<System.Collections.Generic.List<SpGetTripFareReturnModel>> SpGetTripFareAsync(int? destinationId, string ticketType, bool? businessClass, System.DateTime? requestDate);

        int SpUpdateCountries();
        // SpUpdateCountriesAsync cannot be created due to having out parameters, or is relying on the procedure result (int)

    }

}
// </auto-generated>
