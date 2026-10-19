using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

/// <summary>Manages participant enrolments and organiser enrolment views.</summary>
[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly AppDbContext _context;
    public EnrolmentsController(AppDbContext context) => _context = context;

    [SessionAuthorize("Participant")]
    [HttpPost]
    public async Task<IActionResult> Enrol([FromBody] EnrolDto dto)
    {
        var participantId = HttpContext.Session.GetInt32("UserId")!.Value;
        var ec = await _context.EventCategories.Include(x => x.Event).FirstOrDefaultAsync(x => x.EventId == dto.EventId && x.CategoryId == dto.CategoryId);
        if (ec is null) return BadRequest(new { message = "Category is not linked to this event." });
        if (ec.Event is null || !string.Equals(ec.Event.Status, "Open", StringComparison.OrdinalIgnoreCase)) return BadRequest(new { message = "This event is not open for enrolment." });
        if (await _context.Enrolments.AnyAsync(e => e.ParticipantId == participantId && e.EventId == dto.EventId)) return Conflict(new { message = "You are already enrolled in this event." });
        var total = await _context.Enrolments.CountAsync(e => e.EventId == dto.EventId && e.Status == "Confirmed");
        if (total >= ec.Event.MaxParticipants) return Conflict(new { message = "This event has reached capacity." });
        if (ec.Capacity.HasValue && await _context.Enrolments.CountAsync(e => e.EventId == dto.EventId && e.CategoryId == dto.CategoryId && e.Status == "Confirmed") >= ec.Capacity.Value)
            return Conflict(new { message = "This category has reached capacity." });
        var enrol = new Enrolment { ParticipantId = participantId, EventId = dto.EventId, CategoryId = dto.CategoryId, EnrolledAt = DateTime.UtcNow, Status = "Confirmed" };
        _context.Enrolments.Add(enrol); await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(MyEnrolments), new { }, new { enrol.EnrolmentId, enrol.ParticipantId, enrol.EventId, enrol.CategoryId, enrol.EnrolledAt, enrol.Status });
    }

    [SessionAuthorize("Organiser")]
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var myId = HttpContext.Session.GetInt32("UserId")!.Value;
        var ev = await _context.Events.FindAsync(eventId);
        if (ev is null) return NotFound(); if (ev.OrganizerId != myId) return Forbid();
        var list = await _context.Enrolments.AsNoTracking().Include(e => e.Participant).Include(e => e.Event).Where(e => e.EventId == eventId)
            .Select(e => new { e.EnrolmentId, e.EventId, e.CategoryId, e.EnrolledAt, e.Status, Participant = new { e.Participant!.UserId, e.Participant.FirstName, e.Participant.LastName, e.Participant.Email } }).ToListAsync();
        return Ok(list);
    }

    [SessionAuthorize("Participant")]
    [HttpGet("my")]
    public async Task<IActionResult> MyEnrolments()
    {
        var pid = HttpContext.Session.GetInt32("UserId")!.Value;
        return Ok(await _context.Enrolments.AsNoTracking().Where(e => e.ParticipantId == pid).Include(e => e.Event).Include(e => e.Participant).Select(e => new { e.EnrolmentId, e.EventId, e.CategoryId, e.EnrolledAt, e.Status, EventName = e.Event!.EventName, e.Event!.EventDate, e.Event!.Location }).ToListAsync());
    }
}
