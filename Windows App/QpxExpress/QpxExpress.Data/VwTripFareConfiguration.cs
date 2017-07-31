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

    // vw_TripFare
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.32.0.0")]
    public class VwTripFareConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<VwTripFare>
    {
        public VwTripFareConfiguration()
            : this("dbo")
        {
        }

        public VwTripFareConfiguration(string schema)
        {
            ToTable("vw_TripFare", schema);
            HasKey(x => new { x.DestinationId, x.Country, x.Destination, x.Code, x.Airline, x.BusinessClass, x.TicketType });

            Property(x => x.DestinationId).HasColumnName(@"DestinationId").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Country).HasColumnName(@"Country").HasColumnType("nvarchar").IsRequired().HasMaxLength(100).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Destination).HasColumnName(@"Destination").HasColumnType("nvarchar").IsRequired().HasMaxLength(100).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Code).HasColumnName(@"Code").HasColumnType("nvarchar").IsRequired().HasMaxLength(10).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Airline).HasColumnName(@"Airline").HasColumnType("nvarchar").IsRequired().HasMaxLength(3).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.BusinessClass).HasColumnName(@"BusinessClass").HasColumnType("bit").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.TicketType).HasColumnName(@"TicketType").HasColumnType("nvarchar").IsRequired().HasMaxLength(10).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Fare).HasColumnName(@"Fare").HasColumnType("float").IsOptional();
        }
    }

}
// </auto-generated>