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
    public class VwTripFare
    {
        public int DestinationId { get; set; } // DestinationId (Primary key)
        public string Country { get; set; } // Country (Primary key) (length: 100)
        public string Destination { get; set; } // Destination (Primary key) (length: 100)
        public string Code { get; set; } // Code (Primary key) (length: 10)
        public string Airline { get; set; } // Airline (Primary key) (length: 3)
        public bool BusinessClass { get; set; } // BusinessClass (Primary key)
        public string TicketType { get; set; } // TicketType (Primary key) (length: 10)
        public double? Fare { get; set; } // Fare
    }

}
// </auto-generated>