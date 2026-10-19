using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

/// <summary>Lists events and lets organisers manage their own events.</summary>
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;
    public EventsController(AppDbContext context) => _context = context;

    [SessionAuthorize]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Events.AsNoTracking().Select(e => new { e.EventId, e.EventName, e.Description, e.EventDate, e.Location, e.MaxParticipants, e.Status, e.Distance, e.EventType, e.OrganizerId, OrganizerName = e.Organizer == null ? "" : e.Organizer.FirstName + " " + e.Organizer.LastName }).ToListAsync());

    [SessionAuthorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var ev = await _context.Events.AsNoTracking().Include(e => e.EventCategories).ThenInclude(ec => ec.Category).FirstOrDefaultAsync(e => e.EventId == id);
        return ev is null ? NotFound() : Ok(ev);
    }

    [SessionAuthorize("Organiser")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventDto dto)
    {
        var error = Validate(dto); if (error is not null) return BadRequest(new { message = error });
        var organizerId = HttpContext.Session.GetInt32("UserId")!.Value;
        var ev = new Event { OrganizerId = organizerId, EventName = dto.EventName.Trim(), Description = dto.Description, EventDate = dto.EventDate, Location = dto.Location, MaxParticipants = dto.MaxParticipants, Status = dto.Status, Distance = dto.Distance, EventType = dto.EventType!.Trim().ToLowerInvariant(), CreatedAt = DateTime.UtcNow };
        _context.Events.Add(ev); await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = ev.EventId }, ev);
    }

    [SessionAuthorize("Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventDto dto)
    {
        var error = Validate(dto); if (error is not null) return BadRequest(new { message = error });
        var ev = await _context.Events.FindAsync(id); if (ev is null) return NotFound();
        if (ev.OrganizerId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        ev.EventName = dto.EventName.Trim(); ev.Description = dto.Description; ev.EventDate = dto.EventDate; ev.Location = dto.Location;
        ev.MaxParticipants = dto.MaxParticipants; ev.Distance = dto.Distance; ev.EventType = dto.EventType!.Trim().ToLowerInvariant(); ev.Status = dto.Status;
        await _context.SaveChangesAsync(); return Ok(ev);
    }

    [SessionAuthorize("Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _context.Events.FindAsync(id); if (ev is null) return NotFound();
        if (ev.OrganizerId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        _context.Events.Remove(ev); await _context.SaveChangesAsync(); return NoContent();
    }

    private static string? Validate(EventDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EventName)) return "Event name is required.";
        if (string.IsNullOrWhiteSpace(dto.Location)) return "Location is required.";
        if (dto.EventDate == default) return "A valid event date is required.";
        if (dto.MaxParticipants <= 0) return "Maximum participants must be greater than zero.";
        if (dto.Distance is <= 0) return "Distance must be greater than zero.";
        if (dto.EventType is null || !new[] { "run", "walk", "cycle" }.Contains(dto.EventType.Trim().ToLowerInvariant())) return "Event type must be run, walk, or cycle.";
        return null;
    }
}
