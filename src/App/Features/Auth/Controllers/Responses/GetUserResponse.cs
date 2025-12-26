namespace App.Features.Auth.Controllers.Responses;

public record GetUserResponse(string Id, string? Email, List<string> Roles);
