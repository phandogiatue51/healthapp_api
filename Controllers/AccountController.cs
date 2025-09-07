using HealthApp.Data;
using HealthApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static HealthApp.Models.Account;

namespace HealthAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (await _context.Accounts.AnyAsync(a => a.Email == request.Email))
                return Conflict("Email already exists");

            var account = new Account
            {
                Email = request.Email,
                Password = HashPassword(request.Password), // hash trước khi lưu
                FullName = request.FullName
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new { account.Email, account.FullName });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var hashedPassword = HashPassword(request.Password);

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == request.Email && a.Password == hashedPassword);

            if (account == null)
                return Unauthorized();

            return Ok(new { account.Email, account.FullName });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
