using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;

namespace RaceDayAPI.Controllers;

/// <summary>Allows a signed-in user to view and update their own profile.</summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) => _context = context;

    [SessionAuthorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        return Ok(new { user.UserId, user.FirstName, user.LastName, user.Email, user.Role, user.Phone, user.CreatedAt });
    }

    [SessionAuthorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest(new { message = "First name and last name are required." });
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.FirstName = dto.FirstName.Trim(); user.LastName = dto.LastName.Trim(); user.Phone = dto.Phone;
        await _context.SaveChangesAsync();
        return Ok(new { user.UserId, user.FirstName, user.LastName, user.Email, user.Role, user.Phone, user.CreatedAt });
    }
}
