using System;

namespace QpxExpressApp.Model
{
    public class Slice
    {
        public string Origin { get; set; }

        public string Destination { get; set; }

        public DateTime Date { get; set; }

        public string PreferredCabin { get; set; }

        public int MaxStops { get; set; }

        public string[] PermittedCarrier { get; set; }
    }
}
