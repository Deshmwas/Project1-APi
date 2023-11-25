using System.ComponentModel.DataAnnotations;

namespace Project1.Controllers
{
    public class OrderViewModel
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime? OrderDate { get; internal set; }
        public int OrderId { get; internal set; }
    }
}