using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

/// <summary>Registers users, creates authenticated sessions, and ends sessions.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context) => _context = context;

    /// <summary>Register an Organiser or Participant account.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) ||
            string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "First name, last name, email and password are required." });
        if (dto.Password.Length < 8) return BadRequest(new { message = "Password must contain at least 8 characters." });
        if (!string.Equals(dto.Role, "Organiser", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(dto.Role, "Participant", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Role must be Organiser or Participant." });
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _context.Users.AnyAsync(u => u.Email == email)) return Conflict(new { message = "Email already exists." });
        var user = new User { FirstName = dto.FirstName.Trim(), LastName = dto.LastName.Trim(), Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = char.ToUpperInvariant(dto.Role.Trim()[0]) + dto.Role.Trim()[1..].ToLowerInvariant(),
            Phone = dto.Phone, CreatedAt = DateTime.UtcNow };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Created($"/api/Users/me", new { user.UserId, user.FirstName, user.LastName, user.Email, user.Role, user.Phone });
    }

    /// <summary>Validate credentials and create a session cookie.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });
        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetString("Email", user.Email);
        return Ok(new { message = "Logged in", user.UserId, user.FirstName, user.LastName, user.Email, user.Role });
    }

    /// <summary>Clear the current session.</summary>
    [HttpPost("logout")]
    public IActionResult Logout() { HttpContext.Session.Clear(); return Ok(new { message = "Logged out" }); }
}
