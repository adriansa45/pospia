namespace POS.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
    }
}
