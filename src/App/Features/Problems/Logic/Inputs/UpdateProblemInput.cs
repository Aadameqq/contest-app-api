namespace App.Features.Problems.Logic.Inputs;

public record UpdateProblemInput(string Slug, string Title, List<string> TagSlugs);
