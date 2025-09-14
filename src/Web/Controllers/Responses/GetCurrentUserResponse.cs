namespace Web.Controllers.Responses;

public record GetCurrentUserResponse(Guid Id, string? UserName, string? Email);
