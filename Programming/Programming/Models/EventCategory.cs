namespace RaceDayAPI.Models;

public class EventCategory { public int EventId { get; set; } public int CategoryId { get; set; } public decimal? EntryFee { get; set; } public int? Capacity { get; set; } public Event? Event { get; set; } public Category? Category { get; set; } }
