namespace Core.Problems.Application.Services.Problems.Inputs;

public record CreateProblemInput(string Title, List<string> TagSlugs);
