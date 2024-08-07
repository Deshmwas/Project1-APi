﻿namespace Project1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public byte[]? Image { get; set; }
        public string? UserEmail { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ImageData => Image != null ? Convert.ToBase64String(Image) : null;
        public virtual ICollection<Order>? Orders { get; set; }
        public int ProductId { get; internal set; }
        public int? Quantity { get; internal set; }
    }
}
