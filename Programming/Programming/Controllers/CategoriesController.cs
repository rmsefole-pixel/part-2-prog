using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

/// <summary>Lists categories and lets organisers define categories and link them to their events.</summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    public CategoriesController(AppDbContext context) => _context = context;

    [SessionAuthorize]
    [HttpGet]
    public async Task<IActionResult> GetCategories() => Ok(await _context.Categories.AsNoTracking().ToListAsync());

    [SessionAuthorize("Organiser")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CategoryName)) return BadRequest(new { message = "Category name is required." });
        if (await _context.Categories.AnyAsync(c => c.CategoryName == dto.CategoryName.Trim())) return Conflict(new { message = "Category already exists." });
        var cat = new Category { CategoryName = dto.CategoryName.Trim(), Description = dto.Description?.Trim() };
        _context.Categories.Add(cat); await _context.SaveChangesAsync(); return CreatedAtAction(nameof(GetCategories), new { id = cat.CategoryId }, cat);
    }

    [SessionAuthorize("Organiser")]
    [HttpPost("event-category")]
    public async Task<IActionResult> AssignToEvent([FromBody] EventCategoryDto dto)
    {
        var ev = await _context.Events.FindAsync(dto.EventId); if (ev is null) return NotFound(new { message = "Event not found." });
        if (ev.OrganizerId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (!await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId)) return NotFound(new { message = "Category not found." });
        if (await _context.EventCategories.AnyAsync(ec => ec.EventId == dto.EventId && ec.CategoryId == dto.CategoryId)) return Conflict(new { message = "Category is already linked to this event." });
        if (dto.Capacity is <= 0) return BadRequest(new { message = "Capacity must be greater than zero when provided." });
        if (dto.EntryFee is < 0) return BadRequest(new { message = "Entry fee cannot be negative." });
        var ec = new EventCategory { EventId = dto.EventId, CategoryId = dto.CategoryId, EntryFee = dto.EntryFee, Capacity = dto.Capacity };
        _context.EventCategories.Add(ec); await _context.SaveChangesAsync(); return Ok(ec);
    }

    [SessionAuthorize]
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!await _context.Events.AnyAsync(e => e.EventId == eventId)) return NotFound();
        var list = await _context.EventCategories.AsNoTracking().Include(ec => ec.Category).Where(ec => ec.EventId == eventId).Select(ec => new { ec.EventId, ec.CategoryId, ec.EntryFee, ec.Capacity, CategoryName = ec.Category!.CategoryName, ec.Category!.Description }).ToListAsync();
        return Ok(list);
    }
}
