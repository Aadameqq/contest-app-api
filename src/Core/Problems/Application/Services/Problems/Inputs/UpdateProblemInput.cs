namespace Core.Problems.Application.Services.Problems.Inputs;

public record UpdateProblemInput(string Slug, string Title, List<string> TagSlugs);
