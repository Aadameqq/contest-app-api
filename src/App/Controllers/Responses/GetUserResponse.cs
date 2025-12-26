namespace App.Controllers.Responses;

public record GetUserResponse(string Id, string? Email, List<string> Roles);
