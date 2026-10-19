using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

/// <summary>Records race results and exposes results to participants and event organisers.</summary>
[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ResultsController(AppDbContext context) => _context = context;

    [SessionAuthorize("Organiser")]
    [HttpPost]
    public async Task<IActionResult> RecordResult([FromBody] ResultDto dto)
    {
        if (dto.EnrolmentId <= 0) return BadRequest(new { message = "A valid enrolment ID is required." });
        if (dto.Position is <= 0) return BadRequest(new { message = "Position must be greater than zero." });
        TimeSpan? finishTime = null;
        if (!string.IsNullOrWhiteSpace(dto.FinishTime) && (!TimeSpan.TryParse(dto.FinishTime, out var parsed) || parsed < TimeSpan.Zero))
            return BadRequest(new { message = "FinishTime must be a valid non-negative time, e.g. 00:45:32." });
        else if (!string.IsNullOrWhiteSpace(dto.FinishTime)) finishTime = TimeSpan.Parse(dto.FinishTime);
        var enrol = await _context.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EnrolmentId == dto.EnrolmentId);
        if (enrol is null) return NotFound(new { message = "Enrolment not found." });
        if (enrol.Event!.OrganizerId != HttpContext.Session.GetInt32("UserId")) return Forbid();
        if (await _context.Results.AnyAsync(r => r.EnrolmentId == dto.EnrolmentId)) return Conflict(new { message = "A result already exists for this enrolment." });
        var result = new Result { EnrolmentId = dto.EnrolmentId, FinishTime = finishTime, Position = dto.Position, Score = dto.Score, ResultStatus = "Finished", RecordedAt = DateTime.UtcNow };
        _context.Results.Add(result); await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(EventResults), new { eventId = enrol.EventId }, new { result.ResultId, result.EnrolmentId, result.FinishTime, result.Position, result.Score, result.ResultStatus, result.RecordedAt });
    }

    [SessionAuthorize("Participant")]
    [HttpGet("my")]
    public async Task<IActionResult> MyResults()
    {
        var pid = HttpContext.Session.GetInt32("UserId")!.Value;
        return Ok(await _context.Results.AsNoTracking().Include(r => r.Enrolment).ThenInclude(e => e!.Event).Where(r => r.Enrolment!.ParticipantId == pid)
            .Select(r => new { r.ResultId, r.FinishTime, r.Position, r.Score, r.ResultStatus, r.RecordedAt, r.Enrolment!.EventId, EventName = r.Enrolment.Event!.EventName }).ToListAsync());
    }

    [SessionAuthorize]
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> EventResults(int eventId)
    {
        var ev = await _context.Events.FindAsync(eventId); if (ev is null) return NotFound();
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var role = HttpContext.Session.GetString("Role");
        if (role != "Organiser" && role != "Participant") return Forbid();
        if (role == "Organiser" && ev.OrganizerId != userId) return Forbid();
        return Ok(await _context.Results.AsNoTracking().Include(r => r.Enrolment).Where(r => r.Enrolment!.EventId == eventId)
            .Select(r => new { r.ResultId, r.EnrolmentId, r.FinishTime, r.Position, r.Score, r.ResultStatus }).ToListAsync());
    }
}
