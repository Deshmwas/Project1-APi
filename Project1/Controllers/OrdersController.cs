using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using Project1.Models.Responses;

namespace Project1.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ProjectDbContext _context;

        public OrdersController(ProjectDbContext context)
        {
            _context = context;
        }

        // GET: api/orders

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetOrders()
        {
            var orders = await _context.Orders.Select(order => new OrderViewModel
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                UserId = order.UserId,
                // Include additional properties from the Order entity
                OrderDate = order.OrderDate,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                ShippingDate = order.ShippingDate,
                ShippingAddress = order.ShippingAddress
            }).ToListAsync();

            return Ok(orders);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }


        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] List<OrderViewModel> orderViewModels)
        {
            ApplicationResponse applicationResponse = new ApplicationResponse();

            if (orderViewModels.Count == 0 || orderViewModels.Count < 0)
            {
                applicationResponse.responseCode = false;
                applicationResponse.responseMessage = "The cart is empty.";

                return BadRequest(applicationResponse);

            }


            int cartItem = 0;

            for (cartItem = 0; cartItem < orderViewModels.Count; cartItem++)
            {

                var product = await _context.Products.FindAsync(orderViewModels[cartItem].ProductId);

                if (product == null || product.Equals(""))
                {
                    applicationResponse.responseCode = false;
                    applicationResponse.responseMessage = "There is no product that matches " + product + ". ";

                    return BadRequest(applicationResponse);
                }

                //store orders in databases
                var order = new Order
                {
                    ProductId = orderViewModels[cartItem].ProductId,
                    UserId = orderViewModels[cartItem].UserId,
                    OrderDate = DateTime.Now,
                    Quantity = orderViewModels[cartItem].Quantity,
                    TotalPrice = orderViewModels[cartItem].TotalPrice,
                    Status = "Ordered",
                    ShippingDate = DateTime.Now,
                    ShippingAddress = orderViewModels[cartItem].ShippingAddress,
                };

                _context.Orders.Add(order);
                product.Stock -= orderViewModels[cartItem].Quantity ?? 0;
                await _context.SaveChangesAsync();
            }

            applicationResponse.responseCode = true;
            applicationResponse.responseMessage = "Orders placed successfully.";
            return Ok(applicationResponse);
        }

           
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.ProductId = orderViewModel.ProductId;
            order.UserId = orderViewModel.UserId;
            order.OrderDate = orderViewModel.OrderDate;
            order.Quantity = orderViewModel.Quantity;
            order.TotalPrice = orderViewModel.TotalPrice;
            order.Status = orderViewModel.Status;
            order.ShippingDate = orderViewModel.ShippingDate;
            order.ShippingAddress = orderViewModel.ShippingAddress;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
    

