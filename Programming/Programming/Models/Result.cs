namespace RaceDayAPI.Models;

public class Result { public int ResultId { get; set; } public int EnrolmentId { get; set; } public TimeSpan? FinishTime { get; set; } public int? Position { get; set; } public int? Score { get; set; } public string ResultStatus { get; set; } = "Finished"; public DateTime RecordedAt { get; set; } public Enrolment? Enrolment { get; set; } }
