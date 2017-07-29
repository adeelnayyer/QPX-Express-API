namespace QpxExpressApp.Model
{
    public class Request
    {
        public Slice[] Slice { get; set; }

        public Passengers Passengers { get; set; }

        public int Solutions { get; set; }

        public bool Refundable { get; set; }
    }
}
