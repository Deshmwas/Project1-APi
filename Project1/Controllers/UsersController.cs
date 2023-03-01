using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using System.Security.Cryptography;
using System.Text;

namespace Project1.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ProjectDbContext _context;

        public UsersController(ProjectDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            if (_context.Users.Any(u => u.UserName == user.UserName || u.Email == user.Email))
            {
                return BadRequest("Username or email already exists. Please choose another.");
            }

            // Hash the password
            var hashedPassword = HashPassword(user.Password);
            user.Password = hashedPassword;

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "Successfully registered." });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user exists
            var existingUser = _context.Users.SingleOrDefault(u => u.UserName == user.UserName || u.Email == user.Email);
            if (existingUser == null)
            {
                return BadRequest(new { message = "Incorrect username or email or password." });
            }

            // Verify password
            var passwordVerified = VerifyPassword(user.Password, existingUser.Password);
            if (!passwordVerified)
            {
                return BadRequest(new { message = "Incorrect username or email or password." });
            }

             return Ok(new { message = "Successfully logged in." });
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedPassword);
            }
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
                var passwordVerified = sha256.ComputeHash(passwordBytes).SequenceEqual(hashedPasswordBytes);
                return passwordVerified;
            }
        }


    }
}
