namespace RaceDayAPI.Models;

public class Enrolment { public int EnrolmentId { get; set; } public int ParticipantId { get; set; } public int EventId { get; set; } public int CategoryId { get; set; } public DateTime EnrolledAt { get; set; } public string Status { get; set; } = "Confirmed"; public User? Participant { get; set; } public Event? Event { get; set; } }
