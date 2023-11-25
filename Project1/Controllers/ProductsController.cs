using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using Project1.Services;

namespace Project1.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProjectDbContext _context;
        private readonly IEmailService _emailService;

        public ProductsController(ProjectDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;

        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stock = p.Stock,
                    Description = p.Description,
                    Price = p.Price,
                    ImageData = p.Image != null ? Convert.ToBase64String(p.Image) : null,
                    UserEmail = p.UserEmail,
                    TotalPrice = p.TotalPrice
                })
                .ToListAsync();


            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _context.Products
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stock = p.Stock,
                    Description = p.Description,
                    Price = p.Price,


                    ImageData = p.Image != null ? Convert.ToBase64String(p.Image) : null,
                    UserEmail = p.UserEmail
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(query.ToLower()) || p.Description.ToLower().Contains(query.ToLower()))
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stock = p.Stock,
                    Description = p.Description,
                    Price = p.Price,
                    ImageData = p.Image != null ? Convert.ToBase64String(p.Image) : null,
                    UserEmail = p.UserEmail,
                    TotalPrice = p.TotalPrice
                })
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return Ok(new { message = "No products found for the given search query." });
            }

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var file = Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    product.Image = memoryStream.ToArray();
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Check if stock is about to be depleted (e.g., less than 10 items)
            if (product.Stock < 10)
            {
                // Send email alert
                var emailSubject = "Stock Alert";
                var emailBody = $"The stock of {product.Name} is running low. Current stock: {product.Stock}";

                // Assuming you have an email address to send the alert to.
                var adminEmail = "mwangiderrick27@gmail.com";

                await _emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);
            }

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Product product)
        {
            if (id != product.Id)
            {
                return Ok(new { message = "The ID in the URL and the ID in the request payload do not match" });
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return Ok(new { message = "The product with the specified ID was not found" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            byte[] img;

            var file = Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                // Retrieve the original file
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    product.Image = memoryStream.ToArray();
                }

                existingProduct.Image = product.Image;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;

                _context.Entry(existingProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Check if stock is about to be depleted (e.g., less than 10 items)
                if (existingProduct.Stock < 10)
                {
                    // Send email alert
                    var emailSubject = "Stock Alert";
                    var emailBody = $"The stock of {existingProduct.Name} is running low. Current stock: {existingProduct.Stock}";

                    // Assuming you have an email address to send the alert to.
                    var adminEmail = "mwangiderrick27@gmail.com";

                    await _emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);
                }

                return Ok(new { message = "Product Updated  Successfully" });
            }
            else
            {
                // existingProduct.Image = product.Image;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;

                _context.Entry(existingProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Check if stock is about to be depleted (e.g., less than 10 items)
                if (existingProduct.Stock < 10)
                {
                    // Send email alert
                    var emailSubject = "Stock Alert";
                    var emailBody = $"The stock of {existingProduct.Name} is running low. Current stock: {existingProduct.Stock}";

                    // Assuming you have an email address to send the alert to.
                    var adminEmail = "mwangiderrick27@gmail.com";

                    await _emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);
                }

                return Ok(new { message = "Product Updated  Successfully" });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                var result = new
                {
                    message = $"{product.Name} was deleted successfully."
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while deleting the product.",
                    error = ex.Message
                });
            }
        }
}
    }
