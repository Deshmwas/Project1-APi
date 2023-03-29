namespace Project1.Controllers
{
    public class ProductViewModel
    {
        

        public int Id { get; set; }
        public string? Name { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public string? ImageData { get; set; }
        public string? UserEmail { get; set; }
        public decimal TotalPrice { get; set; }
    }
}