using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs;

public class RegisterDto
{
    [Required, StringLength(80, MinimumLength = 1)] public string FirstName { get; set; } = "";
    [Required, StringLength(80, MinimumLength = 1)] public string LastName { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, MinLength(8), StringLength(100)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "";
    [Phone, StringLength(30)] public string? Phone { get; set; }
}
public class LoginDto { [Required, EmailAddress] public string Email { get; set; } = ""; [Required] public string Password { get; set; } = ""; }
public class UpdateProfileDto { [Required, StringLength(80)] public string FirstName { get; set; } = ""; [Required, StringLength(80)] public string LastName { get; set; } = ""; [Phone, StringLength(30)] public string? Phone { get; set; } }
public class EventDto { [Required, StringLength(150)] public string EventName { get; set; } = ""; [StringLength(2000)] public string? Description { get; set; } [Required] public DateTime EventDate { get; set; } [Required, StringLength(250)] public string? Location { get; set; } [Range(1, 1000000)] public int MaxParticipants { get; set; } = 100; [Required] public string Status { get; set; } = "Open"; [Range(typeof(double), "0.01", "10000")] public double? Distance { get; set; } [Required] public string? EventType { get; set; }
}
public class CategoryDto { [Required, StringLength(100)] public string CategoryName { get; set; } = ""; [StringLength(500)] public string? Description { get; set; } }
public class EventCategoryDto { [Range(1, int.MaxValue)] public int EventId { get; set; } [Range(1, int.MaxValue)] public int CategoryId { get; set; } [Range(typeof(decimal), "0", "1000000")] public decimal? EntryFee { get; set; } [Range(1, 1000000)] public int? Capacity { get; set; } }
public class EnrolDto { [Range(1, int.MaxValue)] public int EventId { get; set; } [Range(1, int.MaxValue)] public int CategoryId { get; set; } }
public class ResultDto { [Range(1, int.MaxValue)] public int EnrolmentId { get; set; } public string? FinishTime { get; set; } [Range(1, int.MaxValue)] public int? Position { get; set; } [Range(0, int.MaxValue)] public int? Score { get; set; } }
