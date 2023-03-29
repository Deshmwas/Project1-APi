namespace Project1.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string? ShippingAddress { get; set; }
        public virtual Product? Product { get; set; }

    }
}
