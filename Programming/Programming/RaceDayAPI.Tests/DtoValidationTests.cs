using System.ComponentModel.DataAnnotations;
using RaceDayAPI.DTOs;
using Xunit;

namespace RaceDayAPI.Tests;

public class DtoValidationTests
{
    private static IList<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void RegisterDto_RejectsInvalidEmailAndShortPassword()
    {
        var dto = new RegisterDto { FirstName = "A", LastName = "B", Email = "not-an-email", Password = "123", Role = "Participant" };
        Assert.NotEmpty(Validate(dto));
    }

    [Fact]
    public void RegisterDto_AcceptsValidRequiredFields()
    {
        var dto = new RegisterDto { FirstName = "A", LastName = "B", Email = "person@example.com", Password = "LongPassword123", Role = "Participant" };
        Assert.Empty(Validate(dto));
    }

    [Fact]
    public void EventDto_RejectsMissingNameAndInvalidCapacity()
    {
        var dto = new EventDto { EventName = "", EventDate = DateTime.UtcNow, Location = "Park", MaxParticipants = 0, EventType = "run" };
        Assert.NotEmpty(Validate(dto));
    }

    [Fact]
    public void EnrolDto_RejectsNonPositiveIds()
    {
        var dto = new EnrolDto { EventId = 0, CategoryId = -1 };
        Assert.NotEmpty(Validate(dto));
    }

    [Fact]
    public void PasswordHash_IsNotPlainTextAndVerifies()
    {
        const string password = "RaceDay!12345";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        Assert.NotEqual(password, hash);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hash));
        Assert.False(BCrypt.Net.BCrypt.Verify("wrong-password", hash));
    }
}
